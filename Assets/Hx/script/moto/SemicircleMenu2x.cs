public class SemicircleMenu2x : SemicircleMenuRoot
{
    public override void UpdateGetTargetValue()
    {
        if (m_Scrollbar.value == 1)
        {
            btns[0].transform.localScale *= 1.3f;
            BtnEvent(0);
        }
        else if (m_Scrollbar.value == 0f)
        {
            btns[1].transform.localScale *= 1.3f;
            BtnEvent(1);
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
                mTargetValue = 0f;
                break;
        }
    }

    public override void EndDragGetTargetValue()
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

}
