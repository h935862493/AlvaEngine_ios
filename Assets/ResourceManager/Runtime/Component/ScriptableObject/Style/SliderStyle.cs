using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create SliderStyle", fileName = "SliderStyle")]
    public class SliderStyle : ScriptableObject
    {
        public List<SliderStyleItem> SliderStyleList;
    }
    [System.Serializable]
    public class SliderStyleItem
    {
        public SliderType sliderType = SliderType.normal;
        public bool showBackground;
        public bool showHintInfo;
        public Sprite sliderForgroundImage;
        public Sprite sliderSmallForgroundImage;
        public Sprite sliderBackgroundImage;      
        public Sprite sliderHandleImage;
    }
    public enum SliderType {
        normal,
        round
    }

}
