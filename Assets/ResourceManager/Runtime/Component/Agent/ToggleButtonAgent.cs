using Alva.Style;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class ToggleButtonAgent : Agent
    {
        public enum ChooseItemNumber
        {
            first,
            second,
            third,
        }
        public ToggleButtonStyle toggleButtonStyle;
        RectTransform rectTransform;
        Image background;
        Image toggleBackground1;
        Image toggleCheckmark1;
        TextMeshProUGUI toggleText1;
        Image toggleBackground2;
        Image toggleCheckmark2;
        TextMeshProUGUI toggleText2;
        Image toggleBackground3;
        Image toggleCheckmark3;
        TextMeshProUGUI toggleText3;
        Image splite1;
        Image splite2;
        Button currentButton;
        bool isInit = false;       
        ToggleGroup toggleGroup;
        Toggle[] toggles;
        [HideInInspector]
        public int chooseIndex = 0;
        [HideInInspector]
        public ChooseItemNumber chooseItemNumber = ChooseItemNumber.first;
        public UnityEvent<int> onValueChange;
        public UnityEvent ChooseFirstEvent;
        public UnityEvent ChooseSecondEvent;
        public UnityEvent ChooseThirdEvent;
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            background = transform.Find("Background").GetComponent<Image>();
            currentButton = GetComponent<Button>();
            toggleBackground1 = background.transform.Find("ToggleButton1/Background").GetComponent<Image>();
            toggleCheckmark1 = toggleBackground1.transform.Find("Checkmark").GetComponent<Image>();
            toggleText1 = toggleCheckmark1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            toggleBackground2 = background.transform.Find("ToggleButton2/Background").GetComponent<Image>();
            toggleCheckmark2 = toggleBackground2.transform.Find("Checkmark").GetComponent<Image>();
            toggleText2 = toggleCheckmark2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            toggleBackground3 = background.transform.Find("ToggleButton3/Background").GetComponent<Image>();
            toggleCheckmark3 = toggleBackground3.transform.Find("Checkmark").GetComponent<Image>();
            toggleText3 = toggleCheckmark3.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            splite1= background.transform.Find("Splite1").GetComponent<Image>();
            splite2 = background.transform.Find("Splite2").GetComponent<Image>();
            isInit = true;
        }
        void Awake()
        {
            toggleGroup = transform.GetComponentInChildren<ToggleGroup>();
            toggles = GetComponentsInChildren<Toggle>();
            AddAllTogglesEvent();
        }

        private void AddAllTogglesEvent()
        {
            if (toggles == null)
            {
                return;
            }
            for (int i = 0; i < toggles.Length; i++)
            {
                AddToggleEvent(i);
            }
        }

#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction<int> unityAction = (UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityAction<int>), target, "Execute");
                UnityEditor.Events.UnityEventTools.AddPersistentListener(onValueChange, unityAction);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Equals("Couldn't bind to method 'Execute'."))
                {
                    if (UnityEditor.EditorPrefs.GetString("Language") == "Chinese")
                    {
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", "不能绑定执行方法，请手动绑定！", "确定");
                    }
                    else
                    {
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", "Couldn't bind to method 'Execute'.Please bind manually!", "Submit");
                    }
                }
                else
                {
                    if (UnityEditor.EditorPrefs.GetString("Language") == "Chinese")
                    {
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", ex.Message, "确定");
                    }
                    else
                    {
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", ex.Message, "Submit");
                    }
                }
            }

        }
#endif

        private void AddToggleEvent(int i)
        {
            toggles[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    chooseItemNumber = (ChooseItemNumber)i;
                    chooseIndex = i;
                    onValueChange.Invoke(i);
                    switch (i)
                    {
                        case 0:
                            ChooseFirstEvent?.Invoke();
                            break;
                        case 1:
                            ChooseSecondEvent?.Invoke();
                            break;
                        case 2:
                            ChooseThirdEvent?.Invoke();
                            break;
                    }
                    //平台交互信息同步---发送
                    Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { i.ToString() }).MessageToJson());
                }
            });
        }
        public int GetContent()
        {
            return chooseIndex;
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
            int totalStyle = toggleButtonStyle.ToggleButtonStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i < totalStyle; i++)
            {
                styleList[i] = "style" + (i + 1);
            }
            return styleList;
        }
      
        public override void OnStyleValueSelected(object value)
        {       
            MyInit();
            if (defaultElement == value.ToString() || value.ToString() == "None")
            {
                return;
            }
            base.OnStyleValueSelected(value);
            for (int i = 0; i < toggleButtonStyle.ToggleButtonStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    ToggleButtonStyleItem item = toggleButtonStyle.ToggleButtonStyleList[i];
                    background.sprite = item.toggleButtonBackground;
                    toggleBackground1 .sprite= toggleBackground2.sprite = toggleBackground3.sprite = item.toggleBackground;
                    if (item.showToggleBackground)
                    {
                        toggleBackground1.enabled = true;
                        toggleBackground2.enabled = true;
                        toggleBackground3.enabled = true;
                    }
                    else
                    {
                        toggleBackground1.enabled = false;
                        toggleBackground2.enabled = false;
                        toggleBackground3.enabled = false;
                    }
                    toggleCheckmark1.sprite = toggleCheckmark2.sprite = toggleCheckmark3.sprite = item.toggleCheckmark;
                    toggleText1.text = item.toggleList[0].name;
                    toggleText2.text = item.toggleList[1].name;
                    toggleText3.text = item.toggleList[2].name;
                    splite1.sprite = splite2.sprite = item.spliteSprite;
                    if (item .showSpliteSprite)
                    {
                        splite1.enabled = true;
                        splite2.enabled = true;
                    }
                    else
                    {
                        splite1.enabled = false;
                        splite2.enabled = false;
                    }
                    break;
                }
            }
            rectTransform.sizeDelta = new Vector2(background.sprite.texture.width, background.sprite.texture.height);
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }
        public override void ExternalCall(string[] parameter, string methodName)
        {
            /*for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].onValueChanged.RemoveAllListeners();
            }*/
            int m_index;
            if(int.TryParse(parameter[0], out m_index))
            {
                toggles[m_index].isOn = true;
                AddAllTogglesEvent();
            }
        }

        /// <summary>
        /// 选项卡————同步功能测试
        /// </summary>
        public void OnA()
        {
            ExternalCall(new string[1] { "0" }, string.Empty);
        }

    }
}

