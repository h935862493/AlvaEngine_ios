using Alva.Style;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class ProgressBarAgent : Agent
    {
        public ProgressBarStyle progressBarStyle;
        Image Background;
        Image Progress;
        TextMeshProUGUI progressText;
        [Range(0, 100),HideInInspector]
        public float value = 50;
        private float current_value;
        public UnityEngine.Events.UnityEvent<float> OnValueChanged;
        public UnityEngine.Events.UnityEvent<string> OnValueChanged1;
        public UnityEngine.Events.UnityEvent EndEvent;
        float progressValue = 0.5f;
        float timer = 0;
        float showTime = 10f;
        bool isProgressPlaying = false;
        bool isWaitForFinish = false;
        bool isInfomFinished = false;
        float finishProgressValePerFrame = 0.02f;
        bool isInit = false;
        private void MyInit()
        {
            if (isInit)
            {
                return;
            }
            Background = GetComponent<Image>();
            Progress = transform.Find("Progress").GetComponent<Image>();
            progressText = transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        }
        public override void Init()
        {
            defaultElement = "style1";
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.05F, ShrinkageAllExcludeAgent);
#endif
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            MyInit();
            if (current_value != value)
            {
                current_value = value;
                OnRefreshProgressValue();
            }
        }
#endif
        public override bool GetShowStyle()
        {
            return true;
        }
        public override string[] GetStyle()
        {
            int totalStyle = progressBarStyle.ProgressBarStyleList.Count;
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
            string style = value.ToString();
            if (defaultElement.Equals(style))
            {
                return;
            }
            string[] styles = GetStyle();
            Progress.type = Image.Type.Filled;
            //ÇÐ»»ÑùÊ½
            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].Equals(style))
                {
                    Progress.sprite = progressBarStyle.ProgressBarStyleList[i].progress;
                    Background.sprite = progressBarStyle.ProgressBarStyleList[i].background;
                    Progress.fillMethod = progressBarStyle.ProgressBarStyleList[i].fillType == 0 ? Image.FillMethod.Horizontal : Image.FillMethod.Radial360;
                    Progress.fillOrigin = progressBarStyle.ProgressBarStyleList[i].fillType == 0 ? (int)Image.OriginHorizontal.Left : (int)Image.Origin360.Top;
                    SetProgreeTextShow(progressBarStyle.ProgressBarStyleList[i]);
                }
            }
            GetComponent<RectTransform>().sizeDelta = new Vector2(Progress.sprite.texture.width, Progress.sprite.texture.height);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            defaultElement = value.ToString();
        }
        private void SetProgreeTextShow(ProgressBarStyleItem item)
        {
            switch (item.textPosition)
            {
                case TextPosition.NoShow:
                    progressText.gameObject.SetActive(false);
                    break;
                case TextPosition.Center:
                    progressText.gameObject.SetActive(true);
                    progressText.transform.localPosition = Vector3.zero;
                    break;
                case TextPosition.Upper:
                    progressText.gameObject.SetActive(true);
                    progressText.transform.localPosition = new Vector3(0, item.background.texture.height / 2 + 15, 0);
                    break;
                default:
                    progressText.gameObject.SetActive(false);
                    break;
            }
        }


        private void Awake()
        {
            MyInit();
            current_value = value;
            this.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (isProgressPlaying)
            {
                if (isWaitForFinish)
                {
                    if (isInfomFinished)
                    {
                        timer += finishProgressValePerFrame;
                    }
                    else if (timer / showTime < 0.9f)
                    {
                        timer += Time.deltaTime;
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                }
                float progressValue = timer / showTime > 1 ? 1f : timer / showTime;
                SetContent(progressValue);
                if (timer >= showTime)
                {
                    isProgressPlaying = false;
                    isWaitForFinish = false;
                    isInfomFinished = false;
                    this.gameObject.SetActive(false);
                    timer = 0;
                    EndEvent?.Invoke();
                }
            }
        }
        public void ShowProgressBar()
        {
            ShowProgressBar(10);
        }
        public void ShowProgressBar(float showTime = 10f)
        {
            this.gameObject.SetActive(true);
            SetContent(0);
            isProgressPlaying = true;
            isWaitForFinish = false;
            this.showTime = showTime;
        }
        public void ShowProgressBarWaitForFinish()
        {
            ShowProgressBarWaitForFinish(10);
        }
        public void ShowProgressBarWaitForFinish(float showTime = 10f)
        {
            this.gameObject.SetActive(true);
            SetContent(0);
            isProgressPlaying = true;
            this.showTime = showTime;
            isWaitForFinish = true;
        }
        public void InformFinish()
        {
            this.gameObject.SetActive(true);
            isWaitForFinish = true;
            isProgressPlaying = true;
            isInfomFinished = true;
            finishProgressValePerFrame = (showTime - timer) / 60;
        }
        public void OnRefreshProgressValue()
        {
            if (Progress != null)
            {
                Progress.fillAmount = value / 100.0f;
                OnValueChanged.Invoke(value / 100.0f);
                OnValueChanged1.Invoke((value / 100.0f).ToString());
                progressText.text = Progress.fillAmount.ToString("P0");
            }
            this.progressValue = value / 100.0f;
        }
        public void SetContent(float progressValue)
        {
            if (Progress != null)
            {
                value = progressValue * 100;
                Progress.fillAmount = value / 100.0f;
                OnValueChanged.Invoke(progressValue);
                OnValueChanged1.Invoke(progressValue.ToString());
                progressText.text = Progress.fillAmount.ToString("P0");
            }
            this.progressValue = progressValue;
        }
        public float GetContent()
        {
            return progressValue;
        }
    }
}