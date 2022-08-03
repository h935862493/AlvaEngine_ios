using Alva.Runtime.Toolbox;
using Alva.Style;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class VideoAgent : Agent
    {
        public VideoStyle videoStyle;
        RectTransform rectTransform;
        Button playButton;
        Button soundButton;
        Button fullScreenButton;
        Image soundButtonImage;
        Image playButtonImage;
        Image fullScreenButtonImage;
        TextMeshProUGUI duration;
        bool isInit = false;
        Transform noPlayState;
        Transform playState;
        Transform movieImageTran;
        Transform soundVolumePanel;
        Slider progressSlider;
        Slider volumeSlider;
        VideoPlayer videoPlayer;
        AudioSource audioSource;
        Sprite soundOpen;
        Sprite soundShut;
        Sprite fullScreen;
        Sprite noFullScreen;
        Sprite play;
        Sprite stop;
        bool soundIsOpen = true;
        public bool isFullScreen = false;
        bool isPlay = false;
        float totalDuration = 0f;
        float progressTime = 0;
        EventTrigger pointDownEventTrigger;
        EventTrigger sliderEventTrigger;
        EventTrigger soundButtonEventTrigger;
        EventTrigger soundVolumePanelEventTrigger;
        bool recordPlayStatePlaying = false; //记录拖拽前是播放还算非播放状态
        Vector3 videoTransformPos;
        Vector2 videoSize;
        float defaultShowVideoUITime = 5f;
        [HideInInspector]
        public VideoClip videoClip;
        float currentProcessValue;
        [SerializeField, HideInInspector]
        public bool isNewInstantiate = false;
        [SerializeField, HideInInspector]
        public RenderTexture renderTexture;
        public bool playOnAwake = true;
        bool recordPlayOnAwake = true;
        public bool loop = false;
        bool recordLoop = false;
        bool isFirstPlay = true;
        [SerializeField, HideInInspector]
        int recordSiblingIndex = 0;
        Canvas canvas;
        RectTransform canvasRectTransform;
        RectTransform BottomUIRectTransform;
        float canvasRatio = 1;
        bool isPortrait = true;
        bool recordIsPortrait = false;
        Transform videoParent;
        Vector3 initScale;
        GameObject canvasGO;
        RectTransform canvasGORT;
        int systemHeight=1080;
        int systemWidth = 1920;
        int SelecterSizeIndex = -1;
        private void Awake()
        {
            MyInit();
            canvas = transform.root.GetComponentInChildren<Canvas>();       
            BottomUIRectTransform = playState.transform.GetComponent<RectTransform>();
            videoParent = transform.parent;
            initScale = transform.localScale;
            if (canvas)
            {    
                canvasRatio = canvas.transform.localScale.x;
                canvasRectTransform= canvas.GetComponent<RectTransform>();         
                isPortrait = canvasRectTransform.sizeDelta.x < canvasRectTransform.sizeDelta.y ? true : false;
            }      
            videoPlayer.Stop();
            if (videoPlayer.targetTexture != null)
            {
                renderTexture = new RenderTexture(videoPlayer.targetTexture);
                videoPlayer.targetTexture = renderTexture;
                movieImageTran.GetComponent<RawImage>().texture = renderTexture;
            }

        }

        private void MyInit()
        {
            if (isInit)
            {
                return;
            }
            soundOpen = videoStyle.VideoStyleList[0].soundOpen;
            soundShut = videoStyle.VideoStyleList[0].soundShut;
            play = videoStyle.VideoStyleList[0].play;
            stop = videoStyle.VideoStyleList[0].stop;
            fullScreen = videoStyle.VideoStyleList[0].fullScreen;
            noFullScreen = videoStyle.VideoStyleList[0].noFullScreen;
            GetComponent<Image>().sprite = videoStyle.VideoStyleList[0].background;
            rectTransform = GetComponent<RectTransform>();
            noPlayState = transform.Find("NoPlayState");
            playState = transform.Find("PlayState");
            movieImageTran = transform.Find("MovieImage");
            soundVolumePanel = playState.Find("SoundVolumePanel");
            volumeSlider = soundVolumePanel.Find("SoundSlider").GetComponent<Slider>();
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChangeEvent);
            progressSlider = playState.Find("ProgressSlider").GetComponent<Slider>();
            progressSlider.onValueChanged.AddListener(OnSliderValueChangeEvent);
            videoPlayer = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();
            playButton = playState.Find("PlayButton").GetComponent<Button>();
            playButtonImage = playButton.GetComponent<Image>();
            soundButton = playState.Find("SoundButton").GetComponent<Button>();
            soundButtonImage = soundButton.GetComponent<Image>();
            fullScreenButton = playState.Find("FullScreenButton").GetComponent<Button>();
            fullScreenButton.onClick.AddListener(ClickFullScreenButton);
            fullScreenButtonImage = fullScreenButton.GetComponent<Image>();
            duration = playState.Find("Duration").GetComponent<TextMeshProUGUI>();
            playButton.onClick.AddListener(ClickPlayButton);
            soundButton.onClick.AddListener(ClickSoundButton);
            soundIsOpen = true;
            soundButtonImage.sprite = soundOpen;
            isPlay = false;
            playButtonImage.sprite = play;
            isInit = true;
           
        }
        
        void Start()
        {
            if (!videoPlayer)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }
            #region 各种EventTrigger事件
            if (movieImageTran)
            {
                pointDownEventTrigger = movieImageTran.GetComponent<EventTrigger>();
                EventTrigger.Entry myPointerDown = new EventTrigger.Entry();
                myPointerDown.eventID = EventTriggerType.PointerDown;
                myPointerDown.callback.AddListener(OnPointerDownEvent);
                pointDownEventTrigger.triggers.Add(myPointerDown);
            }

            if (progressSlider)
            {
                sliderEventTrigger = progressSlider.GetComponent<EventTrigger>();

                EventTrigger.Entry myDrageBegin = new EventTrigger.Entry();
                myDrageBegin.eventID = EventTriggerType.BeginDrag;
                myDrageBegin.callback.AddListener(OnDrageSliderBegin);

                EventTrigger.Entry myDrageEnd = new EventTrigger.Entry();
                myDrageEnd.eventID = EventTriggerType.EndDrag;
                myDrageEnd.callback.AddListener(OnDrageSliderEnd);

                sliderEventTrigger.triggers.Add(myDrageBegin);
                sliderEventTrigger.triggers.Add(myDrageEnd);
            }
            if (soundButton)
            {
                soundButtonEventTrigger = soundButton.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener(OnSoundButtonEnterEvent);

                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener(OnSoundButtonExitEvent);

                soundButtonEventTrigger.triggers.Add(pointerEnter);
                soundButtonEventTrigger.triggers.Add(pointerExit);
            }
            if (soundVolumePanel)
            {
                soundVolumePanelEventTrigger = soundVolumePanel.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener(OnSoundButtonEnterEvent);

                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener(OnSoundButtonExitEvent);

                soundVolumePanelEventTrigger.triggers.Add(pointerEnter);
                soundVolumePanelEventTrigger.triggers.Add(pointerExit);
            }
            #endregion 
            videoTransformPos = rectTransform.localPosition;
            videoSize = rectTransform.sizeDelta;
            if (playOnAwake)
            {
                PlayVideo();
                isFirstPlay = false;
            }                     
        }

        private void OnPointerDownEvent(BaseEventData arg0)
        {
            CancelInvoke("HideVideoControlUI");
            ShowVideoControlUI();
        }

        private void OnSoundButtonExitEvent(BaseEventData arg0)
        {
            Invoke("HideSoundVolumePanel", 0.3f);
        }
       
        private void HideSoundVolumePanel()
        {
            soundVolumePanel.gameObject.SetActive(false);
        }

        private void OnSoundButtonEnterEvent(BaseEventData arg0)
        {
            CancelInvoke("HideSoundVolumePanel");
            soundVolumePanel.gameObject.SetActive(true);
            volumeSlider.value = audioSource.volume;
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
            videoPlayer.Pause();
        }
        void OnDrageSliderEnd(BaseEventData data)
        {
            if (recordPlayStatePlaying)
            {
                videoPlayer.Play();
            }
        }
        void Update()
        {
            if (isPlay)
            {
                progressTime = (float)videoPlayer.time > totalDuration ? totalDuration : (float)videoPlayer.time;
                currentProcessValue = progressTime / totalDuration;
                progressSlider.value = currentProcessValue;
               // duration.text = ToolsHelper.FormatTime(progressTime) + "/" + ToolsHelper.FormatTime(totalDuration);
                duration.text = ToolsHelper.FormatTime(progressTime) ;
                if (ToolsHelper.FormatTime(progressTime) == ToolsHelper.FormatTime(totalDuration) && videoPlayer.isPaused)
                {
                    isPlay = false;
                    playButtonImage.sprite = play;
                    if (loop)
                    {
                        PlayVideo();
                    }
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
                videoPlayer.playOnAwake = playOnAwake;
            }
            else
            {
                if (videoPlayer.playOnAwake != playOnAwake)
                {
                    playOnAwake = videoPlayer.playOnAwake;
                    recordPlayOnAwake = playOnAwake;
                }
            }
            if (recordLoop != loop)
            {
                recordLoop = loop;
                videoPlayer.isLooping = loop;
            }
            else
            {
                if (videoPlayer.isLooping != loop)
                {
                    loop = videoPlayer.isLooping;
                    recordLoop = loop;
                }
            }
        }
#endif

        public void OnSliderValueChangeEvent(float value)
        {
            if (currentProcessValue.Equals(value))
            {
                return;
            }
            videoPlayer.time = totalDuration * value;

            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { value.ToString() }).MessageToJson());
        }
        public void OnVolumeSliderValueChangeEvent(float value)
        {
            audioSource.volume = value;
        }
        #region 播放重载
        public void PlayVideoLoop()
        {
            loop = true;
            PlayVideo();
        }
        public void PlayVideoLoop(string videoURL)
        {
            loop = true;
            PlayVideo(videoURL);
        }
        public void PlayVideoLoop(VideoClip videoClip)
        {
            loop = true;
            PlayVideo(videoClip);
        }
        public void PlayVideo()
        {
            if (videoClip)
            {
                PlayVideo(videoClip);
            }
        }
        public void PlayVideo(string videoURL)
        {
            PlayVideo(videoURL);
        }
        public void PlayVideo(VideoClip videoClip)
        {
            if (videoClip)
            {
                PlayVideo("", videoClip, VideoSource.VideoClip);
            }
        }
        public void PlayVideo(string videoURL, VideoClip videoClip = null, VideoSource videoSource = VideoSource.Url)
        {
            if (string.IsNullOrEmpty(videoURL) && !videoClip)
            {
                return;
            }
            videoPlayer.targetTexture.Release();
            videoPlayer.targetTexture.MarkRestoreExpected();
            videoPlayer.source = videoSource;
            if (videoPlayer.source == VideoSource.Url)
            {
                videoPlayer.url = videoURL;
            }
            else
            {
                videoPlayer.clip = videoClip;
            }
            progressTime = 0f;
            noPlayState.gameObject.SetActive(false);
            ShowVideoControlUI();
            movieImageTran.gameObject.SetActive(true);
            isPlay = false;
            ClickPlayButton();
            totalDuration = (int)videoPlayer.length;
            audioSource.volume = 0.5f;
            audioSource.mute = false;

        }
        #endregion
        public void ShowVideoControlUI()
        {
            playState.gameObject.SetActive(true);
            Invoke("HideVideoControlUI", defaultShowVideoUITime);
        }
        public void HideVideoControlUI()
        {
             playState.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            RestoreNormalProtraitOrHorizontalState();
#endif
        }
