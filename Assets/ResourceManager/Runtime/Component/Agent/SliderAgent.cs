using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class SliderAgent : Agent
    {
        public SliderStyle sliderStyle;
        // [Header("加号减号按钮每次移动滑条数百分数")]
        // [Header("The plus or minus button moves the slider a few percent at a time")]
        public float stepLength = 1;
        //public Button addButton;
        //public Button reduceButton;
        //public Slider slider;
        int sliderValueChange;
        Image BG;
        Slider normalSlider;
        Button normalAddButton;
        Button normalReduceButton;
        Image normalbg;
        Image normalFillImage;
        Image normalHandle;
        Transform normalProgressHintTransform;
        TextMeshProUGUI normalProgressHintText;
        Slider roundSlider;
        Button roundAddButton;
        Button roundReduceButton;
        Image roundbg;
        Image roundFillImage;
        Image roundForeFillImage;
        //[Header("SliderMinValue")]
        [SerializeField]
        public int minValue = 0;
        //[Header("SliderMaxValue")]
        [SerializeField]
        public int maxValue = 100;
        // [Range(0, 100)]
        [SerializeField, HideInInspector]
        float value = 0;
        private float current_value;
        bool isInit = false;
        [SerializeField,HideInInspector]
        float progressValue = 0f;
        public UnityEvent<float> onValueChange;
        public UnityEvent<string> onValueChange1;
        string sliderValue;
        void Awake()
        {
            current_value = value;
            MyInit();
        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            BG = transform.Find("BG").GetComponent<Image>();
            normalSlider = BG.transform.Find("ProgressSlider").GetComponent<Slider>();
            normalReduceButton = normalSlider.transform.Find("ReduceButton").GetComponent<Button>();
            normalAddButton = normalSlider.transform.Find("AddButton").GetComponent<Button>();
            normalbg = normalSlider.transform.Find("Background").GetComponent<Image>();
            normalFillImage = normalSlider.transform.Find("FillArea/Fill").GetComponent<Image>();
            normalHandle = normalSlider.transform.Find("HandleSlideArea/Handle").GetComponent<Image>();
            normalProgressHintTransform = normalHandle.transform.Find("HintImage");
            normalProgressHintText = normalProgressHintTransform.transform.GetComponentInChildren<TextMeshProUGUI>();
            normalAddButton.onClick.AddListener(ClickAddButtonEvent);
            normalReduceButton.onClick.AddListener(ClickReduceButtonEvent);
            normalSlider.onValueChanged.AddListener(SliderValueChangeEvent);

            roundSlider = BG.transform.Find("RoundProgressSlider").GetComponent<Slider>();
            roundReduceButton = roundSlider.transform.Find("ReduceButton").GetComponent<Button>();
            roundAddButton = roundSlider.transform.Find("AddButton").GetComponent<Button>();
            roundbg = roundSlider.transform.Find("Background").GetComponent<Image>();
            roundFillImage = roundSlider.transform.Find("FillArea/Fill").GetComponent<Image>();
            roundForeFillImage = roundFillImage.transform.Find("foregroundImage").GetComponent<Image>();
            roundAddButton.onClick.AddListener(ClickAddButtonEvent1);
            roundReduceButton.onClick.AddListener(ClickReduceButtonEvent1);
            roundSlider.onValueChanged.AddListener(SliderValueChangeEvent1);
            isInit = true;
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Slider", GUILayout.Width(100));
            value = GUILayout.HorizontalSlider(value, minValue, maxValue);
            value = EditorGUILayout.FloatField(value, GUILayout.Width(60));
            // sliderValue = GUILayout.TextField(value.ToString(), GUILayout.Width(60));
            //  float.TryParse(sliderValue, out value);
            if (value > maxValue)
            {
                value = maxValue;
            }
            if (value < minValue)
            {
                value = minValue;
            }
            GUILayout.EndHorizontal();
            if (current_value != value)
            {
                current_value = value;
                OnRefreshProgressValue();
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(this, "Changed default element");
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
            }
        }

#endif

#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            try
            {
                UnityAction<float> unityAction = (UnityAction<float>)Delegate.CreateDelegate(typeof(UnityAction<float>), target, "Execute");
                UnityEditor.Events.UnityEventTools.AddPersistentListener<float>(onValueChange, unityAction);
            }
            catch (Exception ex)
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

        private void OnRefreshProgressValue()
        {
            // SetContent(value / 100.0f);
            SetContent((value - minValue) / (maxValue - minValue));
            //Debug.Log((value - minValue) / (maxValue - minValue));
        }
        void ClickAddButtonEvent()
        {
            normalSlider.value = normalSlider.value + stepLength / (maxValue - minValue) > 1 ? 1 : normalSlider.value + stepLength / (maxValue - minValue);
            value = minValue + normalSlider.value * (maxValue - minValue);
            current_value = value;
        }
        void ClickReduceButtonEvent()
        {
            normalSlider.value = normalSlider.value - stepLength / (maxValue - minValue) < 0 ? 0 : normalSlider.value - stepLength / (maxValue - minValue);
            value = minValue + normalSlider.value * (maxValue - minValue);
            current_value = value;
        }
        void SliderValueChangeEvent(float value)
        {
            if (normalProgressHintTransform.gameObject.activeInHierarchy)
            {
                sliderValueChange = (int)(value * (maxValue - minValue) + minValue);
                //Debug.Log(sliderValueChange);
                normalProgressHintText.text = sliderValueChange.ToString();
                //Debug.Log(normalProgressHintText.text);
            }
            progressValue = value;
            //onValueChange?.Invoke(value);
            //onValueChange1?.Invoke(value.ToString());
            onValueChange?.Invoke(value * (maxValue - minValue) + minValue);
            onValueChange1?.Invoke((value * (maxValue - minValue) + minValue).ToString());
            this.value = (int)(minValue + normalSlider.value * (maxValue - minValue));
            current_value = this.value;

            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { current_value.ToString() }).MessageToJson());
        }
        void ClickAddButtonEvent1()
        {
            roundSlider.value = roundSlider.value + stepLength / (maxValue - minValue) > 1 ? 1 : roundSlider.value + stepLength / (maxValue - minValue);
            value = minValue + roundSlider.value * (maxValue - minValue);
            current_value = value;
            //EditorApplication.RepaintHierarchyWindow();
        }
        void ClickReduceButtonEvent1()
        {
            roundSlider.value = roundSlider.value - stepLength / (maxValue - minValue) < 0 ? 0 : roundSlider.value - stepLength / (maxValue - minValue);
            value =minValue + roundSlider.value * (maxValue - minValue);
            current_value = value;
        }
        void SliderValueChangeEvent1(float value)
        {
            progressValue = value;
            //onValueChange?.Invoke(value);
            //onValueChange1?.Invoke(value.ToString());
            onValueChange?.Invoke(value * (maxValue - minValue) + minValue);
            onValueChange1?.Invoke((value * (maxValue - minValue) + minValue).ToString());
            this.value = minValue + roundSlider.value * (maxValue - minValue);
            current_value = this.value;

            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { current_value.ToString() }).MessageToJson());
        }
        public void SetContent(float progressValue)
        {
            MyInit();
            if (normalSlider.gameObject.activeInHierarchy)
            {
                normalSlider.value = progressValue;
            }
            if (roundSlider.gameObject.activeInHierarchy)
            {
                roundSlider.value = progressValue;
            }
            this.progressValue = progressValue;
        }
        public float GetContent()
        {
            return progressValue;
        }
        public override bool GetShowStyle()
        {
            return true; 
        }
        public override void Init()
        {
            defaultElement = "style1";
            transform.Find("BG/ProgressSlider").gameObject.hideFlags = HideFlags.None;
            transform.Find("BG/RoundProgressSlider").gameObject.hideFlags = HideFlags.HideInHierarchy;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public override string[] GetStyle()
        {
            int totalStyle = sliderStyle.SliderStyleList.Count;
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
            if (defaultElement.Equals(style))
            {
                return;
            }
            string[] styles = GetStyle();
            //切换样式
            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].Equals(style))
                {
                    SliderStyleItem item = sliderStyle.SliderStyleList[i];
                    if (item.sliderType == SliderType.normal)
                    {
                        normalSlider.gameObject.SetActive(true);
                        roundSlider.gameObject.SetActive(false);
                        normalSlider.gameObject.hideFlags = HideFlags.None;
                        roundSlider.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        BG.enabled = item.showBackground;
                        normalbg.sprite = item.sliderBackgroundImage;
                        normalFillImage.sprite = item.sliderForgroundImage;
                        normalHandle.sprite = item.sliderHandleImage;
                        if (item.showHintInfo)
                        {
                            normalProgressHintTransform.gameObject.SetActive(true);
                        }
                        else
                        {
                            normalProgressHintTransform.gameObject.SetActive(false);
                        }
                        // normalSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(item.sliderBackgroundImage.texture.width, item.sliderBackgroundImage.texture.height);
                    }
                    else if ((item.sliderType == SliderType.round))
                    {
                        normalSlider.gameObject.SetActive(false);
                        roundSlider.gameObject.SetActive(true);
                        normalSlider.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        roundSlider.gameObject.hideFlags = HideFlags.None;
                        BG.enabled = false;
                        roundbg.sprite = item.sliderBackgroundImage;
                        roundFillImage.sprite = item.sliderForgroundImage;
                        roundForeFillImage.sprite = item.sliderSmallForgroundImage;
                        if (roundForeFillImage.sprite == null)
                        {
                            roundForeFillImage.transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            roundForeFillImage.transform.gameObject.SetActive(true);
                            roundForeFillImage.GetComponent<RectTransform>().sizeDelta = new Vector2(item.sliderSmallForgroundImage.texture.width, item.sliderSmallForgroundImage.texture.height);
                        }
                    }
                }
            }
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            defaultElement = value.ToString();
        }
        /// <summary>
        /// 同步消息接收
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="methodName"></param>
        public override void ExternalCall(string[] parameter, string methodName)
        {
            base.ExternalCall(parameter);
            if (onValueChange != null)
            {
                onValueChange?.Invoke(float.Parse(parameter[0]));
            }

            roundSlider.onValueChanged.RemoveAllListeners();
            normalSlider.onValueChanged.RemoveAllListeners();

            normalSlider.value = float.Parse(parameter[0]);
            roundSlider.value = float.Parse(parameter[0]);

            roundSlider.onValueChanged.AddListener(SliderValueChangeEvent1);
            normalSlider.onValueChanged.AddListener(SliderValueChangeEvent);
        }
    }
}

