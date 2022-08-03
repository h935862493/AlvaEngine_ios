using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake2 : SportEffectsBase
{
    //[Header("时间")]
    public float shakeTime = 1f;
    //[Header("循环")]
    public bool circulate = false;
    //[Header("抖动幅度")]
    public float shakeRange = 6f;
    //[Header("频率")]
    public float frequency = 3f;

    private bool IsExecute;
    private bool IsShake;
    private Vector3 originRotation;
    private float Timeber = 0f;
    private float halfShakeTime;
    private float currentAngle;
    private float totalAngle;
    private float flag = 0;

    void Update()
    {
        if (IsExecute)
        {
            Timeber += Time.deltaTime;
            currentAngle = shakeRange * 4 * frequency * Time.deltaTime;

            if (!IsShake)
            {
                totalAngle -= currentAngle;
                if (totalAngle <= -shakeRange)
                {
                    IsShake = true;
                }
            }
            else
            {
                totalAngle += currentAngle;
                if (totalAngle >= shakeRange)
                {
                    IsShake = false;
                }
            }

            if (totalAngle>=0)
            {
                this.GetComponent<RectTransform>().localEulerAngles = new Vector3(originRotation.x , originRotation.y + totalAngle, originRotation.z + totalAngle);
            }
            else
            {
                this.GetComponent<RectTransform>().localEulerAngles = new Vector3(originRotation.x , originRotation.y - totalAngle, originRotation.z - totalAngle);
            }
            
        }
        if (Timeber >= shakeTime)
        {
            StopExecute();
            if (circulate)
            {
                IsExecute = true;
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Shake2>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        originRotation = this.GetComponent<RectTransform>().localEulerAngles;
        totalAngle = shakeRange;
        Recover();
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Shake2>().enabled)
            return;
        IsExecute = false;
        Timeber = 0f;
        Recover();
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Shake2>().enabled)
            return;
        IsShake = false;
        totalAngle = 0f;
        this.GetComponent<RectTransform>().localEulerAngles = originRotation;
    }
}
