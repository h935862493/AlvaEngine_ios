using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ÇÐÈëÐ§¹û
/// </summary>

public class CutOut2 : EffectsBase
{
    public Direct direction = Direct.down;
    public float time = 1f;

    private bool IsExecute;
    private float currentSpeed;
    private float currentSpeed2;
    private Vector3 startPosition;
    private float timeber;

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            this.transform.GetComponent<Image>().fillAmount -= 1 / time * Time.deltaTime;
            switch (direction)
            {
                case Direct.down:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(0, currentSpeed2,0);
                    }
                    else
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y ;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(0, currentSpeed2,0);
                    }
                    break;
                case Direct.up:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(0, currentSpeed2,0);
                    }
                    else
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.y / time * Time.deltaTime  * this.transform.GetComponent<RectTransform>().localScale.y;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(0, currentSpeed2,0);
                    }
                    break;
                case Direct.left:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(currentSpeed2, 0,0);
                    }
                    else
                    { 
                         currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x/ time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition -= new Vector3(currentSpeed2, 0,0);
                    }
                    break;
                case Direct.right:
                    if (this.transform.parent.name == "Space(Clone)")
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(currentSpeed2, 0,0);
                    }
                    else
                    {
                        currentSpeed2 = this.transform.GetComponent<RectTransform>().sizeDelta.x / time * Time.deltaTime * this.transform.GetComponent<RectTransform>().localScale.x;
                        this.GetComponent<RectTransform>().localPosition += new Vector3(currentSpeed2, 0,0);
                    }
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            if (timeber >= time)
            {
                StopExecute();
                Recover();
                timeber = 0;
                EndEvent?.Invoke();
            }
        }
    }

    public void Execute()
    {
        if (!this.GetComponent<CutOut2>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        timeber = 0;
        this.transform.GetComponent<Image>().fillAmount = 1f;
        startPosition = this.GetComponent<RectTransform>().localPosition;
        switch (direction)
        {
            case Direct.down:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Top;
                break;
            case Direct.up:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Bottom;
                break;
            case Direct.left:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Right;
                break;
            case Direct.right:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Left;
                break;
            default:
                Debug.Log("error");
                break;
        }
    }

    public void StopExecute()
    {
        if (!this.GetComponent<CutOut2>().enabled)
            return;
        IsExecute = false;
        gameObject.SetActive(false);
    }

    public void Recover()
    {
        if (!this.GetComponent<CutOut2>().enabled)
            return;
        this.transform.GetComponent<Image>().fillAmount = 1f;
        this.GetComponent<RectTransform>().localPosition = startPosition;
    }
}
