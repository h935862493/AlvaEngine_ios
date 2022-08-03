using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 围绕一个中心点进行旋转(公转)
/// </summary>

public class CircleMotion : SportEffectsBase
{
    public Vector3 AroundPoint = new Vector3(1,0,0);
    public SnapAxis axis = SnapAxis.y;
    [HideInInspector]
    public bool IsWorldAxis = true;
    public bool direction = true;
    public float turnNum = 1f;
    public float time = 2f;
    public bool  circulate = false;

    private bool IsExecute;
    private bool Inscrease;
    private float speed;
    private float Treshold;
    private Vector3 originPosition;
    private Vector3 originRotation;

    void Update()
    {
        if (IsExecute)
        {
            speed = turnNum * 360 / time;
            if (direction)
            {
                Treshold += speed * Time.deltaTime;
                switch (axis)
                {
                    case SnapAxis.x:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.right, speed * Time.deltaTime);
                            //this.transform.localPosition
                        else
                            this.transform.RotateAround(AroundPoint, new Vector3(AroundPoint.x, 0, 0), speed * Time.deltaTime);
                        break;
                    case SnapAxis.y:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.up, speed * Time.deltaTime);
                        else
                            this.transform.RotateAround(AroundPoint, new Vector3(0, AroundPoint.y, 0), speed * Time.deltaTime);
                        break;
                    case SnapAxis.z:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.forward, speed * Time.deltaTime);
                        else
                            this.transform.RotateAround(AroundPoint, new Vector3(0, 0, AroundPoint.z), speed * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }

                if (Treshold >= turnNum * 360)
                {
                    StopExecute();
                    if (circulate)
                    {
                        Treshold = 0f;
                        IsExecute = true;
                    }
                }
            }
            else
            {
                Treshold -= speed * Time.deltaTime;
                switch (axis)
                {
                    case SnapAxis.x:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.right, -speed * Time.deltaTime);
                        else
                            this.transform.RotateAround(AroundPoint, Vector3.right, -speed * Time.deltaTime);
                        break;
                    case SnapAxis.y:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.up, -speed * Time.deltaTime);
                        else
                            this.transform.RotateAround(AroundPoint, Vector3.up, -speed * Time.deltaTime);
                        break;
                    case SnapAxis.z:
                        if (IsWorldAxis) 
                            this.transform.RotateAround(AroundPoint, Vector3.forward, -speed * Time.deltaTime);
                        else
                            this.transform.RotateAround(AroundPoint, Vector3.up, -speed * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }

                if (Treshold <= -turnNum * 360)
                {
                    StopExecute();
                    Recover();
                    if (circulate)
                    {
                        Treshold = 0f;
                        IsExecute = true;
                    }
                }
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<CircleMotion>().enabled)
            return;
        if (IsExecute)
            return;
        originPosition = transform.localPosition;
        originRotation = transform.localEulerAngles;
        IsExecute = true;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<CircleMotion>().enabled)
            return;
        IsExecute = false;
        Treshold = 0f;
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<CircleMotion>().enabled)
            return;
        transform.localPosition = originPosition;
        transform.localEulerAngles = originRotation;
    }
}
