using Alva.Common;
using Alva.Runtime.Toolbox;
using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class AudioAgent : Agent
    {
        public AudioStyle audioStyle;
        RectTransform rectTransform;
        Button playButton;
        Button soundButton;
        Image soundButtonImage;
        Image playButtonImage;
        TextMeshProUGUI duration;
        bool isInit = false;
        Transform noPlayState;
        Transform playState;
        Slider progressSlider;
        AudioSource audioSource;
        Sprite soundOpen;
        Sprite soundShut;
        Sprite play;
        Sprite stop;
        bool soundIsOpen = true;
        bool isPlay = false;
        float totalDuration = 0f;
        float progressTime = 0;
        [HideInInspector]
        public AudioClip audioClip;
        float currentProcessValue;
        EventTrigger sliderEventTrigger;
        bool recordPlayStatePlaying = false; //记录拖拽前是播放还算非播放状态  
        [SerializeField, HideInInspector]
        public bool isNewInstantiate = false;
        public bool playOnAwake = true;
        bool recordPlayOnAwake = true;
        public bool loop = false;
        bool recordLoop = false;
        bool isFirstPlay = true;
        private void Awake()
        {
            MyInit();
            audioSource.Stop();
        }
        void Start()
        {
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
            }
            if (progressSlider)
            {
                sliderEventTrigger = progressSlider.GetComponent<EventTrigger>();

                EventTrigger.Entry myDrageBegin = new EventTrigger.Entry();
                myDrageBegin.eventID = EventTriggerType.BeginDrag;
                myDrageBegin.callback.AddListener(OnDrageSliderBegin);

                EventTrigger.Entry myDrageEnd = new EventTrigger.Entry();
                myDrageBegin.eventID = EventTriggerType.EndDrag;
                myDrageBegin.callback.AddListener(OnDrageSliderEnd);

                sliderEventTrigger.triggers.Add(myDrageBegin);
                sliderEventTrigger.triggers.Add(myDrageEnd);
            }
            if (playOnAwake)
            {
                PlayAudioClip();
                isFirstPlay = false;
            }

        }
        void OnDrageSliderBegin(BaseEventData data)
        {
            if (isPlay)
            {
                recordPlayStatePlaying = true;
            }
            else
            {
                recordPlayStatePlaying = false;
            }
            audioSource.Pause();
        }
        void OnDrageSliderEnd(BaseEventData data)
        {
            if (recordPlayStatePlaying)
            {
                audioSource.Play();
            }
        }

        void Update()
        {
            if (isPlay)
            {
                progressTime = audioSource.time > totalDuration ? totalDuration : audioSource.time;
                currentProcessValue = progressTime / totalDuration;
                progressSlider.value = currentProcessValue;
                duration.text = Toolbox.ToolsHelper.FormatTime(progressTime) + "/" + ToolsHelper.FormatTime(totalDuration);
            }
            if (ToolsHelper.FormatTime(progressTime) == ToolsHelper.FormatTime(totalDuration) && !audioSource.isPlaying)
            {
                isPlay = false;
                playButtonImage.sprite = play;
                audioSource.time = 0;
                if (loop&& !isFirstPlay)
                {
                    PlayAudioClip();
                }
            }
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            MyInit();
            if (recordPlayOnAwake != playOnAwake)
            {
                recordPlayOnAwake = playOnAwake;
                audioSource.playOnAwake = playOnAwake;
            }
            else
            {
                if (audioSource.playOnAwake != playOnAwake)
                {
                    playOnAwake = audioSource.playOnAwake;
                    recordPlayOnAwake = playOnAwake;
                }
            }
            if (recordLoop != loop)
            {
                recordLoop = loop;
                audioSource.loop = loop;
            }
            else
            {
                if (audioSource.loop != loop)
                {
                    loop = audioSource.loop;
                    recordLoop = loop;
                }
            }
        }
