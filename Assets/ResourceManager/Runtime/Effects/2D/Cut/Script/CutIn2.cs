using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 2D切入效果
/// </summary>

public class CutIn2 : EffectsBase
{
    //[Header("方向")]
    public Direct direction = Direct.down;
    //[Header("时间")]
    public float time = 1f;

    private bool IsExecute;
    private float currentSpeed;
    private float currentSpeed2;
    private Vector2 startPosition;
    private float timeber;

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            this.transform.GetComponent<Image>().fillAmount += 1 / time * Time.deltaTime;
            switch (direction)
            {
                case Direct.up:
                    if(this.transform.parent.name == "Space(Clone)")    
                    {
                        currentSpeed = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(0, currentSpeed,0);
                    }
                    else
                    {
                        currentSpeed = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(0, currentSpeed, 0);
                    }
                    break;
                case Direct.down:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(0, currentSpeed,0);
                    }
                    else
                    {
                        currentSpeed = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(0, currentSpeed, 0);
                    }
                    break;
                case Direct.left:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(currentSpeed2,0,0);
                    }
                    else
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(currentSpeed2, 0, 0);
                    }
                    break;
                case Direct.right:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(currentSpeed2,0,0);
                    }
                    else
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(currentSpeed2, 0, 0);
                    }
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            if (timeber >= time)
            {
                StopExecute();
                EndEvent?.Invoke();
            }
        }
    }

    public void Execute()
    {
        if (!this.GetComponent<CutIn2>().enabled)
            return;
        if (IsExecute)
            return;
        timeber = 0;
        gameObject.SetActive(true);
        IsExecute = true;
        this.transform.GetComponent<Image>().fillAmount = 0f;
        startPosition = this.GetComponent<RectTransform>().localPosition;
        switch (direction)
        {
            case Direct.up:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Top;
                if (this.transform.parent.name == "Space(Clone)")
                {
                    this.GetComponent<RectTransform>().localPosition -= new Vector3(0, this.transform.GetComponent<RectTransform>().sizeDelta.y * this.transform.GetComponent<RectTransform>().localScale.y, 0);
                }
                else
                {
                    this.GetComponent<RectTransform>().localPosition -= new Vector3(0, this.transform.GetComponent<RectTransform>().sizeDelta.y * this.transform.GetComponent<RectTransform>().localScale.y, 0);
                }
                break;
            case Direct.down:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Bottom;
                if (this.transform.parent.name == "Space(Clone)")
                {
                    this.GetComponent<RectTransform>().localPosition += new Vector3(0, this.transform.GetComponent<RectTransform>().sizeDelta.y * this.transform.GetComponent<RectTransform>().localScale.y, 0);
                }
                else
                {
                    this.GetComponent<RectTransform>().localPosition += new Vector3(0, this.transform.GetComponent<RectTransform>().sizeDelta.y * this.transform.GetComponent<RectTransform>().localScale.y, 0);
                }
                break;
            case Direct.left:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Left;
                if (this.transform.parent.name == "Space(Clone)")
                {
                    this.GetComponent<RectTransform>().localPosition += new Vector3( this.transform.GetComponent<RectTransform>().sizeDelta.x * this.transform.GetComponent<RectTransform>().localScale.x, 0,0);
                }
                else
                {
                    this.GetComponent<RectTransform>().localPosition += new Vector3(this.transform.GetComponent<RectTransform>().sizeDelta.x * this.transform.GetComponent<RectTransform>().localScale.x, 0, 0);
                }
                break;
            case Direct.right:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Right;
                if (this.transform.parent.name == "Space(Clone)")
                {
                    this.GetComponent<RectTransform>().localPosition -= new Vector3( this.transform.GetComponent<RectTransform>().sizeDelta.x * this.transform.GetComponent<RectTransform>().localScale.x, 0,0);
                }
                else
                {
                    this.GetComponent<RectTransform>().localPosition -= new Vector3(this.transform.GetComponent<RectTransform>().sizeDelta.x * this.transform.GetComponent<RectTransform>().localScale.x, 0, 0);
                }
                break;
            default:
                Debug.Log("error");
                break;
        }
    }

    public void StopExecute()
    {
        if (!this.GetComponent<CutIn2>().enabled)
            return;
        IsExecute = false;
        timeber = 0;
    }

    public void Recover()
    {
        if (!this.GetComponent<CutIn2>().enabled)
            return;
        this.transform.GetComponent<Image>().fillAmount = 0f;
        this.GetComponent<RectTransform>().anchoredPosition = startPosition;
    }
}