#if UNITY_EDITOR
        public void RestoreNormalProtraitOrHorizontalState()
        {
            if (SelecterSizeIndex != -1 && recordIsPortrait)
            {
                recordIsPortrait = false;
                SetSize(SelecterSizeIndex);
                SelecterSizeIndex = -1;
            }
        }
#endif
        public void SetVideoProtraitState(bool _isPortrait)
        {
            isPortrait = _isPortrait;
            recordIsPortrait = _isPortrait;
        }
        private void ClickFullScreenButton()
        {
            if (isFullScreen)
            {
                isFullScreen = false;
#if UNITY_EDITOR
                if (recordIsPortrait)
                {
                    recordIsPortrait = false;
                    SetSize(SelecterSizeIndex);
                    SelecterSizeIndex = -1;
                }
#endif
                BottomUIRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
                RestoreNoFullScreen();
            }
            else
            {
                isFullScreen = true;
#if UNITY_EDITOR
                isPortrait = Screen.height > Screen.width;
                recordIsPortrait = isPortrait;
                if (isPortrait)
                {
                    ChangeDisplayOriention();
                }
                isPortrait = false;
#endif
                recordSiblingIndex = transform.GetSiblingIndex();
                transform.SetAsLastSibling();                      
                FullScreen();
            }
        }

        public void RestoreNoFullScreen()
        {
            rectTransform.parent = videoParent;
            transform.SetSiblingIndex(recordSiblingIndex);
            fullScreenButtonImage.sprite = fullScreen;
            rectTransform.localPosition = videoTransformPos;
            rectTransform.sizeDelta = videoSize;
            rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            rectTransform.localScale = initScale;
            if (canvasGO )
            {
                Destroy(canvasGO);
            }
        }

        private void FullScreen()
        {
            canvasGO = GetUIRoot();
           canvasGORT = canvasGO.GetComponent<RectTransform>();
            isPortrait = canvasGORT.sizeDelta.x < canvasGORT.sizeDelta.y ? true : false;
            rectTransform.transform.parent = canvasGO.transform;
            fullScreenButtonImage.sprite = noFullScreen;
            rectTransform.localPosition = Vector3.zero;    
            rectTransform.sizeDelta = canvasGORT.sizeDelta;
            rectTransform.localScale = Vector3.one;
            canvasRatio = canvasGO.transform.localScale.x;           
            if (isPortrait)
            {
#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
                rectTransform.localEulerAngles = new Vector3(0, 0, -90);
                rectTransform.sizeDelta = new Vector2(canvasGORT.sizeDelta.y, canvasGORT.sizeDelta.x);
#endif

            }
#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
            BottomUIRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x - 120 / canvasRatio);
