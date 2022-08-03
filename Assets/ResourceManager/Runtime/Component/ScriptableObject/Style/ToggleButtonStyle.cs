using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create ToggleButtonStyle", fileName = "ToggleButtonStyle")]
    public class ToggleButtonStyle : ScriptableObject
    {
        public List<ToggleButtonStyleItem> ToggleButtonStyleList;
    }
    [System.Serializable]
    public class ToggleButtonStyleItem
    {
        public Sprite toggleButtonBackground;
        public Sprite spliteSprite;
        public bool showToggleBackground;
        public Sprite toggleBackground;
        public Sprite toggleCheckmark;
        public bool showSpliteSprite;
        public List<ChooseToggleItem> toggleList=new List<ChooseToggleItem> (3);
    }
    [System.Serializable]
    public struct ChooseToggleItem
    {       
        public string name;
    }
}
