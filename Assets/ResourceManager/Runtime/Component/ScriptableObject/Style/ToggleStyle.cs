using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName= "MySubMenue/Create ToggleStyle", fileName= "ToggleStyle")]
    public class ToggleStyle : ScriptableObject
    {
        public List<ToggleStyleItem> ToggleStyleList;
    }
    [System.Serializable]
    public class ToggleStyleItem
    {
        public Sprite background;
        public Sprite checkmark;
    }
}
