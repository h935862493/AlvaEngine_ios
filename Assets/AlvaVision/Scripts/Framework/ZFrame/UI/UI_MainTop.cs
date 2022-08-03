using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainTop : MonoBehaviour
{
    public ScrollRect rect;
    public Button ProjectButton;
    public Button LocationButton;
    public Button SetButton;
    public Text TitleText;
    public Button PublicButton;

    private List<Image> SelectImageList;
    public GameObject publicItem, projectItem, localItem;
    void Start()
    {
        ProjectButton.onClick.AddListener(OnProjectBtnClick);
        LocationButton.onClick.AddListener(OnLocationBtnClick);
        SetButton.onClick.AddListener(OnSetBtnClick);
        PublicButton.onClick.AddListener(OnPublicBtnClick);

        SelectImageList = new List<Image>();
        SelectImageList.Add(ProjectButton.GetComponent<Image>());
        SelectImageList.Add(LocationButton.GetComponent<Image>());
        SelectImageList.Add(SetButton.GetComponent<Image>());
        SelectImageList.Add(PublicButton.GetComponent<Image>());

        //SetSelectFalse();

        GlobalData.SetPageIndexAction += OnSetPageDragAction;

        if (MyMessageData.isFirstMainTop)//第一次启动不进
        {
            MyMessageData.isFirstMainTop = false;

            if (PlayerPrefs.GetInt(GlobalData.AutoLogin) == 1)
            {
                OnSetBtnClick();
                UI_SettingPanel.instance.AutoLoginEvent();
            }
        }
        else
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)//有网
                OnSetPageDragAction(MyMessageData.pageIndex);
            else if (MyMessageData.pageIndex != 0)//没网且从下载栏进去过
            {
                OnLocationBtnClick();
            }
        }

    }
    private void OnSetPageDragAction(int index)
    {
        //if (MyMessageData.pageIndex != 0)
        //{
        //    //HxMainManager.instance.panel_bg.SetActive(true);
        //}

        SetSelectFalse();
        MyMessageData.pageIndex = index;

        switch (index)
        {
            case 0:
                OnPublicBtnClick();
                break;
            case 1:
                OnProjectBtnClick();
                break;
            case 2:
                OnLocationBtnClick();
                break;
            case 3:
                OnSetBtnClick();
                break;
            default:
                break;
        }
    }

    private void OnPublicBtnClick()
    {
        publicItem.SetActive(true);
        projectItem.SetActive(false);
        localItem.SetActive(false);
        rect.horizontalNormalizedPosition = 0f;
        GlobalData.pageValue = 0f;
        SetSelectFalse();
        TitleText.text = "展厅";
        SelectImageList[3].sprite = SpriteManager.Instance.展厅点击;
        GlobalData.SetOptionPage(0);
        MyMessageData.pageIndex = 0;
    }

    /// <summary>
    /// 设置页面 最后一页
    /// </summary>
    public void OnSetBtnClick()
    {
        print("/////////OnSetBtnClick()");
        projectItem.SetActive(false);
        publicItem.SetActive(false);
        localItem.SetActive(false);

        rect.horizontalNormalizedPosition = 1f;
        GlobalData.pageValue = 1f;
        SetSelectFalse();
        TitleText.text = "设置";
        SelectImageList[2].sprite = SpriteManager.Instance.登录点击;
    }

    /// <summary>
    /// 下载到本地页面  第二页
    /// </summary>
    private void OnLocationBtnClick()
    {
        localItem.SetActive(true);
        publicItem.SetActive(false);
        projectItem.SetActive(false);
        rect.horizontalNormalizedPosition = 2 / 3f;
        GlobalData.pageValue = 2 / 3f;

        SetSelectFalse();
        SelectImageList[1].sprite = SpriteManager.Instance.下载点击;
        TitleText.text = "下载";

        GlobalData.SetOptionPage(2);
        MyMessageData.pageIndex = 2;
    }


    /// <summary>
    /// 项目页面 第一页
    /// </summary>
    private void OnProjectBtnClick()
    {
        projectItem.SetActive(true);
        publicItem.SetActive(false);
        localItem.SetActive(false);
        rect.horizontalNormalizedPosition = 1 / 3f;
        GlobalData.pageValue = 1 / 3f;

        SetSelectFalse();
        SelectImageList[0].sprite = SpriteManager.Instance.云项目点击;
        TitleText.text = "云项目";
        GlobalData.SetOptionPage(1);
        MyMessageData.pageIndex = 1;
    }

    private void SetSelectFalse()
    {
        SelectImageList[0].sprite = SpriteManager.Instance.云项目未点击;
        SelectImageList[1].sprite = SpriteManager.Instance.下载未点击;
        SelectImageList[2].sprite = SpriteManager.Instance.登录未点击;
        SelectImageList[3].sprite = SpriteManager.Instance.展厅未点击;
    }

    private void OnDestroy()
    {
        GlobalData.SetPageIndexAction -= OnSetPageDragAction;

        ProjectButton.onClick.RemoveAllListeners();
        LocationButton.onClick.RemoveAllListeners();
        SetButton.onClick.RemoveAllListeners();
        PublicButton.onClick.RemoveAllListeners();
    }
}