#endif

        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            soundOpen = audioStyle.AudioStyleList[0].soundOpen;
            soundShut = audioStyle.AudioStyleList[0].soundShut;
            play = audioStyle.AudioStyleList[0].play;
            stop = audioStyle.AudioStyleList[0].stop;
            GetComponent<Image>().sprite = audioStyle.AudioStyleList[0].background;
            rectTransform = GetComponent<RectTransform>();
            noPlayState = transform.Find("NoPlayState");
            playState = transform.Find("PlayState");
            progressSlider = playState.Find("ProgressSlider").GetComponent<Slider>();
            progressSlider.onValueChanged.AddListener(OnSliderValueChangeEvent);
            audioSource = GetComponent<AudioSource>();
            playButton = playState.Find("PlayButton").GetComponent<Button>();
            playButtonImage = playButton.GetComponent<Image>();
            soundButton = playState.Find("SoundButton").GetComponent<Button>();
            soundButtonImage = soundButton.GetComponent<Image>();
            duration = playState.Find("Duration").GetComponent<TextMeshProUGUI>();
            playButton.onClick.AddListener(ClickPlayButton);
            soundButton.onClick.AddListener(ClickSoundButton);
            soundIsOpen = true;
            soundButtonImage.sprite = soundOpen;
            isPlay = false;
            playButtonImage.sprite = play;
            isInit = true;
        }
        public void OnSliderValueChangeEvent(float value)
        {
            if (currentProcessValue.Equals(value))
            {
                return;
            }
            audioSource.time = totalDuration * value;
            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { value.ToString() }).MessageToJson());
        }
        public void PlayAudioClipLoop()
        {
            loop = true;
            PlayAudioClip();
        }
        public void PlayAudioClipLoop(AudioClip clip)
        {
            loop = true;
            PlayAudioClip(clip);
        }
        public void PlayAudioClip()
        {
            PlayAudioClip(audioClip);
        }
        public void PlayAudioClip(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }
            totalDuration = (int)clip.length;
            progressTime = 0f;
            audioSource.clip = clip;
            noPlayState.gameObject.SetActive(false);
            playState.gameObject.SetActive(true);
            isPlay = false;
            ClickPlayButton();
            soundIsOpen = false;
            ClickSoundButton();
        }
        private void ClickSoundButton()
        {
            if (soundIsOpen)
            {
                soundIsOpen = false;
                soundButtonImage.sprite = soundShut;
                if (audioSource)
                {
                    audioSource.volume = 0;
                };
            }
            else
            {
                soundIsOpen = true;
                soundButtonImage.sprite = soundOpen;
                if (audioSource)
                {
                    audioSource.volume = 1;
                };
            }
        }
        private void ClickPlayButton()
        {
            if (isPlay)
            {
                isFirstPlay = false;
                PauseAudioClip();
            }
            else
            {
                if (isFirstPlay)
                {
                    isFirstPlay = false;
                    PlayAudioClip();
                    isPlay = true;
                    playButtonImage.sprite = stop;
                }
                else
                {
                    isPlay = true;
                    playButtonImage.sprite = stop;
                    if (audioSource)
                    {
                        audioSource.Play();
                    };
                }
            }

            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { isPlay.ToString() }).MessageToJson());
        }

        public void PauseAudioClip()
        {
            isPlay = false;
            playButtonImage.sprite = play;
            if (audioSource)
            {
                audioSource.Pause();
            };
        }
        public void StopAudioClip()
        {
            isPlay = false;
            playButtonImage.sprite = play;
            if (audioSource)
            {
                audioSource.Stop();
            };
        }

        public override bool GetShowStyle()
        {
            return true;
        }
        public override void Init()
        {
            defaultElement = "style1";
            isNewInstantiate = true;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public override string[] GetStyle()
        {
            int totalStyle = audioStyle.AudioStyleList.Count;
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
            for (int i = 0; i < audioStyle.AudioStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    soundOpen = audioStyle.AudioStyleList[i].soundOpen;
                    soundShut = audioStyle.AudioStyleList[i].soundShut;
                    play = audioStyle.AudioStyleList[i].play;
                    stop = audioStyle.AudioStyleList[i].stop;
                    GetComponent<Image>().sprite = audioStyle.AudioStyleList[i].background;
                    break;
                }
            }
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }


        public override void ExternalCall(string[] parameter, string methodName)
        {
            base.ExternalCall(parameter);
            //bool isPlay;
            float progress;
            if (float.TryParse(parameter[0], out progress))
            {
                progressSlider.onValueChanged.RemoveAllListeners();
                audioSource.time = totalDuration * progress;
                progressSlider.value = progress;
                progressSlider.onValueChanged.AddListener(OnSliderValueChangeEvent);
            }
            else
            {
              
                bool m_isPlay;
                bool.TryParse(parameter[0], out m_isPlay);
                if (!m_isPlay)
                {
                    isPlay = false;
                    playButtonImage.sprite = play;
                    if (audioSource)
                    {
                        audioSource.Pause();
                    };
                }
                else
                {
                    if (isPlay)
                        return;
                    if (isFirstPlay)
                    {
                        isFirstPlay = false;
                        PlayAudioClip();
                    }
                    else
                    {
                        if (audioSource)
                        {
                            audioSource.Play();
                        };
                    }
                    isPlay = true;
                    playButtonImage.sprite = stop;
                    
                }
            }
        }

        public void OnA()
        {
            ExternalCall(new string[1] { "true" }, string.Empty);
        }
        public void OnB()
        {
            ExternalCall(new string[1] { "false" }, string.Empty);
        }
    }
}

