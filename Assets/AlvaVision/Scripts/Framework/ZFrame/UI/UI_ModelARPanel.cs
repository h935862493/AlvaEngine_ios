using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_ModelARPanel : MonoBehaviour
{
    public Transform UIParentTrs;

    public Transform TopTipTrs;

    public Button ReturnButton;

    public Toggle PictureToggle;

    GameObject[] objects;

    public Button ResetButton;

    public Transform ModelObjects;

    public GameObject Lines;

    public Material ModelsShaderMat;

    //public int aninum = 0;
    //public bool isFoundbol = false;
    //public Animation ani;
    //public List<string> aniClipsName = new List<string>();
    //public string curentAni;

    void Start()
    {
        ReturnButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main");
        });

        StartCoroutine(InitScane());

        GlobalData.IsTrackerFoundAction += OnTrackerFound;

        ResetButton.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
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
            //Debug.Log("*****************"+xmlPathRoot);
            //print("///////////////////////// GlobalData.ProjectConfigDatStr:" + GlobalData.ProjectConfigDatStr);
            string xmlPath = System.IO.Directory.GetFiles(xmlPathRoot + "/dataset", "*.xml")[0];
            //Debug.Log("*****************" + xmlPath);
            if (System.IO.File.Exists(xmlPath))
            {
                objects = StartAR(xmlPath);
                while (objects.Length < 1)
                {
                    Debug.Log("3333---Found Objects Null");
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
              //  Debug.Log(item.name);
                ZManager.instnace.zLoadARRes.OnLoadRes(UIParentTrs, item.transform);
                ZManager.instnace.zLoadARRes.OnAddTouchMe(item.transform.GetChild(0).gameObject);
            }
        }
        yield return new WaitForEndOfFrame();

        UIParentTrs.gameObject.SetActive(true);

        TopTipTrs.gameObject.SetActive(true);
        //添加任务信息
        ZManager.instnace.zLoadARRes.AddTaskOfUI();
        UIParentTrs.gameObject.SetActive(false);

        TopTipTrs.gameObject.SetActive(false);

        LookToCam[] lookToCams = ModelObjects.GetComponentsInChildren<LookToCam>(true);
      //  Debug.Log(lookToCams.Length);
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

        //modelsMatsDic = new Dictionary<MeshRenderer, Material[]>();
        //EffectList = new List<MachineEffectCtrl>();
        //Transform _model = ModelObjects.GetComponentInChildren<LineToLookAt>(true).transform.parent;
        //MeshRenderer[] renders = _model.GetComponentsInChildren<MeshRenderer>();
        //foreach (MeshRenderer item in renders)
        //{
        //    Material[] mats = item.materials;
        //    modelsMatsDic.Add(item, mats);
        //    MachineEffectCtrl eff = item.gameObject.AddComponent<MachineEffectCtrl>();
        //    EffectList.Add(eff);
        //}


        ModelsActiveDic = new Dictionary<GameObject, bool>();
        GetObjActive(ModelObjects);
        SetObjActive(false);

        //ani = ModelObjects.GetComponentInChildren<Animation>();
        //if (ani)
        //{
        //    foreach (AnimationState animationState in ani)
        //    {
        //        aniClipsName.Add(animationState.clip.name);
        //    }
        //}
    }

    private void GetObjActive(Transform trs)
    {
        for (int i = 0; i < trs.GetChild(0).childCount - 1; i++)
        {
            GameObject game = trs.GetChild(0).GetChild(i).gameObject;
            if (game.GetComponent<ObjectItem>() != null)
            {
                ModelsActiveDic.Add(game, game.activeSelf);
            }
        }
    }

    private void SetObjActive(bool isActive)
    {
        foreach (var item in ModelsActiveDic.Keys)
        {
            item.SetActive(isActive);
            if (item.GetComponent<ObjectItem>().editable.type == "Model" && item.GetComponent<ObjectItem>().editable.name.Equals("Moto"))
            {
                item.GetComponentInChildren<Animation>().playAutomatically = false;
                item.SetActive(true);
            }
        }
    }

    Dictionary<MeshRenderer, Material[]> modelsMatsDic;
    List<MachineEffectCtrl> EffectList;

    bool isInitPlay = false;
    private void OnTrackerFound(bool isFound)
    {
        //isFoundbol = isFound;
        //if (isInitPlay)
        //{
        //    Trs.gameObject.SetActive(isFound);
        //    TopTipTrs.gameObject.SetActive(isFound);
        //    Lines.SetActive(isFound);
        //}
        //if (isFound && !isInitPlay)
        //{
        //    Debug.Log("1111==============进来了=============================");
        //    if (ModelObjects.GetComponentInChildren<LineToLookAt>(true) == null)
        //    {
        //        return;
        //    }
        //    Debug.Log("1111==============开始扫描了=============================");
        //    //ModelObjects.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(true);
        //    //Invoke("CloseLine", 6f);
        //}
        //else if (!isFound && !isInitPlay)
        //{
        //    Debug.Log("1111==============又识别le=============================");
        //    if (!isShan)
        //    {
        //        return;
        //    }
        //    Debug.Log("1111==============闪完了=============================");
        //    foreach (var item in EffectList)
        //    {
        //        item.StopMachineEffect();
        //    }
        //    foreach (var item in modelsMatsDic.Keys)
        //    {
        //        item.materials = modelsMatsDic[item];
        //    }
        //    StopCoroutine(SetModelMats());
        //}
        //if (isInitPlay)
        {
            //TopTipTrs.gameObject.SetActive(isFound);
            //Lines.SetActive(isFound);
            UIParentTrs.gameObject.SetActive(isFound);
        }
    }

    Dictionary<GameObject, bool> ModelsActiveDic;

    private void CloseLine()
    {
        ModelObjects.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(false);
        //开始闪一闪
        StartCoroutine(SetModelMats());
    }

    bool isShan = false;
    IEnumerator SetModelMats()
    {
        isShan = true;
        foreach (var item in modelsMatsDic.Keys)
        {
            Material[] mats = new Material[item.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = ModelsShaderMat;
            }
            item.materials = mats;
        }
        foreach (var item in EffectList)
        {
            item.Init();
        }
        yield return new WaitForSeconds(5f);
        foreach (var item in EffectList)
        {
            item.StopMachineEffect();
        }
        foreach (var item in modelsMatsDic.Keys)
        {
            item.sharedMaterials = modelsMatsDic[item];
        }
        isShan = false;

        isInitPlay = true;
        UIParentTrs.gameObject.SetActive(true);
        //TopTipTrs.gameObject.SetActive(true);
        //Lines.SetActive(true);

        foreach (var item in ModelsActiveDic.Keys)
        {
            item.SetActive(ModelsActiveDic[item]);
            if (item.GetComponentInChildren<Animation>())
            {
                item.GetComponentInChildren<Animation>().playAutomatically = true;
            }
        }
    }



    private GameObject[] StartAR(string url)
    {
        GameObject[] target = null;
        //VuforiaControler.GetInstance().ClearAll(true);
        //VuforiaControler.GetInstance().Load(url);
        ////VuforiaControler.GetInstance().SetAllTrackablerObject();
        //VuforiaControler.GetInstance().GetAllTrackablerObject(out target);
        return target;
    }

    private void OnDestroy()
    {
        GlobalData.IsTrackerFoundAction -= OnTrackerFound;
        //modelsMatsDic.Clear();
        //EffectList.Clear();
    }
}
