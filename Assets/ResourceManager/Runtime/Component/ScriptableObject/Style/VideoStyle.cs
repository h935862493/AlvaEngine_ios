using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Style
{
    [CreateAssetMenu(menuName = "MySubMenue/Create VideoStyle", fileName = "VideoStyle")]
    public class VideoStyle : ScriptableObject
    {
        public List<VideoStyleItem> VideoStyleList;
    }
    [System.Serializable]
    public class VideoStyleItem
    {
        public Sprite background;
        public Sprite soundOpen;
        public Sprite soundShut;
        public Sprite fullScreen;
        public Sprite noFullScreen;
        public Sprite play;
        public Sprite stop;
    }
}
