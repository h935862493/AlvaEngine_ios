//using cn.sharesdk.unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProjectUIItem : MonoBehaviour
{
    public ProjectType projectType;

    public VisionProjectInfo ItemInfo;

    /// <summary>
    /// 该子是否在下载列表中
    /// </summary>
    public bool IsLocalPanelItem = false;

    public Button ProjectButton;
    public Image TypeImage;
    //public Button TopButton;
    private Image IsTopImage;
    public Button UpdateButton;
    //private Button ShareButton;
    public Text NameText, Text_updatedTime;
    private Image VersionImage;
    public Button DeleteButton;
    ResPackages packagesItem, thisPackagesItem;

    public bool IsTop;
    public int IndexSibling = -1;

    public bool IsLocal, IsUpdate;
    Vector2 parentCellSize;
    public void Awake()
    {
        //ProjectButton = transform.Find("Button").GetComponent<Button>();
        ////TopButton = transform.Find("Title/SetTopButton").GetComponent<Button>();
        //UpdateButton = transform.Find("Title/DownButton").GetComponent<Button>();
        //DeleteButton = transform.Find("Title/DelectButton").GetComponent<Button>();
        ////ShareButton = transform.Find("Title/ShareButton").GetComponent<Button>();

        ////IsTopImage = TopButton.GetComponent<Image>();
        //TypeImage = transform.Find("TypeImage").GetComponent<Image>();

        //NameText = transform.Find("Title/Text").GetComponent<Text>();
        //Text_updatedTime = transform.Find("Title/Text_updatedTime").GetComponent<Text>();

        ProjectButton.onClick.AddListener(() => { OnProjectButtonClick(); });
        //TopButton.onClick.AddListener(() => { OnTopButtonClick(); });
        UpdateButton.onClick.AddListener(() => { OnUpdateButtonCLick(); });
        DeleteButton.onClick.AddListener(OnDelectButtonClick);
        //ShareButton.onClick.AddListener(WeChatBtnClick);

        //UpdateButton.gameObject.SetActive(false);
        DeleteButton.gameObject.SetActive(false);

        GlobalData.DelectProjectAction += OnDelectProjectAction;
    }

    void DownBtnState(bool bo)
    {
        if (UpdateButton)
        {
            if (!bo)//已下载，下载图标灰色
            {
                UpdateButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
            }
            else//未下载，下载图标白色
            {
                UpdateButton.GetComponent<Image>().color = Color.white;
            }
        }
    }
    /// <summary>
    /// 删除项目事件信息
    /// </summary>
    /// <param name="pro"></param>
    private void OnDelectProjectAction(VisionProjectInfo pro)
    {
        if (pro.serialId == ItemInfo.serialId && IsLocal && !IsLocalPanelItem)
        {
            //取消已下载的图标显示
            DownBtnState(true);
            //刷新项目信息
            OnInitSceneInfo(pro, false, false);
        }
    }


    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <param name="ItemInfo">项目信息</param>
    /// <param name="isLocal">本地是否存储</param>
    /// <param name="isUpdate">是否需要更新版本</param>
    public void OnInitSceneInfo(VisionProjectInfo ItemInfo, bool isLocal, bool isUpdate)
    {
        if (transform.parent && transform.parent.gameObject.activeSelf)
        {
            parentCellSize = transform.parent.GetComponent<GridLayoutGroup>().cellSize;
            ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentCellSize.x);
            ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentCellSize.y);
        }
        else
        {
            ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MyMessageData.parentCellSize.x);
            ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MyMessageData.parentCellSize.y);
        }

        packagesItem = GlobalData.GetTargetPlatform(ItemInfo);
        if (this.ItemInfo != null)
        {
            thisPackagesItem = GlobalData.GetTargetPlatform(this.ItemInfo);
        }

        IsLocal = isLocal;
        IsUpdate = isUpdate;

        //如果版本相同，只更新封面即可
        if (this.ItemInfo != null && thisPackagesItem.resPackage.file.hash == packagesItem.resPackage.file.hash)
        {
            isUpdate = false;
        }
        this.ItemInfo = ItemInfo;
        //项目名
        NameText.text = ItemInfo.name;
        //更新时间
        if (ItemInfo.updatedTime == DateTime.MinValue)
            HideUpdatedTime(true);
        else
            HideUpdatedTime(false);


        if (Enum.IsDefined(typeof(ProjectType), ItemInfo.category))
        {
            projectType = (ProjectType)System.Enum.Parse(typeof(ProjectType), ItemInfo.category);
        }
        else
        {
            projectType = ProjectType.Empty;
        }


        //置顶UI
        //if (!string.IsNullOrEmpty(ItemInfo.top.ToString()))
        //{
        //    IsTop = ItemInfo.top;
        //    if (IsTop)
        //    {
        //        transform.SetAsFirstSibling();
        //        IsTopImage.sprite = SpriteManager.Instance.置顶;
        //    }
        //    else
        //    {
        //        IsTopImage.sprite = SpriteManager.Instance.未置顶;
        //    }
        //}
        //else
        //{
        //    IsTopImage.sprite = SpriteManager.Instance.未置顶;
        //}

        //项目类型UI
        switch (projectType)
        {
            case ProjectType.ImageRecognition:
                TypeImage.sprite = SpriteManager.Instance.二维识别;
                break;
            case ProjectType.SlamRecognition:
                TypeImage.sprite = SpriteManager.Instance.平面识别;
                break;
            case ProjectType.ModelRecognition:
                TypeImage.sprite = SpriteManager.Instance.模型识别;
                break;
            case ProjectType.Empty:
                TypeImage.sprite = SpriteManager.Instance.空识别描述;
                break;
            default:
                break;
        }

        //更新缩略图
        UpdateThumbnail(ItemInfo);

        //非本地列表里的项目
        if (!IsLocalPanelItem)
        {
            //需要更新
            if (isUpdate && IsLocal)
            {
                UpdateButton.GetComponent<Image>().sprite = SpriteManager.Instance.需要更新项目;
                DownBtnState(true);
            }
            else if (IsLocal)//不需要更新
            {
                UpdateButton.GetComponent<Image>().sprite = SpriteManager.Instance.已下载项目;
                DownBtnState(false);
            }
        }
        else
        {
            DownBtnState(false);
            //TopButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(true);
            //ShareButton.gameObject.SetActive(false);
        }
    }

    /// <summary>更新缩略图</summary>
    public void UpdateThumbnail(VisionProjectInfo iinfo)
    {
        //更新UI
        if (iinfo.thumbnail.Count > 0 && iinfo.thumbnail[0].file.extension != ".zip")
        {
            thumbnailFullName = GlobalData.LocalPath + iinfo.thumbnail[0].file.hash + (iinfo.thumbnail[0].file.extension == "" ? ".jpg" : iinfo.thumbnail[0].file.extension);

            if (File.Exists(thumbnailFullName))
            {
                //本地的
                //设置封面
                ProjectButton.GetComponent<Image>().sprite = GlobalData.Texture2DToSprite(GlobalData.LoadImage(thumbnailFullName));

                if (transform.parent && transform.parent.gameObject.activeSelf)
                {
                    //ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentCellSize.x);
                    ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ProjectButton.GetComponent<Image>().sprite.rect.height * parentCellSize.x / ProjectButton.GetComponent<Image>().sprite.rect.width);
                }
                else
                {
                    ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ProjectButton.GetComponent<Image>().sprite.rect.height * MyMessageData.parentCellSize.x / ProjectButton.GetComponent<Image>().sprite.rect.width);
                }
            }
            else
            {
                SetProjectButtonImage();
            }
        }
    }

    //隐藏更新时间
    public void HideUpdatedTime(bool isHide)
    {
        if (isHide)
        {
            NameText.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, 25);
            Text_updatedTime.text = "";
        }
        else
        {
            NameText.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, 51);
            //Text_updatedTime.text = ItemInfo.updatedTime.ToString();
            Text_updatedTime.text = packagesItem.updatedTime.ToString();
        }
    }

    DownloadInfo info;
    string textureUrl = "";
    string textureFileName = "";
    string thumbnailFullName = "";
    void SetProjectButtonImage()
    {

        if (ItemInfo.thumbnail.Count > 0 && !string.IsNullOrEmpty(ItemInfo.thumbnail[0].file.downloadUrl))
        {

            info = new DownloadInfo();
            textureUrl = ItemInfo.thumbnail[0].file.downloadUrl;

            textureFileName = ItemInfo.thumbnail[0].file.hash + (ItemInfo.thumbnail[0].file.extension == "" ? ".jpg" : ItemInfo.thumbnail[0].file.extension);

            Thread thread = new Thread(new ThreadStart(DownLoadFile));
            thread.Start();
        }
    }
    /// <summary>
    /// 下载文件
    /// </summary>
    private void DownLoadFile()
    {
        lock (ZManager.instnace.zServer)
        {
            ZManager.instnace.zServer.DownloadFile(
               textureUrl,
               textureFileName,
               info, false,
               (y) =>
               {
                   if (!string.IsNullOrEmpty(y))
                   {
                       ZManager.instnace.zServer.QueueOnMainThread((new_y) =>
                       {
                           string sp = GlobalData.LocalPath + textureFileName;

                           while (true)
                           {
                               Thread.Sleep(1);
                               if (File.Exists(sp))
                               {
                                   Texture2D tex = GlobalData.LoadImage(sp);
                                   ProjectButton.GetComponent<Image>().sprite = GlobalData.Texture2DToSprite(tex);

                                   if (transform.parent && transform.parent.gameObject.activeSelf)
                                   {
                                       //ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentCellSize.x);
                                       ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ProjectButton.GetComponent<Image>().sprite.rect.height * parentCellSize.x / ProjectButton.GetComponent<Image>().sprite.rect.width);
                                   }
                                   else
                                   {
                                       ProjectButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ProjectButton.GetComponent<Image>().sprite.rect.height * MyMessageData.parentCellSize.x / ProjectButton.GetComponent<Image>().sprite.rect.width);
                                   }
                                   break;
                               }
                           }
                       }, y);

                   }
               }, (x) => { });
        }
    }

    private void OnProjectButtonClick()
    {
        if (!AlvaLicenseHttp.instance.OnCheckDevice())
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData(GlobalData.GetPlatformInfo(3), (IsYes) =>
            {
                if (!IsYes)
                {
                    return;
                }

            });

            return;
        }

        if (string.IsNullOrEmpty(packagesItem.description))
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("场景名为空，请确认后再使用", (IsYes) =>
            {
                return;
            });
            return;
        }

        //模型面数
        int surface = 0;
        if (!string.IsNullOrEmpty(packagesItem.resPackage.description) && int.TryParse(packagesItem.resPackage.description, out surface))
        {
            if (surface > 8000000)
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetWarnData("模型面数" + packagesItem.resPackage.description + "过大，可能引起程序异常，是否继续？", (IsYes) =>
                {
                    if (IsYes)
                        GoOn();
                });
            }
            else
                GoOn();
        }
        else
        {
            GoOn();
        }
    }

    private void GoOn()
    {
        print("*****************************cProperty:" + packagesItem.resPackage.cProperty);
        if (!GlobalData.isTestVersions && !string.IsNullOrEmpty(packagesItem.resPackage.cProperty))//正式版，有编辑器版本号
        {
            var tempV = packagesItem.resPackage.cProperty.Split('.');

            if (tempV.Length > 3)
            {
                if (int.Parse(tempV[3]) > GlobalData.local_version)//末位号不相同
                {
                    UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                    box.SetWarnData("资源版本号" + packagesItem.resPackage.cProperty + "与程序版本号" + Application.version + "." + GlobalData.local_version.ToString() + "不兼容，请升级应用程序", (IsYes) => { });
                    return;
                }

            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetWarnData("该资源包版本号格式错误" + packagesItem.resPackage.cProperty, (IsYes) => { });
                return;
            }
        }

        GlobalData.ProjectID = ItemInfo.serialId;
        //Debug.Log("项目ID " + GlobalData.ProjectID);
        if (IsUpdate && !IsLocalPanelItem)
        {
            //弹出下载提示
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("该项目有新版本，是否继续下载？", (IsYes) =>
            {
                if (IsYes)
                {
                    if (packagesItem.category == GlobalData.GetCategory() && !string.IsNullOrEmpty(packagesItem.resPackage.file.downloadUrl))
                    {
                        print("下载连接：" + packagesItem.resPackage.file.downloadUrl);
                        DownloadInfo info = new DownloadInfo();
                        LoadProjectResAction(info, packagesItem.resPackage.file.downloadUrl, packagesItem.description);

                    }
                    else
                    {
                        UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                        box.SetWarnData(GlobalData.GetPlatformInfo(1), (IsYes) =>
                        {
                        });
                    }
                }
            });
        }
        else
        {
            if (IsLocal)
            {
                //本地加载
                //项目包类别( 1-Android、2-IOS、3-HoloLens、4-其它
                if (packagesItem.category == GlobalData.GetCategory() && !string.IsNullOrEmpty(packagesItem.description))
                {
                    print("本地加载description：" + packagesItem.description);
                    LoadProject(packagesItem.description);
                    return;
                }
                else
                {
                    UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                    box.SetWarnData(GlobalData.GetPlatformInfo(1), (IsYes) =>
                    {
                    });
                }
            }
            else
            {
                //项目包类别( 1-Android、2-IOS、3-HoloLens、4-其它
                if (packagesItem.category == GlobalData.GetCategory() && !string.IsNullOrEmpty(packagesItem.resPackage.file.downloadUrl))
                {
                    print("下载连接：" + packagesItem.resPackage.file.downloadUrl);
                    DownloadInfo info = new DownloadInfo();
                    LoadProjectResAction(info, packagesItem.resPackage.file.downloadUrl, packagesItem.description);
                }
                else
                {
                    UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                    box.SetWarnData(GlobalData.GetPlatformInfo(1), (IsYes) =>
                    {
                    });
                }
            }
        }
    }

    private void LoadProjectResAction(DownloadInfo info, string url, string description = null)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("LoadPanel"), TypeImage.transform.root);
        UI_DownLoadPanel t = go.GetComponent<UI_DownLoadPanel>();
        t.DownLoadResourcesFromServer(url, ItemInfo.serialId + ".zip", info,
            (filePath) =>
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }
                //使用id下载
                // ItemInfo.id;
                //删除下载列表中已经更新的项目
                for (int i = 0; i < GlobalData.LocalPanelItemObjList.Count; i++)
                {
                    ProjectUIItem item = GlobalData.LocalPanelItemObjList[i].GetComponent<ProjectUIItem>();
                    if (item.ItemInfo.serialId == ItemInfo.serialId)
                    {
                        GameObject obj = GlobalData.LocalPanelItemObjList[i];
                        GlobalData.LocalPanelItemObjList.Remove(GlobalData.LocalPanelItemObjList[i]);
                        if (obj)
                            DestroyImmediate(obj);
                        continue;
                    }
                }
                //删除旧项目
                GlobalData.DelectDir(GlobalData.LocalPath + ItemInfo.serialId);

                if (transform.parent.CompareTag("PublicItems") || GlobalData.publicInfo.Find((x) => x.serialId == ItemInfo.serialId) != null)
                {
                    GlobalData.VersionLocalData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == null);
                }
                else
                {
                    GlobalData.VersionLocalData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == GlobalData.getUserInfo.name);
                }


                if (GlobalData.VersionLocalData != null)//登录情况下
                {
                    bool isHave = false, isUpdate = false;

                    //遍历记录列表
                    for (int i = 0; i < GlobalData.AllUserVersionLocalData.Count; i++)
                    {
                        //如果有账号信息
                        if (GlobalData.AllUserVersionLocalData[i].UserID == GlobalData.VersionLocalData.UserID)
                        {
                            for (int j = 0; j < GlobalData.VersionLocalData.UserProList.Count; j++)
                            {
                                if (GlobalData.VersionLocalData.UserProList[j].serialId == ItemInfo.serialId)
                                {
                                    GlobalData.VersionLocalData.UserProList[j] = ItemInfo;
                                    isUpdate = true;
                                }
                            }
                            GlobalData.AllUserVersionLocalData[i] = GlobalData.VersionLocalData;
                            isHave = true;
                            break;
                        }
                    }
                    if (!isUpdate)
                    {
                        //没有信息
                        GlobalData.VersionLocalData.UserProList.Add(ItemInfo);
                    }
                    if (!isHave)
                    {
                        //无信息  初始化信息后  更新信息
                        GlobalData.VersionLocalData = new VersionData();
                        GlobalData.VersionLocalData.UserID = GlobalData.getUserInfo.name;
                        GlobalData.VersionLocalData.UserProList = new List<VisionProjectInfo>();
                        GlobalData.VersionLocalData.UserProList.Add(ItemInfo);
                    }
                }
                else if (transform.parent.CompareTag("PublicItems"))
                {
                    //无信息  初始化信息后  更新信息
                    GlobalData.VersionLocalData = new VersionData();
                    GlobalData.VersionLocalData.UserID = null;
                    GlobalData.VersionLocalData.UserProList = new List<VisionProjectInfo>();
                    GlobalData.VersionLocalData.UserProList.Add(ItemInfo);
                }
                else
                {
                    //无信息  初始化信息后  更新信息
                    GlobalData.VersionLocalData = new VersionData();
                    GlobalData.VersionLocalData.UserID = GlobalData.getUserInfo.name;
                    GlobalData.VersionLocalData.UserProList = new List<VisionProjectInfo>();
                    GlobalData.VersionLocalData.UserProList.Add(ItemInfo);
                }

                if (GlobalData.AllUserVersionLocalData == null)
                {
                    GlobalData.AllUserVersionLocalData = new List<VersionData>();
                }
                bool isContain = false;
                //遍历记录列表
                for (int i = 0; i < GlobalData.AllUserVersionLocalData.Count; i++)
                {
                    //如果有信息
                    if (GlobalData.AllUserVersionLocalData[i].UserID == GlobalData.VersionLocalData.UserID)
                    {
                        GlobalData.AllUserVersionLocalData[i] = GlobalData.VersionLocalData;
                        isContain = true;
                        break;
                    }
                }
                //不包含当前信息
                if (!isContain)
                {
                    GlobalData.AllUserVersionLocalData.Add(GlobalData.VersionLocalData);
                }
                //更新项目
                IsUpdate = false;
                IsLocal = true;

                //成功下载项目后，添加到下载列表
                //GameObject itemLocal = Instantiate<GameObject>(gameObject);
                //ProjectUIItem proInfo = itemLocal.GetComponent<ProjectUIItem>();
                //proInfo.IsLocalPanelItem = true;
                //proInfo.OnInitSceneInfo(ItemInfo, true, false);
                //GlobalData.UpdateLocationProjectAction(itemLocal);

                //解析包
                bool IsZip = ZipHelper.UnzipFile(filePath, GlobalData.LocalPath + Path.GetFileNameWithoutExtension(filePath));
                File.Delete(filePath);
                if (!IsZip)
                {
                    unzipCont++;
                    if (unzipCont > 3)
                    {
                        //弹出下载提示
                        UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                        box.SetTipData("该项目资源损坏，请重新下载!");
                        //删除旧项目
                        GlobalData.DelectDir(GlobalData.LocalPath + ItemInfo.serialId);
                        DestroyImmediate(go);
                        return;
                    }
                    //删除旧项目
                    GlobalData.DelectDir(GlobalData.LocalPath + ItemInfo.serialId);
                    IsZip = ZipHelper.UnzipFile(filePath, GlobalData.LocalPath + Path.GetFileNameWithoutExtension(filePath));
                }

                //写入缓存记录
                GlobalData.WriteCachedJson(GlobalData.SerializeObject(GlobalData.AllUserVersionLocalData), GlobalData.VersionLocalPath);

                if (go)
                    DestroyImmediate(go);
                if (UpdateButton)
                    UpdateButton.GetComponent<Image>().sprite = SpriteManager.Instance.已下载项目;
                DownBtnState(false);

                LoadProject(description);
            }
            );

    }
    int unzipCont = 0;

    private void LoadProject(string description = null)
    {
        GlobalData.ProjectSettingData = new ProjectDescriptionDataModel();
        GlobalData.ProjectSettingData.ID = ItemInfo.serialId;
        GlobalData.ProjectSettingData.Type = ItemInfo.category;
        GlobalData.ProjectSettingData.ProjectName = ItemInfo.name;

        string path = GlobalData.LocalPath + GlobalData.ProjectID + GlobalData.GetPlatformInfo(2);

        if (File.Exists(path))
            StartCoroutine(LoadScene(path, description));
        else
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("该路径下无此文件: " + GlobalData.ProjectID + GlobalData.GetPlatformInfo(2) + " <color=red>请联系编辑器人员</color>", (IsYes) => { });
        }


    }


    private IEnumerator LoadScene(string path, string description)
    {
        if (HxMainManager.instance)
        {
            HxMainManager.instance.loadAni.SetActive(true);
        }

        //description描述里面是要加载的场景名
        if (MyMessageData.assetBundle && MyMessageData.curentBundleName != packagesItem.resPackage.file.hash)
        {
            MyMessageData.assetBundle.Unload(true);
            MyMessageData.assetBundle = null;
            print("清空MyMessageData.assetBundle");
        }
        if (description != "FDJ1" && MyMessageData.assetBundle_FDJ1 != null)
        {
            MyMessageData.assetBundle_FDJ1.Unload(true);
            MyMessageData.assetBundle_FDJ1 = null;
            print("清空MyMessageData.assetBundle_FDJ1");
        }


        if (MyMessageData.assetBundle)
        {
            print("加载过:" + description);
            if (!string.IsNullOrEmpty(packagesItem.resPackage.cTag) && packagesItem.resPackage.cTag.Contains("Horizontal"))
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            MyMessageData._asyncOperation = SceneManager.LoadSceneAsync(description);
        }
        else if (description == "FDJ1" && MyMessageData.assetBundle_FDJ1 != null)
        {
            MyMessageData._asyncOperation = SceneManager.LoadSceneAsync(description);
        }
        else
        {

            print("没没没载过:" + description);
            var download = AssetBundle.LoadFromFileAsync(path);
            while (!download.isDone)
            {
                HxMainManager.instance.progressText.text = "解析包:" + (download.progress * 100).ToString("0.0") + "%";


                yield return new WaitForEndOfFrame();
            }
            HxMainManager.instance.progressText.text = "解析包:100%";

            yield return download;
            AlvaLicenseHttp.instance.progressText = HxMainManager.instance.progressText;
            if (description == "FDJ1")
            {
                MyMessageData.assetBundle_FDJ1 = download.assetBundle;
            }
            else
            {
                MyMessageData.assetBundle = download.assetBundle;
                MyMessageData.curentBundleName = packagesItem.resPackage.file.hash;
            }

            if (!string.IsNullOrEmpty(packagesItem.resPackage.cTag) && packagesItem.resPackage.cTag.Contains("Horizontal"))
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }

            MyMessageData._asyncOperation = SceneManager.LoadSceneAsync(description);
        }
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        yield return new WaitForEndOfFrame();//加上这么一句就可以先显示加载画面然后再进行加载
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        //读取完毕后返回， 系统会自动进入C场景
        yield return async;
    }

    /// <summary>
    /// 解析ini配置文件
    /// </summary>
    /// <param name="ini"></param>
    /// <returns></returns>
    ProjectDescriptionDataModel GetModel(IniFile ini)
    {
        ProjectDescriptionDataModel projectDescriptionDataModel = new ProjectDescriptionDataModel();
        projectDescriptionDataModel.ID = ini.Get("ID");
        projectDescriptionDataModel.ProjectName = ini.Get("ProjectName");
        projectDescriptionDataModel.Type = ini.Get("Type");
        projectDescriptionDataModel.Path = ini.Get("Path");
        projectDescriptionDataModel.Time = ini.Get("Time");
        projectDescriptionDataModel.PublishStatus = ini.Get("PublishStatus");
        projectDescriptionDataModel.Version = ini.Get("Version");
        return projectDescriptionDataModel;
    }

    /// <summary>
    /// 置顶按钮事件
    /// </summary>
    private void OnTopButtonClick()
    {
        IsTop = !IsTop;
        SetTopInfo top = new SetTopInfo();
        Debug.Log(System.Convert.ToInt16(IsTop).ToString());
        top.top = IsTop;
        Debug.Log("------------------置顶------------------------" + GlobalData.BaseUrl + GlobalData.SetTopUrl);
        string tokenName = "alva_author_token";
        StartCoroutine(ZManager.instnace.zServer.Put(GlobalData.BaseUrl + GlobalData.SetTopUrl + ItemInfo.serialId, new string[2] { tokenName, GlobalData.getUserInfo.token }, top, OnTopCallBack));
    }

    private void OnTopCallBack(DownloadHandler handler)
    {
        Debug.Log("置顶返回信息" + handler.text);
        NewServerMessage<bool> mgs = GlobalData.DeserializeObject<NewServerMessage<bool>>(handler.text);
        if (mgs.code.Equals(0))
        {
            Debug.Log("置顶设置成功" + IndexSibling);
            if (IsTop)
            {
                transform.SetAsFirstSibling();
                IsTopImage.sprite = SpriteManager.Instance.置顶;
            }
            else if (IndexSibling >= 0)
            {
                int itemIndex = 0;
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    Transform child = transform.parent.GetChild(i);
                    if (child != transform && child.GetComponent<ProjectUIItem>().IsTop)
                    {
                        itemIndex++;
                    }
                }
                transform.SetSiblingIndex(IndexSibling + itemIndex);
                IsTopImage.sprite = SpriteManager.Instance.未置顶;
            }
        }
    }

    private void OnUpdateButtonCLick()
    {

    }
    /// <summary>
    /// 删除本地项目
    /// </summary>
    private void OnDelectButtonClick()
    {
        UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
        box.SetWarnData("是否删除该项目本地记录？", (IsYes) =>
        {
            if (!IsYes)
            {
                return;
            }
            //该条记录的序号
            int index = -1;

            VersionData VersionLocalData = GlobalData.VersionLocalData;
            //GlobalData.versionTempData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == null);
            string delectID = "";
            foreach (var userItem in GlobalData.AllUserVersionLocalData)
            {
                for (int x = 0; x < userItem.UserProList.Count; x++)
                {
                    if (userItem.UserProList[x].serialId == ItemInfo.serialId)
                    {
                        delectID = userItem.UserID;
                        VersionLocalData = GlobalData.AllUserVersionLocalData.Find((x) => x.UserID == delectID);
                        break;
                    }
                }
            }

            //遍历记录列表
            for (int i = 0; i < GlobalData.AllUserVersionLocalData.Count; i++)
            {
                //如果有信息
                if (GlobalData.AllUserVersionLocalData[i].UserID == VersionLocalData.UserID)
                {
                    for (int j = 0; j < VersionLocalData.UserProList.Count; j++)
                    {
                        if (VersionLocalData.UserProList[j].serialId == ItemInfo.serialId)
                        {
                            //移除缓存信息
                            VisionProjectInfo info = VersionLocalData.UserProList[j];
                            VersionLocalData.UserProList.Remove(info);
                            index = i;
                            break;
                        }
                    }
                }
            }
            //删掉了该账号下的最后一条本地项目，删除账号缓存信息
            if (VersionLocalData.UserProList.Count < 1)
            {
                GlobalData.AllUserVersionLocalData.Remove(VersionLocalData);
                //GlobalData.AllUserVersionLocalData.Clear();
            }
            else
            {
                //更新本地缓存列表
                GlobalData.AllUserVersionLocalData[index] = VersionLocalData;
            }
            //写入缓存记录
            GlobalData.WriteCachedJson(GlobalData.SerializeObject(GlobalData.AllUserVersionLocalData), GlobalData.VersionLocalPath);
            GlobalData.DelectDir(GlobalData.LocalPath + ItemInfo.serialId);


            GlobalData.DelectProjectAction?.Invoke(ItemInfo);

            GlobalData.LocalPanelItemObjList.Remove(gameObject);

            if (ZManager.instnace.zServer.m_Urls.Contains(packagesItem.resPackage.file.downloadUrl))
            {
                ZManager.instnace.zServer.m_Urls.Remove(packagesItem.resPackage.file.downloadUrl);
            }
            //
            //删除自己
            Destroy(gameObject);
        });
    }

    public void WeChatBtnClick()
    {
        //Debug.Log(ItemInfo.shareCode);
        //ShareContent content = new ShareContent();
        //content.SetText("this is a test string.");
        ////content.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
        //content.SetTitle("test title");
        ////content.SetTitleUrl("http://www.mob.com");
        ////content.SetSite("Mob-ShareSDK");
        ////content.SetSiteUrl("http://www.mob.com");
        //content.SetUrl(GlobalData.BaseUrl + "share/project/" + ItemInfo.shareCode);
        ////content.SetComment("test description");
        ////content.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
        //content.SetShareType(ContentType.Webpage);
        //ssdk.ShareContent(PlatformType.WeChat, content);
    }

    private void OnDestroy()
    {
        GlobalData.DelectProjectAction -= OnDelectProjectAction;

        ProjectButton.onClick.RemoveAllListeners();
        //TopButton.onClick.RemoveAllListeners();
        UpdateButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.RemoveAllListeners();
    }

}
