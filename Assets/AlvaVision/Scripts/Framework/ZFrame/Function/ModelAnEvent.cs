using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnEvent : MonoBehaviour
{
    private Animation ani;
    private string EventIndex;
    private GameObject EventGameObject;
    private string AniClipName;
    private bool EvenGameObjectEvent;
    private string myName;

    private void Start()
    {
        ani = GetComponentInChildren<Animation>();
        GlobalData.CreateID(ref myName);
        if (string.IsNullOrEmpty(AniClipName) || string.IsNullOrEmpty(EventIndex) || EventGameObject == null)
        {
            return;
        }
        AnimationClip clip = ani.GetClip(AniClipName);
        AnimationEvent evt = new AnimationEvent();
        evt.functionName = "AniPlayEvent";
        if (EventIndex.Equals("first"))
        {
            evt.time = 0f;

        }
        else if (EventIndex.Equals("end"))
        {
            evt.time = clip.length;
        }
        else
        {
            int _eventIndex = -1;
            try
            {
                _eventIndex = Convert.ToInt16(EventIndex);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
                box.SetTipData("绑定的动画帧为非法数据，请查证后重试！");
            }
            if (_eventIndex < 0 || _eventIndex > GetTotalFrame(ani,AniClipName))
            {
                Debug.LogError("编辑器绑定事件错误");
                return;
            }
            else
            {
                evt.time = GetAniTimeFormFrame(ani, AniClipName, _eventIndex);
            }
        }
        evt.stringParameter = myName;

        clip.AddEvent(evt);
    }

    public void InitData(string eventIndex,GameObject eventGameObject,string aniClipName,int function)
    {
        EventIndex = eventIndex;
        EventGameObject = eventGameObject;
        AniClipName = aniClipName;

        if (function.Equals(1))
        {
            EvenGameObjectEvent = true;
        }
        else
        {
            EvenGameObjectEvent = false;
        }
    }

    void AniPlayEvent(string eventName)
    {
        if (!string.Equals(eventName,myName))
        {
            return;
        }
        Debug.Log("执行动画事件" + ani.clip.name + GetCurrentFrame(ani,AniClipName) + EvenGameObjectEvent + "  " + myName);
        EventGameObject.SetActive(EvenGameObjectEvent);
    }
    #region 测试
    //bool isPlay = false;

    //private void Update()
    //{
    //    if (ani.isPlaying && ani.IsPlaying(AniClipName))
    //    {
    //        if (!isPlay)
    //        {
    //            StartCoroutine(PlayEvent());
    //        }
    //        isPlay = true;
    //        Debug.Log("当前帧 " + GetCurrentFrame(ani, ani.clip.name));
    //        Debug.Log("当前动画名 " + ani.clip.name);
    //    }
    //    if (!ani.isPlaying)
    //    {
    //        isPlay = false;
    //        StopCoroutine(PlayEvent());
    //    }
    //}

    //IEnumerator PlayEvent()
    //{
    //    bool isRun = false;
    //    switch (EventIndex)
    //    {
    //        case "first":
    //            isRun = true;
    //            Debug.Log("------------------------------------" + transform.name + EvenGameObjectEvent);
    //            EventGameObject.SetActive(EvenGameObjectEvent);
    //            break;
    //        case "end":
    //            isRun = true;
    //            int frame = GetTotalFrame(ani,ani.clip.name);
    //            int i = GetCurrentFrame(ani, ani.clip.name);
    //            while (i < frame - 2)
    //            {
    //                Debug.Log(frame + "  " + i);
    //                yield return new WaitForEndOfFrame();
    //                i = GetCurrentFrame(ani, ani.clip.name);
    //            }
    //            Debug.Log("/////////////////////////////////////" + ani.clip.length + EvenGameObjectEvent);
    //            EventGameObject.SetActive(EvenGameObjectEvent);
    //            break;
    //        default:
    //            break;
    //    }
    //    if (!isRun)
    //    {
    //        int eventIndex = -1;
    //        try
    //        {
    //            eventIndex = Convert.ToInt16(EventIndex);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError(ex.Message);
    //            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
    //            box.SetTipData("绑定的动画帧为非法数据，请查证后重试！");
    //        }
    //        int i = GetCurrentFrame(ani, ani.clip.name);
    //        if (eventIndex > 0)
    //        {
    //            while (i < eventIndex)
    //            {
    //                yield return new WaitForEndOfFrame();
    //                i = GetCurrentFrame(ani, ani.clip.name);
    //            }
    //            EventGameObject.SetActive(EvenGameObjectEvent);
    //        }
    //    }
    //    yield return 0;
    //}
    #endregion 

    int GetCurrentFrame(Animation ani, string name)
    {
        if (ani.IsPlaying(name) == false)
            return 0;
        float length = ani[name].length;
        float frameRate = ani[name].clip.frameRate;
        float currentTime = ani[name].time;

        float totalFrame = length / (1 / frameRate);
        int currentFrame = (int)(Mathf.Ceil(totalFrame * currentTime) / length);
        return currentFrame;
    }

    int GetTotalFrame(Animation ani, string name)
    {
        float length = ani[name].length;
        float frameRate = ani[name].clip.frameRate;
        return (int)(length / (1 / frameRate));
    }

    float GetAniTimeFormFrame(Animation ani, string name,int currentFrame)
    {
        float length = ani[name].length;
        float frameRate = ani[name].clip.frameRate;

        float totalFrame = length / (1 / frameRate);
        float currentTime = currentFrame/ totalFrame * length;
        return currentTime;
    }

    //private void OnDestroy()
    //{
    //    StopCoroutine(PlayEvent());
    //}
}
