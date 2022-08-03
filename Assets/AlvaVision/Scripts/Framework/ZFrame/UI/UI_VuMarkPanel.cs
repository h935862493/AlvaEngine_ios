using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UI_VuMarkPanel : MonoBehaviour
{

    GameObject[] objects;

    public GameObject ScanGameObject;
    public GameObject ModelParent;

    public Button PartButton;

    GameObject tempObj;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<MarkData>().RecognitionAction += Init;
        PartButton.gameObject.SetActive(false);
        tempObj = new GameObject();
        tempObj.transform.SetParent(Camera.main.transform);
    }

    //private void LateUpdate()
    //{
    //    tempObj.transform.localPosition = new Vector3(0, 0, 5f);
    //    tempObj.transform.localRotation = Quaternion.identity;
    //    ModelParent.transform.position = tempObj.transform.position;
    //}

    public void Init(Pro2DRecognition recoType)
    {
        switch (recoType)
        {
            case Pro2DRecognition.None:
                break;
            case Pro2DRecognition.Image:

                break;
            case Pro2DRecognition.VuMark:
                GlobalData.FileSelectAction += OnFileSelectAction;
                GlobalData.IsTrackerFoundAction += OnTrackFound;
                StartCoroutine(InitSceneData());
                break;
            default:
                break;
        }
    }

    private void OnTrackFound(bool isFound)
    {
        if (isFound)
        {
            ScanGameObject.SetActive(false);
            //BgImage.SetActive(true);
          
        }
    }

    private void OnFileSelectAction(string modelID,string modelName)
    {
        if (File.Exists(GlobalData.LocalPath + modelName))
        {
            InstantiateModel(GlobalData.LocalPath + modelName);
        }
        else
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("LoadPanel"), transform.parent);
            UI_DownLoadPanel t = go.GetComponent<UI_DownLoadPanel>();
            t.DownLoadModelsFromServer(GlobalData.BaseUrl + GlobalData.MarkJsonModelIDUrl, modelID, modelName, new DownloadInfo(),
                (path) => {
                    //Debug.Log(modelID + "下载模型");
                    InstantiateModel(path);
                    DestroyImmediate(go);
                });
        }
       
    }

    void InstantiateModel(string path)
    {
        GameObject obj = LoadModelMethod.LoadModel(path);
        Bounds bounds = GlobalData.CalculateBounds(obj);
        float max = Mathf.Max(new float[3] { bounds.size.x, bounds.size.y, bounds.size.z });
        float index = 1 / max;

        tempObj.transform.localPosition = new Vector3(0, 0, 5f);
        tempObj.transform.localRotation = Quaternion.identity;
        ModelParent.transform.position = tempObj.transform.position;

        obj.transform.SetParent(ModelParent.transform);
        obj.transform.localScale = Vector3.one * index;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;


        ZManager.instnace.zLoadARRes.OnAddTouchMe(ModelParent);

        //FindObjectOfType<Battlehub.UIControls.VirtualizingTreeViewDemo_DataItems>().gameObject.SetActive(false);

        GameObject modelTree = Instantiate<GameObject>(Resources.Load("TreeViewCanvas") as GameObject);
        IEnumerable<GameObject> objs = new GameObject[] { obj };
        //modelTree.GetComponentInChildren<Battlehub.UIControls.TreeViewDemo>().MyDataStart(objs);

        PartButton.gameObject.SetActive(true);
        PartButton.onClick.AddListener(delegate { modelTree.SetActive(!modelTree.activeSelf); });
    }

    IEnumerator InitSceneData()
    {
        string xmlPath = GlobalData.LocalPath + GlobalData.ProjectID + "/Library/Dat/" + GlobalData.ProjectConfigDatStr;   
        //Debug.Log(xmlPath);
        if (File.Exists(xmlPath))
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


    private GameObject[] StartAR(string url)
    {
        GameObject[] target = null;
        //VuforiaControler.GetInstance().ClearAll(true);
        //VuforiaControler.GetInstance().Load(url, Pro2DRecognition.VuMark,"");
        ////VuforiaControler.GetInstance().SetAllTrackablerObject();
        //VuforiaControler.GetInstance().GetAllTrackablerObject(out target);
        return target;
    }

    private void OnDestroy()
    {
        GlobalData.FileSelectAction -= OnFileSelectAction;
        GlobalData.IsTrackerFoundAction -= OnTrackFound;
    }
}
