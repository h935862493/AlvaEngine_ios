using Alva.Recognition;
using arsdk;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AlvaLicenseHttp : MonoBehaviour
{
    public static AlvaLicenseHttp instance;
    bool isSurport = false;
    string imageAuth, modelAuth;
    public GameObject tipgo, updateApp;
    public Text tip_code, tip_updateApp;
    [HideInInspector]
    public Text progressText;

    void Start()
    {
        instance = this;

        if (!MyMessageData.isFirstCheck)
        {
            return;
        }
        //Application.lowMemory += OnLowMemory;
        MyMessageData.isFirstCheck = false;

        //Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Invoke("Init0", 1);
    }

    private void Init0()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)//当网络不可用时 
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络异常，请检查网络连接");
        }
        else
        {
            if (!GlobalData.isTestVersions)
            {
                StartCoroutine(ZManager.instnace.zServer.Get(GlobalData.UpgradeUrl, null, null, UpgradeCallBack));
            }
        }
        Init();
    }

    void UpgradeCallBack(DownloadHandler handle)
    {
        //print("请求版本返回:" + handle.text);
        if (string.IsNullOrEmpty(handle.text))
        {
            return;
        }
        AppleRoot mgs = GlobalData.DeserializeObject<AppleRoot>(handle.text);

        if (mgs.results.Count > 0)
        {
            if (Application.version != mgs.results[0].version)//更新
            {
                var appleVersion = mgs.results[0].version.Split('.');//2.0.12
                var localVersion = Application.version.Split('.');//2.0.16
                 if (int.Parse(localVersion[0]) < int.Parse(appleVersion[0]) ||
                   (int.Parse(localVersion[0]) == int.Parse(appleVersion[0]) && int.Parse(localVersion[1]) < int.Parse(appleVersion[1])) ||
                   (int.Parse(localVersion[0]) == int.Parse(appleVersion[0]) && int.Parse(localVersion[1]) == int.Parse(appleVersion[1]) && int.Parse(localVersion[2]) < int.Parse(appleVersion[2])))
                {
                    tip_updateApp.text = "发现新版本" + mgs.results[0].version + "，请到AppStore更新!";
                    updateApp.SetActive(true);
                }
            }
        }
    }
    private void Init()
    {
#if UNITY_EDITOR
        //Debug.Log("现在是editor环境，不检测核心码");
        return;
#elif UNITY_ANDROID
       // Debug.Log("现在是ANDROID");
#elif UNITY_IOS
       // Debug.Log("现在是IOS");
#else
       // Debug.Log("其他平台");
#endif

        imageAuth = PlayerPrefs.GetString("AlvaImageTarget");
        modelAuth = PlayerPrefs.GetString("AlvaModelTarget");

        // print("imageAuth:" + imageAuth);
        if (string.IsNullOrEmpty(imageAuth))
        {
            PlayerPrefs.SetString("AlvaImageTarget", GlobalData.imageAuth);
            PlayerPrefs.SetString("AlvaModelTarget", GlobalData.modelAuth);

        }
        imageAuth = PlayerPrefs.GetString("AlvaImageTarget");

        CheckLicense();

        //Debug.Log("设备模型：" + SystemInfo.deviceModel);
        //Debug.Log("设备名称：" + SystemInfo.deviceName);
        //Debug.Log("设备类型：" + SystemInfo.deviceType);
        //Debug.Log("系统内存大小（单位：MB）：" + SystemInfo.systemMemorySize);

        //请求白名单
        if (Application.internetReachability != NetworkReachability.NotReachable)
            StartCoroutine(ZManager.instnace.zServer.Get("https://appstore.alva.com.cn/whiteApi/list/" + SystemInfo.deviceModel, null, null, WhiteListCallBack));
    }

    private void Update()
    {
        if (MyMessageData._asyncOperation != null)
        {
            //print("加载场景进度:" + (MyMessageData._asyncOperation.progress * 100).ToString("0.0") + "%");
            if (progressText)
            {
                progressText.text = "加载场景进度:" + (MyMessageData._asyncOperation.progress * 100).ToString("0.0") + "%";
            }

            if (MyMessageData._asyncOperation.isDone)
            {
                //print("场景切换完毕");
                if (progressText)
                {
                    progressText.text = "";
                }
                if (HxMainManager.instance)
                {
                    HxMainManager.instance.loadAni.SetActive(false);
                }
                MyMessageData._asyncOperation = null;
                if (!FindObjectOfType<ImageRecognition>() && !FindObjectOfType<ModelRecognition>()
                    && !FindObjectOfType<SlamRecognition>() && !FindObjectOfType<SwitchScene>()
                    && !FindObjectOfType<UnityARCameraManager>() && !FindObjectOfType<ImageTargetCameraManager>()
                    && !FindObjectOfType<HxSLAMManager>() && !FindObjectOfType<ALVAModelTarget>())
                {
                    var g = Instantiate(Resources.Load<GameObject>("AlvaCore/Canvas_visions_main"));
                    g.GetComponent<LoadingErrorTips>().ShowTip("没有找到识别组件，请在编辑器确认");
                }

            }
        }
    }

    //检测核心是否可用
    void CheckLicense()
    {
        //print("检测核心是否可用");
        if (Application.internetReachability == NetworkReachability.NotReachable)//当网络不可用时 
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("网络连接失败，请重试。", (IsYes) =>
            {
                if (!IsYes)
                {
                    return;
                }

            });
        }
        else
        {
            //print("333imageAuth:" + imageAuth);
            //初始化alva核心环境

            int cadStatus = AlvaARWrapper.Instance.InitIR(AlvaARUnity.AlvaDataType.Alva_Memory, MyMessageData.companyName, imageAuth);
            //Debug.Log("444arsdk1 IR_Init " + cadStatus);
            if (cadStatus != 0)
            {
                //tip_code.text = cadStatus.ToString();
                //tipgo.SetActive(true);
                GetLicense();
            }
            arsdk.AlvaARWrapper.Instance.UnitIR();
        }
    }
    //获取核心码
    public void GetLicense()
    {
        //print("获取核心码");
        if (Application.internetReachability == NetworkReachability.NotReachable)//当网络不可用时 
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("网络连接失败，请重试。", (IsYes) =>
            {
                if (!IsYes)
                {
                    return;
                }
            });
        }
        else
        {
            //1=ImageTarget授权码，2=ModelTarget授权码
            StartCoroutine(ZManager.instnace.zServer.Get("https://editorapi.alva.com.cn/api/client/auth/current/1", null, null, ImageTargetCallBack));
            StartCoroutine(ZManager.instnace.zServer.Get("https://editorapi.alva.com.cn/api/client/auth/current/2", null, null, ModelTargetCallBack));
        }
    }

    void ImageTargetCallBack(DownloadHandler handle)
    {

        NewServerMessage<AlvaLicenseData> mgs = GlobalData.DeserializeObject<NewServerMessage<AlvaLicenseData>>(handle.text);
        //print("获取核心码ImageTarget返回：" + mgs.data.clientSecret);
        if (mgs.code.Equals(0))
        {
            PlayerPrefs.SetString("AlvaImageTarget", mgs.data.clientSecret);
        }
    }

    void ModelTargetCallBack(DownloadHandler handle)
    {
        NewServerMessage<AlvaLicenseData> mgs = GlobalData.DeserializeObject<NewServerMessage<AlvaLicenseData>>(handle.text);
        //print("获取核心码ModelTarget返回：" + mgs.data.clientSecret);
        if (mgs.code.Equals(0))
        {
            PlayerPrefs.SetString("AlvaModelTarget", mgs.data.clientSecret);
        }
    }
    //取消
    public void Cancel_btn()
    {
        tipgo.SetActive(false);
    }

    public void OK_btn()
    {
        tipgo.SetActive(false);
        GetLicense();
    }
    public class AlvaLicenseData
    {
        public string serialId { get; set; }
        public string description { get; set; }
        public int clientType { get; set; }
        public string clientKey { get; set; }
        public string clientSecret { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
    }
    public class WhiteListData
    {
        public string code { get; set; }
        public string desc { get; set; }
        public int functionValue { get; set; }
    }

    public bool OnCheckDevice()
    {
#if UNITY_EDITOR
        Debug.Log("现在是editor环境，不检测核心码");
        return true;
#endif

        if (Application.internetReachability == NetworkReachability.NotReachable || !isWhiteBack)
        {
            if (SystemInfo.systemMemorySize > 2048)
                isSurport = true;
            else
                isSurport = false;
        }
        return isSurport;

    }
    //请求白名单有结果
    bool isWhiteBack = false;
    void WhiteListCallBack(DownloadHandler handle)
    {
        if (!string.IsNullOrEmpty(handle.text))
        {
            NewServerMessage<List<WhiteListData>> mgs = GlobalData.DeserializeObject<NewServerMessage<List<WhiteListData>>>(handle.text);
            //print("请求白名单返回:" + handle.text);
            isWhiteBack = true;
            if (mgs.code == 200 && mgs.data.Count > 0)//后台有记录，肯定知道支持不支持
            {
                WhiteListData whitedata = mgs.data.Find((x) => x.code == "vision");
                if (whitedata != null)
                {
                    if (whitedata.functionValue == 0)//0=不支持，10=完美支持
                        isSurport = false;
                    else
                        isSurport = true;
                }
            }
            else//后台没记录，用运行内存判断
            {
                if (SystemInfo.systemMemorySize > 2048)
                {
                    isSurport = true;
                }
                else
                    isSurport = false;
            }
        }
        else//后台没记录，用运行内存判断
        {
            if (SystemInfo.systemMemorySize > 2048)
            {
                isSurport = true;
            }
            else
                isSurport = false;
        }

    }

    #region json
    public class AppleResults
    {
        public string artworkUrl60 { get; set; }
        public string artworkUrl512 { get; set; }
        public string artworkUrl100 { get; set; }
        public string artistViewUrl { get; set; }
        public List<string> screenshotUrls { get; set; }
        public List<string> appletvScreenshotUrls { get; set; }
        public List<string> ipadScreenshotUrls { get; set; }
        public List<string> features { get; set; }
        public List<string> supportedDevices { get; set; }
        public List<string> advisories { get; set; }
        public bool isGameCenterEnabled { get; set; }
        public string kind { get; set; }
        public string trackViewUrl { get; set; }
        public string minimumOsVersion { get; set; }
        public string trackCensoredName { get; set; }
        public List<string> languageCodesISO2A { get; set; }
        public string fileSizeBytes { get; set; }
        public string formattedPrice { get; set; }
        public string contentAdvisoryRating { get; set; }
        public Double averageUserRatingForCurrentVersion { get; set; }
        public int userRatingCountForCurrentVersion { get; set; }
        public Double averageUserRating { get; set; }
        public string trackContentRating { get; set; }
        public string bundleId { get; set; }
        public int trackId { get; set; }
        public string trackName { get; set; }
        public string releaseDate { get; set; }
        public string sellerName { get; set; }
        public string primaryGenreName { get; set; }
        public List<string> genreIds { get; set; }
        public bool isVppDeviceBasedLicensingEnabled { get; set; }
        public string currentVersionReleaseDate { get; set; }
        public string releaseNotes { get; set; }
        public int primaryGenreId { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public int artistId { get; set; }
        public string artistName { get; set; }
        public List<string> genres { get; set; }
        public Double price { get; set; }
        public string version { get; set; }
        public string wrapperType { get; set; }
        public int userRatingCount { get; set; }
    }

    public class AppleRoot
    {
        public int resultCount { get; set; }
        public List<AppleResults> results { get; set; }
    }
    #endregion

    #region 检测账号是否被顶
    int codeAccount = -1;
    string tipAccount = "";

    public void BeginAccountBeat()
    {
        InvokeRepeating("AccountBeat", 0, 600);
    }
    public void StopAccountBeat()
    {
        CancelInvoke("AccountBeat");
    }
    void AccountBeat()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)//有网
            StartCoroutine(ZManager.instnace.zServer.Get(GlobalData.BaseUrl + GlobalData.GetAccountBeat, new string[2] { "alva_author_token", GlobalData.getUserInfo.token }, null, AccountBeatCallBack));
        else
            StopAccountBeat();
    }

    void AccountBeatCallBack(DownloadHandler handle)
    {
        if (MyMessageData._asyncOperation != null)//加载场景过程中
        {
            return;
        }
        JsonData data = JsonMapper.ToObject(handle.text);

        codeAccount = (int)data["code"];

        if (codeAccount != 0)
        {
            switch (codeAccount)
            {
                case -10002:
                    tipAccount = "账号信息超时，请重新登录";
                    break;
                case -10003:
                    tipAccount = "账号重复登录";
                    break;
                default:
                    tipAccount = (string)data["message"];
                    break;
            }
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData(tipAccount);// + "<color=red>" + codeAccount + "</color>");
            if (UI_SettingPanel.instance)
                UI_SettingPanel.instance.ResetScene();
            else
            {
                StopAccountBeat();
                MyMessageData.pageIndex = 0;
                PlayerPrefs.SetInt(GlobalData.LoginStateStr, 0);
            }

        }

    }
    #endregion
}
