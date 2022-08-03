public class SemicircleMenu4x : SemicircleMenuRoot
{
    public override void UpdateGetTargetValue()
    {
        if (m_Scrollbar.value == 1)
        {
            btns[0].transform.localScale *= 1.3f;
            BtnEvent(0);
        }
        else if (m_Scrollbar.value == 0.66f)
        {
            btns[1].transform.localScale *= 1.3f;
            BtnEvent(1);
        }
        else if (m_Scrollbar.value == 0.33f)
        {
            btns[2].transform.localScale *= 1.3f;
            BtnEvent(2);
        }
        else if (m_Scrollbar.value == 0)
        {
            btns[3].transform.localScale *= 1.3f;
            BtnEvent(3);
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
                mTargetValue = 0.66f;
                break;
            case 2:
                mTargetValue = 0.33f;
                break;
            case 3:
                mTargetValue = 0;
                break;
        }
    }

    public override void EndDragGetTargetValue()
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
            mTargetValue = 1f;
        }
    }


}
