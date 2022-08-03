using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class SelectAgent : Agent
    {
        public SelectStyle selectStyle;
        TMP_Dropdown dropdown;
        Image downArrow;
        Image upArrow;
        Toggle itemToggle;
        Sprite downImage;
        Sprite upImage;
        EventTrigger dropdownEventTrigger;  
        Button blockerButton;
        bool isInit = false;
        Image selectImage;
        Image contentImage;
        Transform srollViewTran;
        Vector3 initSrollViewTranPositon;
        RectTransform rectTransform;
        int chooseIndex = 0;
        TMP_Text label;
        [SerializeField]
        public List<UnityEvent> optionsChooseEvent = new List<UnityEvent>();

        public UnityEvent<int> optionsValueChange;
       
        void Awake()
        {
            MyInit();
            if (dropdown)
            {
                dropdownEventTrigger = dropdown.GetComponent<EventTrigger>();

                EventTrigger.Entry clickEntry = new EventTrigger.Entry();
                clickEntry.eventID = EventTriggerType.PointerClick;
                clickEntry.callback.AddListener(OnClickDropdown);
                dropdownEventTrigger.triggers.Add(clickEntry);
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            }
            downArrow.gameObject.SetActive(true);
            upArrow.gameObject.SetActive(false);
        }
        public void InitOptionsChooseEvent(TMP_Dropdown dropdown)
        {
           // dropdown= GetComponent<TMP_Dropdown>();
            if (optionsChooseEvent.Count== dropdown.options.Count)
            {
                return;
            }
            if (optionsChooseEvent.Count < dropdown.options.Count)
            {
                int diff1 = dropdown.options.Count - optionsChooseEvent.Count;
                for (int i = 0; i < diff1; i++)
                {        
                    optionsChooseEvent.Add(new UnityEvent());
                }
            }
            if (optionsChooseEvent.Count > dropdown.options.Count)
            {
                int diff2 = optionsChooseEvent.Count - dropdown.options.Count;
                for (int i = 0; i < diff2; i++)
                {
                    optionsChooseEvent.RemoveAt(optionsChooseEvent.Count-1);
                }
            }          
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            MyInit();
            if (!dropdown)
            {
                dropdown = GetComponent<TMP_Dropdown>();             
            }
            if (dropdown)
            {                
                SetLabeContent(dropdown.value);
            }
        }
#endif
        void SetLabeContent(int index)
        {
            if (dropdown.options==null || dropdown.options.Count== 0)
            {
                return;
            }
            if (index<=0 )
            {
                label .text = dropdown.options[0].text;
            }
            else if (index > 0 && index< dropdown.options.Count)
            {
                label.text = dropdown.options[index].text;
            }
            else if (index > 0 && index >=dropdown.options.Count)
            {
                label.text = dropdown.options[dropdown.options.Count-1].text;
            }
        }   
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            dropdown = GetComponent<TMP_Dropdown>();
            selectImage = GetComponent<Image>();
            srollViewTran = dropdown.template;           
            rectTransform = GetComponent<RectTransform>();
            downArrow = transform.Find("DownArrow").GetComponent<Image>();
            upArrow = transform.Find("UpArrow").GetComponent<Image>();
            label = transform.Find("Label").GetComponent<TMP_Text>();
            if (srollViewTran)
            {
                contentImage = srollViewTran.GetComponent<Image>();
                itemToggle = srollViewTran.Find("Viewport/Content/Item").GetComponent<Toggle>();
            }
          
            isInit = true;
        }
        private void OnDropdownValueChanged(int arg0)
        {
           // Debug.LogError("index--" + arg0);
            SetLabeContent(arg0);
            OnDropDownValueChangedResult(arg0);
            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { arg0.ToString() }).MessageToJson());

        }
     
