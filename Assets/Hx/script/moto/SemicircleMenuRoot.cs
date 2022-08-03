using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>半圆UI</summary>
public class SemicircleMenuRoot : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public Scrollbar m_Scrollbar;
    public GameObject rp;
    public bool mNeedMove = false;
    public float mTargetValue;
    public int MaxAngle = 15;
    public List<GameObject> btns = new List<GameObject>();

    public float mMoveSpeed = 0f;
    protected const float SMOOTH_TIME = 0.2F;
    /// <summary>点击按钮触发的旋转</summary>
    protected bool isClickBtn = false;
    
    void Start()
    {
        
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].GetComponent<SemicircleBtnItem>().id = i;
        }

        m_Scrollbar.onValueChanged.AddListener(ScrollValueChange);
    }
    private void Update()
    {
        if (mNeedMove)
        {
            if (Mathf.Abs(m_Scrollbar.value - mTargetValue) < 0.01f)
            {
                m_Scrollbar.value = mTargetValue;
                mNeedMove = false;

                UpdateGetTargetValue();

                return;
            }
            m_Scrollbar.value = Mathf.SmoothDamp(m_Scrollbar.value, mTargetValue, ref mMoveSpeed, SMOOTH_TIME);
        }
    }

    public virtual void UpdateGetTargetValue()
    {
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        mNeedMove = false;
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].transform.localScale = Vector3.one;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        EndDragGetTargetValue();
        mNeedMove = true;
        mMoveSpeed = 0;

    }
    //按钮事件时候获取目标值
    public virtual void BtnSelectGetTargetValue(int num)
    {
    }
    //滑动结束时判断当前跟哪个相邻
    public virtual void EndDragGetTargetValue()
    {
    }

    void ScrollValueChange(float v)
    {
        rp.transform.localEulerAngles = new Vector3(0, 0, (m_Scrollbar.value - 1) * MaxAngle);//60---15
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].transform.eulerAngles = Vector3.one;
        }
    }

    //直接点击按钮
    public  void BtnSelect(int num)
    {
        isClickBtn = true;
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].transform.localScale = Vector3.one;
        }

        BtnSelectGetTargetValue(num);

        mNeedMove = true;
        mMoveSpeed = 0;
        btns[num].GetComponent<SemicircleBtnItem>().btnEvent.Invoke();
    }

    //顺滑结束要执行的事件
    public void BtnEvent(int num)
    {
        if (!isClickBtn)
        {
            btns[num].GetComponent<SemicircleBtnItem>().btnEvent.Invoke();
        }
        isClickBtn = false;
    }
 
    void OnDestroy()
    {
        m_Scrollbar.onValueChanged.RemoveAllListeners();
    }
}
