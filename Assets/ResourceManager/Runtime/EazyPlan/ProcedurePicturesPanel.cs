using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alva.EazyPlan
{
    public class ProcedurePicturesPanel : MonoBehaviour
    {
        public static ProcedurePicturesPanel Instance;
        public Transform bg;
        public ImageItem imageModel;
        public Transform imageItemParent;
        WorkProceduresItem workProceduresItem;
        OperationStepModels operationStepModels;
        public string id;
        public List<string> pictureURLList;
        public List<Texture2D> pictureList;
        List<ImageItem> alreadyInstantiateImageItems = new List<ImageItem>();
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
                GetPictureURLs();
                GetPictures();
                HideAllAlreadyInstantiateImageItems();
                InstantiateAllPictures();
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
                //GetPictureURLs();
                //GetPictures();
                //  pictureList = operationStepModels.stepImages;
                GetNotEmptyStepImages(operationStepModels.stepImages);
                HideAllAlreadyInstantiateImageItems();
                InstantiateAllPictures();
            }
        }
        public List<Texture2D> GetNotEmptyStepImages(List<Texture2D> _stepImages)
        {
            pictureList = new List<Texture2D>();
            for (int i = 0; i < _stepImages.Count; i++)
            {
                if (_stepImages[i])
                {
                    pictureList.Add(_stepImages[i]);
                }
            }
            return pictureList;
        }
        private void GetPictures()
        {
            pictureList = new List<Texture2D>();
            for (int i = 0; i < pictureURLList.Count; i++)
            {
                if (pictureURLList[i] != null)
                {
                    //  Texture2D texture = Resources.Load<Texture2D>("PLMXML/" + pictureURLList[i]);
                    Texture2D texture = Resources.Load<Texture2D>(OperationProcedureConfigDecoder.Instance.resourceFolderName + "/" + pictureURLList[i]);
                    if (texture)
                    {
                        pictureList.Add(texture);
                    }
                }
            }
        }

        private void InstantiateAllPictures()
        {
            if (pictureList != null && pictureList.Count > 0)
            {
                for (int i = 0; i < pictureList.Count; i++)
                {
                    if (alreadyInstantiateImageItems.Count > i)
                    {
                        alreadyInstantiateImageItems[i].gameObject.SetActive(true);
                        alreadyInstantiateImageItems[i].ShowImage(pictureList[i]);
                    }
                    else
                    {
                        ImageItem imageItem = Instantiate(imageModel, imageItemParent).GetComponent<ImageItem>();
                        imageItem.gameObject.SetActive(true);
                        imageItem.ShowImage(pictureList[i]);
                        alreadyInstantiateImageItems.Add(imageItem);
                    }
                }
            }
        }
        void HideAllAlreadyInstantiateImageItems()
        {
            for (int i = 0; i < alreadyInstantiateImageItems.Count; i++)
            {
                if (alreadyInstantiateImageItems[i])
                {
                    alreadyInstantiateImageItems[i].gameObject.SetActive(false);
                }
            }
        }
        private void GetPictureURLs()
        {
            pictureURLList = new List<string>();
            for (int i = 0; i < workProceduresItem.workResources.Count; i++)
            {
                if (workProceduresItem.workResources[i] != null && workProceduresItem.workResources[i].wrType == 0)
                {
                    string picturePath = workProceduresItem.workResources[i].localPath.Replace("\\", "/").Replace("%20", " ").Split('.')[0];
                    pictureURLList.Add(picturePath);
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
        public void ShowPictures(List<string> pictureURLList)
        {

        }
    }
}
