using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IOSDemo : MonoBehaviour
{
    public Button AniButton;
    public Button NextButton;
    public Button PreButton;
    public Button ErroButton;
    public Button HotButton;
    public Button ReturnButton;

    public GameObject chejia, shitiMT;
    public GameObject Hot;
    public List<GameObject> ObjAniList;

    public List<string> animations = new List<string>();
    public int num = 0;
    public GameObject curentTip;
    public Material mt2;
    
    private void Start()
    {
        AniButton.onClick.AddListener(OnAniButtonClick);
        NextButton.onClick.AddListener(OnNextButtonClick);
        PreButton.onClick.AddListener(OnPreButtonClick);
        ErroButton.onClick.AddListener(OnErroButtonClick);
        HotButton.onClick.AddListener(OnHotButtonClick);
        ReturnButton.onClick.AddListener(OnReturnButtonClick);
    }
    //返回
    public void OnReturnButtonClick()
    {
        //HXMTmanager.instance.audio0.Play();

       // Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("MainTemp");
    }
    //点击扇形UI主页按钮
    public void OnHomeButtonClick()
    {
        HXMTmanager.instance.audio0.Play();
        if (curentTip)
            curentTip.SetActive(false);
        Hot.SetActive(false);
        PreButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);
        HXMTmanager.instance.errorModel.SetActive(false);
        chejia.SetActive(true);
        shitiMT.SetActive(false);
        HXMTmanager.instance.modelAni.SetActive(false);
        HXMTmanager.instance.Btn_Home();
    }
    //热点
    public void OnHotButtonClick()
    {
        //mt2.SetFloat("_Scale", 1);
        HXMTmanager.instance.audio0.Play();
        if (curentTip)
            curentTip.SetActive(false);
        HXMTmanager.instance.gif.SetActive(false);
        HXMTmanager.instance.modelUV.SetActive(false);
        HXMTmanager.instance.modelAni.SetActive(true);
        Hot.SetActive(true);
        HXMTmanager.instance.errorModel.SetActive(false);
        PreButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);
        chejia.SetActive(true);
        shitiMT.SetActive(false);
        //chejia.GetComponent<Animator>().Play("motuo_idle");
    }
    //报警
    public void OnErroButtonClick()
    {
        //mt2.SetFloat("_Scale", 1);
        HXMTmanager.instance.audio0.Play();
        if (curentTip)
            curentTip.SetActive(false);
        HXMTmanager.instance.gif.SetActive(false);
        HXMTmanager.instance.modelUV.SetActive(false);
        HXMTmanager.instance.modelAni.SetActive(true);
        Hot.SetActive(false);
        PreButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);

        chejia.SetActive(true);
        shitiMT.SetActive(false);
        //chejia.GetComponent<Animator>().Play("motuo_idle");

        HXMTmanager.instance.ErrorShow();
    }
    //动画
    public void OnAniButtonClick()
    {
        //mt2.SetFloat("_Scale", 100);
        HXMTmanager.instance.audio0.Play();
        HXMTmanager.instance.gif.SetActive(false);
        HXMTmanager.instance.modelUV.SetActive(false);
        Hot.SetActive(false);
        HXMTmanager.instance.errorModel.SetActive(false);


        PreButton.gameObject.SetActive(!PreButton.gameObject.activeSelf);
        NextButton.gameObject.SetActive(!NextButton.gameObject.activeSelf);

        chejia.SetActive(false);
        shitiMT.SetActive(true);
        HXMTmanager.instance.modelAni.SetActive(true);
        
        if (curentTip)
            curentTip.SetActive(false);
        num = 0;

    }

    //上一个
    public void OnPreButtonClick()
    {
        HXMTmanager.instance.audio0.Play();
        if (curentTip)
            curentTip.SetActive(false);

        if (num > 1)
            num--;
        else
            num = 6;

        shitiMT.GetComponent<Animator>().Play(animations[num - 1]);
        ObjAniList[num - 1].SetActive(true);
        curentTip = ObjAniList[num - 1];
    }
    //下一个
    public void OnNextButtonClick()
    {
        HXMTmanager.instance.audio0.Play();
        if (curentTip)
            curentTip.SetActive(false);

        if (num < 6)
            num++;
        else
            num = 1;

        shitiMT.GetComponent<Animator>().Play(animations[num - 1]);
        ObjAniList[num - 1].SetActive(true);
        curentTip = ObjAniList[num - 1];


    }
    public void Rest_btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
}
