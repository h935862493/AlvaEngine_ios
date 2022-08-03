using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemicircleMenu3x : SemicircleMenuRoot
{
    public override void UpdateGetTargetValue()
    {
        if (m_Scrollbar.value == 1)
        {
            btns[0].transform.localScale *= 1.3f;
            BtnEvent(0);
        }
        else if (m_Scrollbar.value == 0.5f)
        {
            btns[1].transform.localScale *= 1.3f;
            BtnEvent(1);
        }
        else if (m_Scrollbar.value == 0f)
        {
            btns[2].transform.localScale *= 1.3f;
            BtnEvent(2);
        }
    }
    public override void BtnSelectGetTargetValue(int num)
    {
        switch (num)
        {
            case 0:
                mTargetValue = 1;
                break;
            case 1:
                mTargetValue = 0.5f;
                break;
            case 2:
                mTargetValue = 0;
                break;
        }
    }

    public override void EndDragGetTargetValue()
    {
        if (m_Scrollbar.value <= 0.33f)
        {
            mTargetValue = 0;
        }
        else if (m_Scrollbar.value <= 0.66f)
        {
            mTargetValue = 0.5f;
        }
        else
        {
            mTargetValue = 1;
        }
    }
}
