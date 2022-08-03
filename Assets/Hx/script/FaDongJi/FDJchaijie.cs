using UnityEngine;

public class FDJchaijie : MonoBehaviour
{
    public void ErrorAniEvent(float speed)
    {
        GetComponent<Animator>().enabled = false;
    }

    public void HideBangzi(int i)
    {
        if (GetComponent<FDJSpecialControl>())
        {
            if (i == 1)
            {
                GetComponent<FDJSpecialControl>().HideBangziGG(true);
            }
            else if (i == 2)//维修
            {
                GetComponent<FDJSpecialControl>().HideBangziGG(true, true);
            }
            else
            {
                GetComponent<FDJSpecialControl>().HideBangziGG(false);
            }

        }
    }

    public void WeiXiuBangZi(int i)
    {
        if (GetComponent<FDJSpecialControl>())
        {
            if (i == 1)
            {
                GetComponent<FDJSpecialControl>().HideBangziGG(true);

            }
            else
            {
                GetComponent<FDJSpecialControl>().HideBangziGG(false);
            }

        }
    }
}
