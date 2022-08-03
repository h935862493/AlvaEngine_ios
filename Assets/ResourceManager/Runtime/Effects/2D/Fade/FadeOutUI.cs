using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeOutUI : EffectsBase
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
            this.GetComponent<Image>().color -= new Color(0, 0, 0, currentSpeed);
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
        if (!this.GetComponent<FadeOutUI>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        originColor = this.GetComponent<Image>().color;
    }

    public void StopExecute()
    {
        if (!this.GetComponent<FadeOutUI>().enabled)
            return;
        IsExecute = false;
        gameObject.SetActive(false);
        this.GetComponent<Image>().color = originColor;
    }

    public void Recover()
    {
        if (!this.GetComponent<FadeOutUI>().enabled)
            return;
        this.GetComponent<Image>().color = originColor;
    }
}
