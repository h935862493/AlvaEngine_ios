using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessageBoxPanel : MonoBehaviour
{
    public Button CloseButton;
    public Button SubmitButton;
    public Button CancelButton;

    public Text TipText;
    public Text WarnText;

    public GameObject Tip;
    public GameObject Warn;

    
    Action<bool> ResultCallBack;

    private void Start()
    {
        CloseButton.onClick.AddListener(OnCloseButtonClick);
        SubmitButton.onClick.AddListener(OnSubmitButtonClick);
        CancelButton.onClick.AddListener(OnCancelButtonClick);
       
    }

    /// <summary>
    /// 设置提示信息
    /// </summary>
    /// <param name="mgs"></param>
    public void SetTipData(string mgs)
    {
        TipText.text = mgs;
        Tip.SetActive(true);
    }

    /// <summary>
    /// 设置警告信息
    /// </summary>
    /// <param name="mgs"></param>
    /// <param name="ResultCallBack"></param>
    public void SetWarnData(string mgs, Action<bool> tResultCallBack)
    {

        WarnText.text = mgs;
        Warn.SetActive(true);
        //StartCoroutine(Result(tResultCallBack));
        ResultCallBack = tResultCallBack;

    }

    private void OnCancelButtonClick()
    {
        if (ResultCallBack != null)
        {
            ResultCallBack?.Invoke(false);
        }
        Warn.SetActive(false);
        ResultCallBack = null;
    }

    private void OnSubmitButtonClick()
    {
        if (ResultCallBack != null)
        {
            ResultCallBack?.Invoke(true);
        }
        Warn.SetActive(false);
        ResultCallBack = null;
    }

    private void OnCloseButtonClick()
    {
        Tip.SetActive(false);
    }

    private void OnDestroy()
    {
        CancelButton.onClick.RemoveAllListeners();
        SubmitButton.onClick.RemoveAllListeners();
        CloseButton.onClick.RemoveAllListeners();
    }
}
