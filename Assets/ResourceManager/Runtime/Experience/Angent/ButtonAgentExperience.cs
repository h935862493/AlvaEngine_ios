using Alva.Common;
using Alva.Style;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



namespace Alva.Runtime.Components
{
    public class ButtonAgentExperience : Agent
    {
        public ButtonStyle buttonStyle;
        [SerializeField]
        public List<UnityEvent> OnClickEvent = new List<UnityEvent>();
        RectTransform rectTransform;
        Image background;
        Button currentButton;
        TextMeshProUGUI buttonText;
        bool isInit = false;
        public override void Init()
        {
            defaultElement = "style1";
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
                UnityEvent onClickEvent = new UnityEvent();
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(onClickEvent, unityAction);
                OnClickEvent.Add(onClickEvent);
            }
            catch (Exception ex)
            {
                UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", ex.Message, "确定");
            }

        }
#endif
        public void SetContent(string content)
        {
            buttonText.text = content;
        }
        public string GetContent()
        {
            return buttonText.text;
        }
        public void SetContentImage(Sprite sprite)
        {
            background.sprite = sprite;
        }
        public Sprite GetContentImage()
        {
            return background.sprite;
        }
        public override bool GetShowStyle()
        {
            return true;
        }
        public override string[] GetStyle()
        {
            int totalStyle = buttonStyle.ButtonStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i < totalStyle; i++)
            {
                styleList[i] = "style" + (i + 1);
            }
            return styleList;
        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            background = GetComponent<Image>();
            currentButton = GetComponent<Button>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            isInit = true;
        }
        private void Awake()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(ButtonClick);
        }
        protected virtual void ButtonClick()
        {
            foreach (UnityEvent item in OnClickEvent)
            {
                if (item != null)
                    item.Invoke();
            }
            //平台交互信息同步---发送
            PlayerData.Instance().SendSyncInfo(new UnityMessageInfo(id, "ExternalCall", new string[0]).MessageToJson());
        }
        public override void ExternalCall(string[] parameter, string methName)
        {
            foreach (UnityEvent item in OnClickEvent)
            {
                if (item != null)
                    item.Invoke();
            }

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
            for (int i = 0; i < buttonStyle.ButtonStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    background.sprite = buttonStyle.ButtonStyleList[i].normal;
                    buttonText.enabled = buttonStyle.ButtonStyleList[i].showButtonText;
                    SetButtonSprites(buttonStyle.ButtonStyleList[i]);
                    rectTransform.sizeDelta = new Vector2(buttonStyle.ButtonStyleList[i].width, buttonStyle.ButtonStyleList[i].height);
                    break;
                }
            }
            //    rectTransform.sizeDelta = new Vector2(background.sprite.texture.width, background.sprite.texture.height);
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        private void SetButtonSprites(ButtonStyleItem styleItem)
        {
            switch (styleItem.buttonTransiton)
            {
                case ButtonTransiton.Color:
                    currentButton.transition = Selectable.Transition.ColorTint;
                    break;
                case ButtonTransiton.Sprite:
                    currentButton.transition = Selectable.Transition.SpriteSwap;
                    break;
                default:
                    break;
            }
            currentButton.spriteState = new SpriteState()
            {
                highlightedSprite = styleItem.sprites.highlightedSprite,
                pressedSprite = styleItem.sprites.pressedSprite,
                selectedSprite = styleItem.sprites.selectedSprite,
                disabledSprite = styleItem.sprites.disabledSprite
            };
        }
    }
}