#endif
        }
        /// <summary>
        /// 获取UI根节点对象
        /// </summary>
        /// <returns> UI根节点 </returns>
        public UnityEngine.GameObject GetUIRoot()
        {        
            GameObject cavasGO = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load("AugmentationObject/UI")) as UnityEngine.GameObject;
            Canvas canvas = cavasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            return cavasGO;
        }
#if UNITY_EDITOR
#region GameViewDisplay
        void ChangeDisplayOriention()
        {
            systemWidth = Display.main.systemWidth;
            systemHeight = Display.main.systemHeight;
          // Debug.LogError("Display.main .systemWidth--" + Display.main.systemWidth + "Display.main .systemHeight--" + Display.main.systemHeight);        
           SelecterSizeIndex = GetSelecterSizeIndex();
            int oldSizeIndex = FindSize(GetGameViewSizeGroupType(), "videoFullScreen");
            if (oldSizeIndex != -1)
            {
                RemoveCustomSize(oldSizeIndex);
            }
            AddCustomSize(systemHeight, systemWidth, "videoFullScreen");  
            int sizeIndex = FindSize(GetGameViewSizeGroupType(), "videoFullScreen");
            if (sizeIndex!=-1)
            {
                SetSize(sizeIndex);
            }
            //Debug.LogError("sizeIndex--" + sizeIndex);
            //Debug.LogError("Display.main .systemWidth1--" + Display.main.systemWidth + "Display.main .systemHeight1--" + Display.main.systemHeight);
        }
         void AddCustomSize(int width, int height, string name)
        {
            System.Type type = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            object gameViewSizesInstance = instanceProp.GetValue(null, null);
            System.Reflection.MethodInfo getGroupMethod = sizesType.GetMethod("GetGroup");
            var group = getGroupMethod.Invoke(gameViewSizesInstance, new object[] { (int)GetGameViewSizeGroupType() });
            var addCustomSize = getGroupMethod.ReturnType.GetMethod("AddCustomSize");
            var gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var constructor = gameViewSizeType.GetConstructor(new System.Type[] { type, typeof(int), typeof(int), typeof(string) });
            var newSize = constructor.Invoke(new object[] { (int)GameViewSizeType.AspectRatio, width, height, name });
            addCustomSize.Invoke(group, new object[] { newSize });
        }
        void RemoveCustomSize(int sizeIndex)
        {
            System.Type type = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            object gameViewSizesInstance = instanceProp.GetValue(null, null);
            System.Reflection.MethodInfo getGroupMethod = sizesType.GetMethod("GetGroup");
            var group = getGroupMethod.Invoke(gameViewSizesInstance, new object[] { (int)GetGameViewSizeGroupType() });
            var RemoveCustomSize = getGroupMethod.ReturnType.GetMethod("RemoveCustomSize");            
            RemoveCustomSize.Invoke(group, new object[] { sizeIndex });
        }
        void SetSize(int index)
        {
            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var gameViewWindow = EditorWindow.GetWindow(gameViewType);
            var sizeSelectionCallback = gameViewType.GetMethod("SizeSelectionCallback");
            try
            {
                sizeSelectionCallback.Invoke(gameViewWindow, new object[] { index, null });
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }
        public int GetSelecterSizeIndex()
        {
            int selectedSize= 0;
            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var gameViewWindow = EditorWindow.GetWindow(gameViewType);
            System.Reflection.PropertyInfo selectedSizeIndex = gameViewType.GetProperty("selectedSizeIndex", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            selectedSize = (int)(selectedSizeIndex.GetValue(gameViewWindow));
           // UnityEngine.Debug.LogError("selectedSize--"+ selectedSize);
            return selectedSize;
        }
        int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            System.Type type = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            object gameViewSizesInstance = instanceProp.GetValue(null, null);
            System.Reflection.MethodInfo getGroupMethod = sizesType.GetMethod("GetGroup");
         //   UnityEngine.Debug.Log((int)GetGameViewSizeGroupType());
            var group = getGroupMethod.Invoke(gameViewSizesInstance, new object[] { sizeGroupType });
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];
                int pren = display.IndexOf('(');
                if (pren != -1)
                    display = display.Substring(0, pren - 1);
                if (display == text)
                { 
                  //  Debug.LogError("Display--" + display);
                    return i;
                }
            }

            return -1;
        }
       GameViewSizeGroupType GetGameViewSizeGroupType()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                  //  UnityEngine.Debug.Log(BuildTarget.StandaloneWindows64);
                    return GameViewSizeGroupType.Standalone;
                case BuildTarget.StandaloneWindows:
                  //  UnityEngine.Debug.Log(BuildTarget.StandaloneWindows);
                    return GameViewSizeGroupType.Standalone;
                case BuildTarget.Android:
                 //   UnityEngine.Debug.Log(BuildTarget.Android);
                    return GameViewSizeGroupType.Android;
                case BuildTarget.iOS:
                 //   UnityEngine.Debug.Log(BuildTarget.iOS);
                    return GameViewSizeGroupType.iOS;
                default:
                 //   UnityEngine.Debug.Log(BuildTarget.Android);
                    return GameViewSizeGroupType.Android;
            }
        }
        enum GameViewSizeType
        {
            AspectRatio,
            FixedResolution
        }
