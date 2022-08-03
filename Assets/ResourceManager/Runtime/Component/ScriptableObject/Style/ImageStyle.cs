using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create ImageStyle", fileName = "ImageStyle")]
    public class ImageStyle : ScriptableObject
    {
        public List<ImageStyleItem> ImageStyleList;
    }
    [System.Serializable]
    public class ImageStyleItem
    {
        public Sprite background;
        [SerializeField]
        public float width = 160;
        [SerializeField]
        public float height = 30;
    }
}
