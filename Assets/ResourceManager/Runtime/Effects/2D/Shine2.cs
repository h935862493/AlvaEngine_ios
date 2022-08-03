using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[DisallowMultipleComponent]
public class Shine2 : MaterialEffectsBase
{
    //[Header("颜色")]
    public Color color = new Color(1, 0, 0, 1);
    public float frequency = 3f;
    public float time = 2f;
    public bool IsCircle;

    /*[System.Serializable]
    public struct Alphatest
    {
        [Range(0, 100)]
        public int alphaRange;
        public float frequency;
        public float time;
        //public bool IsCircle;
    }*/

    /*//*[ShowWhen("IsShine", true)]
    public int alphaRange;
    //[ShowWhen("IsShine", true)]
    [Header("闪烁频率")]
    public float frequency = 3f;
    //[ShowWhen("IsShine", true)]
    [Header("时间")]
    public float time = 2f;
    //[ShowWhen("IsShine", true)]
    [Header("循环")]
    public bool IsCircle = false;*/

    private Color colorStart;
    private float Timeber;
    private float duration;
    private float duration2;

    void Update()
    {
        if (IsExecute)
        {
            Timeber += Time.deltaTime;
            duration = 1 / frequency ;
            //duration2 = duration / time;
            var lerp = Mathf.PingPong(Time.time, duration) / duration;
            this.GetComponent<Image>().color = Color.Lerp(colorStart, color, lerp);
            if (Timeber >= time)
            {
                StopExecute();
                if (IsCircle)
                {
                    Execute();
                }
                EndEvent?.Invoke();
            }
        }
    }

    public new void Execute()
    {
        if (!this.GetComponent<Shine2>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        colorStart = this.GetComponent<Image>().color;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Shine2>().enabled)
            return;
        IsExecute = false;
        this.GetComponent<Image>().color = colorStart;
        Timeber = 0f;
    }

    public override void Recover()
    {
        if (!this.GetComponent<Shine2>().enabled)
            return;
        this.GetComponent<Image>().color = colorStart;
    }
}
