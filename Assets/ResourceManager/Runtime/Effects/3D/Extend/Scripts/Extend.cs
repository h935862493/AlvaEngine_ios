using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ÉìÕ¹Ð§¹û
/// </summary>
public enum ExtendDir
{ 
    center,
    horizontal,
    vertical
}
[DisallowMultipleComponent]
public class Extend : SportEffectsBase
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
        if(IsExecute){
            
            timeber += Time.deltaTime;

            speedx = originScale.x / time;
            speedy = originScale.y / time;
            speedz = originScale.z / time;

            currentSpeedx =speedx * Time.deltaTime;
            currentSpeedy = speedy * Time.deltaTime;
            currentSpeedz = speedz * Time.deltaTime;

            totalx += currentSpeedx;
            totaly += currentSpeedy;
            totalz += currentSpeedz;

            switch (direction)
            {
                case ExtendDir.center:
                    transform.localScale += new Vector3(currentSpeedx, currentSpeedy, currentSpeedz);
                    break;
                case ExtendDir.horizontal:
                    transform.localScale = new Vector3( totalx, transform.localScale.y, transform.localScale.z);
                    break;
                case ExtendDir.vertical:
                    transform.localScale = new Vector3(transform.localScale.x, totaly, transform.localScale.z);
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            if(timeber >= time)
            {
                StopExecute();
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Extend>().enabled)
            return;
        if (IsExecute)
            return;
        gameObject.SetActive(true);
        IsExecute = true;
        originScale = transform.localScale;
        switch (direction)
        {
            case ExtendDir.center:
                transform.localScale = new Vector3(0, 0, 0);
                break;
            case ExtendDir.horizontal:
                transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
                break;
            case ExtendDir.vertical:
                transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
                break;
            default:
                Debug.Log("error");
                break;
        }
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Extend>().enabled)
            return;
        IsExecute = false;
        timeber = 0f;
        totalx = 0f;
        totaly = 0f;
        totalz = 0f;
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Extend>().enabled)
            return;
        transform.localScale = originScale;
    }
}
