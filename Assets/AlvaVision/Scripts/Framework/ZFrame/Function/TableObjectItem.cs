using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TableObjectItem : MonoBehaviour
{
    public ProjectType projectType;

    private Button ProjectButton;
    private Image TypeImage;
    private Text TitleText;

    public VisionProjectInfo ItemInfo;
    public bool IsLocal = false;

    private void OnFindObj()
    {
        ProjectButton = transform.Find("ProjectButton").GetComponent<Button>();
        TypeImage = transform.Find("BgImage").GetComponent<Image>();
        TitleText = transform.GetComponentInChildren<Text>();
        ProjectButton.onClick.AddListener(OnProjectButtonClick);
    }

    public void OnSetData(VisionProjectInfo ItemInfo, bool isLocal)
    {
        OnFindObj();
        IsLocal = isLocal;
        this.ItemInfo = ItemInfo;
        //Debug.Log(ItemInfo);
        TitleText.text = ItemInfo.name;


        if (Enum.IsDefined(typeof(ProjectType), ItemInfo.category))
        {
            projectType = (ProjectType)System.Enum.Parse(typeof(ProjectType), ItemInfo.category);
        }
        else
        {
            projectType = ProjectType.Empty;
        }

        switch (projectType)
        {
            case ProjectType.ImageRecognition:
                TypeImage.sprite = SpriteManager.Instance.二维Table;
                break;
            case ProjectType.ModelRecognition:
                TypeImage.sprite = SpriteManager.Instance.三维Table;
                break;
            case ProjectType.SlamRecognition:
                TypeImage.sprite = SpriteManager.Instance.平面Table;
                break;
            case ProjectType.Empty:
                TypeImage.sprite = SpriteManager.Instance.空识别描述;
                break;
            default:
                break;
        }

    }

    void SetProjectButtonImage()
    {
        if (string.IsNullOrEmpty(ItemInfo.thumbnailMD5))
        {
            return;
        }
        info = new DownloadInfo();
        info.ProjectID = ItemInfo.serialId;
        info.token = GlobalData.getUserInfo.token;
        //Debug.Log("this 封面 " + info.ProjectID);
        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DownLoadFile));
        thread.Start();
    }
    private DownloadInfo info;
    private void DownLoadFile()
    {
        ZManager.instnace.zServer.DownloadFile(
           GlobalData.BaseUrl + GlobalData.GetImage + ItemInfo.serialId,
           ItemInfo.thumbnailMD5 + ItemInfo.thumbnailExtension, info, false,
           (y) => {
               if (!string.IsNullOrEmpty(y))
               {

                   ZManager.instnace.zServer.QueueOnMainThread((new_y) =>
                   {
                       //Debug.Log(GlobalData.ProjectID + "封面下载结果");
                       Texture2D tex = GlobalData.LoadImage(GlobalData.LocalPath + ItemInfo.thumbnailMD5 + ItemInfo.thumbnailExtension);
                       //Debug.Log(tex.name);
                       ProjectButton.GetComponent<Image>().sprite = GlobalData.Texture2DToSprite(tex);
                   }, y);
               }
           },
           (x) => { Debug.Log("缩略图下载进度：" + x); });
    }

    /// <summary>
    /// 点击项目按钮事件
    /// </summary>
    private void OnProjectButtonClick()
    {
        GlobalData.ProjectID = ItemInfo.serialId;
        //Debug.Log("项目ID " + GlobalData.ProjectID);
        if (IsLocal)
        {
            //本地加载
            LoadProject();
        }
        else
        {
            //使用id下载
            // ItemInfo.id;
            DownloadInfo info = new DownloadInfo();
            info.ProjectID = ItemInfo.serialId;
            info.token = GlobalData.getUserInfo.token;
            LoadProjectResAction(info);
        }
    }

    private void LoadProjectResAction(DownloadInfo info)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("LoadPanel"), transform.root);
        UI_DownLoadPanel t = go.GetComponent<UI_DownLoadPanel>();
        t.DownLoadResourcesFromServer(GlobalData.BaseUrl + GlobalData.DownLoadResUrl, ItemInfo.serialId + ".zip", info,
            (filePath) =>
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }
                //删除旧项目
                GlobalData.DelectDir(GlobalData.LocalPath + ItemInfo.serialId);
                //解析包
                bool IsZip = ZipHelper.UnzipFile(filePath, GlobalData.LocalPath + Path.GetFileNameWithoutExtension(filePath));
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
                //IniFile SettingIni = new IniFile();
                //SettingIni.Load(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ConfigSettingFile);
                //GlobalData.ProjectSettingData = GetModel(SettingIni);
                LoadProject();
            }
            );

    }


    int unzipCont = 0;

    private void LoadProject()
    {
        GlobalData.ProjectSettingData = new ProjectDescriptionDataModel();
        GlobalData.ProjectSettingData.ID = ItemInfo.serialId;
        GlobalData.ProjectSettingData.Type = ItemInfo.category;
        GlobalData.ProjectSettingData.ProjectName = ItemInfo.name;
        if (!string.Equals(GlobalData.ProjectSettingData.Type, "GroundPlane"))
        {
            #region AlvaSDK Imagetarget封装路径
            ///读取Imagetarget.json 获取xml路径
            //string[] path = System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfig3DStr);
            //if (System.IO.File.Exists(path[0]))
            //{
            //    string json3D = System.IO.File.ReadAllText(path[0]);
            //    EditableObjectsDataModel editableObjectsDataModel3D = GlobalData.DeserializeObject<EditableObjectsDataModel>(json3D);
            //    string arPath = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ARSDKPath;
            //    if (!Directory.Exists(arPath))
            //    {
            //        Directory.CreateDirectory(arPath);
            //    }
            //    else
            //    {
            //        Directory.Delete(arPath, true);
            //        if (!Directory.Exists(arPath))
            //        {
            //            Directory.CreateDirectory(arPath);
            //        }
            //    }
            //    //文件夹ID
            //    string[] xmlPath = System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfigDatStr + editableObjectsDataModel3D.style.Split('/')[0]);
            //    Debug.Log(xmlPath.Length);
            //    foreach (var item in xmlPath)
            //    {
            //        if (File.Exists(item))
            //        {
            //            File.Copy(item, arPath + Path.GetFileName(item));
            //        }
            //    }

            //}
            #endregion
        }

        //根据类型判断进入那个类型识别
        switch (GlobalData.ProjectSettingData.Type)
        {
            case "ImageTarget":
                SceneManager.LoadScene("ImageAR");
                break;
            case "ModelTarget":
                SceneManager.LoadScene("ModelAR");
                break;
            case "GroundPlane":
                SceneManager.LoadScene("PlaneAR");
                break;
            default:
                break;
        }
    }

}
