using System.Collections.Generic;
using UnityEngine;
namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create CheckBoxStyle", fileName = "CheckBoxStyle")]
    public class CheckBoxStyle : ScriptableObject
    {
        public List<CheckBoxStyleItem> CheckBoxStyleList;
    }
    [System.Serializable]
    public class CheckBoxStyleItem
    {
        public CheckBoxType checkBoxType= CheckBoxType.NoWord;
        public Sprite toggleBackground;
        public Sprite check;
        public Sprite checkBoxBackground;

    }
    public enum CheckBoxType
    {
        NoWord,
        OnlyWord,
        WordAndBackground
    }
}
