using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaneTargetTipUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public Scrollbar m_Scrollbar;//水平滚动条

    float mTargetValue;//鼠标触发点的值
    bool mNeedMove = false;//是否需要运动
    const float SMOOTH_TIME = 0.2F;//平滑运动时的时间
    float mMoveSpeed = 0f;//滚动的速度
    bool isChange = true;
    public Button btn_L, btn_R, btn_X;

    public float totalNum = 0;
    public float intervalNum = 0.25f;
    public GameObject go_all, go_rota, go_scale1, go_scale2, go_move;
    public GridLayoutGroup layoutGroup;
    public void InitData(bool isDisableMove, bool isDisableRot, bool isDisableScale)
    {
        go_all.SetActive(true);
        totalNum++;
        if (!isDisableMove)
        {
            totalNum++;
            go_move.SetActive(true);
        }
        if (!isDisableRot)
        {
            totalNum++;
            go_rota.SetActive(true);
        }
        if (!isDisableScale)
        {
            totalNum++;
            go_scale1.SetActive(true);
            totalNum++;
            go_scale2.SetActive(true);
        }

        intervalNum = (1 / (totalNum - 1));
        layoutGroup.constraintCount = (int)totalNum;


    }

    void Update()
    {
        if (mNeedMove)
        {

            if (isChange)
            {
                isChange = false;
                if (mTargetValue >= 1)
                {
                    btn_R.gameObject.SetActive(false);
                    btn_X.gameObject.SetActive(true);
                }
                else if (mTargetValue <= 0)
                {
                    btn_L.gameObject.SetActive(false);
                }
                else
                {
                    btn_L.gameObject.SetActive(true);
                    btn_R.gameObject.SetActive(true);
                    btn_X.gameObject.SetActive(false);
                }
            }
            if (Mathf.Abs(m_Scrollbar.value - mTargetValue) < 0.01f)
            {
                m_Scrollbar.value = mTargetValue;
                mNeedMove = false;
                isChange = true;
                return;
            }
            //平滑阻尼运动
            m_Scrollbar.value = Mathf.SmoothDamp(m_Scrollbar.value, mTargetValue, ref mMoveSpeed, SMOOTH_TIME);
        }
    }


    public void OnButtonClick_R()
    {
        if (!mNeedMove)
        {
            if (mTargetValue >= 1)
            {
                mTargetValue = 1;
            }

            else
                mTargetValue += intervalNum;
            //mTargetValue += 0.25f;

            mNeedMove = true;
        }
    }



    public void OnButtonClick_L()
    {
        if (!mNeedMove)
        {
            btn_R.gameObject.SetActive(true);
            if (mTargetValue <= 0)
            {
                mTargetValue = 0;
            }
            else
                mTargetValue -= intervalNum;
            //mTargetValue -= 0.25f;

            mNeedMove = true;
        }
    }

    private float beginV, endV;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginV = m_Scrollbar.value;
        mNeedMove = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endV = m_Scrollbar.value;

        if (Mathf.Abs(endV - beginV) > 0.03f)
        {
            if (endV > beginV)
                mTargetValue += intervalNum;
            else
                mTargetValue -= intervalNum;
        }
        else
        {
            switch (totalNum)
            {
                case 2:
                    GetTargetValue2();
                    break;
                case 3:
                    GetTargetValue3();
                    break;
                case 4:
                    GetTargetValue4();
                    break;
                case 5:
                    GetTargetValue5();
                    break;
            }
        }


        mNeedMove = true;
        mMoveSpeed = 0;
    }
    private void GetTargetValue2()
    {
        if (m_Scrollbar.value <= 0.5f)
        {
            mTargetValue = 0;
        }
        else
        {
            mTargetValue = 1;
        }
    }
    private void GetTargetValue3()
    {
        if (m_Scrollbar.value <= 0.25f)
        {
            mTargetValue = 0;
        }
        else if (m_Scrollbar.value <= 0.75f)
        {
            mTargetValue = 0.5f;
        }
        else
        {
            mTargetValue = 1;
        }
    }
    private void GetTargetValue4()
    {
        if (m_Scrollbar.value <= 0.165f)
        {
            mTargetValue = 0;
        }
        else if (m_Scrollbar.value <= 0.495f)
        {
            mTargetValue = 0.33f;
        }
        else if (m_Scrollbar.value <= 0.825f)
        {
            mTargetValue = 0.66f;
        }
        else
        {
            mTargetValue = 1;
        }
    }
    private void GetTargetValue5()
    {
        if (m_Scrollbar.value <= 0.125f)
        {
            mTargetValue = 0;
        }
        else if (m_Scrollbar.value <= 0.375f)
        {
            mTargetValue = 0.25f;
        }
        else if (m_Scrollbar.value <= 0.625f)
        {
            mTargetValue = 0.5f;
        }
        else if (m_Scrollbar.value <= 0.875f)
        {
            mTargetValue = 0.75f;
        }
        else
        {
            mTargetValue = 1;
        }
    }
}
