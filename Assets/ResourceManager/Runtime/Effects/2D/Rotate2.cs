using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 绕物体旋转(自转)
/// </summary>
public class Rotate2 : SportEffectsBase
{
    public AxisUI axis = AxisUI.y;
    [HideInInspector]
    public bool IsWorldAxis = true;
    public bool direction = true;
    public float TurnCount = 1f;
    public float time = 1f;
    public bool circulate = false;

    private bool IsExecute;
    private bool Inscrease;
    private float speed;
    private float Treshold;
    private Vector3 originRotation;


    void Update()
    {
        if (IsExecute)
        {
            if (direction)
            {
                Treshold += speed * Time.deltaTime;
                switch (axis)
                {
                    case AxisUI.x:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, speed * Time.deltaTime,  0, Space.World);
                            else
                                this.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
                        }
                        break;
                    case AxisUI.y:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(speed * Time.deltaTime, 0, 0, Space.World);
                            else
                                this.transform.Rotate(speed * Time.deltaTime, 0, 0, Space.Self);
                        }
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
                if (Treshold >= TurnCount * 360)
                {
                    StopExecute();
                    Recover();
                    if (circulate)
                    {
                        IsExecute = true;
                    }
                }
            }
            else
            {
                Treshold -= speed * Time.deltaTime;
                switch (axis)
                {
                    case AxisUI.x:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(-speed * Time.deltaTime, 0, 0, Space.World);
                            else
                                this.transform.Rotate(-speed * Time.deltaTime, 0, 0, Space.Self);
                        }
                        break;
                    case AxisUI.y:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, -speed * Time.deltaTime, 0, Space.World);
                            else
                                this.transform.Rotate(0, -speed * Time.deltaTime, 0, Space.Self);
                        }
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
                if (Treshold <= -TurnCount * 360)
                {
                    StopExecute();
                    if (circulate)
                    {
                        IsExecute = true;
                    }
                }
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<Rotate2>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        originRotation = transform.localEulerAngles;
        speed = TurnCount * 360 / time;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Rotate2>().enabled)
            return;
        IsExecute = false;
        Treshold = 0f;
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Rotate2>().enabled)
            return;
        transform.localEulerAngles = originRotation;
    }
}
