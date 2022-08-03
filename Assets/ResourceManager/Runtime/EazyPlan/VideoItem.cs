using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Alva.EazyPlan
{
    public class VideoItem : MonoBehaviour
    {
        Button videoButton;
        public VideoClip videoClip;
        VideoPlayer videoPlayer;
        public Texture2D videoFrameTexture;
        RenderTexture renderTexture;
        int framesValue = 0;//获得视频第几帧的图片
        Image videoImage;
        // Start is called before the first frame update
        void Awake()
        {
            videoButton = GetComponent<Button>();
            videoButton.onClick.AddListener(ClickVideoButton);
            videoPlayer = GetComponent<VideoPlayer>();
            videoImage = GetComponent<Image>();
        }

        private void ClickVideoButton()
        {
            VideoPlayPanel.Instance.PlayVideo(videoClip);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetVideoClip(VideoClip _videoClip)
        {
            videoClip = _videoClip;
            videoFrameTexture = new Texture2D(2, 2);
            videoPlayer = GetComponent<VideoPlayer>();
          //  VideoClip  Resources.Load<VideoClip>("PLMXML/" + _videoURL);
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.clip = videoClip;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += OnNewFrame;
            videoPlayer.Play();
        }

        private void OnNewFrame(VideoPlayer source, long frameIdx)
        {
            framesValue++;
            if (framesValue == 100)
            {
                renderTexture = source.texture as RenderTexture;
                if (videoFrameTexture.width != renderTexture.width || videoFrameTexture.height != renderTexture.height)
                {
                    videoFrameTexture.Resize(renderTexture.width, renderTexture.height);
                }
                RenderTexture.active = renderTexture;
                videoFrameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                videoFrameTexture.Apply();
                RenderTexture.active = null;
                videoPlayer.frameReady -= OnNewFrame;
                videoPlayer.sendFrameReadyEvents = false;
                if (videoImage)
                {
                    videoImage.sprite = Sprite.Create(videoFrameTexture, new Rect(0, 0, videoFrameTexture.width, videoFrameTexture.height), Vector2.zero);
                }
            }
        }
    }
}
