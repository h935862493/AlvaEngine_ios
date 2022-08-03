using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class sceneGo : MonoBehaviour
{
    public Sprite Sp;
    public Image MyImage;
    public Button bt;
    public bool isLoadAB = false;
    public string tempSceneName = "MT1";
    public string abFolderName = "motuo2_2";
    public ABType abType = ABType.moto;
    string deviceError = "";
    public enum ABType
    {
        /// <summary>发动机资源包</summary>
        fdj,
        /// <summary>摩托车资源包</summary>
        moto,
    }
    private void Start()
    {
        MyImage.sprite = Sp;
        bt.onClick.AddListener(Go);
    }

    private void Go()
    {
        if (isLoadAB)
            FindAB();
        else
            GOscene();
    }

    void GOscene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(tempSceneName);
    }
    void FindAB()
    {
        string path = GlobalData.LocalPath + abFolderName;
        //string path = Application.streamingAssetsPath + "/motuo2.unity3d";
        //print("abPath:" + path);

        if (Directory.Exists(path))
        {
            GOscene();
        }
        else
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetWarnData("请下载场景资源", (IsYes) =>
            {
                if (!IsYes)
                {
                    return;
                }

                if (Application.internetReachability == NetworkReachability.NotReachable)//当网络不可用时 
                {
                    box.SetTipData("网络连接失败，请重试。");
                }
                else
                {
                    string url = "";
                    switch (abType)
                    {
                        case ABType.fdj:
                            url = "http://dld.alva.com.cn/Vision-Model/fdj/1.0/Alva_FDJ_ios.zip";
                            break;
                        case ABType.moto:
                            url = "http://dld.alva.com.cn/Vision-Model/moto/1.0/Alva_MT_ios.zip";
                            break;
                    }
                    GameObject go = Instantiate(Resources.Load<GameObject>("HxLoadPanel"), gameObject.transform.root);
                    HxUI_DownLoadPanel t = go.GetComponent<HxUI_DownLoadPanel>();
                    t.DownLoadResourcesFromServer(
                       url,
                       abFolderName + ".zip",
                    (filePath) =>
                    {
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        //解析包
                        bool IsZip = ZipHelper.UnzipFile(filePath, GlobalData.LocalPath + Path.GetFileNameWithoutExtension(filePath));
                        File.Delete(filePath);
                        //print("解压完毕:" + IsZip);
                        GOscene();
                    }
                    );
                }

            });
        }
    }

}
