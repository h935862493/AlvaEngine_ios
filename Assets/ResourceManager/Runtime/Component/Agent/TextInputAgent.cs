using Alva.Style;
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
    public class TextInputAgent : Agent
    {
        public TextInputStyle textInputStyle;
        RectTransform rectTransform;
        Image background;
        bool isInit = false;
        TMP_InputField textMesh;
        public UnityEvent<string> onValueChanged;
        public UnityEvent<string> onEndEdit;
        private void Awake()
        {
            MyInit();
            textMesh.onValueChanged.AddListener(OnTextInputValueChangeEvent);
            textMesh.onEndEdit.AddListener(OnTextInputEndEditEvent);
        }

        private void OnTextInputEndEditEvent(string arg0)
        {
            onEndEdit?.Invoke(arg0);
        }

        private void OnTextInputValueChangeEvent(string arg0)
        {
            onValueChanged?.Invoke(arg0);
        }

        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            background = GetComponent<Image>();
            textMesh= GetComponent<TMP_InputField>();
            isInit = true;
        }
        public void SetContent(string content)
        {
            textMesh.text = content;
        }
        public string GetContent()
        {
            return textMesh.text;
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
            int totalStyle = textInputStyle.TextInputStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i < totalStyle; i++)
            {
                styleList[i] = "style" + (i + 1);
            }
            return styleList;
        }
        public override void OnStyleValueSelected(object value)
        {
            Debug.Log(value.ToString());
            MyInit();
            if (defaultElement == value.ToString() || value.ToString() == "None")
            {
                return;
            }
            base.OnStyleValueSelected(value);
            for (int i = 0; i < textInputStyle.TextInputStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    background.sprite = textInputStyle.TextInputStyleList[i].background;                
                    break;
                }
            }
            // rectTransform.sizeDelta = new Vector2(background.sprite.texture.width, background.sprite.texture.height);
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

    }
}

