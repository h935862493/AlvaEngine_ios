using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System.Text;
using System;

public static class GlobalData
{
    /// <summary> true=测试版, false=正式版</summary>
    public static bool isTestVersions = false;
    /// <summary>对应编辑器版本号的最后一位</summary>
    public static int local_version = 66;
    /// <summary>默认模型识别授权码2022.07.15To2022.10.14</summary>
    public static string modelAuth = "9092243A5FEE4A3C38CE8210F01E6401EAA36110624A3014C58B2542B1BE3C944100E5CA0A8858B356BD524E40881DBFCBC1A7A6508302C0BE90BD1193F58672F678829F12EFA46E8670002B22EAE00D826BD8558F8C5687FB484CEB117FA9E2B02D2AC10960240810224A2400042650104652219042241809245AA411662452096642241120445898600081E6DDFDAD6E99E7DBFF9FDFFF7E99F7FB7FBDDFEFEEDDEDDA77DDD9BFE79FE5DEEED9D9AFEEF9EFFA8F4BFBEF27A1A1AFCB794FDFA34A7CA3FB858162000008269763FE7AAE0EA474AF493DBBE2E6CF9332EC7B98560928B042EF9972161B501E49F51D822502D48746FD07A6438D1E29226ED762";
    /// <summary>默认图片识别授权码2022.07.15To2022.10.14</summary>
    public static string imageAuth = "5319F5147CF57A56C18F0347611D1434F5122644AF044012361C462611C1306E044BC421419976C0162EB8A7C284D4A56118B6113010216455D1F14C2C2AC754BC2C6B25C09FFC7A14941461C29B2EEA84C865C8D59487267A9B41EB07C228E3777F22D724912604108020508480004008886618250140200008601084102046001144528599464250016010FF5E7FB9DDA577D9EF5B66FF9DAFEEBBE5DEEED9BBA5FEBDE75F66BBDDAFE69BEFDEEFDBBBBFEEDFADFD22D5E70888DBF7D1B0E067E69E18634008810052840896931FFA0F81E5E4BF0FC9455678FB84CB8494874C8D5DEC96104248620761A00B9D0CF70F41BB289E9A41DADE6F3ADDF50715A8";
    public static string BaseUrl
    {
        get
        {
            if (isTestVersions)
            {
                //测试服
                //return "http://47.105.194.106:8202";
                return "https://editorapi-t.alva.com.cn";
            }
            else
            {
                //正式服
                return "https://ed.alva.com.cn";
                //紫晶
                //return "http://172.16.56.174:9003";
            }
        }
    }

    public static string AlvaAppStoreUrl
    {
        get
        {
            if (isTestVersions)
            {
                //测试服
                return "http://rb.alva.com.cn:9091";
            }
            else
            {
                //正式服
                return "https://appstore.alva.com.cn";
            }
        }
    }
    /// <summary>后台获取版本号</summary>
    public static string AppVersionUrl
    {
        get
        {
            if (isTestVersions)
            {
                //测试服  版本类型,1测试，2，正式
                return "/version/appVersion/upgrade/vision/Android/1";
            }
            else
            {
                //正式服
                return "/version/appVersion/upgrade/vision/Android/2";
            }
        }
    }
    /// <summary>苹果专用</summary>
    public static string UpgradeUrl = "https://itunes.apple.com/cn/lookup?id=1520157964";
    /// <summary>生成二维码认证码:expmin 认证码过期时间( 分钟 )</summary>
    public static string QRcodeUrl = "/api/user/auth/qrcode?expmin=3";

    public static string BaseUrl_vision = "http://editor.alva.com.cn/";
    public static string BaseUrl_rainbow = "http://47.105.194.106:8112/";
    public static string BaseUrl_rainbow_download = "http://47.105.194.106:8110/";//下载

    public static string LoginUrl = "/api/user/sso/auth/vision";//登录接口地址
    public static string LoginUrl_vision = "api/user/auth";
    public static string LoginUrl_rainbow = "api/user/sso/auth";

    public static string GetAccountBeat = "/api/user";
    public static string GetInfoUrl = "UserAPI/GetInfo";//获取用户信息
    public static string GetProjectListUrl = "/api/project/grant?index=1&size=80&serialId=&serialIds=&name=&category=&shareCode=&visibleState=&platform=2";
    public static string DownLoadResUrl = "api/project/download/attachment/";//下载资源
    public static string SetTopUrl = "/api/project/top/";//项目置顶
    public static string LogoutUrl = "UserAPI/Logout";//登出
    public static string GetImage = "api/project/download/thumbnail/";
    public static string GetPublicProjectListUrl = "/api/project/list/public?platform=2";//获取公共项目列表
    public static string DownLoadModelIDUrl = "api/model/download";//根据ID下载模型
    public static string ModelListUrl = "api/model/list";

