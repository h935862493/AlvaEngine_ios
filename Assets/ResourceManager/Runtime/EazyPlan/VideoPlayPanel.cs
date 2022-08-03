using Alva.Runtime.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
namespace Alva.EazyPlan
{
    public class VideoPlayPanel : MonoBehaviour
    {
        public static VideoPlayPanel Instance;
        public Button closeButton;
        public VideoAgent videoAgent;
        VideoClip videoClip;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            closeButton.onClick.AddListener(OnCloseButton);
            Hide();
        }

        private void OnCloseButton()
        {
            if (videoAgent)
            {
                videoAgent.StopVideo();
                videoAgent.RestoreNoFullScreen();
#if UNITY_EDITOR
                videoAgent.RestoreNormalProtraitOrHorizontalState();
#endif
            }
            Hide();
        }
        public void PlayVideo(VideoClip _videoClip)
        {
            Show();
            videoClip = _videoClip;
            Invoke("PlayVideo", 0.1f);
            PlayVideo();
        }

        private void PlayVideo()
        {
            if (videoClip)
            {
                videoAgent.SetVideoProtraitState(true);
                videoAgent.PlayVideo(videoClip);
                videoAgent.isFullScreen = false;
            }
        }

        public void Show()
        {
            if (videoAgent)
            {
                videoAgent.gameObject.SetActive(true);
            }
        }
        public void Hide()
        {
            if (videoAgent)
            {
                videoAgent.gameObject.SetActive(false);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
