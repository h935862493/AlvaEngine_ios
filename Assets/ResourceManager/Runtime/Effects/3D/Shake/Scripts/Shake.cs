using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
/// <summary>
/// ¶¶¶¯Ð§¹û
/// </summary>
public class Shake : SportEffectsBase
{
    public float shakeTime = 1f;
    public bool circulate = false;
    public float shakeRange = 6f;
    public float frequency = 3f;

    private bool IsExecute; 
    private bool IsShake;
    private Vector3 originRotation;
    private float Timeber;
    private float halfShakeTime;
    private float currentAngle;
    private float totalAngle;
    private float flag;

    private void Awake()
    {
        
    }

    void FixedUpdate()
    {
        halfShakeTime = 0.25f / frequency;
        if (IsExecute)
        {
            Timeber += Time.deltaTime;
            currentAngle = shakeRange / halfShakeTime * Time.deltaTime;

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

            transform.localEulerAngles = new Vector3(originRotation.x + totalAngle, originRotation.y + totalAngle, originRotation.z);
        }

        if (Timeber >= shakeTime)
        {
            transform.localEulerAngles = originRotation;
            StopExecute();
            if (circulate)
            {
                Recover();
                IsExecute = true;
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Shake>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        originRotation = transform.localEulerAngles;
        totalAngle = shakeRange;
        Timeber = 0;
        currentAngle = 0;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Shake>().enabled)
            return;
        IsExecute = false;
        Timeber = 0f;
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Shake>().enabled)
            return;
        IsShake = false;
        totalAngle = 0f;
        transform.localEulerAngles = originRotation;
    }
}
