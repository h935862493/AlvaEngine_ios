using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_SettingPanel : MonoBehaviour
{
    public static UI_SettingPanel instance;
    public Button LoginPanelActiveBtn;
    public Button LoginButton;
    public Button UnLoginButton;
    public InputField Email;
    public InputField Password;

    public GameObject TipPanel;
    public GameObject LoginPanel;
    public GameObject UnLoginPanel;
    public GameObject UnLoginTip, TipBg_remainingDay;

    public Text EmailText, text_version, text_deviceModel, text_deviceName, Text_tip, expireTimeText;

    public Sprite 登录置灰;
    public Sprite 登录中;

    private int LoginState = -1;// 0 登录 1 未登录

    public float mTime = 0f;
    bool isAutoLogin = true;
    public GameObject autoLoginTip;
    private void Awake()
    {

        instance = this;
    }

    void Start()
    {
        LoginPanelActiveBtn.onClick.AddListener(OnLoginPanelActiveBtnClick);
        LoginButton.onClick.AddListener(OnLoginBtnClick);
        UnLoginButton.onClick.AddListener(OnUnLoginBtnClick);
        Email.onValueChanged.AddListener(OnInputChange);


        LoginButton.interactable = false;
        //UnLoginTip.SetActive(false);

        //Debug.Log(PlayerPrefs.GetInt(GlobalData.LoginStateStr) + "    " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        OnInitScene(PlayerPrefs.GetInt(GlobalData.LoginStateStr));

        string name = PlayerPrefs.GetString(GlobalData.UserNameStr);
        string paw = PlayerPrefs.GetString(GlobalData.UserPawStr);
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(paw))
        {
            Email.text = name;
            Password.text = paw;
        }
        text_version.text = Application.version + "." + GlobalData.local_version;
        text_deviceModel.text = SystemInfo.deviceModel;
        text_deviceName.text = SystemInfo.deviceName;


        if (GlobalData.isTestVersions)
        {
            Text_tip.text = "登录可以查看自己的项目_<color=red>测试版！！！</color>";
        }
        //Email.text = "alva";
        //Password.text = "1";
        //Email.gameObject.SetActive(false);
        //Password.gameObject.SetActive(false);
        //UnLoginButton.gameObject.SetActive(false);

        //PlayerPrefs.SetInt(GlobalData.AutoLogin, 1);
        //yield return new WaitForSeconds(1);

    }


    private void OnInitScene(int state)
    {
        if (state.Equals(1))
        {
            TipPanel.SetActive(false);
            UnLoginPanel.SetActive(false);
            LoginPanel.SetActive(true);
            EmailText.text = GlobalData.userInfo.profiles.userName;
            expireTimeText.text = GlobalData.expireTime;
        }
    }

    private void OnInputChange(string _value)
    {
        if (!string.IsNullOrEmpty(_value))
        {
            LoginButton.GetComponent<Image>().sprite = 登录中;
            LoginButton.interactable = true;
        }
        else
        {
            LoginButton.GetComponent<Image>().sprite = 登录置灰;
            LoginButton.interactable = false;
        }
    }
    /// <summary>
    /// 退出登录
    /// </summary>
    private void OnUnLoginBtnClick()
    {
        //清数据
        ResetScene();
    }

    public void OnLoginBtnClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接错误，请重试！");
            return;
        }
        string email = Email.text;
        string password = Password.text;
        StartCoroutine(SetTimeLine(LoginButton));
        //如果密码正确，
        //EmailText.text = "";
        LoginJson json = new LoginJson();
        json.deviceSerialId = GlobalData.UserDeviceSerialId;
        json.loginId = Email.text;
        json.loginPassword = Password.text;
        GlobalData.getUserInfo.name = Email.text;
        Debug.Log(json.loginId + json.loginPassword);
        StartCoroutine(ZManager.instnace.zServer.Post(GlobalData.BaseUrl + GlobalData.LoginUrl, null, json, OnLoginActionCallBack, delegate { autoLoginTip.SetActive(false); }));
    }

    public void OnLoginPanelActiveBtnClick()
    {
        TipPanel.SetActive(false);
        UnLoginPanel.SetActive(true);
    }

    IEnumerator SetTimeLine(Button btn)
    {
        btn.interactable = false;
        mTime = Time.time;
        while (Time.time - mTime <= 2f)
        {
            yield return new WaitForSeconds(0.2f);
        }
        btn.interactable = true;
    }

    /// <summary>
    /// 登陸成功回调
    /// </summary>
    /// <param name="handle"></param>
    void OnLoginActionCallBack(DownloadHandler handle)
    {
        //Debug.Log("登录成功handle.text:" + handle.text);
        NewServerMessage<UserInfo> mgs = GlobalData.DeserializeObject<NewServerMessage<UserInfo>>(handle.text);
        if (mgs == null)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接错误，请重试！");
            return;
        }
        if (mgs.code.Equals(0))
        {
            if (mgs.data.permission == null)
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("很抱歉，该账户没有权限!");
            }
            else if (DateTime.Compare(DateTime.Now, mgs.data.permission.expireTime) <= 0)//在有效期内
            {
                if (isAutoLogin)
                    PlayerPrefs.SetInt(GlobalData.AutoLogin, 1);
                else
                    PlayerPrefs.SetInt(GlobalData.AutoLogin, 0);

                GlobalData.expireTime = mgs.data.permission.expireTime.ToString("d");
                expireTimeText.text = GlobalData.expireTime;
                if (mgs.data.permission.remainingDay <= 30)
                {
                    TipBg_remainingDay.SetActive(true);
                }

                if (GlobalData.getUserInfo != null && !string.IsNullOrEmpty(GlobalData.getUserInfo.name))
                {
                    GlobalData.getUserInfo.token = mgs.data.token;

                    OnGetUserInfoActionCallBack(mgs.data);

                    //开始检测账号状态
                    AlvaLicenseHttp.instance.BeginAccountBeat();
                }
            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("很抱歉，您的账户已过有效期");
            }

        }
        else
        {
            string tip = "";
            //登录失败
            switch (mgs.code)
            {
                case 100001:
                    tip = "账户密码错误";
                    break;
                case 100002:
                    tip = "Token验证失败";
                    break;
                case 100003:
                    tip = "Token已过期";
                    break;
                case 100005:
                    tip = "Token为空";
                    break;
                case 300008:
                    tip = "应用程序授权已过期";
                    break;
                case 100016:
                    tip = "应用程序的数量已经用完";
                    break;
                case 401:
                    tip = "该用户没有权限";
                    break;
                default:
                    tip = mgs.message;
                    break;
            }

            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData(tip);// + "<color=red>" + mgs.code + "</color>");
        }
    }
    /// <summary>
    /// 获取用户信息回调
    /// </summary>
    /// <param name="handle"></param>
    void OnGetUserInfoActionCallBack(UserInfo handle)
    {
        Debug.Log("用户信息获取成功");
        if (!string.IsNullOrEmpty(handle.token))
        {
            UnLoginPanel.SetActive(false);
            LoginPanel.SetActive(true);

            GlobalData.userInfo = handle;
            EmailText.text = GlobalData.userInfo.profiles.userName;

            GlobalData.LoginSucessfulAction?.Invoke();

            PlayerPrefs.SetInt(GlobalData.LoginStateStr, 1);

            StartCoroutine(ZManager.instnace.zServer.Get(GlobalData.BaseUrl + GlobalData.GetProjectListUrl,
                new string[2] { "alva_author_token", GlobalData.getUserInfo.token }, null, OnProjectListCallBack));
            //记住登录的账号密码
            PlayerPrefs.SetString(GlobalData.UserNameStr, Email.text);
            PlayerPrefs.SetString(GlobalData.UserPawStr, Password.text);
        }
        autoLoginTip.SetActive(false);
    }

    void OnProjectListCallBack(DownloadHandler handle)
    {
        //print("项目列表：" + handle.text);
        //#if UNITY_EDITOR
        //        StreamWriter streamWriter = new StreamWriter(@"C:\Users\Administrator\Desktop\wre7.txt", true);
        //        streamWriter.Write(handle.text);
        //        streamWriter.Close();
        //#endif
        NewServerMessage<PageProjectInfo> mgs = GlobalData.DeserializeObject<NewServerMessage<PageProjectInfo>>(handle.text);

        if (mgs.code.Equals(0))
        {
            if (mgs.data.items == null || mgs.data.items.Count < 1)
            {
                return;
            }
            //筛选vision平台的项目
            List<VisionProjectInfo> infoList2 = new List<VisionProjectInfo>();
            for (int i = 0; i < mgs.data.items.Count; i++)
            {
                if (mgs.data.items[i].platform != 1)
                    infoList2.Add(mgs.data.items[i]);
            }

            UI_PublicProPanel.instance.UpdatePublicPanel();
            GlobalData.GetProjectListAction?.Invoke(infoList2);

        }
        else
        {
            //获取用户列表失败
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            if (mgs.code == -10003)
            {
                box.SetTipData("您异地登录，登录信息已过期，请重新登录！ " + mgs.code);
            }
            else
            {
                box.SetTipData(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(mgs.message)));
            }
        }
    }

    IEnumerator SetLoginErrorTip()
    {
        UnLoginTip.SetActive(true);
        Debug.Log("登录失败");
        yield return new WaitForSeconds(0.7f);
        UnLoginTip.SetActive(false);
    }

    public void ResetScene()
    {
        //停止检测账号状态
        AlvaLicenseHttp.instance.StopAccountBeat();
        PlayerPrefs.SetInt(GlobalData.AutoLogin, 0);
        LoginButton.GetComponent<Image>().sprite = 登录置灰;
        LoginButton.interactable = false;
        TipPanel.SetActive(true);
        UnLoginPanel.SetActive(false);
        LoginPanel.SetActive(false);
        Email.text = "";
        Password.text = "";
        GlobalData.expireTime = null;
        MyMessageData.pageIndex = 0;
        PlayerPrefs.SetInt(GlobalData.LoginStateStr, 0);
        MyMessageData.publicContentY = 0;
        MyMessageData.projectContentY = 0;
        MyMessageData.locationContentY = 0;
        GlobalData.IsResetSceneAction?.Invoke(true);
    }
    /// <summary>自动登录事件</summary>
    public void AutoLoginEvent()
    {
        autoLoginTip.SetActive(true);
        OnLoginPanelActiveBtnClick();
        OnLoginBtnClick();
    }

    /// <summary>自动登录开关</summary>
    public void ToggleValueChange(bool ison)
    {
        print("自动登录ison=" + ison);
        isAutoLogin = ison;
    }

    private void OnDestroy()
    {
        LoginPanelActiveBtn.onClick.RemoveAllListeners();
        LoginButton.onClick.RemoveAllListeners();
        UnLoginButton.onClick.RemoveAllListeners();
        Email.onValueChanged.RemoveAllListeners();
    }
}
