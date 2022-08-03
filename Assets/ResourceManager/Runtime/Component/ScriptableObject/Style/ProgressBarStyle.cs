using System.Collections.Generic;
using UnityEngine;
namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create ProgressBarStyle", fileName = "ProgressBarStyle")]
    public class ProgressBarStyle : ScriptableObject
    {
        public List<ProgressBarStyleItem> ProgressBarStyleList;
    }
    [System.Serializable]
    public class ProgressBarStyleItem
    {
        public Sprite background;
        public Sprite progress;
        [SerializeField]
        public FillType fillType = FillType.horizental;
        [SerializeField]
        public TextPosition textPosition = TextPosition.NoShow;
    }
    public enum FillType
    {
        horizental=0,
        Radial360=1
    }
    public enum TextPosition
    {
        NoShow,
        Center,
        Upper
    }
}