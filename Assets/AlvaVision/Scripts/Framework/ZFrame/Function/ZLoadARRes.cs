using System.Collections.Generic;
using UnityEngine;

public class ZLoadARRes : ZBase
{
    public override void OnInitComp()
    {
    }

    public override void OnInitData()
    {

    }

    public override void OnInitFunc()
    {

    }

    public override void OnInstance()
    {

    }

    public bool OnLoadDatSet()
    {
        string[] path = System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfig3DStr);
        return LoadDatSet(path);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="uiParent">UI父物体</param>
    /// <param name="modelParent">模型父物体</param>
    public void OnLoadRes(Transform uiParent, Transform modelParent)
    {
        //灯光初始化

        string[] path = System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfig3DStr);
        TaskJson task = LoadTask();
        if (task != null)
        {
            GlobalData.BindsList.Clear();
            for (int i = 0; i < task.tasks.Count; i++)
            {
                StepBind stepBind = new StepBind(null, task.tasks[i].number, task.tasks[i].title);
                GlobalData.BindsList.Add(stepBind);
            }
        }
        LoadModel(path, modelParent, uiParent);
        LoadUI(uiParent, modelParent);
        BindModelAniEvent(modelParent);
    }

    public void OnAddTouchMe(GameObject obj)
    {
        InputTouchTest touch = obj.gameObject.AddComponent<InputTouchTest>();
        if (GameObject.FindGameObjectWithTag("TouchMe") == null)
        {
            GameObject newTouchme = new GameObject();
            newTouchme.tag = "TouchMe";
            newTouchme.transform.localPosition = Vector3.zero;
            newTouchme.transform.localRotation = Quaternion.identity;
            newTouchme.transform.localScale = Vector3.one;
        }
        // touch.relativeTrf = GameObject.FindGameObjectWithTag("TouchMe").transform;
        //touch.cam = Camera.main.transform;
    }

    public void AddTaskOfUI()
    {
        List<EditableData> objs = new List<EditableData>();
        //for (int i = 0; i < GlobalData.EditableDataTaskObjs.Count; i++)
        //{
        //    Debug.LogError(GlobalData.EditableDataTaskObjs.Count);
        //    Debug.LogError(GlobalData.EditableDataTaskObjs[i]);
        //    Debug.LogError(GlobalData.EditableDataTaskObjs[i].GetComponent<EditableData>());
        //    objs.Add(GlobalData.EditableDataTaskObjs[i].GetComponent<EditableData>());
        //}
        foreach (var item in GlobalData.EditableDataTaskObjs)
        {
            objs.Add(item.GetComponent<EditableData>());
        }
        if (objs.Count < 1)
        {
            Debug.LogError(GlobalData.EditableDataTaskObjs.Count);
            return;
        }
        //Debug.Log("AddTaskOfUI()：" + objs.Count);
        StepItem step = FindObjectOfType<StepItem>();
        if (GlobalData.BindsList != null && GlobalData.BindsList.Count > 0)
        {
            for (int j = 0; j < GlobalData.BindsList.Count; j++)
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    if (objs[i].eoData != null)
                    {
                        //模型子物体有任务
                        if (objs[i].eoData.task == GlobalData.BindsList[j].step)
                        {
                            if (GlobalData.BindsList[j].objList == null)
                            {
                                GlobalData.BindsList[j].objList = new List<GameObject>();
                            }
                            GlobalData.BindsList[j].objList.Add(objs[i].gameObject);
                        }
                    }
                    else if (objs[i].eoModel != null)
                    {
                        //模型有任务
                        if (objs[i].eoModel.task == GlobalData.BindsList[j].step)
                        {
                            if (GlobalData.BindsList[j].objList == null)
                            {
                                GlobalData.BindsList[j].objList = new List<GameObject>();
                            }
                            GlobalData.BindsList[j].objList.Add(objs[i].gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            GlobalData.EditableDataTaskObjs.Clear();
        }
        if (step)
        {
            //Debug.Log("step");
            step.stepList = GlobalData.BindsList;
            step.parent = objs[0].transform.root;
            GlobalData.EditableDataTaskObjs.Clear();
            //Debug.Log("step end");
        }
    }

    public void BindModelAniEvent(Transform modelTransform)
    {
        //2D
        string path = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectAnimationEventStr;
        if (!System.IO.File.Exists(path))
        {
            return;
        }
        string json = System.IO.File.ReadAllText(path);
        //Debug.Log(json + "==========================");
        AnimationEventJsons eventJsons = GlobalData.DeserializeObject<AnimationEventJsons>(json);
        //foreach (AnimationEventJson item in eventJsons.animationEventJsons)
        //{
        //    Debug.Log("绑定动画***********************" + item.animationEvent + "  " + item.parameter + "   " + item.animationClipName + item.function);
        //    GameObject aniModel = FindObj(modelTransform, item.modelID);
        //    if (aniModel != null)
        //    {
        //        ModelAnEvent mevent = aniModel.GetComponentInChildren<Animation>().gameObject.AddComponent<ModelAnEvent>();
        //        GameObject obj = FindObj(modelTransform, item.parameter);
        //        if (obj == null)
        //        {
        //            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
        //            box.SetTipData("动画事件绑定失败！错误信息：物体不存在，物体代码：" + item.parameter);
        //            return;
        //        }
        //        mevent.InitData(item.animationEvent, obj.gameObject, item.animationClipName,item.function);
        //    }
        //}
        for (int i = eventJsons.animationEventJsons.Count - 1; i >= 0; i--)
        {
            //Debug.Log("绑定动画***********************" + eventJsons.animationEventJsons[i].animationEvent + "  " + eventJsons.animationEventJsons[i].parameter + "   " + eventJsons.animationEventJsons[i].animationClipName + eventJsons.animationEventJsons[i].function);
            GameObject aniModel = FindObj(modelTransform, eventJsons.animationEventJsons[i].modelID);
            if (aniModel != null)
            {
                ModelAnEvent mevent = aniModel.GetComponentInChildren<Animation>().gameObject.AddComponent<ModelAnEvent>();
                GameObject obj = FindObj(modelTransform, eventJsons.animationEventJsons[i].parameter);
                if (obj == null)
                {
                    UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                    box.SetTipData("动画事件绑定失败！错误信息：物体不存在，物体代码：" + eventJsons.animationEventJsons[i].parameter);
                    return;
                }
                mevent.InitData(eventJsons.animationEventJsons[i].animationEvent, obj.gameObject, eventJsons.animationEventJsons[i].animationClipName, eventJsons.animationEventJsons[i].function);
            }
        }
    }

    private GameObject FindObj(Transform modelTransform, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        ObjectItem[] objs = modelTransform.GetComponentsInChildren<ObjectItem>(true);
        Transform obj = null;
        foreach (var o in objs)
        {
            if (o.editable.id.Equals(id))
            {
                obj = o.transform;
                return obj.gameObject;
            }
        }
        return null;
    }
}
