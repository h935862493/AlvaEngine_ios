using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create SelectStyle", fileName = "SelectStyle")]
    public class SelectStyle : ScriptableObject
    {
        public Sprite upImage;
        public Sprite downImage;
        public List<SelectStyleItem> SelectStyleList;
    }
    [System.Serializable]
    public class SelectStyleItem
    {
        public Sprite titleBackground;
        public Sprite contentBackground;
        public float distanceToTop ;
        public Sprite tagBackground;
        public Sprite tagChooseImage;
        public List<TagItem> tagItemList;

    }
    [System.Serializable]
    public class TagItem
    {
        public string tagName;     
    }
}
