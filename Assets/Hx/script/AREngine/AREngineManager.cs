using Alva.Recognition;
using arsdk;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AREngineManager : MonoBehaviour
{
    public GameObject load;
    public Text progressText;

    string alvaInitErrorTip;

    private void Update()
    {
        if (MyMessageData._asyncOperation != null)
        {
            print("加载场景进度:" + (MyMessageData._asyncOperation.progress * 100).ToString("0.0") + "%");
            if (progressText)
            {
                progressText.text = "加载场景进度:" + (MyMessageData._asyncOperation.progress * 100).ToString("0.0") + "%";
            }

            if (MyMessageData._asyncOperation.isDone)
            {
                print("场景切换完毕");
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

    public void GoModel()
    {
        //GetInfo(@"E:\AlvaView\model,MainScene,2,9092243A5FEE4A3C38CE8210F01E6401EAA36110624A3014C58B2542B1BE3C944100E5CA0A8858B356BD524E40881DBFCBC1A7A6508302C0BE90BD1193F58672F678829F12EFA46E8670002B22EAE00D826BD8558F8C5687FB484CEB117FA9E2B02D2AC10960240810224A2400042650104652219042241809245AA411662452096642241120445898600081E6DDFDAD6E99E7DBFF9FDFFF7E99F7FB7FBDDFEFEEDDEDDA77DDD9BFE79FE5DEEED9D9AFEEF9EFFA8F4BFBEF27A1A1AFCB794FDFA34A7CA3FB858162000008269763FE7AAE0EA474AF493DBBE2E6CF9332EC7B98560928B042EF9972161B501E49F51D822502D48746FD07A6438D1E29226ED762");
        GetInfo(Application.temporaryCachePath + "/model" + ",MainScene,2,9092243A5FEE4A3C38CE8210F01E6401EAA36110624A3014C58B2542B1BE3C944100E5CA0A8858B356BD524E40881DBFCBC1A7A6508302C0BE90BD1193F58672F678829F12EFA46E8670002B22EAE00D826BD8558F8C5687FB484CEB117FA9E2B02D2AC10960240810224A2400042650104652219042241809245AA411662452096642241120445898600081E6DDFDAD6E99E7DBFF9FDFFF7E99F7FB7FBDDFEFEEDDEDDA77DDD9BFE79FE5DEEED9D9AFEEF9EFFA8F4BFBEF27A1A1AFCB794FDFA34A7CA3FB858162000008269763FE7AAE0EA474AF493DBBE2E6CF9332EC7B98560928B042EF9972161B501E49F51D822502D48746FD07A6438D1E29226ED762");

        //GetInfo(@"E:\AlvaView\0d769a59a2e34105b105a86ab2e84e00,MainScene,1,5319F5147CF57A56C18F0347611D1434F5122644AF044012361C462611C1306E044BC421419976C0162EB8A7C284D4A56118B6113010216455D1F14C2C2AC754BC2C6B25C09FFC7A14941461C29B2EEA84C865C8D59487267A9B41EB07C228E3777F22D724912604108020508480004008886618250140200008601084102046001144528599464250016010FF5E7FB9DDA577D9EF5B66FF9DAFEEBBE5DEEED9BBA5FEBDE75F66BBDDAFE69BEFDEEFDBBBBFEEDFADFD22D5E70888DBF7D1B0E067E69E18634008810052840896931FFA0F81E5E4BF0FC9455678FB84CB8494874C8D5DEC96104248620761A00B9D0CF70F41BB289E9A41DADE6F3ADDF50715A8");
    }
    public void GoImage()
    {
        GetInfo(Application.temporaryCachePath + "/image" + ",MainScene,1,5319F5147CF57A56C18F0347611D1434F5122644AF044012361C462611C1306E044BC421419976C0162EB8A7C284D4A56118B6113010216455D1F14C2C2AC754BC2C6B25C09FFC7A14941461C29B2EEA84C865C8D59487267A9B41EB07C228E3777F22D724912604108020508480004008886618250140200008601084102046001144528599464250016010FF5E7FB9DDA577D9EF5B66FF9DAFEEBBE5DEEED9BBA5FEBDE75F66BBDDAFE69BEFDEEFDBBBBFEEDFADFD22D5E70888DBF7D1B0E067E69E18634008810052840896931FFA0F81E5E4BF0FC9455678FB84CB8494874C8D5DEC96104248620761A00B9D0CF70F41BB289E9A41DADE6F3ADDF50715A8");
    }
    public void GoPlane()
    {
        GetInfo(Application.temporaryCachePath + "/pingmian" + ",MainScene,1,5319F5147CF57A56C18F0347611D1434F5122644AF044012361C462611C1306E044BC421419976C0162EB8A7C284D4A56118B6113010216455D1F14C2C2AC754BC2C6B25C09FFC7A14941461C29B2EEA84C865C8D59487267A9B41EB07C228E3777F22D724912604108020508480004008886618250140200008601084102046001144528599464250016010FF5E7FB9DDA577D9EF5B66FF9DAFEEBBE5DEEED9BBA5FEBDE75F66BBDDAFE69BEFDEEFDBBBBFEEDFADFD22D5E70888DBF7D1B0E067E69E18634008810052840896931FFA0F81E5E4BF0FC9455678FB84CB8494874C8D5DEC96104248620761A00B9D0CF70F41BB289E9A41DADE6F3ADDF50715A8");
    }
    public void GetInfo(string s)
    {
        print("StartInfo:" + s);//路径,场景名,识别类型,授权码

        var info = s.Split(',');
        if (info.Length < 4)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("传入参数有误", (IsYes) =>
            {
                PlantformInterface.ReturnAppCalBack();
            });
        }
        else
        {
            //directoryName = Path.GetFileNameWithoutExtension(info[0]);
            //GlobalData.ProjectID = directoryName;
            MyMessageData.ProjectPath = info[0];
            MyMessageData.BtnName = info[1];
            MyMessageData.scanType = info[2];
            MyMessageData.alvaLicense = info[3];

            print("MyMessageData.ProjectPath=" + MyMessageData.ProjectPath);
            print("MyMessageData.BtnName=" + MyMessageData.BtnName);
            print("MyMessageData.scanType=" + MyMessageData.scanType);
            print("MyMessageData.alvaLicense=" + MyMessageData.alvaLicense);

#if UNITY_EDITOR
            //Debug.Log("现在是editor环境，不检测核心码");
            FindAB(MyMessageData.ProjectPath);
#elif UNITY_ANDROID
       // Debug.Log("现在是ANDROID");
#elif UNITY_IOS
        if (CheckLicense(MyMessageData.alvaLicense))
            {
                FindAB(MyMessageData.ProjectPath);
            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetWarnData(alvaInitErrorTip, (IsYes) =>
                {
                    PlantformInterface.ReturnAppCalBack();
                });
            }
#else
       // Debug.Log("其他平台");
#endif


        }
    }
    //检测核心是否可用
    bool CheckLicense(string auth)
    {
        bool isCheckOK = true;
        int cadStatus;
        print("检测核心是否可用MyMessageData.scanType=" + MyMessageData.scanType + "  auth=" + auth);

        //初始化alva核心环境
        switch (MyMessageData.scanType)
        {
            case "1":
                cadStatus = AlvaARWrapper.Instance.InitIR(AlvaARUnity.AlvaDataType.Alva_Memory, MyMessageData.companyName, auth);
                Debug.Log("IR_Init " + cadStatus);
                if (cadStatus != 0)
                {
                    isCheckOK = false;
                    if (cadStatus == -2147047410 || cadStatus == -2146981874 || cadStatus == -2146916338 || cadStatus == -2146850802)
                        ARSceneComonUI.instance.ShowTip("特征版本号不匹配" + cadStatus);
                    else
                        alvaInitErrorTip = "授权码已过期" + cadStatus;
                }
                AlvaARWrapper.Instance.UnitIR();
                break;
            case "2":
                cadStatus = AlvaARWrapper.Instance.InitMT(MyMessageData.companyName, auth);
                Debug.Log("InitMT cadStatus= " + cadStatus);
                if (cadStatus != 0)
                {
                    isCheckOK = false;
                    if (cadStatus == -2146850802 || cadStatus == -2146981874)
                        alvaInitErrorTip = "特征版本号不匹配" + cadStatus;
                    else
                        alvaInitErrorTip = "授权码已过期" + cadStatus;
                }
                AlvaARWrapper.Instance.UnitMT();
                break;
        }

        return isCheckOK;
    }

    void FindAB(string path)
    {

        if (Directory.Exists(path))
        {
            BeginLoadAB();
        }
        else
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("文件夹路径错误", (IsYes) =>
            {
                PlantformInterface.ReturnAppCalBack();
            });
        }
    }

    void BeginLoadAB()
    {
        StartCoroutine(LoadScene(MyMessageData.BtnName));
    }

    private IEnumerator LoadScene(string abName)
    {

        //print("////////////////////44444444LoadScene():" + abName);
        if (load)
            load.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        string path = MyMessageData.ProjectPath + GlobalData.GetPlatformInfo(2);

        print("////////////////////55555555LoadScene() path:" + path);

        if (MyMessageData.assetBundle && MyMessageData.curentBundleName != abName)
        {
            MyMessageData.assetBundle.Unload(true);
            MyMessageData.assetBundle = null;
            print("清空MyMessageData.assetBundle");
        }

        if (File.Exists(path))
        {
            if (MyMessageData.assetBundle)
            {
                print("加载过:");
                Go(abName);
            }
            else
            {
                print("没没没载过");
                var download = AssetBundle.LoadFromFileAsync(path);
                while (!download.isDone)
                {
                    progressText.text = "解析包:" + (download.progress * 100).ToString("0.0") + "%";
                    yield return new WaitForEndOfFrame();
                }
                progressText.text = "解析包:100%";

                yield return download;

                MyMessageData.assetBundle = download.assetBundle;
                MyMessageData.curentBundleName = abName;
                Go(abName);
            }
        }
        else
        {
            print("路径错误xxxx");
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("该路径下无此文件：" + path + "，导致AR功能无法正常使用，请联系在线客服", (IsYes) =>
            {
                PlantformInterface.ReturnAppCalBack();
            });
        }
        if (load)
            load.SetActive(false);
    }
    void Go(string name)
    {
        MyMessageData._asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
    }

    private void OnDestroy()
    {
        if (load)
        {

            load.SetActive(false);
        }
        MyMessageData._asyncOperation = null;
    }
}
