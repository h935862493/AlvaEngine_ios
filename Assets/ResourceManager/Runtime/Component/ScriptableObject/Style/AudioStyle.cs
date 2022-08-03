using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create AudioStyle", fileName = "AudioStyle")]
    public class AudioStyle : ScriptableObject
    {
        public List<AudioStyleItem> AudioStyleList;     
    }
    [System.Serializable]
    public class AudioStyleItem {
        public Sprite background;
        public Sprite soundOpen;
        public Sprite soundShut;
        public Sprite play;
        public Sprite stop;
    }

}
