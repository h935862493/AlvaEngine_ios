using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInUI : EffectsBase
{
    //[Header("Ê±¼ä")]
    public float time = 1f;

    private float currentSpeed;
    private bool IsExecute;
    private float timeber;
    private Color originColor;
    private Color originTmpColor;

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            currentSpeed = 1 / time * Time.deltaTime;
            this.GetComponent<Image>().color += new Color(0, 0, 0, currentSpeed);
            if (timeber > time)
            {
                StopExecute();
                timeber = 0f;
                EndEvent?.Invoke();
            }
        }
    }

    public void Execute()
    {
        if (!this.GetComponent<FadeInUI>().enabled)
            return;
        if (IsExecute)
            return;
        gameObject.SetActive(true);
        IsExecute = true;
        originColor = this.GetComponent<Image>().color;
        this.GetComponent<Image>().color = new Color(
            this.GetComponent<Image>().color.r,
            this.GetComponent<Image>().color.g, 
            this.GetComponent<Image>().color.b,
            0f);
    }

    public void StopExecute()
    {
        if (!this.GetComponent<FadeInUI>().enabled)
            return;
        IsExecute = false;
    }

    public void Recover()
    {
        if (!this.GetComponent<FadeInUI>().enabled)
            return;
        this.GetComponent<Image>().color = originColor;
    }
}
