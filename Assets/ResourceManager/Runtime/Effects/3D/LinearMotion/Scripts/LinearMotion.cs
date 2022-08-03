using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
/// <summary>
/// 沿坐标轴匀速直线运动(直线运动）
/// </summary>
public class LinearMotion : SportEffectsBase
{
    public SnapAxis axis = SnapAxis.x;
    public bool direction = true;
    public float distance = 1f;
    public float time = 1f;
    public bool circulate = false;
    public bool IsRecover = false;

    private Vector3 m_position;  
    private bool IsExecute; 
    private float Treshold;

    void Update()
    {
        float speed = distance / time ;
        if (IsExecute)
        {
            Treshold += speed * Time.deltaTime;
            if (direction)
                switch (axis)
                {
                    case SnapAxis.x:
                        transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
                        break;
                    case SnapAxis.y:
                        transform.localPosition += new Vector3(0,speed * Time.deltaTime, 0);
                        break;
                    case SnapAxis.z:
                        transform.localPosition += new Vector3(0, 0, speed * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
            else
                switch (axis)
                {
                    case SnapAxis.x:
                        transform.localPosition -= new Vector3(speed * Time.deltaTime, 0, 0);
                        break;
                    case SnapAxis.y:
                        transform.localPosition -= new Vector3(0, speed * Time.deltaTime, 0);
                        break;
                    case SnapAxis.z:
                        transform.localPosition -= new Vector3(0, 0, speed * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
            
            if (Treshold >= distance)
            {
                StopExecute();
               
                if (circulate)
                {
                    IsExecute = true;
                    Recover();
                }
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<LinearMotion>().enabled)
            return;
        if (IsExecute)
            return;
        m_position = transform.localPosition;
        IsExecute = true;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<LinearMotion>().enabled)
            return;
        IsExecute = false;
        Treshold = 0f;
        if (IsRecover)
        {
            Recover();
        }
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<LinearMotion>().enabled)
            return;
        transform.localPosition = m_position;
    }
}
