using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
/// <summary>
/// Âö¶¯Ð§¹û
/// </summary>
public class Pulsate : SportEffectsBase
{
    public float pulsateTime = 1f;
    public bool circulate = false;
    public float pulsateRange = 1.2f;
    public float frequency = 3f;

    private bool IsExecute;
    private bool IsSmall;
    private float Timeber;
    private float Timeber1;
    private float Timeber2;
    private Vector3 originScale;
    private float currentRange;
    private float totalRange;
    private float halfCycleTime;

    void Update()
    {
        halfCycleTime = 0.5f/frequency;
        if (IsExecute)
        {
            Timeber += Time.deltaTime;
            currentRange = (pulsateRange - 1) / halfCycleTime * Time.deltaTime;
            totalRange += currentRange;

            if (!IsSmall)
            {
                Timeber1 += Time.deltaTime;
                transform.localScale = originScale * (1 + totalRange);
                if (Timeber1 >= halfCycleTime)
                {
                    IsSmall = true;
                    Timeber1 = 0f;
                    totalRange = 0f;
                }
            }
            else
            {
                Timeber2 += Time.deltaTime;
                transform.localScale = originScale * (pulsateRange - totalRange);
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
                Recover();
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
        if (!this.GetComponent<Pulsate>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        Timeber = 0;
        originScale = transform.localScale;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Pulsate>().enabled)
            return;
        IsExecute = false;
        Timeber = 0f;
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Pulsate>().enabled)
            return;
        transform.localScale = originScale;
    }
}
