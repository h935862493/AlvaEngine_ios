using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LocationPanel : MonoBehaviour
{
    /// <summary>
    /// 提示界面
    /// </summary>
    public GameObject TipImage;
    /// <summary>
    /// 项目界面
    /// </summary>
    public GameObject ProjectContent;
    /// <summary>
    /// 项目列表
    /// </summary>
    public GameObject ScrollView;
    bool isFirstSetScroll = true;

    private void Awake()
    {
        GlobalData.UpdateLocationProjectAction += OnUpdateProjectPanel;

        GlobalData.IsResetSceneAction += OnResetSceneAction;
        GlobalData.SetPageIndexAction += OnSetPageDragAction;

        GlobalData.SetOptionPage += OnSetPageDragAction;
    }

    void Start()
    {
        if (ProjectContent.transform.childCount > 0)
            TipImage.SetActive(false);
        else
            TipImage.SetActive(true);


        //GlobalData.AllUserVersionLocalData = GlobalData.GetLocalCatchRes();
        //GlobalData.LoginSucessfulAction += OnInitList;

        //OnInitScene(PlayerPrefs.GetInt(GlobalData.LoginStateStr));


        if (Application.internetReachability == NetworkReachability.NotReachable)//没网时，检查本地下载记录
            OnInitList();
    }

    private void OnSetPageDragAction(int index)
    {
        if (index == 2)
        {
            if (ProjectContent != null)
            {
                List<GameObject> objs = new List<GameObject>();
                for (int i = 0; i < ProjectContent.transform.childCount; i++)
                {
                    objs.Add(ProjectContent.transform.GetChild(i).gameObject);
                }
                GlobalData.SetOptionTypeObject?.Invoke(objs);
                if (isFirstSetScroll)
                {
                    isFirstSetScroll = false;
                    ProjectContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(ProjectContent.GetComponent<RectTransform>().anchoredPosition.x, MyMessageData.locationContentY);
                }

            }
        }
    }

    private void OnInitScene(int loginState)
    {
        //if (loginState.Equals(1))
        //{
        //    if (GlobalData.ProjectLocalGameObjects.Count > 0)
        //    {
        //        TipImage.SetActive(false);
        //        ScrollView.SetActive(true);

        //        GlobalData.LocalPanelItemObjList.Clear();

        //        for (int i = 0; i < GlobalData.ProjectLocalGameObjects.Count; i++)
        //        {
        //            GameObject obj = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
        //            ProjectUIItem itemUI = obj.AddComponent<ProjectUIItem>();
        //            VisionProjectInfo itemPro = GlobalData.ProjectLocalGameObjects[i].ItemInfo;
        //            itemUI.IndexSibling = GlobalData.ProjectLocalGameObjects[i].IndexSibling;
        //            itemUI.IsLocalPanelItem = true;
        //            itemUI.OnInitSceneInfo(itemPro, true, false);//本地列表  只记录本地信息  不更新

        //            GlobalData.LocalPanelItemObjList.Add(obj);
        //        }

        //    }
        //}
    }

    private void OnInitList()
    {
        GlobalData.AllUserVersionLocalData = GlobalData.GetLocalCatchRes();
        if (GlobalData.AllUserVersionLocalData != null && GlobalData.AllUserVersionLocalData.Count > 0)
        {
            if (PlayerPrefs.GetInt(GlobalData.LoginStateStr).Equals(1))
            {
                VersionData dataItem_user = GlobalData.AllUserVersionLocalData.Find((data) => data.UserID == PlayerPrefs.GetString(GlobalData.UserNameStr));
                if (dataItem_user != null)
                {
                    int index = -1;

                    foreach (var pro in dataItem_user.UserProList)
                    {
                        GameObject item = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                        ProjectUIItem itemUI = item.GetComponent<ProjectUIItem>();
                        itemUI.IsLocalPanelItem = true;
                        itemUI.OnInitSceneInfo(pro, true, false);
                        index++;
                        itemUI.IndexSibling = index;

                        GlobalData.LocalPanelItemObjList.Add(item);
                    }
                    TipImage.SetActive(false);
                }
            }

            VersionData dataItem_public = GlobalData.AllUserVersionLocalData.Find((data) => data.UserID == null);
            if (dataItem_public != null)
            {
                int index = -1;
                Debug.Log(dataItem_public.UserProList.Count);
                foreach (var pro in dataItem_public.UserProList)
                {
                    GameObject item = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                    ProjectUIItem itemUI = item.GetComponent<ProjectUIItem>();
                    itemUI.IsLocalPanelItem = true;
                    itemUI.OnInitSceneInfo(pro, true, false);
                    index++;
                    itemUI.IndexSibling = index;

                    GlobalData.LocalPanelItemObjList.Add(item);
                }
                TipImage.SetActive(false);
            }
        }
    }

    private void OnResetSceneAction(bool obj)
    {
        if (!obj)
        {
            return;
        }
        TipImage.SetActive(true);
        StartCoroutine(DestoryProItem(OnInitList));

    }

    IEnumerator DestoryProItem(Action overAction = null)
    {
        while (ProjectContent.transform.childCount > 0)
        {
            DestroyImmediate(ProjectContent.transform.GetChild(0).gameObject);
            yield return new WaitForEndOfFrame();
        }
        GlobalData.LocalPanelItemObjList.Clear();
        overAction?.Invoke();
    }

    private void OnUpdateProjectPanel(GameObject obj)
    {

        ProjectUIItem objInfo = obj.GetComponent<ProjectUIItem>();

        foreach (var item in GlobalData.LocalPanelItemObjList)
        {
            if (item == null)
            {
                return;
            }
            ProjectUIItem ui = item.GetComponent<ProjectUIItem>();
            if (ui != null && ui.ItemInfo.serialId == objInfo.ItemInfo.serialId)
            {
                obj.SetActive(false);
                return;
            }
        }

        TipImage.SetActive(false);
        obj.transform.SetParent(ProjectContent.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        //Debug.Log("本地有:" + obj.GetComponent<ProjectUIItem>().ItemInfo.name);
        objInfo.IndexSibling = obj.transform.GetSiblingIndex();
        GlobalData.LocalPanelItemObjList.Add(obj);

        if (GlobalData.publicInfo.Count > 0)
        {
            foreach (var item in GlobalData.publicInfo)
            {
                if (item.serialId == objInfo.ItemInfo.serialId)
                {
                    objInfo.HideUpdatedTime(true);
                    return;
                }
            }
        }
    }

    private void OnDestroy()
    {
        MyMessageData.locationContentY = ProjectContent.GetComponent<RectTransform>().anchoredPosition.y;
        GlobalData.UpdateLocationProjectAction -= OnUpdateProjectPanel;

        GlobalData.IsResetSceneAction -= OnResetSceneAction;

        GlobalData.SetPageIndexAction -= OnSetPageDragAction;

        GlobalData.SetOptionPage -= OnSetPageDragAction;
        GlobalData.LocalPanelItemObjList.Clear();
        // GlobalData.LoginSucessfulAction -= OnInitList;

        //GlobalData.ProjectLocalGameObjects.Clear();
        //for (int i = 0; i < ProjectContent.transform.childCount; i++)
        //{
        //    ProjectUIItem item = ProjectContent.transform.GetChild(i).GetComponent<ProjectUIItem>();
        //    if (item == null)
        //    {
        //        continue;
        //    }
        //    item.gameObject.SetActive(true);
        //    GlobalData.ProjectLocalGameObjects.Add(item);
        //}
    }
}
