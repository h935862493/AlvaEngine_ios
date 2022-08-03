using Alva.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Alva.Effect;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Alva.Attributes;

namespace Alva.Runtime.Components
{
    public enum ImageShowType
    {
        Simple,
        LongType,
        ShortType
    }
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class ImageAgent : Agent, IPointerClickHandler
    {
        public ImageStyle imageStyle;
        RectTransform rectTransform;
        Image background;
        bool isInit = false;
        public bool CanPreview = false;
        [ConditionalHide("is3D",true)]
        public bool ShowBillboard = false;
        Camera camera;
        [SerializeField, HideInInspector]
        public bool isNewInstantiate = false;
        public GameObject previewImageGO;
        [HideInInspector]
        public GameObject imageMask;
        [HideInInspector]
        public GameObject imageShow;
        [SerializeField, HideInInspector]
        bool is3D = true;
        public ImageShowType ImageShowType;

        private ImageShowType CurrentType;

        private void Awake()
        {
            MyInit();
        }
        public override void Init()
        {
            defaultElement = "style1";
            isNewInstantiate = true;
            is3D = componentPrefabType == ComponentPrefabType.Image;
            CurrentType = ImageShowType;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }

        
        void LateUpdate()
        {
            if (ShowBillboard && is3D)
            {
                if (camera == null)
                {
                    camera = FindObjectOfType<Camera>();
                    if (camera == null)
                    {
                        return;
                    }
                }
                if (camera)
                {
                    // 这里我的角色朝向和UI朝向是相反的，如果直接用LookAt()还需要把每个UI元素旋转过来。
                    // 为了简单，用了下面这个方法。它实际上是一个反向旋转，可以简单理解为“负负得正”吧
                    transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
                 //   transform .localEulerAngles =new Vector3(0 , transform.localEulerAngles.y,0);
                }
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (CanPreview)
            {
                //  Debug.LogError("预览图片");

                GameObject canvasGO = GetUIRoot();
              //  GameObject previewImageGO = Resources.Load<GameObject>("Effect/PreviewImage");
                if (previewImageGO)
                {
                    background = GetComponent<Image>();
                    GameObject go = Instantiate(previewImageGO, canvasGO.transform);
                    go.transform.SetAsLastSibling();
                    PreviewImage previewImage = go.GetComponent<PreviewImage>();
                    previewImage.Preview(background.sprite);
                }
            }
        }
        /// <summary>
        /// 获取UI根节点对象
        /// </summary>
        /// <returns> UI根节点 </returns>
        public UnityEngine.GameObject GetUIRoot()
        {            
           // System.Collections.Generic.IEnumerable<UnityEngine.Canvas> canvas = GameObject.FindObjectsOfType<UnityEngine.Canvas>().Where(canvas => canvas.renderMode != UnityEngine.RenderMode.WorldSpace && canvas.hideFlags != (UnityEngine.HideFlags.NotEditable | UnityEngine.HideFlags.HideInHierarchy | UnityEngine.HideFlags.HideInInspector));
            //if (canvas.Count() > 0)
            //{
            //    return canvas.First().gameObject;
            //}
            //else
            //{
            GameObject cavasGO= UnityEngine.Object.Instantiate(UnityEngine.Resources.Load("AugmentationObject/UI")) as UnityEngine.GameObject;
            Canvas canvas = cavasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            return cavasGO;

            //}
        }
        public void SetContent(Sprite sprite)
        {
            background = GetComponent<Image>();
            background.sprite = sprite;
        }
        public Sprite GetContent()
        {
            background = GetComponent<Image>();
            return background.sprite;
        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            rectTransform = GetComponent<RectTransform>();
            background = GetComponent<Image>();

            isInit = true;
        }
        public Image GetImage()
        {
            return GetComponent<Image>();
        }
        public GameObject GetGameObject()
        {
            return this.gameObject;
        }
        #region 组件样式
        public override bool GetShowStyle()
        {
            return true;
        }

        public override string[] GetStyle()
        {
            int totalStyle = imageStyle.ImageStyleList.Count;
            string[] styleList = new string[totalStyle];
            for (int i = 0; i < totalStyle; i++)
            {
                styleList[i] = "style" + (i + 1);
            }
            return styleList;
        }

        private Action ChangeStyle;
        
        public override void OnStyleValueSelected(object value)
        {
            Debug.Log(value.ToString());
            MyInit();
            if (defaultElement == value.ToString() || value.ToString() == "None")
            {
                return;
            }
            base.OnStyleValueSelected(value);
            for (int i = 0; i < imageStyle.ImageStyleList.Count; i++)
            {
                if (value.ToString() == GetStyle()[i])
                {
                    background.sprite = imageStyle.ImageStyleList[i].background;
                    rectTransform.sizeDelta = new Vector2(imageStyle.ImageStyleList[i].width, imageStyle.ImageStyleList[i].height);
                    break;
                }
            }
            // rectTransform.sizeDelta = new Vector2(background.sprite.texture.width, background.sprite.texture.height);
            defaultElement = value.ToString();
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Changed default element");
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }
        #endregion
        }
}