    public static string MarkJsonUrl = "api/model/demo";//根据MarkID获取对应Json
    public static string MarkJsonModelIDUrl = "api/model/demo/download";//根据ID下载模型(远景测试用)

    public static List<GameObject> EditableDataTaskObjs = new List<GameObject>();

    public static string UploadProjectUrl = "ProInfoAPI/Upload";
    /// <summary>
    /// 登录状态记录  0 未登录 1 登录
    /// </summary>
    public const string LoginStateStr = "LoginState";//登录状态记录  0 未登录 1 登录
    public const string UserNameStr = "username";//上次登录的用户名
    public const string UserPawStr = "userpaw";//上次登录的密码
    /// <summary>自动登录，0=否，1=是</summary>
    public const string AutoLogin = "AutoLogin";
    public const string UserDeviceSerialId = "06a64536e6b04510abc406d4a4daeeef";//客户端标识唯一码
    public static string expireTime;

    public static List<VisionProjectInfo> proInfo = new List<VisionProjectInfo>();
    public static List<VisionProjectInfo> publicInfo = new List<VisionProjectInfo>();
    //public static List<ProjectUIItem> ProjectGameObjects = new List<ProjectUIItem>();
    //public static List<ProjectUIItem> ProjectLocalGameObjects = new List<ProjectUIItem>();
    public static List<ProjectUIItem> PublicGameObjects = new List<ProjectUIItem>();

    public static List<GameObject> LocalPanelItemObjList = new List<GameObject>();

    public static List<string> AniNameList = new List<string>();
    public static int AniCurrentIndex = -1;

    public static GetUserInfo getUserInfo = new GetUserInfo();
    public static List<StepBind> BindsList = new List<StepBind>();
    public static UserInfo userInfo = new UserInfo();

    public static bool IsBegin = false;
    public static float pageValue;
    /// <summary>
    /// 滑屏时候调用
    /// </summary>
    public static Action<int> SetPageIndexAction;
    public static Action<bool> IsTrackerFoundAction;
    public static Action<bool> IsResetSceneAction;
    public static Action<GameObject> UpdateLocationProjectAction;
    public static Action LoginSucessfulAction;
    public static Action<List<GameObject>> SetOptionTypeObject;
    public static Action<int> SetOptionPage;
    public static Action<bool> IsModelAniPlayAction;
    public static Action IsInitAniPlayAction;

    public static Action<List<VisionProjectInfo>> GetProjectListAction;
    public static Action<List<VisionProjectInfo>, string> RefreshProjectListAction;

    public static Action<string, string> FileSelectAction;
    /// <summary>
    /// 删除项目事件,项目信息
    /// </summary>
    public static Action<VisionProjectInfo> DelectProjectAction;

    public static ProjectDescriptionDataModel ProjectSettingData = new ProjectDescriptionDataModel();

    public const string ProjectConfig3DStr = "Library/3D/";
    public const string ProjectConfig2DStr = "Library/2D/";
    public const string ProjectResStr = "Asset";
    public static string ProjectConfigDatStr = "Library/Dat/";
    public const string ProjectTaskStr = "Library/Task";
    public const string ProjectAnimationEventStr = "Library/AnimationEvent/event.json";

    public static Dictionary<string, string> ImageTargetPathStrList = new Dictionary<string, string>();

    public const string ConfigJson3D = "PlayerPrefs3D.json";
    public const string ConfigJson2D = "PlayerPrefs2D.json";

    public const string ARSDKPath = "Modeul/";

    public const string ProjectItemPrefabPath = "UI/ProjectItem";

