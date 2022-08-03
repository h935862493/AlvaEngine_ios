using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragEventSyn : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    public ScrollRect parentScrollRect;
    private Scrollbar bar;
    private const float subValue = 1 / 3f;

    //private GameObject MainTopObj;

    void Start()
    {
        if (parentScrollRect == null)
        {
            GameObject parentScrollRectObj = GameObject.Find("Canvas-category/Scroll View-content");
            //Debug.Log(parentScrollRectObj);
            parentScrollRect = parentScrollRectObj.GetComponent<ScrollRect>();
        }
        bar = parentScrollRect.horizontalScrollbar;

        //GlobalData.SetPageIndexAction?.Invoke(1);
    }

    float startData;
    bool isBegin = false;
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isBegin)
        {
            return;
        }
        if (parentScrollRect)
        {
            parentScrollRect.OnEndDrag(eventData);
        }
        //print("-----------------------" + gameObject.transform.parent.name);
        //Debug.Log(eventData.position.x + "_" + startData + "_" +(Screen.width / 3));
        //Debug.Log(eventData.position.x - startData + "//////Screen.width/3:" + Screen.width / 3);
        //Debug.Log(" GlobalData.pageValue 0：" + GlobalData.pageValue);
        //print("bar.value:" + bar.value);

        if (Mathf.Abs(eventData.position.x - startData) < Screen.width / 3)
        {
            DOTween.To(() => bar.value, x => bar.value = x, GlobalData.pageValue, 0.2f);
            //bar.value = GlobalData.pageValue;
            return;
        }
        //Debug.Log(" GlobalData.pageValue 1：" + GlobalData.pageValue);

        if (eventData.position.x - startData > 0)//上一页
        {

            if (IsPointInRight(eventData))//翻页
            {
                //进入上一页
                if (GlobalData.pageValue <= subValue)
                {
                    GlobalData.pageValue = 0;
                    DOTween.To(() => bar.value, x => bar.value = x, 0, 0.2f);
                    //bar.value = 0;
                    GlobalData.SetPageIndexAction?.Invoke(1);
                }
                else if (GlobalData.pageValue <= (2 * subValue))
                {
                    GlobalData.pageValue = 0.3333333f;
                    DOTween.To(() => bar.value, x => bar.value = x, subValue, 0.2f);
                    //bar.value = subValue;
                    GlobalData.SetPageIndexAction?.Invoke(2);
                }
                else
                {
                    GlobalData.pageValue = 0.6666667f;
                    GlobalData.SetPageIndexAction?.Invoke(3);
                    DOTween.To(() => bar.value, x => bar.value = x, 2 * subValue, 0.3f);
                    //bar.value = 1;
                }
            }
            else
            {
                if (GlobalData.pageValue >= 3 * subValue)
                {
                    GlobalData.pageValue = 1;
                    GlobalData.SetPageIndexAction?.Invoke(4);
                }
                DOTween.To(() => bar.value, x => bar.value = x, GlobalData.pageValue, 0.3f);
            }
        }
        else//下一页
        {
            if (!IsPointInRight(eventData))//翻页
            {
                if (GlobalData.pageValue < subValue)
                {
                    GlobalData.pageValue = 0.3333333f;
                    DOTween.To(() => bar.value, x => bar.value = x, subValue, 0.3f);
                    //bar.value = subValue;
                    GlobalData.SetPageIndexAction?.Invoke(2);
                }
                else if (GlobalData.pageValue < (2 * subValue))
                {
                    GlobalData.pageValue = 0.6666667f;
                    DOTween.To(() => bar.value, x => bar.value = x, 2 * subValue, 0.3f);
                    //bar.value = subValue;
                    GlobalData.SetPageIndexAction?.Invoke(3);
                }
                else if (GlobalData.pageValue <= (3 * subValue))
                {
                    GlobalData.pageValue = 1;
                    DOTween.To(() => bar.value, x => bar.value = x, 3 * subValue, 0.3f);
                    //bar.value = 2 * subValue;
                    GlobalData.SetPageIndexAction?.Invoke(4);
                }
                else
                {
                    GlobalData.pageValue = 0;
                    DOTween.To(() => bar.value, x => bar.value = x, 0, 0.3f);
                    //DOTween.To(() => bar.value, x => bar.value = x, 1, 0.3f);
                }
            }
            else
            {
                if (GlobalData.pageValue >= 3 * subValue)
                {
                    GlobalData.pageValue = 1;
                    GlobalData.SetPageIndexAction?.Invoke(4);
                }
                DOTween.To(() => bar.value, x => bar.value = x, GlobalData.pageValue, 0.3f);
            }
        }
        //GlobalData.pageValue = bar.value;

        //Debug.Log(" GlobalData.pageValue 2：" + GlobalData.pageValue);
        //if (GlobalData.pageValue >= 1) GlobalData.SetPageIndexAction?.Invoke(4);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        float angle = Vector2.Angle(eventData.delta, Vector2.up);
        //判断拖动方向，防止水平与垂直方向同时响应导致的拖动时整个界面都会动
        if (angle > 60f && angle < 120f)
        {
            isBegin = true;
        }
        else
        {
            isBegin = false;
            return;
        }

        startData = eventData.position.x;
        if (parentScrollRect)
        {
            parentScrollRect.OnBeginDrag(eventData);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            if (parentScrollRect)
            {
                parentScrollRect.OnDrag(eventData);
            }
        }

    }

    public bool IsPointInRight(PointerEventData eventData)
    {
        if (eventData.position.x > Screen.width / 2)
        {
            return true;
        }
        return false;
    }

}
