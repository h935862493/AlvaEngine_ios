using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingConfrim : MonoBehaviour
{
    public RectTransform TipRect;
    public RectTransform LoginRect;
    public RectTransform UnLoginRect;

    private void Awake()
    {
        float ratio = Screen.height / 1920f;
        foreach (RectTransform item in TipRect)
        {
            item.sizeDelta = new Vector2(item.sizeDelta.x * ratio, item.sizeDelta.y * ratio);
        }
        foreach (RectTransform item in LoginRect)
        {
            item.sizeDelta = new Vector2(item.sizeDelta.x * ratio, item.sizeDelta.y * ratio);
        }
        foreach (RectTransform item in UnLoginRect)
        {
            item.sizeDelta = new Vector2(item.sizeDelta.x * ratio, item.sizeDelta.y * ratio);
        }


        // RectTransform tipButtonRect = TipRect.GetComponentInChildren<Button>().GetComponent<RectTransform>();
        // //offsetMin ： 对应Left、TopBottom
        //// offsetMax ： 对应Right、Top
        // tipButtonRect.offsetMin = new Vector2(0.305f * Screen.width, tipButtonRect.offsetMin.y);
        // tipButtonRect.offsetMax = new Vector2(-0.305f * Screen.width, tipButtonRect.offsetMax.y);
        // //sizeDelta.x即为RectTransform中的width 
        // //sizeDelta.y即为RectTransform中的height
        // tipButtonRect.sizeDelta = new Vector2(tipButtonRect.sizeDelta.x, 0.08f * Screen.height);

        // RectTransform loginButtonRect = LoginRect.GetComponentInChildren<Button>().GetComponent<RectTransform>();
        // loginButtonRect.offsetMin = new Vector2(0.35f * Screen.width, loginButtonRect.offsetMin.y);
        // loginButtonRect.offsetMax = new Vector2(-0.35f * Screen.width, loginButtonRect.offsetMax.y);
        // loginButtonRect.sizeDelta = new Vector2(loginButtonRect.sizeDelta.x, 0.16f * Screen.height);

        // RectTransform loginLogoRect = LoginRect.Find("LogoImage").GetComponent<RectTransform>();
        // loginLogoRect.sizeDelta = new Vector2(0.25f * Screen.width,0.14f * Screen.height);

        // RectTransform loginBgRect = LoginRect.Find("Bg").GetComponent<RectTransform>();
        // //anchoredPosition:对应PosX PosY
        // loginBgRect.anchoredPosition = new Vector2(loginBgRect.anchoredPosition.x,0.14f * Screen.height);

        // RectTransform unLoginLogoRect = UnLoginRect.Find("LogoImage").GetComponent<RectTransform>();
        // unLoginLogoRect.sizeDelta = new Vector2(0.25f * Screen.width, 0.14f * Screen.height);
        // unLoginLogoRect.anchoredPosition = new Vector2(unLoginLogoRect.anchoredPosition.x, 0.13f * Screen.height);

        // RectTransform unLoginEmailRect = unLoginLogoRect.Find("EmailInputField").GetComponent<RectTransform>();
        // unLoginEmailRect.offsetMin = new Vector2(-0.16f * Screen.width, unLoginEmailRect.offsetMin.y);
        // unLoginEmailRect.offsetMax = new Vector2(0.09f * Screen.height, unLoginEmailRect.offsetMax.y);

        // RectTransform unLoginPwsRect = unLoginLogoRect.Find("PasswordInputField").GetComponent<RectTransform>();
        // unLoginPwsRect.offsetMin = new Vector2(-0.16f * Screen.width, unLoginPwsRect.offsetMin.y);
        // unLoginPwsRect.offsetMax = new Vector2(0.09f * Screen.height, unLoginPwsRect.offsetMax.y);

        // RectTransform unLoginBtnRect = unLoginLogoRect.GetComponentInChildren<Button>().GetComponent<RectTransform>();
        // unLoginBtnRect.offsetMin = new Vector2(-0.16f * Screen.width, unLoginBtnRect.offsetMin.y);
        // unLoginBtnRect.offsetMax = new Vector2(0.09f * Screen.height, unLoginBtnRect.offsetMax.y);
        // unLoginBtnRect.anchoredPosition = new Vector2(unLoginBtnRect.anchoredPosition.x, -0.06f * Screen.height);

        // RectTransform unLoginTipRect = UnLoginRect.Find("TipImage").GetComponent<RectTransform>();
        // unLoginTipRect.offsetMin = new Vector2(0.15f * Screen.width, unLoginTipRect.offsetMin.y);
        // unLoginTipRect.offsetMax = new Vector2(-0.09f * Screen.height, unLoginTipRect.offsetMax.y);
        // unLoginTipRect.anchoredPosition = new Vector2(unLoginTipRect.anchoredPosition.x, 0.04f * Screen.height);
    }
}
