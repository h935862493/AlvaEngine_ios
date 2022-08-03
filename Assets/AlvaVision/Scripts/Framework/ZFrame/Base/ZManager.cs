using System;
using UnityEngine;

public class ZManager : MonoBehaviour
{
    public static ZManager instnace;
    public UnityEngine.UI.Button btn;

    #region 注册

    public ZServer zServer;
    public ZLoadARRes zLoadARRes;
    public ZBindPermissions zBindPermissions;

    void Register()
    {
        zServer = CreateScriptApi(typeof(ZServer)) as ZServer;
        zLoadARRes = CreateScriptApi(typeof(ZLoadARRes)) as ZLoadARRes;
        zBindPermissions = CreateScriptApi(typeof(ZBindPermissions)) as ZBindPermissions;
    }


    #endregion

    private void Awake()
    {
       // print("*********************");
        // arsdk.AlvaARConfiguration.Instance.AlvaARConfig.LicenseKey = "565C4DA6F1F3A17EFAA692034B1AB16ED1AD13A6F71235042C22C2E77EA0140648184AA221D2BD19EEAC114BFA175C24B26ADC09760A8C604DCECEDE9E703A78F599E1C811C7CB26DBAB208704AD9E3A002E154EEA54763DC83CDD9A2F358E18F69FCB0D1A11221AA199202048184012810924460210025A2508226612996458A591022412116248218000429BE5FE9BEDFBEF9DFBAFF7F9FFFEE7FD99F576B9E7DBFFFBDDE5EFFFE7DB77999FAD67FFBDFF6EDD4141C7C614438B90B160FC7871BCEBD254100188606200985C74874FB2F3EF16D7057B9ABC3F4FD517769FB6021D40A30DBB7BACC6EE1A0863179121AE1BC406EE67306FDD9DFC386506CF8B";
        //Debug.LogError("执行了ZManager的Awake了");

        if (GlobalData.IsBegin == false)
        {
            DontDestroyOnLoad(gameObject);
            instnace = this;
            Register();
            PlayerPrefs.SetInt(GlobalData.LoginStateStr, 0);
            GlobalData.IsBegin = true;

           #if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId("7cd8b704b2");
#elif UNITY_ANDROID
            BuglyAgent.InitWithAppId("f0fc81ab77");
#endif
            BuglyAgent.EnableExceptionHandler();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        //Debug.LogError("执行了ZManager的Start了");
        ZBase[] hrArray = GetComponentsInChildren<ZBase>();
        foreach (var item in hrArray)
        {
            item.OnInstance();
        }
        foreach (var item in hrArray)
        {
            item.OnInitComp();
        }
        foreach (var item in hrArray)
        {
            item.OnInitData();
        }
        foreach (var item in hrArray)
        {
            item.OnInitFunc();
        }
    }

    //private void Update()
    //{
    //if (Input.GetMouseButtonDown(0))
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hitInfo;
    //    if (Physics.Raycast(ray, out hitInfo))
    //    {
    //        GameObject gameObj = hitInfo.collider.gameObject;
    //        if (gameObj.name == "point")
    //        {
    //            if (gameObj.GetComponentInParent<Hotspot>())
    //            {
    //                gameObj.GetComponentInParent<Hotspot>().InitObjectAvtive();
    //            }
    //        }
    //    }
    //}
    //}

    /// <summary>
    /// 注册代码
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    ZBase CreateScriptApi(Type type)
    {
        ZBase zr = GetComponentInChildren(type) as ZBase;
        if (zr) return zr;

        GameObject go = new GameObject(type.Name);
        go.transform.SetParent(transform);
        go.AddComponent(type);
        zr = go.GetComponent<ZBase>();
        return zr;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(GlobalData.LoginStateStr, 0);
    }

}
