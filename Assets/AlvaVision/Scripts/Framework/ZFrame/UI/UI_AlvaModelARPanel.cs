using arsdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_AlvaModelARPanel : MonoBehaviour
{
    public Transform UIParentTrs;

    //public Transform TopTipTrs;

    public Button ReturnButton;

    public Toggle PictureToggle;

    private Animator ani;

    GameObject[] objects;

    public Button ResetButton;

    public Transform ModelObjects;

    public GameObject Lines;

    public Material ModelsShaderMat;

    void Start()
    {
        ReturnButton.onClick.AddListener(() =>
        {
            AlvaARWrapper.Instance.UnitMT();
            SceneManager.LoadScene("Main");
        });

        StartCoroutine(InitScane());

        GlobalData.IsTrackerFoundAction += OnTrackerFound;

        ResetButton.onClick.AddListener(() => { AlvaARWrapper.Instance.UnitMT(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); });

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //Debug.Log("按了");
            GlobalData.IsTrackerFoundAction.Invoke(true);
        }
    }

    IEnumerator InitScane()
    {
        yield return new WaitForSeconds(2f);

        ZManager.instnace.zLoadARRes.OnLoadRes(UIParentTrs, ModelObjects);
        //ZManager.instnace.zLoadARRes.OnAddTouchMe(ModelObjects.gameObject);

        yield return new WaitForEndOfFrame();

        //添加任务信息
        ZManager.instnace.zLoadARRes.AddTaskOfUI();
        UIParentTrs.gameObject.SetActive(false);

        LookToCam[] lookToCams = ModelObjects.GetComponentsInChildren<LookToCam>(true);
        Debug.Log("lookToCams.Length:" + lookToCams.Length);
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

        ModelsActiveDic = new Dictionary<GameObject, bool>();
        GetObjActive(ModelObjects);
        SetObjActive(false);
    }

    private void GetObjActive(Transform trs)
    {
        //Debug.Log("初始化了");
        for (int i = 0; i < trs.childCount - 1; i++)
        {
            GameObject game = trs.GetChild(i).gameObject;
            if (game.GetComponent<ObjectItem>() != null)
            {
                ModelsActiveDic.Add(game, game.activeSelf);
            }
        }
    }

    private void SetObjActive(bool isActive)
    {
        //Debug.Log("设置模型");
        foreach (var item in ModelsActiveDic.Keys)
        {
            item.SetActive(isActive);
            if (item.GetComponent<ObjectItem>().editable.type == "Model" && item.GetComponent<ObjectItem>().editable.name.Equals("Moto"))
            {
                //Debug.Log("打开模型");
                item.GetComponentInChildren<Animation>().playAutomatically = false;
                item.SetActive(true);
            }
        }
    }

    Dictionary<MeshRenderer, Material[]> modelsMatsDic;
    List<MachineEffectCtrl> EffectList;
    bool isFoundTemp = false;
    //bool isInitPlay = false;
    private void OnTrackerFound(bool isFound)
    {
        UIParentTrs.gameObject.SetActive(isFound);
    }

    Dictionary<GameObject, bool> ModelsActiveDic;


    private void OnDestroy()
    {
        GlobalData.IsTrackerFoundAction -= OnTrackerFound;
        modelsMatsDic.Clear();
        EffectList.Clear();
    }
}
