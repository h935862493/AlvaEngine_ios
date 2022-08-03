using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate2 : SportEffectsBase
{
    //[Header("时间")]
    public float pulsateTime = 1f;
    //[Header("循环")]
    public bool circulate = false;
    //[Header("幅度")]
    private float pulsateRange = 1.2f;
    //[Header("频率")]
    private float frequency = 3f;

    private bool IsExecute;
    private bool IsSmall = false;
    private float Timeber = 0f;
    private float Timeber1 = 0f;
    private float Timeber2 = 0f;
    private Vector3 originScale;
    private float currentRange;
    private float totalRange;
    private float halfCycleTime;

    private void Awake()
    {
    }

    void Update()
    {
        //半周期时间
        halfCycleTime = 0.5f / frequency;
        if (IsExecute)
        {
            //总计时器
            Timeber += Time.deltaTime;
            //每帧变大程度
            currentRange = (pulsateRange - 1) / halfCycleTime * Time.deltaTime;
            //总变大程度
            totalRange += currentRange;
            if (!IsSmall)
            {
                //变大计时器
                Timeber1 += Time.deltaTime;
                this.GetComponent<RectTransform>().localScale = originScale * (1 + totalRange);
                if (Timeber1 >= halfCycleTime)
                {
                    IsSmall = true;
                    Timeber1 = 0f;
                    totalRange = 0f;
                }
            }
            else
            {
                //变小计时器
                Timeber2 += Time.deltaTime;
                this.GetComponent<RectTransform>().localScale = originScale * (pulsateRange - totalRange);
                if (Timeber2 >= halfCycleTime)
                {
                    IsSmall = false;
                    Timeber2 = 0f;
                    totalRange = 0f;
                    Recover();
                }
            }

            if (Timeber >= pulsateTime)
            {
                StopExecute();
                if (circulate)
                {
                    Timeber = 0f;
                    IsExecute = true;
                }
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Pulsate2>().enabled)
            return;
        if (IsExecute)
            return;
        originScale = this.GetComponent<RectTransform>().localScale;
        IsExecute = true;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Pulsate2>().enabled)
            return;
        IsExecute = false;
        Timeber = 0f;
        Recover();
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Pulsate2>().enabled)
            return;
        this.GetComponent<RectTransform>().localScale = originScale;
    }
}