#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction<int> unityAction = (UnityAction<int>)Delegate.CreateDelegate(typeof(UnityAction<int>), target, "Execute");
                UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(optionsValueChange, unityAction);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Couldn't bind to method 'Execute'."))
                {
                    if (UnityEditor.EditorPrefs.GetString("Language") == "Chinese")
                    {
                        //无法选择加载到哪一个事件上，需要手动选择List中一个对象进行绑定。
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", "不能绑定optionsValueChange(int)事件，请在OptionsChooseEvent中手动绑定。", "确定");
                    }
                    else
                    {
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", "Couldn't binding optionsValueChange(int).Please bind manually in optionschooseevent.", "确定");
                    }
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning",ex.Message, "确定");
                }
            }

        }
#endif

        private void OnDropDownValueChangedResult(int arg0)
        {
            downArrow.gameObject.SetActive(true);
            upArrow.gameObject.SetActive(false);
            chooseIndex = arg0;
            optionsChooseEvent[arg0]?.Invoke();
            optionsValueChange?.Invoke(arg0);
        }
        public int GetContent()
        {
            return chooseIndex;
        }
        private void OnClickDropdown(BaseEventData arg0)
        {
            downArrow.gameObject.SetActive(false);
            upArrow.gameObject.SetActive(true);
            RegisterBlockerEvent();
        }

        private void RegisterBlockerEvent()
        {
            GameObject blocker = GameObject.Find("Blocker");
            if (blocker)
            {
                blockerButton = blocker.GetComponent<Button>();
               // Debug.Log("blocker");
            }
            if (blockerButton)
            {
               // Debug.Log("blockerButton");
                blockerButton.onClick.AddListener(ClickBlockButton);
            }
        }

        void ClickBlockButton()
        {
            downArrow.gameObject.SetActive(true);
            upArrow.gameObject.SetActive(false);
        }

        public override bool GetShowStyle()
        {
            return true;
        }
        public override void Init()
        {
            defaultElement = "style1";
            downImage = selectStyle.downImage;
            upImage = selectStyle.upImage;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
            //  initSrollViewTranPositon = dropdown.template.localPosition;
        }
        public override string[] GetStyle()
        {
            int totalStyle = selectStyle.SelectStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i < totalStyle; i++)
            {
                styleList[i] = "style" + (i + 1);
            }
            return styleList;
        }
        public override void OnStyleValueSelected(object value)
        {
            string style = value.ToString();
            MyInit();
            if (defaultElement == value.ToString() || value.ToString() == "None")
            {
                return;
            }
            string[] styles = GetStyle();
            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].Equals(style))
                {
                    SelectStyleItem item = selectStyle.SelectStyleList[i];
                    selectImage.sprite = item.titleBackground;
                    contentImage.sprite = item.contentBackground;
                    srollViewTran.localPosition = new Vector3(0, item.distanceToTop - rectTransform.sizeDelta.y / 2, 0);
                    itemToggle.spriteState = new SpriteState()
                    {
                        highlightedSprite = item.tagChooseImage,
                        pressedSprite = item.tagChooseImage,
                        selectedSprite = item.tagChooseImage,
                    };
                    dropdown.options = new List<TMP_Dropdown.OptionData>();
                    for (int j = 0; j < item.tagItemList.Count; j++)
                    {
                        dropdown.options.Add(new TMP_Dropdown.OptionData(item.tagItemList[j].tagName, item.tagBackground));
                    }
                    // dropdown.captionText.text = item.tagItemList[0].tagName;
                    if (label)
                    {
                        label.text = item.tagItemList[0].tagName;
                    }              
                    break;
                }
            }
            //  rectTransform.sizeDelta = new Vector2(background.sprite.texture.width, background.sprite.texture.height);
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        public override void ExternalCall(string[] parameter, string methodName)
        {
            base.ExternalCall(parameter);
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.value = int.Parse(parameter[0]);
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            optionsChooseEvent[int.Parse(parameter[0])]?.Invoke();
        }

        public void OnA()
        {
           // MyInit();
            ExternalCall(new string[1] { "1" }, string.Empty);
        }
    }
}

