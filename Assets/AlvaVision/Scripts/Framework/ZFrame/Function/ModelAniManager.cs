using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAniManager : MonoBehaviour
{
    private Animation ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponentInChildren<Animation>();
        GlobalData.IsInitAniPlayAction += OnInitAniPlay;
    }

    private void OnInitAniPlay()
    {
        GlobalData.IsModelAniPlayAction?.Invoke(false);
        isPlay = false;
    }

    bool isPlay = false;

    // Update is called once per frame
    void Update()
    {
        if (ani == null)
        {
            return;
        }
        if (ani.isPlaying && !isPlay)
        {
            isPlay = true;
            Debug.Log("播放动画了");
            GlobalData.IsModelAniPlayAction?.Invoke(true);
        }
        else if(!ani.isPlaying && isPlay)
        {
            isPlay = false;
            Debug.Log("动画停止了");
            GlobalData.IsModelAniPlayAction?.Invoke(false);
        }
    }

    private void OnDestroy()
    {
        GlobalData.IsInitAniPlayAction -= OnInitAniPlay;
    }
}
