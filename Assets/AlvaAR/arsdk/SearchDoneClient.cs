using UnityEngine;
using System;
using AOT;

namespace arsdk
{
    // 用于事件处理器传参数
    internal class SearchDoneEventArgs : EventArgs
    {
        public readonly int foundIndex;
        public readonly int frameIndex;

        public SearchDoneEventArgs(int foundIdx, int frameIdx)
        {
            foundIndex = foundIdx;
            frameIndex = frameIdx;
        }
    }

    public class SearchDoneClient
    {
        public delegate void SearchDoneListener(int foundIdx, int frameIdx);
        public static event EventHandler OnSearchDoneLinkEvent;

        [MonoPInvokeCallback(typeof(SearchDoneListener))]
        private static void Listener(int foundIdx, int frameIdx)
        {
            Debug.Log("SearchDoneListener Event: " + foundIdx);

            var handler = OnSearchDoneLinkEvent;
            if (handler != null)
            {
                handler(null, new SearchDoneEventArgs(foundIdx, frameIdx));
            }
        }

        public static SearchDoneClient _instance;
        public static SearchDoneClient getInstance()
        {
            if (_instance == null)
            {
                _instance = new SearchDoneClient();
            }
            return _instance;
        }

        //初始接口
        public void Init()
        {
            AlvaARWrapper.Instance.TrackerManagerOnSearchDone(new SearchDoneListener(Listener));
            Debug.Log("setOnSearchDone!");
        }
        
        public void Deinit()
        {
            AlvaARWrapper.Instance.TrackerManagerOnSearchDone(null);
            Debug.Log("setOnSearchDone is null!");
        }
    }
}