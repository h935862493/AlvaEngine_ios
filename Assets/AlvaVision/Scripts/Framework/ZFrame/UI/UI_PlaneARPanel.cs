using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UI_PlaneARPanel : MonoBehaviour
{

    public Transform UITrs;
    public Transform TopTipTrs;
    public Transform ModelTrs;
    public Button ReturnButton;

    public GameObject TipImages;

    public Button ResetButton;
    public Button HelpButton;

    public Toggle PictureToggle;

    public GameObject Lines;

    //public int aninum = 0;
    //public bool isFoundbol = false;
    //public Animation ani;
    //public List<string> aniClipsName = new List<string>();
    //public string curentAni;

    void Start()
    {
        StartCoroutine(InitScene());
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

    IEnumerator InitScene()
    {
        yield return new WaitForSeconds(2f);
        ReturnButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main");
        });
        TopTipTrs.gameObject.SetActive(true);
        ZManager.instnace.zLoadARRes.OnLoadRes(UITrs, ModelTrs);
        ZManager.instnace.zLoadARRes.OnAddTouchMe(ModelTrs.gameObject);
        UITrs.gameObject.SetActive(true);
        //添加任务信息
        ZManager.instnace.zLoadARRes.AddTaskOfUI();
        UITrs.gameObject.SetActive(false);
        TopTipTrs.gameObject.SetActive(false);
        GlobalData.IsTrackerFoundAction += OnTrackerFound;

        ResetButton.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
        HelpButton.onClick.AddListener(() => { Transform trs = HelpButton.transform.GetChild(0); trs.gameObject.SetActive(!trs.gameObject.activeSelf); });

        LookToCam[] lookToCams = ModelTrs.GetComponentsInChildren<LookToCam>(true);
        Debug.Log(lookToCams.Length);
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

        //ani = ModelTrs.GetComponentInChildren<Animation>();
        //if (ani)
        //{
        //    foreach (AnimationState animationState in ani)
        //    {
        //        aniClipsName.Add(animationState.clip.name);
        //    }
        //}
    }

    private void OnTrackerFound(bool isFound)
    {
        //isFoundbol = isFound;
        UITrs.gameObject.SetActive(isFound);
        TopTipTrs.gameObject.SetActive(isFound);
        //Lines.SetActive(isFound);
        //if (isFound)
        //{
        //    Debug.Log("1111111111111111111111111111111");
        //    if (ModelTrs.GetComponentInChildren<LineToLookAt>(true) == null)
        //    {
        //        return;
        //    }
        //    Debug.Log("2222222222222222222222222222222");
        //    ModelTrs.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(true);
        //    Invoke("CloseLine", 6f);
        //}
    }

    private void CloseLine()
    {
        //Debug.Log("333333333333333333333333333333");
        ModelTrs.GetComponentInChildren<LineToLookAt>(true).gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GlobalData.IsTrackerFoundAction -= OnTrackerFound;
    }
}
