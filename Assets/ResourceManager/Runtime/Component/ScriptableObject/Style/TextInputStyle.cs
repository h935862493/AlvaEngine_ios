using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create TextInputStyle", fileName = "TextInputStyle")]
    public class TextInputStyle : ScriptableObject
    {
        public List<TextInputStyleItem> TextInputStyleList;
    }
    [System.Serializable]
    public class TextInputStyleItem
    {
        public Sprite background;      
    }
}
