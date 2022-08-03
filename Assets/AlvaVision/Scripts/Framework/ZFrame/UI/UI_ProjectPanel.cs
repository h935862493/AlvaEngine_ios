using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ProjectPanel : MonoBehaviour
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

    public List<ProjectUIItem> mProItemList;

    // Start is called before the first frame update
    void Start()
    {
        GlobalData.GetProjectListAction += OnGetProjectWithUser;

        GlobalData.IsResetSceneAction += OnResetSceneAction;

        GlobalData.RefreshProjectListAction += OnRefreshListAction;


        TipImage.SetActive(true);

        mProItemList = new List<ProjectUIItem>();

        if (Application.internetReachability != NetworkReachability.NotReachable)
            OnInitScene(PlayerPrefs.GetInt(GlobalData.LoginStateStr));

        GlobalData.SetPageIndexAction += OnSetPageDragAction;

        GlobalData.SetOptionPage += OnSetPageDragAction;
    }

    private void OnSetPageDragAction(int index)
    {
        if (index == 1)
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

    private void OnInitScene(int loginState)
    {
        if (loginState.Equals(1))
        {
            mProItemList.Clear();
            if (GlobalData.proInfo.Count > 0)
            {
                TipImage.SetActive(false);
                ScrollView.SetActive(true);
                OnGetProjectWithUser(GlobalData.proInfo);
                ProjectContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(ProjectContent.GetComponent<RectTransform>().anchoredPosition.x, MyMessageData.projectContentY);
            }

        }
    }

    private void OnResetSceneAction(bool IsReset)
    {
        if (IsReset)
        {
            //关闭提示面板
            TipImage.SetActive(true);
            //清空列表
            StartCoroutine(DestoryProItem());
        }
    }

    IEnumerator DestoryProItem(Action overAction = null)
    {
        while (ProjectContent.transform.childCount > 0)
        {
            DestroyImmediate(ProjectContent.transform.GetChild(0).gameObject);
            yield return new WaitForEndOfFrame();
        }
        mProItemList.Clear();
        overAction?.Invoke();
    }
    /// <summary>包含</summary>
    bool isContain = false;
    bool isUpdate = true;
    void OnGetProjectWithUser(List<VisionProjectInfo> proInfo)
    {
        print("////////////////////更新项目列表");

        GlobalData.proInfo = proInfo;
        bool readLocal = ReadLocal();//本地有这个账号信息
        //关闭提示面板
        TipImage.SetActive(false);
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
                    //Debug.Log("网络加载");
                    GameObject item = Instantiate(Resources.Load(GlobalData.ProjectItemPrefabPath), ProjectContent.transform) as GameObject;
                    ProjectUIItem itemUI = item.GetComponent<ProjectUIItem>();

                    isContain = false;
                    isUpdate = false;

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
                    itemUI.OnInitSceneInfo(itemPro, isContain, isUpdate);
                    index++;
                    itemUI.IndexSibling = index;
                    //if (isContain)//本地有这个项目文件，添加到下载列表
                    //{
                    //    GameObject itemLocal = Instantiate<GameObject>(item);
                    //    ProjectUIItem proInfoLocal = itemLocal.GetComponent<ProjectUIItem>();
                    //    proInfoLocal.IsLocalPanelItem = true;

                    //    proInfoLocal.OnInitSceneInfo(itemPro, true, false);
                    //    GlobalData.UpdateLocationProjectAction(itemLocal);
                    //}
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


            //计算滑动条长度
            //RectTransform rect = ProjectContent.transform.parent.GetComponent<RectTransform>();
            //GridLayoutGroup grid = ProjectContent.GetComponent<GridLayoutGroup>();
            //rect.sizeDelta = new Vector2(rect.sizeDelta.x, ProjectContent.transform.childCount * (grid.cellSize.y + grid.spacing.y) + grid.padding.bottom);

        }));

    }

    private bool ReadLocal()
    {
        //GlobalData.VersionLocalData = null;
        //GlobalData.versionTempData = null;
        //检测缓存
        GlobalData.AllUserVersionLocalData = GlobalData.GetLocalCatchRes();
        //GlobalData.VersionLocalData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == GlobalData.getUserInfo.name);
        //GlobalData.versionTempData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == null);
        if (GlobalData.AllUserVersionLocalData != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 刷新回调事件
    /// </summary>
    /// <param name="infos"></param>
    private void OnRefreshListAction(List<VisionProjectInfo> infos, string projectType)
    {
        if (!projectType.Equals("private"))
        {
            return;
        }

        print("-------------OnRefreshListAction");
        MyMessageData.projectContentY = 0;
        GlobalData.proInfo = infos;

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

        //计算滑动条长度
        //RectTransform rect = ProjectContent.transform.parent.GetComponent<RectTransform>();
        //GridLayoutGroup grid = ProjectContent.GetComponent<GridLayoutGroup>();
        //rect.sizeDelta = new Vector2(rect.sizeDelta.x, ProjectContent.transform.childCount * (grid.cellSize.y + grid.spacing.y) + grid.padding.bottom);
    }
    //public void OnUpdateProjectPanel(GameObject obj)
    //{

    //    ProjectUIItem objInfo = obj.GetComponentInChildren<ProjectUIItem>();

    //    TipImage.SetActive(false);
    //    obj.transform.SetParent(ProjectContent.transform);
    //    obj.transform.localPosition = Vector3.zero;
    //    obj.transform.localRotation = Quaternion.identity;
    //    obj.transform.localScale = Vector3.one;

    //    obj.GetComponent<ProjectUIItem>().IndexSibling = obj.transform.GetSiblingIndex();

    //}

    private void OnDestroy()
    {
        MyMessageData.projectContentY = ProjectContent.GetComponent<RectTransform>().anchoredPosition.y;
        GlobalData.GetProjectListAction -= OnGetProjectWithUser;

        GlobalData.IsResetSceneAction -= OnResetSceneAction;

        GlobalData.RefreshProjectListAction -= OnRefreshListAction;

        GlobalData.SetPageIndexAction -= OnSetPageDragAction;

        GlobalData.SetOptionPage -= OnSetPageDragAction;

        //GlobalData.ProjectGameObjects.Clear();
        //for (int i = 0; i < ProjectContent.transform.childCount; i++)
        //{
        //    ProjectUIItem item = ProjectContent.transform.GetChild(i).GetComponent<ProjectUIItem>();
        //    if (item == null)
        //    {
        //        continue;
        //    }
        //    item.gameObject.SetActive(true);
        //    GlobalData.ProjectGameObjects.Add(item);
        //}
    }
}
