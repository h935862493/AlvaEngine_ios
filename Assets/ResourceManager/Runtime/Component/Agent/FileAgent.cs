using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class FileAgent : Agent
    {
        RectTransform rectTransform;
        TextMeshPro text;
        Button button;
        string url = "www.baidu.com";
        bool isInit = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }  
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }  
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TextMeshPro>();
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenFile);
            isInit = true;
        }

        private void OpenFile()
        {

           // Application.OpenURL(url);
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
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAll);
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
    }
}

