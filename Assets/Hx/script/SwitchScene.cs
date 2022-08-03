using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    public bool isLoadAB = false;
    public bool isAutoGo = false;
    GameObject load;
    public string abFolderName = "Alva_MT_android";
    public string abScenName = "MT2";
    public string sceneName = "MT2";

    bool isClick = false;
    public Text progressText;

    private IEnumerator Start()
    {
     
        load = Instantiate(Resources.Load("Loading"), transform) as GameObject;
        progressText = load.transform.Find("Image/progressText").GetComponent<Text>();

        if (AlvaLicenseHttp.instance)
        {
            AlvaLicenseHttp.instance.progressText = progressText;
        }
        load.SetActive(false);
        yield return new WaitForSeconds(5);
        if (isAutoGo && !isClick)
        {
            LoadAB();
        }
    }
    void Go(string name)
    {
        MyMessageData._asyncOperation = SceneManager.LoadSceneAsync(name);
    }

    public void Go2(string name)
    {
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        load.SetActive(true);
        MyMessageData._asyncOperation = SceneManager.LoadSceneAsync(name);
    }

    public void BackMain()
    {
        isClick = true;
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("MainTemp");
    }


    public void LoadAB()
    {
        isClick = true;
        load.SetActive(true);
        if (PlayerPrefs.GetInt(GlobalData.LoginStateStr) == 1 && PlayerPrefs.GetString(GlobalData.UserNameStr) == "6004")
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        //Screen.orientation = ScreenOrientation.AutoRotation;

        if (isLoadAB)
        {
            StartCoroutine(LoadScene(abScenName));
        }
        else
            Go(sceneName);
    }
    public void LoadABmulti(string abName)
    {
        isClick = true;
        //print(isLoadAB + "区分多个场景：" + abName);
        if (abName == "ALVA_FDJ_slam")
            Screen.orientation = ScreenOrientation.AutoRotation;
        else if (abName == "Alva_FDJ_Image")
            Screen.orientation = ScreenOrientation.Portrait;
        else if (abName == "Alva_FDJ_Model")
            Screen.orientation = ScreenOrientation.Portrait;
        //else
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        load.SetActive(true);
        if (isLoadAB)
        {

            StartCoroutine(LoadScene(abName));
        }
        else
            Go(abName);
    }
    private IEnumerator LoadScene(string abName)
    {
       
        string path = GlobalData.LocalPath + GlobalData.ProjectID + "/" + abFolderName + "/" + abName + ".unity3d";

        print("path:" + path);

        if (MyMessageData.assetBundle && MyMessageData.curentBundleName != abName)
        {
            MyMessageData.assetBundle.Unload(true);
            MyMessageData.assetBundle = null;
           // print("清空MyMessageData.assetBundle");
        }

        if (File.Exists(path))
        {
            if (MyMessageData.assetBundle)
            {
               // print("加载过:");
                Go(abName);
            }
            else
            {
               // print("没没没载过");
                var download = AssetBundle.LoadFromFileAsync(path);

                while (!download.isDone)
                {
                    progressText.text = "解析包:" + (download.progress * 100).ToString("0.0") + "%";


                    yield return new WaitForEndOfFrame();
                }
                progressText.text = "解析包:" + "100%";

                yield return download;
                
                MyMessageData.assetBundle = download.assetBundle;
                MyMessageData.curentBundleName = abName;
                Go(abName);
            }
        }
        else
        {
            //print("路径错误xxxx");
            load.SetActive(false);
        }

    }
}
