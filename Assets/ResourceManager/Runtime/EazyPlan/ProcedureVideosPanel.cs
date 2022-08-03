using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Alva.EazyPlan
{
    public class ProcedureVideosPanel : MonoBehaviour
    {
        public static ProcedureVideosPanel Instance;
        public Transform bg;
        public VideoItem videoItemModel;
        public Transform videoItemParent;
        WorkProceduresItem workProceduresItem;
        OperationStepModels operationStepModels;
        public string id;
        public List<string> videoURLList;
        public List<Texture2D> videoFirstFramePictureList;
        public List<VideoClip> videoClipList;
        public List<VideoItem> alreadyInstantiateVideoItems = new List<VideoItem>();
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }
        public void Show(WorkProceduresItem _workProceduresItem)
        {
            if (bg)
            {
                bg.gameObject.SetActive(true);
            }
            workProceduresItem = _workProceduresItem;
            if (workProceduresItem != null)
            {
                id = workProceduresItem.id;
                GetVideoURLs();
                GetVideos();
                HideAllAlreadyInstantiateVideoItems();
                InstantiateAllVideoItems();
            }
        }
        public void Show(OperationStepModels _operationStepModels)
        {
            if (bg)
            {
                bg.gameObject.SetActive(true);
            }
            operationStepModels = _operationStepModels;
            if (operationStepModels != null)
            {
                id = operationStepModels.id;
                //GetVideoURLs();
                //GetVideos();
                //  videoClipList = _operationStepModels.stepVideos;
                GetNotEmptyStepVideos(_operationStepModels.stepVideos);
                HideAllAlreadyInstantiateVideoItems();
                InstantiateAllVideoItems();
            }
        }
        public List<VideoClip> GetNotEmptyStepVideos(List<VideoClip> _stepVideos)
        {
            videoClipList = new List<VideoClip>();
            for (int i = 0; i < _stepVideos.Count; i++)
            {
                if (_stepVideos[i])
                {
                    videoClipList.Add(_stepVideos[i]);
                }
            }
            return videoClipList;
        }
        private void HideAllAlreadyInstantiateVideoItems()
        {
            for (int i = 0; i < alreadyInstantiateVideoItems.Count; i++)
            {
                if (alreadyInstantiateVideoItems[i])
                {
                    alreadyInstantiateVideoItems[i].gameObject.SetActive(false);
                }
            }
        }

        private void GetVideos()
        {
            videoClipList = new List<VideoClip>();
            for (int i = 0; i < videoURLList.Count; i++)
            {
                if (videoURLList[i] != null)
                {
                   // VideoClip videoClip = Resources.Load<VideoClip>("PLMXML/" + videoURLList[i]);
                    VideoClip videoClip = Resources.Load<VideoClip>(OperationProcedureConfigDecoder.Instance.resourceFolderName + "/" + videoURLList[i]);
                    if (videoClip)
                    {
                        videoClipList.Add(videoClip);
                    }
                }
            }
        }

        private void InstantiateAllVideoItems()
        {
            if (videoClipList != null && videoClipList.Count > 0)
            {
                for (int i = 0; i < videoClipList.Count; i++)
                {
                    if (alreadyInstantiateVideoItems.Count > i)
                    {
                        alreadyInstantiateVideoItems[i].gameObject.SetActive(true);
                        alreadyInstantiateVideoItems[i].SetVideoClip(videoClipList[i]);
                    }
                    else
                    {
                        VideoItem videoItem = Instantiate(videoItemModel, videoItemParent).GetComponent<VideoItem>();
                        videoItem.gameObject.SetActive(true);
                        videoItem.SetVideoClip(videoClipList[i]);
                        alreadyInstantiateVideoItems.Add(videoItem);
                    }
                }
            }
        }

        //private void GetPictures()
        //{
        //    videoFirstFramePictureList = new List<Texture2D>();
        //    for (int i = 0; i < videoURLList.Count; i++)
        //    {
        //        if (videoURLList[i] != null)
        //        {
        //            Texture2D texture = Resources.Load<Texture2D>("PLMXML/" + videoURLList[i]);
        //            if (texture)
        //            {
        //                videoFirstFramePictureList.Add(texture);
        //            }
        //        }
        //    }
        //}

        private void GetVideoURLs()
        {
            videoURLList = new List<string>();
            for (int i = 0; i < workProceduresItem.workResources.Count; i++)
            {
                if (workProceduresItem.workResources[i] != null && workProceduresItem.workResources[i].wrType == 2)
                {
                    string picturePath = workProceduresItem.workResources[i].localPath.Replace("\\", "/").Replace("%20", " ").Split('.')[0];
                    videoURLList.Add(picturePath);
                }
            }
        }

        public void Hide()
        {
            if (bg)
            {
                bg.gameObject.SetActive(false);
            }
        }
        public void ShowVideos(List<string> pictureURLList)
        {

        }
    }
}