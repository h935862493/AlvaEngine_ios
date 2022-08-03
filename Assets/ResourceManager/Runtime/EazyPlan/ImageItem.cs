using Alva.Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Alva.EazyPlan
{
    public class ImageItem : MonoBehaviour, IPointerClickHandler
    {
        Image currentImage;
        Texture2D texture;
        Texture2D newCropTexture;
        RectTransform rectTransform;
        public GameObject previewImageGO;
        // Start is called before the first frame update
        void Awake()
        {
            currentImage = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ShowImage(Texture2D _texture)
        {
            texture = _texture;
            newCropTexture = InstantiateCropImage( _texture);        
             Sprite sprite = Sprite.Create(newCropTexture, new Rect(0, 0, newCropTexture.width, newCropTexture.height), Vector2.zero);                  
            currentImage.sprite = sprite;
        }
        public void OnPointerClick(PointerEventData eventData)
        {           
        
                GameObject canvasGO = GetUIRoot();
              // GameObject previewImageGO = Resources.Load<GameObject>("Effect/PreviewImage");
                if (previewImageGO)
                {
                   currentImage = GetComponent<Image>();
                    GameObject go = Instantiate(previewImageGO, canvasGO.transform);
                    go.transform.SetAsLastSibling();
                    PreviewImage previewImage = go.GetComponent<PreviewImage>();
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                previewImage.Preview(sprite);
                } 
        }
        public UnityEngine.GameObject GetUIRoot()
        {       
            GameObject cavasGO = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load("AugmentationObject/UI")) as UnityEngine.GameObject;
            Canvas canvas = cavasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            return cavasGO;
        }
        private Texture2D InstantiateCropImage(Texture2D texture2D)
        {
            if (!texture2D)
            {
                return null;
            }
            Texture2D newTexture;
            if (!rectTransform)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            int desWidth =( int)(rectTransform.sizeDelta.x);
            int desHight = (int)(rectTransform.sizeDelta.y);
            try
            {
                //Pictures.CreatSmallPicture creatSmallPicture = new Pictures.CreatSmallPicture();
                newTexture =CropPicture(texture2D, desWidth / (float)desHight);
            }
            catch (Exception e)
            {
                newTexture = texture2D;
              // Debug.LogError(e.Message);
            }
          
            return newTexture;
        }
        public Texture2D CropPicture(Texture2D picture, float desireImageRatio = 2)
        {
            Texture2D newPicture = null;
            int pictureWidth = picture.width;
            int pictureHeight = picture.height;
            int newPicWidth = 0;
            int newPicHeight = 0;
            if (pictureWidth / (float)pictureHeight > desireImageRatio)
            {
                newPicHeight = picture.height;
                newPicWidth = (int)(newPicHeight * desireImageRatio);
                newPicture = new Texture2D(newPicWidth, newPicHeight, TextureFormat.ARGB32, false);
                for (int i = 0; i < newPicHeight; i++)
                {
                    for (int j = 0; j < newPicWidth; j++)
                    {
                        int picX = (int)((picture.width - newPicWidth) / 2.0) + j;
                        int picY = i;
                        Color color = picture.GetPixel(picX, picY);
                        newPicture.SetPixel(j, i, color);
                    }
                }
            }
            else
            {
                newPicWidth = picture.width;
                newPicHeight = (int)(newPicWidth / desireImageRatio);
                newPicture = new Texture2D(newPicWidth, newPicHeight, TextureFormat.ARGB32, false);
                for (int i = 0; i < newPicHeight; i++)
                {
                    for (int j = 0; j < newPicWidth; j++)
                    {
                        int picX = j;
                        int picY = (int)((picture.height - newPicHeight) / 2.0) + i;
                        Color color = picture.GetPixel(picX, picY);
                        newPicture.SetPixel(j, i, color);
                    }
                }
            }
            newPicture.Apply();
            return newPicture;
        }
    }
}
