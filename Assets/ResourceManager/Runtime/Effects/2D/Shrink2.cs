using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink2 : SportEffectsBase
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
                    this.GetComponent<RectTransform>().localScale -= new Vector3(currentSpeedx, currentSpeedy, currentSpeedz);
                    break;
                case ExtendDir.horizontal:
                    this.GetComponent<RectTransform>().localScale -= new Vector3(currentSpeedx, 0, 0);
                    break;
                case ExtendDir.vertical:
                    this.GetComponent<RectTransform>().localScale -= new Vector3(0, currentSpeedy, 0);
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            if (timeber >= time)
            {
                StopExecute();
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Shrink2>().enabled)
            return;
        if (IsExecute)
            return;
        originPosition = this.GetComponent<RectTransform>().anchoredPosition;
        originScale = this.GetComponent<RectTransform>().localScale;
        originLocalRotation = this.GetComponent<RectTransform>().eulerAngles;
        IsExecute = true;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Shrink2>().enabled)
            return;
        IsExecute = false;
        gameObject.SetActive(false);
        timeber = 0f;
        Recover();
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Shrink2>().enabled)
            return;
        this.GetComponent<RectTransform>().anchoredPosition = originPosition;
        this.GetComponent<RectTransform>().eulerAngles = originLocalRotation;
        this.GetComponent<RectTransform>().localScale = originScale;
    }
}
