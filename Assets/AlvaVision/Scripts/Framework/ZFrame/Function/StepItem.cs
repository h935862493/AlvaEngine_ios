using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepItem : MonoBehaviour
{
    public List<StepBind> stepList = new List<StepBind>();
    public Transform parent;

    private Text step;
    private Text des;
    private Button nextBtn;
    private Button preBtn;

    private int index;

    private Button startBtn;
    private Button stopBtn;

    private GameObject stepParent;
    private List<GameObject> current;
    private List<GameObject> currentParent;

    private GameObject otherCurrentParent;

    public Transform UIParent;

    private void Start()
    {
        step = transform.Find("Step/stepText").GetComponent<Text>();
        des = transform.Find("Step/descText").GetComponent<Text>();
        nextBtn = transform.Find("Step/nextButton").GetComponent<Button>();
        preBtn = transform.Find("Step/preButton").GetComponent<Button>();
        startBtn = transform.Find("StartButton").GetComponent<Button>();
        stopBtn = transform.Find("StopButton").GetComponent<Button>();
        nextBtn.gameObject.SetActive(false);
        preBtn.gameObject.SetActive(false);
        stepParent = transform.Find("Step").gameObject;
        stepParent.SetActive(false);

        startBtn.onClick.AddListener(OnStartBtnCLick);
        stopBtn.onClick.AddListener(OnStopBtnCLick);
        nextBtn.onClick.AddListener(OnNextClicked);
        preBtn.onClick.AddListener(OnPreClicked);

        current = new List<GameObject>();
        currentParent = new List<GameObject>();

        otherCurrentParent = new GameObject();
        otherCurrentParent.transform.localPosition = Vector3.zero;
        otherCurrentParent.transform.localRotation = Quaternion.identity;
        otherCurrentParent.transform.localScale = Vector3.one;

        stopBtn.gameObject.SetActive(false);
    }

    private void OnStopBtnCLick()
    {
        //ResumeCurrentObjs();
        ShowBack();
        stepParent.SetActive(false);
        startBtn.gameObject.SetActive(true);
        stopBtn.gameObject.SetActive(false);
    }

    private void OnStartBtnCLick()
    {
        stepParent.SetActive(true);
        stopBtn.gameObject.SetActive(true);
        startBtn.gameObject.SetActive(false);
        index = -1;
        Animation[] anis = FindObjectsOfType<Animation>();
        foreach (var ani in anis)
        {
            //Debug.Log(ani);
            foreach (AnimationState state in ani)
            {
               // Debug.Log("动画名=============" + state.name + "===============");
                ResetAni(ani, state.name);
            }
        }

        OnNextClicked();
    }

    private void ResetAni(Animation ani, string name)
    {
        AnimationState state = ani[name];
        ani.Play(name);
        state.time = 0;
        ani.Sample();
        state.enabled = false;
    }



    void OnNextClicked()
    {
        if (stepList.Count <= 0) return;

        index++;
        if (index >= stepList.Count)
        {
            index--;
            //超出范围
            return;
        }

        OnCurrentStep();

        if (stepList.Count > 1)
        {
            nextBtn.gameObject.SetActive(true);
           
        }
        if (index > 0)
        {
            preBtn.gameObject.SetActive(true);
        }

        if (index + 1 >= stepList.Count)
        {
            nextBtn.gameObject.SetActive(false);
        }
    }

    void OnPreClicked()
    {
        if (stepList.Count <= 0) return;
        index--;
        if (index < 0)
        {
            index++;
            return;
        }

        OnCurrentStep();

        preBtn.gameObject.SetActive(true);
        nextBtn.gameObject.SetActive(true);

        if (index - 1 < 0)
        {
            preBtn.gameObject.SetActive(false);
        }
    }

    private void OnCurrentStep()
    {
        parent.gameObject.SetActive(true);
        step.text = stepList[index].step.ToString();
        des.text = stepList[index].desc;
        //ResumeCurrentObjs();
        //SetParentObjs(index);
        ResumeData();
        GetCurrentData(index);
        otherCurrentParent.SetActive(false);
    }

    void ShowBack()
    {
        foreach (Transform item in transDic.Keys)
        {
            //Debug.Log(item.name + "==================");
            item.SetParent(transDic[item]);
        }
        Transform tf = otherCurrentParent.transform;
        //Debug.Log(tf.childCount + "***************");
        while (tf && tf.childCount > 0 && tf.GetChild(0))
        {
           // Debug.Log(tf+"************");
            tf.GetChild(0).SetParent(parent);
        }
        foreach (Transform item in TaskObjActive.Keys)
        {
            item.gameObject.SetActive(TaskObjActive[item]);
        }
        transDic.Clear();
        TaskObjActive.Clear();
    }

    public Dictionary<Transform, Transform> transDic = new Dictionary<Transform, Transform>();//父子关系
    public Dictionary<Transform, bool> TaskObjActive = new Dictionary<Transform, bool>();

    void ResumeData()
    {
        //优化测试
        while (parent && parent.childCount>0 && parent.GetChild(0))
        {
            parent.GetChild(0).SetParent(otherCurrentParent.transform);
        }
        //所有上个步骤的物体，恢复父子关系
        foreach (Transform item in transDic.Keys)
        {
            //Debug.Log(item.name + "==================");
            item.SetParent(transDic[item]);
        }
        foreach (Transform item in TaskObjActive.Keys)
        {
            item.gameObject.SetActive(TaskObjActive[item]);
        }
        transDic.Clear();
        TaskObjActive.Clear();
    }



    void GetCurrentData(int index)
    {
        //获取当前步骤，遍历物体，设置到显示层
        //StepBind sb = stepList[index];
        if (stepList[index] == null || stepList[index].objList == null)
        {
            return;
        }
        foreach(GameObject item in stepList[index].objList)
        {
            transDic.Add(item.transform, item.transform.parent);
            TaskObjActive.Add(item.transform, item.activeSelf);
            if (item.name.Contains("ui"))
            {
                item.transform.SetParent(UIParent);
            }
            else
            {
                item.transform.SetParent(parent);
            }
            item.gameObject.SetActive(true);
            if (item.GetComponent<UnityEngine.Video.VideoPlayer>() != null)
            {
                item.GetComponent<UnityEngine.Video.VideoPlayer>().Play();
            }
        }
    }


}
