using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create ButtonStyle", fileName = "ButtonStyle")]
    public class ButtonStyle : ScriptableObject
    {
        public List<ButtonStyleItem> ButtonStyleList;      
    }
    [System.Serializable]
    public class ButtonStyleItem
    {
        public ButtonTransiton buttonTransiton = ButtonTransiton.Color;
        public bool showButtonText = true;
        public Sprite normal;
        /// <summary>
        /// 待解决根据buttonTransiton 变化控制sprites显隐
        /// </summary>
        public ButtonSprites sprites;
        [SerializeField]
        public float width = 160;
        [SerializeField]
        public float height = 30;
    }
    [System.Serializable]
    public enum ButtonTransiton
    {
        Color,
        Sprite,
       // Animation
    }
    [System.Serializable]
    public class ButtonSprites
    {
        public Sprite highlightedSprite;
        public Sprite pressedSprite;
        public Sprite selectedSprite;
        public Sprite disabledSprite;
    }
}
