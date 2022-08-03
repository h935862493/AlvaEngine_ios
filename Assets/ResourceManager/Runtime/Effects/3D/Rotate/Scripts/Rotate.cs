using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 绕物体旋转(自转)
/// </summary>
public class Rotate : SportEffectsBase
{
    public SnapAxis axis = SnapAxis.y;
    [HideInInspector]
    public bool IsWorldAxis = false;
    public bool direction = true;
    public float TurnCount = 1f;
    public float time = 1f;
    public bool circulate = false;

    private bool IsExecute;
    private bool Inscrease;
    private float speed;
    private float Treshold;
    private Vector3 originRotation;

    private void Awake()
    {
        originRotation = this.transform.localEulerAngles;
        
    }

    void Update()
    {
        if (IsExecute) 
        {
            if (direction) 
            {
                Treshold += speed * Time.deltaTime;
                switch (axis)
                {
                    case SnapAxis.x:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(speed * Time.deltaTime, 0, 0, Space.World);
                            else
                                this.transform.Rotate(speed * Time.deltaTime, 0, 0, Space.Self);
                        }
                        break;
                    case SnapAxis.y:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, speed * Time.deltaTime, 0, Space.World);
                            else
                                this.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
                        }
                        break;
                    case SnapAxis.z:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, 0, speed * Time.deltaTime, Space.World);
                            else
                                this.transform.Rotate(0, 0, speed * Time.deltaTime, Space.Self);
                        }
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
                if(Treshold >= TurnCount * 360)
                {
                    StopExecute();
                    //Recover();
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
                    case SnapAxis.x:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(-speed * Time.deltaTime, 0, 0, Space.World);
                            else
                                this.transform.Rotate(-speed * Time.deltaTime, 0, 0, Space.Self);
                        }
                        break;
                    case SnapAxis.y:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, -speed * Time.deltaTime, 0, Space.World);
                            else
                                this.transform.Rotate(0, -speed * Time.deltaTime, 0, Space.Self);
                        }
                        break;
                    case SnapAxis.z:
                        {
                            if (IsWorldAxis)
                                this.transform.Rotate(0, 0, -speed * Time.deltaTime, Space.World);
                            else
                                this.transform.Rotate(0, 0, -speed * Time.deltaTime, Space.Self);
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

    /*private float CountDegree(Transform g,SnapAxis snapAxis)
    {
        switch (snapAxis)
        {
            case SnapAxis.x: 
                return g.localEulerAngles.x; 
                break;
            case SnapAxis.y: 
                return g.localEulerAngles.y; 
                break;
            case SnapAxis.z: 
                return g.localEulerAngles.z;
                break;
            default:break;
        }
        
        return 0;
    }*/

    public override void Execute()
    {
        if (!this.GetComponent<Rotate>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        //Vector3 degree = new Vector3(0, 0, 0);
        var Father = this.transform.parent;
        while (Father != null)
        {
            originRotation -= Father.localEulerAngles;
            Father = Father.parent;
        }
        speed = TurnCount * 360 / time;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<Rotate>().enabled)
            return;
        IsExecute = false;
        Treshold = 0f;
        //Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<Rotate>().enabled)
            return;
        transform.localEulerAngles = originRotation;
    }
}
