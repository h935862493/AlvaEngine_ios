using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class SemicircleMenuManager : MonoBehaviour
{
    public static SemicircleMenuManager instance;
    public GameObject ui1, ui2, yuan, Button_main, Panel2_1, Panel2_2_color, Panel2_3_ani, Panel2_4_hot;
    bool isFirst = true;
    private void Start()
    {
        instance = this;
    }
    //��ť1����
    public void Btn_HideMenu()
    {
        float speed = 0.1f;
        Button_main.transform.DOLocalMoveX(125, speed).SetEase(Ease.Linear);
        yuan.transform.DOLocalMoveX(-50, speed).SetEase(Ease.Linear);
        yuan.transform.DOScale(1, speed).SetEase(Ease.Linear).OnComplete(delegate ()
        {
            ui2.SetActive(true);
            yuan.SetActive(false);
            if (isFirst)
            {
                isFirst = false;
                Panel2_1.GetComponent<SemicircleMenu4x>().BtnSelect(0);
            }

        });
        RigidImpact();

    }
    //��ť1��ʾ
    public void Btn_ShowMenu()
    {
        yuan.SetActive(true);
        ui2.SetActive(false);

        float speed = 0.1f;
        Button_main.transform.DOLocalMoveX(-32, speed).SetEase(Ease.Linear);
        yuan.transform.DOLocalMoveX(-45, speed).SetEase(Ease.Linear);
        yuan.transform.DOScale(0.66f, speed).SetEase(Ease.Linear);
        RigidImpact();
    }

    public void BtnHome()
    {
        //print("BtnHome");
        Panel2_3_ani.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_3_ani.SetActive(false);
        });
        Panel2_4_hot.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_4_hot.SetActive(false);
        });
        Panel2_2_color.SetActive(true);
        Panel2_2_color.transform.DOScale(1, 0.1f).OnComplete(delegate ()
        {
            //Panel2_2_color.GetComponent<SemicircleMenu2x>().BtnSelect(0);
        });
        //HXMTmanager.instance.iOSDemo.OnHomeButtonClick();
        RigidImpact();
    }

    public void BtnHot()
    {
        //print("BtnHot");
        Panel2_2_color.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_2_color.SetActive(false);
        });
        Panel2_3_ani.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_3_ani.SetActive(false);
        });
        Panel2_4_hot.SetActive(true);
        Panel2_4_hot.transform.DOScale(1, 0.1f).OnComplete(delegate ()
        {
            //Panel2_4_hot.GetComponent<SemicircleMenu3x>().BtnSelect(0);
        });
        HXMTmanager.instance.iOSDemo.OnHotButtonClick();
        RigidImpact();
    }

    public void BtnError()
    {
        //print("BtnError");
        Panel2_2_color.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_2_color.SetActive(false);
        });
        Panel2_3_ani.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_3_ani.SetActive(false);
        });
        Panel2_4_hot.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_4_hot.SetActive(false);
        });
        HXMTmanager.instance.iOSDemo.OnErroButtonClick();
        RigidImpact();
    }

    public void BtnAni()
    {
        //print("BtnAni");
        Panel2_2_color.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_2_color.SetActive(false);
        });
        Panel2_4_hot.transform.DOScale(0, 0.05f).OnComplete(delegate ()
        {
            Panel2_4_hot.SetActive(false);
        });
        Panel2_3_ani.SetActive(true);
        Panel2_3_ani.transform.DOScale(1, 0.1f).OnComplete(delegate ()
        {
            //Panel2_3_ani.GetComponent<SemicircleMenu2x>().BtnSelect(0);
        });
        HXMTmanager.instance.iOSDemo.OnAniButtonClick();
        RigidImpact();
    }

    public void BtnColor1()
    {
        HXMTmanager.instance.iOSDemo.OnHomeButtonClick();
        HXMTmanager.instance.Btn_switch1();
        RigidImpact();
    }
    public void BtnColor2()
    {
        HXMTmanager.instance.iOSDemo.OnHomeButtonClick();
        HXMTmanager.instance.Btn_switch2();
        RigidImpact();
    }

    public void BtnHot(GameObject go)
    {
        HXMTmanager.instance.Btn_point(go);
        RigidImpact();
    }

    public void BtnLastAni()
    {
        HXMTmanager.instance.iOSDemo.OnPreButtonClick();
        RigidImpact();
    }

    public void BtnNextAni()
    {
        HXMTmanager.instance.iOSDemo.OnNextButtonClick();
        RigidImpact();
    }

    void RigidImpact()
    {
        //MMVibrationManager.Haptic(HapticTypes.RigidImpact, false, true, this);
    }
}
