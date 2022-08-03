using Alva.Components.Manager;
using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class CheckBoxAgent : Agent
    {
        public CheckBoxStyle checkBoxStyle;
        bool isInit = false;
        RectTransform rectTransform;
        Transform styleTran1;
        Transform styleTran2;
        Transform styleTran3;
        [HideInInspector]
        public Toggle toggle;
        Image background1;
        Image background2;
        Image background3;
        Image checkBoxBackground;
        Image checkmark1;
        Image checkmark2;
        Image checkmark3;            
        public UnityEvent<bool> checkBoxEvent;
        public UnityEvent chooseEvent;
        public UnityEvent noChooseEvent;      
        [SerializeField]
        public ToggleGroup toggleGroup;
        [HideInInspector]
        public GroupManager groupManager;
        public bool isOn = true;
        bool recordIsOn = true;
       
        private void Awake()
        {
            MyInit();
            toggle.group = toggleGroup;
            if (toggleGroup)
            {
                groupManager = toggleGroup.GetComponent<GroupManager>();
                groupManager.AddItem(toggle);
            }
            toggle.onValueChanged.AddListener(ToggleValueChangeEvent);
        }
#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction<bool> unityAction = (UnityAction<bool>)Delegate.CreateDelegate(typeof(UnityAction<bool>), target, "Execute");
                UnityEditor.Events.UnityEventTools.AddPersistentListener(checkBoxEvent, unityAction);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Couldn't bind to method 'Execute'."))
                {
                    if (UnityEditor.EditorPrefs.GetString("Language") == "Chinese")
                    {
                        if (UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", "不能绑定checkBoxEvent(bool)事件，是否绑定其它事件", "确定", "取消"))
                        {
                            if (UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", "请选择绑定事件", "chooseEvent", "noChooseEvent"))
                            {
                                try
                                {
                                    UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
                                    UnityEditor.Events.UnityEventTools.AddPersistentListener(chooseEvent, unityAction);
                                }
                                catch (Exception exc)
                                {
                                    UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", exc.Message, "确定");
                                }
                            }
                            else
                            {
                                try
                                {
                                    UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
                                    UnityEditor.Events.UnityEventTools.AddPersistentListener(noChooseEvent, unityAction);
                                }
                                catch (Exception exnc)
                                {
                                    UnityEditor.EditorUtility.DisplayDialog("AlvaEditor 警告", exnc.Message, "确定");
                                }

                            }
                        }
                    }
                    else
                    {
                        if (UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", "Couldn't binding checkBoxEvent(bool)，do you want to binding other events?", "Yes", "No"))
                        {
                            if (UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", "Choose the event", "chooseEvent", "noChooseEvent"))
                            {
                                try
                                {
                                    UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
                                    UnityEditor.Events.UnityEventTools.AddPersistentListener(chooseEvent, unityAction);
                                }
                                catch (Exception exc)
                                {
                                    UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", exc.Message, "Yes");
                                }
                            }
                            else
                            {
                                try
                                {
                                    UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
                                    UnityEditor.Events.UnityEventTools.AddPersistentListener(noChooseEvent, unityAction);
                                }
                                catch (Exception exnc)
                                {
                                    UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", exnc.Message, "Yes");
                                }

                            }
                        }
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
                        UnityEditor.EditorUtility.DisplayDialog("AlvaEditor Warning", ex.Message, "Yes");
                    }  
                }
            }

        }
#endif
        private void ToggleValueChangeEvent(bool arg0)
        {
            if (arg0)
            {
                chooseEvent?.Invoke();
            }
            else
            {
                noChooseEvent?.Invoke();
            }
            checkBoxEvent?.Invoke(arg0);
            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { arg0.ToString() }).MessageToJson());
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            MyInit();
            if (recordIsOn  != isOn)
            {
                recordIsOn = isOn;
                toggle.isOn =isOn ;
            }
            else
            {
                if (toggle .isOn !=isOn )
                {
                    isOn = toggle.isOn;
                    recordIsOn = isOn;
                }
            }          
        }
#endif
        public void SetToggleGroup(ToggleGroup group)
        {
            toggleGroup = group;
        }
        public Toggle GetToggle()
        {
            return toggle;
        }
        public bool GeContent()
        {
            return toggle.isOn;
        }
        public override void Init()
        {
            defaultElement = "style1";
            transform.Find("Style2").gameObject.hideFlags = HideFlags.HideInHierarchy;
            transform.Find("Style3").gameObject.hideFlags = HideFlags.HideInHierarchy;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            toggle = GetComponent<Toggle>();
            rectTransform = GetComponent<RectTransform>();
            styleTran1 = transform.Find("Style1");
            styleTran2 = transform.Find("Style2");
            styleTran3 = transform.Find("Style3");
            background1 = styleTran1.Find("Background").GetComponent<Image>();
            background2 = styleTran2.Find("Background").GetComponent<Image>();
            background3 = styleTran3.Find("Background").GetComponent<Image>();
            checkBoxBackground = styleTran3.GetComponent<Image>();
            checkmark1 = styleTran1.Find("Background/Checkmark").GetComponent<Image>();
            checkmark2 = styleTran2.Find("Background/Checkmark").GetComponent<Image>();
            checkmark3 = styleTran3.Find("Background/Checkmark").GetComponent<Image>();
            isInit = true;
        }
        public override bool GetShowStyle()
        {
            return true;
        }
        public override string[] GetStyle()
        {
            int totalStyle = checkBoxStyle.CheckBoxStyleList.Count;
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
            Debug.Log(value);
            for (int i = 0; i < checkBoxStyle.CheckBoxStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    CheckBoxStyleItem item = checkBoxStyle.CheckBoxStyleList[i];
                    if (item.checkBoxType == CheckBoxType.NoWord)
                    { 
                        ControlStyleChangeShow(1);
                        toggle.targetGraphic = background1;
                        toggle.graphic = checkmark1;
                        background1.sprite = item.toggleBackground;
                        checkmark1.sprite = item.check;
                        rectTransform.sizeDelta = new Vector2(80, 80);
                    }
                    else if (item.checkBoxType == CheckBoxType.OnlyWord)
                    {                  
                        ControlStyleChangeShow(2);
                        toggle.targetGraphic = background2;
                        toggle.graphic = checkmark2;
                        background2.sprite = item.toggleBackground;
                        checkmark2.sprite = item.check;
                        rectTransform.sizeDelta = new Vector2(240, 80);
                    }
                    else if (item.checkBoxType == CheckBoxType.WordAndBackground)
                    {             
                        ControlStyleChangeShow(3);
                        toggle.targetGraphic = background3;
                        toggle.graphic = checkmark3;
                        background3.sprite = item.toggleBackground;
                        checkmark3.sprite = item.check;
                        checkBoxBackground.sprite = item.checkBoxBackground;
                        rectTransform.sizeDelta = new Vector2(500, 120);
                    }
                    break;
                }
            }
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        private void ControlStyleChangeShow(int type)
        {
            styleTran1.gameObject.SetActive(1== type);
            styleTran2.gameObject.SetActive(2 == type);
            styleTran3.gameObject.SetActive(3 == type);
            styleTran1.gameObject.hideFlags = 1 == type ? HideFlags.None : HideFlags.HideInHierarchy;
            styleTran2.gameObject.hideFlags = 2 == type ? HideFlags.None : HideFlags.HideInHierarchy;
            styleTran3.gameObject.hideFlags = 3 == type ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        public override void ExternalCall(string[] parameter, string methodName)
        {
            base.ExternalCall(parameter);
            bool res;
            bool.TryParse(parameter[0], out res);
            if (res)
            {
                chooseEvent?.Invoke();
            }
            else
            {
                noChooseEvent?.Invoke();
            }
            checkBoxEvent?.Invoke(res);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.isOn = res;
            toggle.onValueChanged.AddListener(ToggleValueChangeEvent);
        }
    }
}

