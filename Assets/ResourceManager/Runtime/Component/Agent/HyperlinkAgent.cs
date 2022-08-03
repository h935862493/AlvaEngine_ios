using Alva.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class HyperlinkAgent : Agent
    {
        UnityEvent OnClick = new UnityEvent();
        RectTransform rectTransform;
        TextMeshProUGUI textMesh;
        Button button;
        public string url = "https://www.baidu.com";
        bool isInit = false;
        public string content = "百度";
        string currentContent = "百度";
        void Awake()
        {
            MyInit();
        }

        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
            button = GetComponent<Button>();
            OnClick.AddListener(OpenHyperlink);
            button.onClick.AddListener(OpenHyperlink);
            isInit = true;
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            MyInit();
            if (currentContent!=content)
            {
                currentContent = content;
                textMesh.text = content;
            }
            else
            {
                if (textMesh.text != content)
                {
                    content = textMesh.text;
                    currentContent = content;
                }
            }
        }
#endif
        private void OpenHyperlink()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Application.identifier == "com.AlvaSystems.AlvaVision")
                PlantformInterface.AlvaAddBrowserView(url);
            else
                Application.OpenURL(url);
#else
            Application.OpenURL(url);
            PlayerData.Instance().SendSyncInfo(new UnityMessageInfo(id, "ExternalCall", new string[1] { url.ToString() }).MessageToJson());
#endif
        }
        public override void ExternalCall(string[] parameter, string methName)
        {
            //Debug.Log("alvaeditor" + parameter[0].ToString());
            if (OnClick != null)
                OnClick.Invoke();
        }
      

        public void SetContent(string _url)
        {
            if (string.IsNullOrEmpty(_url))
            {
                return;
            }
            url = _url;
            rectTransform.sizeDelta = new Vector2(url.Length * 18, 80);
        }
        public string GetContent()
        {
            return url;
        }
        public override bool GetShowStyle()
        {
            return true;
        }
        public override void Init()
        {
            defaultElement = "style1";
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public override string[] GetStyle()
        {
            return new string[1] { "style1" };
        }
        public override void OnStyleValueSelected(object value)
        {
            Debug.Log(value.ToString());
        }
        public void OnA()
        {
            ExternalCall(new string[1] { "true" }, string.Empty);
        }
    }
}

