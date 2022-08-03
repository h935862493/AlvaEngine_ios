using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinearMotion2 : SportEffectsBase
{
    public AxisUI axis = AxisUI.x;
    public bool direction = true;
    public float distance = 100f;
    public float time = 1f;
    public bool circulate = false;
    public bool IsRecover = false;

    private Vector2 m_position;
    private bool IsExecute = false;
    private float Treshold = 0f;
    private float PosX, PosY;

    
    void Update()
    {
        float speed = distance / time;
        if (IsExecute)
        {
            Treshold += speed * Time.deltaTime;
            if (direction)
                switch (axis)
                {
                    case AxisUI.x:
                        if(this.transform.parent.name == "Space(Clone)")
                        {
                            this.GetComponent<RectTransform>().localPosition += new Vector3(speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.x, 0, 0);
                        }
                        else
                        {
                            this.GetComponent<RectTransform>().localPosition += new Vector3(speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.x, 0, 0);
                        }
                        break;
                    case AxisUI.y:
                        if (this.transform.parent.name == "Space(Clone)")
                        {
                            this.GetComponent<RectTransform>().localPosition += new Vector3(0, speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.y, 0);
                        }
                        else
                        {
                            this.GetComponent<RectTransform>().localPosition += new Vector3(0, speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.y, 0);
                        }
                        break;
                    default:
                        break;
                }
            else
                switch (axis)
                {
                    case AxisUI.x:
                        if (this.transform.parent.name == "Space(Clone)")
                        {
                            this.GetComponent<RectTransform>().localPosition -= new Vector3(speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.x, 0, 0);
                        }
                        else
                        {
                            this.GetComponent<RectTransform>().localPosition -= new Vector3(speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.x, 0, 0);
                        }
                        break;
                    case AxisUI.y:
                        if (this.transform.parent.name == "Space(Clone)")
                        {
                            this.GetComponent<RectTransform>().localPosition -= new Vector3(0, speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.y, 0);
                        }
                        else
                        {
                            this.GetComponent<RectTransform>().localPosition -= new Vector3(0, speed * Time.deltaTime * this.GetComponent<RectTransform>().localScale.y, 0);
                        }
                        break;
                    default:
                        break;
                }

            if (Treshold >= distance)
            {
                StopExecute();
                if (circulate)
                {
                    Recover();
                    IsExecute = true;
                }
            }
        }
    }

    public override void Execute()
    {
        if (!this.GetComponent<LinearMotion2>().enabled)
            return;
        if (IsExecute)
            return;
        PosX = this.GetComponent<RectTransform>().localPosition.x;
        PosY = this.GetComponent<RectTransform>().localPosition.y;
        m_position = new Vector2(PosX, PosY);
        IsExecute = true;
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<LinearMotion2>().enabled)
            return;
        IsExecute = false;
        Treshold = 0f;
        if (IsRecover)
        {
            Recover();
        }
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<LinearMotion2>().enabled)
            return;
        this.GetComponent<RectTransform>().anchoredPosition = m_position; 
    }
}
