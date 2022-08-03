using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 切入效果
/// </summary>

public class WipeShowUI : EffectsBase
{
    //[Header("方向")]
    public Direct direction = Direct.down;
    //[Header("时间")]
    public float time = 1f;

    private bool IsExecute;
    private float currentSpeed;
    private Vector2 startPosition;
    private float timeber;

    private void Awake()
    {
    }

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            currentSpeed = 1 / time * Time.deltaTime;
            this.transform.GetComponent<Image>().fillAmount += currentSpeed;
            if (timeber >= time)
            {
                StopExecute();
                EndEvent?.Invoke();
            }
        }
    }

    public void Execute()
    {
        if (!this.GetComponent<WipeShowUI>().enabled)
            return;
        if (IsExecute)
            return;
        startPosition = this.GetComponent<RectTransform>().anchoredPosition;
        gameObject.SetActive(true);
        IsExecute = true;
        Recover();
        this.transform.GetComponent<Image>().fillAmount = 0f;
        //设置切入效果的初始值
        switch (direction)
        {
            case Direct.up:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Bottom;
                break;
            case Direct.down:
                this.transform.GetComponent<Image>().type = Image.Type.Filled;
                this.transform.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
                this.transform.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Top;
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
        if (!this.GetComponent<WipeShowUI>().enabled)
            return;
        IsExecute = false;
        timeber = 0;
        //EndEvent.Invoke();
    }

    public void Recover()
    {
        if (!this.GetComponent<WipeShowUI>().enabled)
            return;
        timeber = 0;
        this.transform.GetComponent<Image>().fillAmount = 0f;
        RecoverEvent.Invoke();
    }
}
