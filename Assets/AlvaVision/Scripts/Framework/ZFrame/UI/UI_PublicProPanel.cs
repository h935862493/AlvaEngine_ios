using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UI_PublicProPanel : MonoBehaviour
{
    public static UI_PublicProPanel instance;
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

    private List<ProjectUIItem> mProItemList;

    private void Start()
    {
        instance = this;
        mProItemList = new List<ProjectUIItem>();

        if (PlayerPrefs.GetInt(GlobalData.AutoLogin) == 0 || !MyMessageData.isFirstMainTop)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                UpdatePublicPanel();
        }

        GlobalData.SetPageIndexAction += OnSetPageDragAction;

        GlobalData.SetOptionPage += OnSetPageDragAction;

        GlobalData.RefreshProjectListAction += OnRefreshListAction;

    }

    /// <summary>刷新列表</summary>
    public void UpdatePublicPanel()
    {
        if (GlobalData.publicInfo.Count > 0)
        {
            OnGetList(GlobalData.publicInfo);
        }
        else
        {
            StartCoroutine(ZManager.instnace.zServer.ServerSimpleGet(
    GlobalData.BaseUrl + GlobalData.GetPublicProjectListUrl,
    (DownloadHandler handle) =>
    {
        //print("公共列表："+handle.text);
        NewServerMessage<ServerMgsList<VisionProjectInfo>> message = GlobalData.DeserializeObject<NewServerMessage<ServerMgsList<VisionProjectInfo>>>(handle.text);
        if (message == null)
        {
            Debug.Log("无返回值");
            return;
        }
        if (message.code.Equals(0))
        {
            if (message.data.items.Count > 0)
            {

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

                OnGetList(infoList2);
                //OnSetPageDragAction(0);
            }
        }
    }));
        }

    }


    private void OnSetPageDragAction(int index)
    {
        if (index == 0)
        {
            if (ProjectContent != null)
            {
                List<GameObject> objs = new List<GameObject>();
                for (int i = 0; i < ProjectContent.transform.childCount; i++)
                {
                    objs.Add(ProjectContent.transform.GetChild(i).gameObject);
                }
                GlobalData.SetOptionTypeObject?.Invoke(objs);
            }
        }
    }

    /// <summary>包含</summary>
    bool isContain = false;
    bool isUpdate = true;
    public void OnGetList(List<VisionProjectInfo> proInfo)
    {
        GlobalData.publicInfo = proInfo;
        bool readLocal = ReadLocal();
        //关闭提示面板
        //TipImage.SetActive(false);
        int index = -1;
        //清空列表
        StartCoroutine(DestoryProItem(() =>
        {
            foreach (var itemPro in proInfo)
            {
                if (!Enum.IsDefined(typeof(ProjectType), itemPro.category) || !GlobalData.JudgePlatform(itemPro))
                {
                    continue;
                }
                if (!readLocal)
                {
                    //本地无记录
                    Debug.Log("网络加载public");
                    GameObject item = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                    ProjectUIItem itemUI = item.GetComponent<ProjectUIItem>();
                    isContain = false;
                    isUpdate = false;
                    if (GlobalData.AllUserVersionLocalData != null)
                    {
                        foreach (var userItem in GlobalData.AllUserVersionLocalData)
                        {
                            if (userItem.UserID != null)
                            {
                                for (int i = 0; i < userItem.UserProList.Count; i++)
                                {
                                    //本地有
                                    if (userItem.UserProList[i].serialId == itemPro.serialId)
                                    {
                                        isContain = true;
                                        //不要更新版本
                                        if (GlobalData.GetTargetPlatform(userItem.UserProList[i]).resPackage.file.hash != GlobalData.GetTargetPlatform(itemPro).resPackage.file.hash)
                                        {
                                            isUpdate = true;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    itemUI.OnInitSceneInfo(itemPro, isContain, isUpdate);
                    index++;
                    itemUI.IndexSibling = index;
                    mProItemList.Add(itemUI);
                }
                else
                {

                    GameObject item = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                    ProjectUIItem itemUI = item.GetComponent<ProjectUIItem>();
                    isContain = false;
                    isUpdate = true;

                    VisionProjectInfo infoLocal = new VisionProjectInfo();

                    if (GlobalData.AllUserVersionLocalData != null)
                    {
                        foreach (var userItem in GlobalData.AllUserVersionLocalData)
                        {
                            //if (userItem.UserID != null)

                            for (int i = 0; i < userItem.UserProList.Count; i++)
                            {
                                //本地有
                                if (userItem.UserProList[i].serialId == itemPro.serialId)
                                {
                                    infoLocal = userItem.UserProList[i];
                                    isContain = true;
                                    //不要更新版本
                                    if (GlobalData.GetTargetPlatform(userItem.UserProList[i]).resPackage.file.hash == GlobalData.GetTargetPlatform(itemPro).resPackage.file.hash)
                                    {
                                        isUpdate = false;
                                    }
                                }
                            }
                        }
                    }

                    if (isContain && !isUpdate)
                    {
                        //初始化下载的项目，添加到下载列表
                        GameObject itemLocal = Instantiate<GameObject>(item);
                        ProjectUIItem proInfoLocal = itemLocal.GetComponent<ProjectUIItem>();
                        proInfoLocal.IsLocalPanelItem = true;
                        proInfoLocal.OnInitSceneInfo(infoLocal, true, false);
                        GlobalData.UpdateLocationProjectAction(itemLocal);
                        //本地有 不用更新
                        itemUI.OnInitSceneInfo(itemPro, true, false);
                    }
                    if (isContain && isUpdate)
                    {
                        //初始化下载的项目，添加到下载列表
                        GameObject itemLocal = Instantiate<GameObject>(item);
                        ProjectUIItem proInfoLocal = itemLocal.GetComponent<ProjectUIItem>();
                        proInfoLocal.IsLocalPanelItem = true;
                        proInfoLocal.OnInitSceneInfo(infoLocal, true, false);
                        GlobalData.UpdateLocationProjectAction(itemLocal);

                        //本地有  需要更新
                        itemUI.OnInitSceneInfo(itemPro, true, true);
                    }
                    if (!isContain && isUpdate)
                    {
                        itemUI.OnInitSceneInfo(itemPro, false, false);
                    }

                    index++;
                    itemUI.IndexSibling = index;
                    mProItemList.Add(itemUI);
                }

            }

            ProjectContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(ProjectContent.GetComponent<RectTransform>().anchoredPosition.x, MyMessageData.publicContentY);
        }));
    }

    private void OnRefreshListAction(List<VisionProjectInfo> infos, string projectType)
    {
        if (!projectType.Equals("public"))
        {
            return;
        }
        MyMessageData.publicContentY = 0;
        GlobalData.publicInfo = infos;
        //记录现有列表中的项目信息
        List<VisionProjectInfo> proInfoList = new List<VisionProjectInfo>();
        //记录需要删除的对象
        List<ProjectUIItem> itemProList = new List<ProjectUIItem>();
        //本地多
        for (int i = 0; i < mProItemList.Count; i++)
        {
            proInfoList.Add(mProItemList[i].ItemInfo);

            bool isContain = false;
            for (int j = 0; j < infos.Count; j++)
            {
                if (mProItemList[i].ItemInfo.serialId == infos[j].serialId)
                {
                    isContain = true;
                    if (!string.Equals(GlobalData.GetTargetPlatform(mProItemList[i].ItemInfo).resPackage.file.hash, GlobalData.GetTargetPlatform(infos[j]).resPackage.file.hash))
                    {
                        //更新版本
                        mProItemList[i].OnInitSceneInfo(infos[j], true, true);
                    }
                    else if (!string.Equals(mProItemList[i].ItemInfo.thumbnail[0].file.hash, infos[j].thumbnail[0].file.hash))
                    {
                        //更新缩略图
                        mProItemList[i].UpdateThumbnail(infos[j]);
                    }
                }
            }
            if (!isContain)
            {
                itemProList.Add(mProItemList[i]);
            }
        }
        //删除多余的项目
        while (itemProList.Count > 0)
        {
            ProjectUIItem uiItem = itemProList[0];
            mProItemList.Remove(uiItem);
            itemProList.Remove(uiItem);
            DestroyImmediate(uiItem.gameObject);
            //TODO 同步下载列表
        }
        //网络多
        foreach (var info in infos)
        {
            bool isContain = false;
            for (int i = 0; i < proInfoList.Count; i++)
            {
                if (string.Equals(proInfoList[i].serialId, info.serialId))
                {
                    isContain = true;
                }
            }
            if (!isContain)
            {
                if (!Enum.IsDefined(typeof(ProjectType), info.category) || !GlobalData.JudgePlatform(info))
                {
                    continue;
                }
                GameObject obj = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                obj.transform.SetAsFirstSibling();
                ProjectUIItem itemUI = obj.GetComponent<ProjectUIItem>();
                itemUI.OnInitSceneInfo(info, false, false);
				mProItemList.Add(itemUI);
            }
        }
    }


    private bool ReadLocal()
    {
        //检测缓存
        GlobalData.AllUserVersionLocalData = GlobalData.GetLocalCatchRes();
        
        if (GlobalData.AllUserVersionLocalData != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator DestoryProItem(Action overAction = null)
    {
        while (ProjectContent.transform.childCount > 0)
        {
            DestroyImmediate(ProjectContent.transform.GetChild(0).gameObject);
            yield return new WaitForEndOfFrame();
        }
        overAction?.Invoke();
    }

    private void OnDestroy()
    {
        MyMessageData.publicContentY = ProjectContent.GetComponent<RectTransform>().anchoredPosition.y;
        GlobalData.SetPageIndexAction -= OnSetPageDragAction;

        GlobalData.SetOptionPage -= OnSetPageDragAction;

        GlobalData.RefreshProjectListAction -= OnRefreshListAction;

        GlobalData.PublicGameObjects.Clear();
        for (int i = 0; i < ProjectContent.transform.childCount; i++)
        {
            ProjectUIItem item = ProjectContent.transform.GetChild(i).GetComponent<ProjectUIItem>();
            if (item == null)
            {
                continue;
            }
            item.gameObject.SetActive(true);
            GlobalData.PublicGameObjects.Add(item);
        }
    }
}
