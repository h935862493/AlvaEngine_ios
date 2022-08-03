using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Text;
using System;

public class UI_RefreshScrollView : MonoBehaviour, IEndDragHandler, IBeginDragHandler//, IDragHandler
{
    PointerEventData startData;

    public Animator ani;
    public RectTransform BarContentRect;
    public string ProjectType;

    private void Start()
    {
        ani.gameObject.SetActive(false);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!ani.gameObject.activeSelf)
        {
            startData = eventData;
            ani.speed = 0;
        }

    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //}

    public void OnEndDrag(PointerEventData eventData)
    {
        if (BarContentRect.anchoredPosition.y < -5f && !ani.gameObject.activeSelf)
        {
            if (eventData.delta.y - startData.delta.y < 30f)
            {
                ani.gameObject.SetActive(true);
                ani.Play("refresh");
                ani.speed = 1;
                //Todo  回调：ani.gameObject.SetActive(false);
                //ZManager.instnace.zServer.
                GetProjectInfo proInfo = new GetProjectInfo();
                proInfo.token = GlobalData.getUserInfo.token;
                proInfo.userName = GlobalData.getUserInfo.name;
                if (ProjectType.Equals("public"))
                {
                    //if (BarContentRect.transform.childCount == 2)
                    //{
                    //    ani.speed = 0;
                    //    ani.gameObject.SetActive(false);
                    //    return;
                    //}
                    StartCoroutine(ZManager.instnace.zServer.ServerSimpleGet(GlobalData.BaseUrl + GlobalData.GetPublicProjectListUrl, OnPublicProCallBack));
                }
                if (ProjectType.Equals("private"))
                {
                    StartCoroutine(ZManager.instnace.zServer.Get(GlobalData.BaseUrl + GlobalData.GetProjectListUrl, new string[2] { "alva_author_token", GlobalData.getUserInfo.token }, null, OnProjectListCallBack));
                }
            }
        }

    }

    private void OnProjectListCallBack(DownloadHandler handle)
    {
        if (string.IsNullOrEmpty(handle.text))
        {
            StartCoroutine(CloseRefresh());
            return;
        }
        //print("刷新返回:" + handle.text);
        NewServerMessage<PageProjectInfo> mgs = GlobalData.DeserializeObject<NewServerMessage<PageProjectInfo>>(handle.text);
        if (mgs.code.Equals(0))
        {
            //获取成功
            if (mgs.data.items == null || mgs.data.items.Count < 1)
            {
                StartCoroutine(CloseRefresh());
                return;
            }
            //筛选vision平台的项目
            List<VisionProjectInfo> infoList2 = new List<VisionProjectInfo>();
            for (int i = 0; i < mgs.data.items.Count; i++)
            {
                if (mgs.data.items[i].platform != 1)
                    infoList2.Add(mgs.data.items[i]);
            }

            GlobalData.RefreshProjectListAction?.Invoke(infoList2, "private");
            StartCoroutine(CloseRefresh());
        }
        else
        {
            //获取用户列表失败
            StartCoroutine(CloseRefresh());
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            if (mgs.code == -10003)
            {
                box.SetTipData("您异地登录，登录信息已过期，请重新登录！ " + mgs.code);
                UI_SettingPanel.instance.ResetScene();
            }
            else
            {
                box.SetTipData(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(mgs.message)));
            }
        }
    }

    private void OnPublicProCallBack(DownloadHandler handle)
    {
        if (string.IsNullOrEmpty(handle.text))
        {
            StartCoroutine(CloseRefresh());
            return;
        }
        NewServerMessage<ServerMgsList<VisionProjectInfo>> message = GlobalData.DeserializeObject<NewServerMessage<ServerMgsList<VisionProjectInfo>>>(handle.text);
        if (message.code.Equals(0))
        {
            if (message.data.items.Count > 0)
            {
                if (message.data.items == null || message.data.items.Count < 1)
                {
                    StartCoroutine(CloseRefresh());
                    return;
                }
                //筛选vision平台的项目
                List<VisionProjectInfo> infoList2 = new List<VisionProjectInfo>();
                for (int i = 0; i < message.data.items.Count; i++)
                {
                    if (message.data.items[i].platform != 1)
                    {
                        message.data.items[i].updatedTime = DateTime.MinValue;
                        infoList2.Add(message.data.items[i]);
                    }
                }

                GlobalData.RefreshProjectListAction?.Invoke(infoList2, "public");
                StartCoroutine(CloseRefresh());
            }
            else
            {
                //获取用户列表失败
                StartCoroutine(CloseRefresh());
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                if (Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.message)).Contains("token"))
                {
                    box.SetTipData("您异地登录，您的登录信息已过期，请重新登录！");
                }
                else
                {
                    box.SetTipData(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.message)));
                }
            }
        }
    }

    IEnumerator CloseRefresh()
    {

        yield return new WaitForSeconds(1f);
        ani.speed = 0;
        ani.gameObject.SetActive(false);
    }
}
