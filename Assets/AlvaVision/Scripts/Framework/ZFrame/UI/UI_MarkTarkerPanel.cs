/**************************************************
 * 作者：			胡金芝
 * FileName: 	UI_MarkTarkerPanel
 * ***********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Vuforia;
using System.IO;

public class UI_MarkTarkerPanel : MonoBehaviour
{

    public Button ReturnButton;

    public Button ResetButton;

    public Toggle PictureToggle;

    //private Button AnimationButton;
    //private Image AniImage;

    public RectTransform CustomButtonContent;

    public GameObject TipPanel;

    public GameObject ScanningImage;

    private List<GameObject> objects;

    public Transform ModelObjects;

    public GameObject Lines;

    bool IsInit = true;

    //public int aninum = 0;
    //public bool isFoundbol = false;
    //public Animation ani;
    //public List<string> aniClipsName = new List<string>();
    //public string curentAni;
    void Awake()
    {
        ReturnButton.onClick.AddListener(OnReturnBtnClik);
        ResetButton.onClick.AddListener(OnResetButton);

        transform.GetComponent<MarkData>().RecognitionAction += Init;

        objects = new List<GameObject>();

        LookToCam[] lookToCams = ModelObjects.GetComponentsInChildren<LookToCam>(true);
        PictureToggle.onValueChanged.AddListener((IsOn) =>
        {

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
    //private void Update()
    //{
    //    if (isFoundbol && Input.GetMouseButtonDown(0) && ani != null)
    //    {
    //        if (aniClipsName.Count > 0)
    //        {
    //            if (aninum < aniClipsName.Count)
    //                aninum++;
    //            else
    //                aninum = 1;

    //            ani.Play(aniClipsName[aninum - 1]);
    //            curentAni = aniClipsName[aninum - 1];
    //        }
    //    }
    //}


    public void Init(Pro2DRecognition recoType)
    {
        switch (recoType)
        {
            case Pro2DRecognition.None:
                break;
            case Pro2DRecognition.Image:
                GlobalData.IsTrackerFoundAction += OnTrackerFoundAction;
                StartCoroutine(InitSceneData());
                break;
            case Pro2DRecognition.VuMark:
                break;
            default:
                break;
        }
    }

    private void OnTrackerFoundAction(bool isFound)
    {
        //isFoundbol = isFound;
        TipPanel.SetActive(isFound);
        CustomButtonContent.gameObject.SetActive(isFound);
        ScanningImage.SetActive(!isFound);

        //Lines.SetActive(isFound); if (isFound)
        //{
        //    if (ModelObjects.GetComponentInChildren<LineToLookAt>(true) == null)
        //    {
        //        return;
        //    }
        //    ModelObjects.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(true);
        //    Invoke("CloseLine", 6f);
        //}
    }

    private void CloseLine()
    {
        ModelObjects.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(false);
    }



    IEnumerator InitSceneData()
    {
        //VuforiaControler.GetInstance().ClearAll(true);
        foreach (string item in GlobalData.ImageTargetPathStrList.Keys)
        {
            string xmlPath = GlobalData.LocalPath + GlobalData.ProjectID + "/Library/Dat/" + item;
            //Debug.Log(xmlPath);
            if (File.Exists(xmlPath))
            {
                GameObject[] objects = StartAR(xmlPath, GlobalData.ImageTargetPathStrList[item]);
                while (objects.Length < 1)
                {
                    Debug.Log("Found Objects Null");
                    yield return new WaitForEndOfFrame();
                }
                this.objects.AddRange(objects);
                //for (int i = 0; i < objects.Length; i++)
                //{
                //    if (objects[i] != null)
                //    {
                //        this.objects.Add(objects[i]);
                //    }
                //}
                //Debug.Log(this.objects.Count + "======================");
            }
            else
            {
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("数据包读取识别或不存在，请检查更正后重新启动项目！");
            }
        }

        if (objects != null && objects.Count > 0)
        {
            //添加资源
            foreach (var item in objects)
            {
                if (item == null)
                {
                    continue;
                }
                //Debug.Log(item.name + objects.Count);
                item.transform.SetParent(ModelObjects);
            }
            ZManager.instnace.zLoadARRes.OnLoadRes(CustomButtonContent.transform, ModelObjects);
            ZManager.instnace.zLoadARRes.OnAddTouchMe(ModelObjects.gameObject);
        }
        TipPanel.gameObject.SetActive(true);
        //添加任务信息
        ZManager.instnace.zLoadARRes.AddTaskOfUI();

        TipPanel.gameObject.SetActive(false);

        ScanningImage.SetActive(true);

        //ani = ModelObjects.GetComponentInChildren<Animation>();
        //if (ani)
        //{
        //    foreach (AnimationState animationState in ani)
        //    {
        //        aniClipsName.Add(animationState.clip.name);
        //    }
        //}
       
    }


    private GameObject[] StartAR(string url, string name)
    {
        GameObject[] target = null;
        //VuforiaControler.GetInstance().Load(url, Pro2DRecognition.Image, name);
        //VuforiaControler.GetInstance().SetAllTrackablerObject();
        //VuforiaControler.GetInstance().GetAllTrackablerObject(out target);
        return target;
    }

    private void OnResetButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.UnloadScene(UtiTool.AMARKSCENENAME);
        //SceneManager.LoadScene(UtiTool.AMARKSCENENAME, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 返回按钮事件
    /// </summary>
    void OnReturnBtnClik()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("Main");
    }

    private void OnDestroy()
    {
        GlobalData.IsTrackerFoundAction -= OnTrackerFoundAction;
        transform.GetComponent<MarkData>().RecognitionAction -= Init;
    }

}
