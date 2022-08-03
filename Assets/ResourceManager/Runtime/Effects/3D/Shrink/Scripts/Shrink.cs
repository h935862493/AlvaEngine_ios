using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Shrink : SportEffectsBase
{
    public float time = 1f;
    public ExtendDir direction = ExtendDir.center;

    private bool IsExecute;
    private Vector3 originScale;
    private Vector3 originPosition;
    private Vector3 originLocalRotation;
    private float timeber;

    private float speedx;
    private float speedy;
    private float speedz;

    private float currentSpeedx;
    private float currentSpeedy;
    private float currentSpeedz;

    private float totalx;
    private float totaly;
    private float totalz;

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;

            speedx = originScale.x / time;
            speedy = originScale.y / time;
            speedz = originScale.z / time;

            currentSpeedx = speedx * Time.deltaTime;
            currentSpeedy = speedy * Time.deltaTime;
            currentSpeedz = speedz * Time.deltaTime;

            totalx += currentSpeedx;
            totaly += currentSpeedy;
            totalz += currentSpeedz;
            switch (direction)
            {
                case ExtendDir.center:
                    transform.localScale -= new Vector3(currentSpeedx, currentSpeedy, currentSpeedz);
                    break;
                case ExtendDir.horizontal:
                    transform.localScale -= new Vector3(0, 0, currentSpeedz);
                    break;
                case ExtendDir.vertical:
                    transform.localScale -= new Vector3(0, currentSpeedy, 0);
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            if (timeber >= time)
            {
                transform.localScale = new Vector3(0, 0, 0);
                StopExecute();
                
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Shrink>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        timeber = 0f;
        originPosition = transform.position;
        originScale = transform.localScale;
        originLocalRotation = transform.localEulerAngles;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Shrink>().enabled)
            return;
        gameObject.SetActive(false);
        IsExecute = false;
        timeber = 0f;
        transform.position = originPosition;
        transform.localEulerAngles = originLocalRotation;
        transform.localScale = originScale;
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Shrink>().enabled)
            return;
        transform.position = originPosition;
        transform.localEulerAngles = originLocalRotation;
        transform.localScale = originScale;
        gameObject.SetActive(true);
    }
}