    public static string ProjectIDZIPUrl
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                localPath = @"E:/hjz/3d.zip";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                localPath = Application.persistentDataPath + "/" + "20200303.zip";
            }
            else//ios
            {
                localPath = Application.temporaryCachePath + "/" + "20200303.zip";
            }
            return localPath;
        }
    }

    public static string ConfigSettingFile = "ProjectConfig";

    public static Pro2DRecognition RecognitionType = Pro2DRecognition.None;

    public static string ProjectID = "";

    public static Vector2 ResolutionRatio = Vector2.one;

    private static string localPath;
    public static string LocalPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                localPath = @"E:/AlvaView/";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                localPath = @"/Users/alva/hx/AlvaView/";
                //localPath = Application.persistentDataPath + "/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                localPath = Application.persistentDataPath + "/";
            }
            else//ios
            {
                localPath = Application.temporaryCachePath + "/";
            }
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            return localPath;
        }

        set
        {
            localPath = value;
        }
    }

    public static string VersionLocalPath = LocalPath + "Version.json";
    public static string ModelVersionPath = LocalPath + "ModelVersion.json";

    public static VersionData VersionLocalData, versionTempData;
    public static List<VersionData> AllUserVersionLocalData;

    /// <summary> 读取缓存的json记录 </summary>
    public static List<VersionData> GetLocalCatchRes()
    {
        string s = ReadCachedJson(GlobalData.VersionLocalPath);
        List<VersionData> infoList = new List<VersionData>();
        if (string.IsNullOrEmpty(s))
        {
            //返回缓存记录
            return infoList;
        }
        else
        {
            //将缓存的数据local.json读到一个json对象中
            infoList = DeserializeObject<List<VersionData>>(s);
            //返回缓存记录
            return infoList;
        }
    }

    #region 读取与写入缓存记录的Json
    /// <summary>
    /// 写入缓存记录的Json
    /// </summary>
    public static void WriteCachedJson(string jsonString, string localPath)
    {
        //Debug.Log("====================写入缓存记录==========================");
        //如果文件名存在则覆盖重新创建
        FileStream fs = new FileStream(localPath, FileMode.Create);
        byte[] bytes = new UTF8Encoding().GetBytes(jsonString);
        //Debug.Log("====================缓存记录大小：" + bytes.Length + " ==========================");
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }

    /// <summary>
    /// 读取缓存记录的Json
    /// </summary>
    /// <returns></returns>
    public static string ReadCachedJson(string localPath)
    {
        try
        {
            if (File.Exists(localPath))
            {
                //FileStream fs = new FileStream(filePath, FileMode.Open);
                //byte[] bytes = new byte[2048];
                //fs.Read(bytes, 0, bytes.Length);
                ////将读取到的二进制转换成字符串
                //string s = new UTF8Encoding().GetString(bytes);
                //fs.Close();
                string s = File.ReadAllText(localPath);
                return s;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
            throw;
        }

        return null;
    }
    #endregion

    #region 时间格式转换
    /// <summary>
    /// DateTime --> long
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static long ConvertDataTimeToLong(DateTime dt)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = dt.Subtract(dtStart);
        long timeStamp = toNow.Ticks;
        timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
        return timeStamp;
    }

    /// <summary>
    /// long --> DateTime
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static DateTime ConvertLongToDateTime(long d)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(d + "0000");
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime dtResult = dtStart.Add(toNow);
        return dtResult;
    }
    #endregion

    #region 解析Json工具方法
    /// <summary>
    /// 反序列化json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_json"></param>
    /// <returns></returns>
    public static T DeserializeObject<T>(string _json)
    {
        T json = JsonMapper.ToObject<T>(_json);
        return json;
    }

    /// <summary>
    /// 序列化json
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeObject(object obj)
    {
        string jsonData = JsonMapper.ToJson(obj);
        return jsonData;
    }
    #endregion

    /// <summary>
    /// 删除指定路径下的子目录和文件
    /// </summary>
    /// <param name="srcPath"></param>
    public static void DelectDir(string srcPath)
    {
        try
        {
            Debug.Log("删除目录:" + srcPath);
            if (!Directory.Exists(srcPath))
            {
                Debug.Log("删除目录不存在:" + srcPath);
                return;
            }
            DirectoryInfo subdir = new DirectoryInfo(srcPath);
            subdir.Delete(true);
            //DirectoryInfo dir = new DirectoryInfo(srcPath);
            //FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            //foreach (FileSystemInfo i in fileinfo)
            //{
            //    if (i is DirectoryInfo)            //判断是否文件夹
            //    {
            //        Debug.Log("目录 " + i);
            //        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
            //        subdir.Delete(true);          //删除子目录和文件
            //    }
            //    else
            //    {
            //        Debug.Log("目录删除 " + i);
            //        File.Delete(i.FullName);      //删除指定文件
            //    }
            //}
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    #region 图片格式转换
    static Dictionary<string, byte[]> fileBytesDir = new Dictionary<string, byte[]>();
    public static Texture2D LoadImage(string fileName)
    {
        Texture2D texture2D = new Texture2D(10, 10);
        if (fileBytesDir.ContainsKey(fileName))
        {
            texture2D.LoadImage(fileBytesDir[fileName]);
        }
        else
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int byteLength = (int)fileStream.Length;
            byte[] fileBytes = new byte[byteLength];
            fileStream.Read(fileBytes, 0, byteLength);
            //fileStream.Close();
            fileStream.Dispose();
            fileBytesDir.Add(fileName, fileBytes);
            texture2D.LoadImage(fileBytes);
        }

        return texture2D;
    }
    public static Sprite Texture2DToSprite(Texture2D texture)
    {
        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return sp;
    }
    #endregion

    /// <summary>
    /// 加载特效
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static List<GameObject[]> LoadParticleSystem(string filePath)
    {
        List<GameObject[]> particleSystemList = new List<GameObject[]>();
        string FilePathStr = "";
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.Android)
        {
            FilePathStr = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            FilePathStr = "iOS";
        }
        string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "/" + FilePathStr));
        foreach (var item in files)
        {
            if (!Path.GetExtension(item).Contains(".manifest") && !Path.GetFileName(item).Contains(FilePathStr))
            {
                AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(item);
                if (myLoadedAssetBundle == null)
                {
                    Debug.Log("Failed to load AssetBundle!");
                    continue;
                }
                Debug.Log(item);
                var prefab = myLoadedAssetBundle.LoadAllAssets<GameObject>();
                particleSystemList.Add(prefab);
                myLoadedAssetBundle.Unload(false);
            }
        }
        return particleSystemList;
    }

    public static IEnumerator LoadAudio(string filePath, AudioSource audioSource)
    {
        Uri uri = new Uri(filePath);
        filePath = uri.AbsoluteUri;
        Debug.Log(filePath);
        using (var uwr = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            //((DownloadHandlerAudioClip)uwr.downloadHandler).compressed = false;
            //((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                Debug.Log(1);
                audioSource.clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(uwr);
            }
        }
    }

    /// <summary>
    /// 创建Guid
    /// </summary>
    /// <param name="id"> id </param>
    /// <param name="length"> id长度 </param>
    public static void CreateID(ref string id, int length = 0)
    {
        id = Guid.NewGuid().ToString("N");
        if (length != 0)
            id = id.Substring(0, length);
    }

    public static Bounds CalculateBounds(GameObject model, float minBoundsSize = 0.1f)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        Vector3 scale = model.transform.localScale;
        model.transform.localScale = Vector3.one;

        if (renderers.Length == 0)
        {
            return new Bounds(model.transform.position, Vector2.one * minBoundsSize);
        }
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        model.transform.localScale = scale;
        return bounds;
    }
    /// <summary>获取符合平台条件的项目内容1-Android、2-IOS、3-HoloLens、4-其它</summary>
    /// <param name="category"> 项目包类别( 1-Android、2-IOS、3-HoloLens、4-其它 </param>
    public static ResPackages GetTargetPlatform(VisionProjectInfo ItemInfo)
    {
        foreach (var item in ItemInfo.resPackages)
        {

            if (item.category == GetCategory())
            {
                return item;
            }
        }
        var p = new ResPackages();
        p.resPackage = new ResPackage();
        p.resPackage.file = new ResPackageFile();
        return p;
    }

    /// <summary>判断是不是这个平台</summary>
    public static bool JudgePlatform(VisionProjectInfo ItemInfo)
    {

        foreach (var item in ItemInfo.resPackages)
        {
            if (item.category == GetCategory())
            {
                return true;
            }
        }
        return false;
    }
    static int category = 0;
    /// <summary>平台category</summary>
    public static int GetCategory()
    {

#if UNITY_ANDROID //|| UNITY_EDITOR_WIN
        category = 1;
#elif UNITY_IOS //|| UNITY_EDITOR_OSX
        category = 2;
#else
        category = 4;
#endif
        return category;
    }

    /// <summary>按平台获取提示</summary>
    /// <param name="num">1=没有资源的错误提示，2=资源名字, 3=黑名单提示</param>
    public static string GetPlatformInfo(int num)
    {
        if (num == 1)
        {
#if UNITY_ANDROID //|| UNITY_EDITOR_WIN
            return "没有安卓平台的场景资源";
#elif UNITY_IOS //|| UNITY_EDITOR_OSX
        return "没有IOS平台的场景资源";
#else
        return "没有其他平台的场景资源";
#endif
        }

        else if (num == 2)
        {
#if UNITY_ANDROID //|| UNITY_EDITOR_WIN
            return "/Android.unity3d";
#elif UNITY_IOS //|| UNITY_EDITOR_OSX
        return "/iOS.unity3d";
#else
        return "/TvT.unity3d";
#endif
        }

        if (num == 3)
        {
#if UNITY_ANDROID || UNITY_EDITOR_WIN
            return "很遗憾！您的设备暂不支持体验AR功能";
#elif UNITY_IOS || UNITY_EDITOR_OSX
            return "很遗憾！您的设备运行内存不足，无法体验AR功能" + SystemInfo.systemMemorySize;
#else
        return "没有其他平台的场景资源";
#endif
        }

        return null;
    }
}
