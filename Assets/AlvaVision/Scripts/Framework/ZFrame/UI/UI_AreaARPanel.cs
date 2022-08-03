using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_AreaARPanel : MonoBehaviour
{
    public Transform Trs;

    public Transform TopTipTrs;

    public Button ReturnButton;

    public Toggle PictureToggle;

    private Animator ani;

    GameObject[] objects;

    public Button ResetButton;

    public Transform ModelObjects;

    public GameObject Lines;

    void Start()
    {
        ReturnButton.onClick.AddListener(() =>
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene("Main");
        });

        StartCoroutine(InitScane());

        GlobalData.IsTrackerFoundAction += OnTrackerFound;

        ResetButton.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
    }


    IEnumerator InitScane()
    {
        yield return new WaitForSeconds(2f);
        //读取并加载识别信息
        //while (!VuforiaControler.GetInstance().GetTracker())
        //{
        //    Debug.Log("Found Vuforia Null");
        //    yield return new WaitForEndOfFrame();
        //}
        bool datesetResult = ZManager.instnace.zLoadARRes.OnLoadDatSet();
        if (!datesetResult)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("数据包读取识别或不存在，请检查更正后重新启动项目！");
        }
        else
        {
            string xmlPathRoot = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfigDatStr;
            //Debug.Log(xmlPathRoot);
            string xmlPath = System.IO.Directory.GetFiles(xmlPathRoot, "*.xml")[0];
            //Debug.Log(xmlPath);
            if (System.IO.File.Exists(xmlPath))
            {
                objects = StartAR(xmlPath);
                while (objects.Length < 1)
                {
                    Debug.Log("Found Objects Null");
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("数据包读取识别或不存在，请检查更正后重新启动项目！");
            }
        }
        if (objects != null)
        {
            //添加资源
            foreach (var item in objects)
            {
                if (item == null)
                {
                    continue;
                }
                item.transform.SetParent(ModelObjects);
                //Debug.Log(item.name);
                ZManager.instnace.zLoadARRes.OnLoadRes(Trs, item.transform);
                ZManager.instnace.zLoadARRes.OnAddTouchMe(item.transform.GetChild(0).gameObject);
            }
        }
        yield return new WaitForEndOfFrame();

        Trs.gameObject.SetActive(true);

        TopTipTrs.gameObject.SetActive(true);
        //添加任务信息
        ZManager.instnace.zLoadARRes.AddTaskOfUI();
        Trs.gameObject.SetActive(false);

        TopTipTrs.gameObject.SetActive(false);

        LookToCam[] lookToCams = ModelObjects.GetComponentsInChildren<LookToCam>(true);
        //Debug.Log(lookToCams.Length);
        PictureToggle.onValueChanged.AddListener((IsOn) => {
            if (IsOn)
            {
                foreach (var item in lookToCams)
                {
                    item.enabled = true;
                }
            }
            else
            {
                foreach (var item in lookToCams)
                {
                    item.enabled = false;
                }
            }
        });
    }

    private void OnTrackerFound(bool isFound)
    {
        Trs.gameObject.SetActive(isFound);
        TopTipTrs.gameObject.SetActive(isFound);
        //Lines.SetActive(isFound);
    }

    private GameObject[] StartAR(string url)
    {
        GameObject[] target = null;
        //VuforiaControler.GetInstance().ClearAll(true);
        //VuforiaControler.GetInstance().Load(url,true);
        //VuforiaControler.GetInstance().SetAllTrackablerObject();
        //VuforiaControler.GetInstance().GetAllTrackablerObject(out target);
        return target;
    }

    private void OnDestroy()
    {
        GlobalData.IsTrackerFoundAction -= OnTrackerFound;
    }
}
