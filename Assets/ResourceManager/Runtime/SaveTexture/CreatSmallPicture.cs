using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Alva.Pictures
{
    public class CreatSmallPicture
    {
        public enum PictureType
        {
            PNG, JPG
        }

        public Texture2D picture;
        public int maxWidth = 300;
        public int maxHeight = 300;
        int newPictureHeight = 0;
        int newPictureWidth = 0;
        public PictureType GetPictureType(string extention)
        {
            PictureType pictureType = PictureType.JPG;
            if (extention == ".png")
            {
                pictureType = PictureType.PNG;
            }
            return pictureType;
        }
        public void SavePicture(Texture2D picture, PictureType pictureType, int maxWidth, int maxHeight, string saveUrlToAssets, float desireImageRatio = 2)
        {
#if UNITY_EDITOR
            if (picture.width ==maxWidth &&picture .height ==maxHeight )
            {

            }
            else
            {
                picture = CropPicture(picture, desireImageRatio);
            }         
            if (picture.width < maxWidth || picture.height < maxHeight)
            {
                if (picture.width > picture.height)
                {
                    this.maxWidth = picture.height;
                    this.maxHeight = picture.height;
                }
                else
                {
                    this.maxWidth = picture.width;
                    this.maxHeight = picture.width;
                }
            }
            else
            {
                this.maxWidth = maxWidth;
                this.maxHeight = maxHeight;
            }


            Texture2D newPicture = InstantiateSmallPicture(picture);
            SavePicture(newPicture, pictureType, saveUrlToAssets);
            ChangeTextureType(saveUrlToAssets);
#endif
        }
#if UNITY_EDITOR
        public void CreatRequiredSizePictureAnsSavePicture(Texture2D picture, PictureType pictureType,int desWidth,int desHeight, string saveUrlToAssets)
        {
            picture = CropPicture(picture, desWidth/(float)desHeight);
            Texture2D newPicture = InstantiateRequiredSizePicture(picture,desWidth ,desHeight );
            SavePicture(newPicture, pictureType, saveUrlToAssets);
            ChangeTextureType(saveUrlToAssets);
        }

        public Texture2D InstantiateRequiredSizePicture(Texture2D picture, int desWidth, int desHeight)
        {
            Texture2D newPicture = new Texture2D(desWidth, desHeight, TextureFormat.ARGB32, false);
            for (int i = 0; i < desHeight; i++)
            {
                for (int j = 0; j < desWidth; j++)
                {
                    int picX =Mathf .Min((int)(j * picture.width /(float) desWidth), picture.width)  ;
                    int picY = Mathf.Min((int)(i * picture.height / (float)desHeight), picture.height);
                    Color color = picture.GetPixel(picX, picY);
                    newPicture.SetPixel(j, i, color);
                }
            }
            return newPicture;
        }

        /// <summary>
        /// 图片按照比例裁剪
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="desireImageRatio"></param>
        /// <returns></returns>
        public Texture2D CropPicture(Texture2D picture, float desireImageRatio = 2)
        {
            Texture2D newPicture = null;
            int pictureWidth = picture.width;
            int pictureHeight = picture.height;
            int newPicWidth = 0;
            int newPicHeight = 0;
            if (pictureWidth / (float)pictureHeight > desireImageRatio)
            {
                newPicHeight= picture.height;
                newPicWidth = (int)(newPicHeight * desireImageRatio);
                newPicture = new Texture2D(newPicWidth, newPicHeight, TextureFormat.ARGB32, false);
                for (int i = 0; i < newPicHeight; i++)
                {
                    for (int j = 0; j < newPicWidth; j++)
                    {
                        int picX =(int)( (picture.width- newPicWidth)/2.0)+j;
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
                        int picY = (int)((picture.height-newPicHeight)/2.0)+i;
                        Color color = picture.GetPixel(picX, picY);
                        newPicture.SetPixel(j, i, color);
                    }
                }
            }
            newPicture.Apply();
            return newPicture;
        }

        private void SavePicture(Texture2D newPicture, PictureType pictureType, string saveUrlToAssets)
        {
            string extension = ".jpg";
            byte[] bytes = new byte[1024];
            if (pictureType == PictureType.JPG)
            {
                extension = ".jpg";
                bytes = newPicture.EncodeToJPG();
            }
            else if (pictureType == PictureType.PNG)
            {
                extension = ".png";
                bytes = newPicture.EncodeToPNG();
            }
            // string thumbnail = System.IO.Path.Combine(UnityEngine.Application.dataPath.Replace("/Assets", string.Empty), Alva.Config.EditorPathConfig.PlayerInfo + "/projectThumbnail" + extension);
            string thumbnail = System.IO.Path.Combine(UnityEngine.Application.dataPath.Replace("/Assets", string.Empty), saveUrlToAssets);
            FileStream fs = File.Open(thumbnail, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
        }
/// <summary>
/// 生成压缩缩放后的图片
/// </summary>
/// <param name="picture"></param>
/// <param name="desireImageRatio"></param>
/// <returns></returns>
        public Texture2D InstantiateSmallPicture(Texture2D picture)
        {
            Texture2D newPicture = null;
           
            if (picture.width > picture.height)
            {
                newPictureWidth = maxWidth;
                newPictureHeight = newPictureWidth * picture.height / picture.width;
            }
            else
            {
                newPictureHeight = maxHeight;
                newPictureWidth = newPictureHeight * picture.width / picture.height;
            }
            newPicture = new Texture2D(newPictureWidth, newPictureHeight, TextureFormat.ARGB32, false);
            float widthRatio = picture.width / (float)newPictureWidth;
            float heightRatio = picture.height / (float)newPictureHeight;
            for (int i = 0; i < newPictureHeight; i++)
            {
                for (int j = 0; j < newPictureWidth; j++)
                {
                    int picX = (int)(j * widthRatio) > picture.width ? picture.width : (int)(j * widthRatio);
                    int picY = (int)(i * heightRatio) > picture.height ? picture.height : (int)(i * widthRatio);
                    Color color = picture.GetPixel(picX, picY);
                    newPicture.SetPixel(j, i, color);
                }
            }
            return newPicture;
        }
        public static void ChangeTextureType(string assetFileName)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetFileName); // 获取文件
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
        }
#endif
    }
}
