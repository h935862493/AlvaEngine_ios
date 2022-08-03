using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHighLightManager : MonoBehaviour
{
    Vector3 scale,position,rotation;
    //HighlightableObject highlightableObject;

    private void Start()
    {
        Invoke("SetInitData", 1f);
        GlobalData.IsModelAniPlayAction += OnAniPlay;
    }

    void SetInitData()
    {
        scale = transform.lossyScale;
        position = transform.position;
        rotation = transform.eulerAngles;
        //highlightableObject = GetComponent<HighlightableObject>();
    }

    bool isPlay = false;
    bool isBegin = false;
    private void OnAniPlay(bool isPlay)
    {
        this.isPlay = isPlay;
        isBegin = true;
    }

    private void Update()
    {
        if (!isBegin)
        {
            return;
        }
        if (!isPlay)
        {
            OffHighLight();
        }
        if (isPlay)
        {
            if (Vector3.Distance(transform.lossyScale, scale) > 0)
            {
                scale = transform.lossyScale;
                //高亮
                OpenHighLight();
            }
            if (Vector3.Distance(transform.position, position) > 0)
            {
                position = transform.position;
                //高亮
                OpenHighLight();
            }
            if (Vector3.Distance(transform.eulerAngles, rotation) > 0)
            {
                rotation = transform.eulerAngles;
                //高亮
                OpenHighLight();
            }
        }
    }

    private void OpenHighLight()
    {
        isBegin = false;
        //Debug.Log("打开高亮" + gameObject.name);
        //高亮
        //if (GetComponent<MeshRenderer>() != null)
        //{
        //    if (highlightableObject == null)
        //    {
        //        highlightableObject = gameObject.AddComponent<HighlightableObject>();
        //    }
        //    highlightableObject.ConstantOn(new Color(1f, 1f, 0f, 1f));
        //}
    }

    private void OffHighLight()
    {
        isBegin = false;
        //Debug.Log("关闭高亮");
        //if (highlightableObject == null)
        //{
        //    return;
        //}
        //highlightableObject.ConstantOff();
    }

    private void OnDestroy()
    {
        GlobalData.IsModelAniPlayAction -= OnAniPlay;
    }
}
