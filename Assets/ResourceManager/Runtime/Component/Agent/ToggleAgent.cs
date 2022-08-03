using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class ToggleAgent : Agent
    {
        public ToggleStyle toggleStyle;
        bool isInit = false;
        RectTransform rectTransform;
        Image background;
        Image checkmark;   
        Toggle toggle;
        public UnityEvent chooseEvent;
        public UnityEvent noChooseEvent;

        public UnityEvent<bool> toggleEvent;
        void Awake()
        {
            MyInit();
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction<bool> unityAction = (UnityAction<bool>)Delegate.CreateDelegate(typeof(UnityAction<bool>), target, "Execute");
                UnityEditor.Events.UnityEventTools.AddPersistentListener<bool>(toggleEvent, unityAction);
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
        public Toggle GetToggle()
        {
            return toggle;
        }
        public bool GeContent()
        {
            return toggle.isOn;
        }
        void OnToggleValueChanged(bool isON)
        {
            if (!background)
            {
                background = transform.Find("Background").GetComponent<Image>();
            }
            if (isON)
            {
                background.enabled = false;
                chooseEvent?.Invoke();
            }
            else
            {
                background.enabled = true;
                noChooseEvent?.Invoke();
            }
            toggleEvent?.Invoke(isON);

            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { isON.ToString() }).MessageToJson());

        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }   
            rectTransform = GetComponent<RectTransform>();
            background = transform.Find("Background").GetComponent<Image>();
            checkmark = transform.Find("Background/Checkmark").GetComponent<Image>();
            isInit = true;
        }
        public override void Init()
        {
            defaultElement = "style1";
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public override bool GetShowStyle()
        {
            return true;
        }
        public override string[] GetStyle()
        {
            int totalStyle = toggleStyle.ToggleStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i <totalStyle; i++)
            {
                styleList[i] = "style" + (i+1);
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
            for (int i = 0; i < toggleStyle.ToggleStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    background.sprite = toggleStyle.ToggleStyleList[i].background;
                    checkmark.sprite = toggleStyle.ToggleStyleList[i].checkmark;
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
            base.ExternalCall(parameter);
            bool isON;
            bool.TryParse(parameter[0],out isON);
            if (isON)
            {
                background.enabled = false;
                chooseEvent?.Invoke();
            }
            else
            {
                background.enabled = true;
                noChooseEvent?.Invoke();
            }
            toggleEvent?.Invoke(isON);
            if (!background)
            {
                background = transform.Find("Background").GetComponent<Image>();
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.isOn = isON;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }


}