#endregion
#endif
        private void ClickSoundButton()
        {
            if (soundIsOpen)
            {
                soundIsOpen = false;
                soundButtonImage.sprite = soundShut;
                if (videoPlayer)
                {
                    audioSource.mute = true;
                };
            }
            else
            {
                soundIsOpen = true;
                soundButtonImage.sprite = soundOpen;
                if (videoPlayer)
                {
                    audioSource.mute = false;
                };
            }
        }
        private void ClickPlayButton()
        {
            if (isPlay)
            {
                isFirstPlay = false;
                PauseVideo();
            }
            else
            {
                if (isFirstPlay)
                {
                    isFirstPlay = false;
                    PlayVideo();
                    isPlay = true;
                    playButtonImage.sprite = stop;
                }
                else
                {
                    isPlay = true;
                    playButtonImage.sprite = stop;
                    if (videoPlayer)
                    {
                        videoPlayer.Play();
                    };
                }

            }
            //平台交互信息同步---发送
            Common.PlayerData.Instance().SendSyncInfo(new Common.UnityMessageInfo(id, "ExternalCall", new string[1] { isPlay.ToString() }).MessageToJson());
        }

        public void PauseVideo()
        {
            isPlay = false;
            playButtonImage.sprite = play;
            if (videoPlayer)
            {
                videoPlayer.Pause();
            };
        }
        public void StopVideo()
        {
            isPlay = false;
            playButtonImage.sprite = play;
            if (videoPlayer)
            {
                videoPlayer.Stop();
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
            int totalStyle = videoStyle.VideoStyleList.Count;
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
            for (int i = 0; i < videoStyle.VideoStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    soundOpen = videoStyle.VideoStyleList[i].soundOpen;
                    soundShut = videoStyle.VideoStyleList[i].soundShut;
                    play = videoStyle.VideoStyleList[i].play;
                    stop = videoStyle.VideoStyleList[i].stop;
                    fullScreen = videoStyle.VideoStyleList[i].fullScreen;
                    noFullScreen = videoStyle.VideoStyleList[i].noFullScreen;
                    GetComponent<Image>().sprite = videoStyle.VideoStyleList[i].background;
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
            float progress;
            if (float.TryParse(parameter[0], out progress))
            {
                progressSlider.onValueChanged.RemoveAllListeners();
                videoPlayer.time = totalDuration * progress;
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
                    if (videoPlayer)
                    {
                        videoPlayer.Pause();
                    };
                }
                else
                {
                    if (isPlay)
                        return;
                    if (isFirstPlay)
                    {
                        isFirstPlay = false;
                        PlayVideo();
                    }
                    else
                    {
                        if (videoPlayer)
                        {
                            videoPlayer.Play();
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

