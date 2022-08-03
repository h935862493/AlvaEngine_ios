using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSizeConfirm : MonoBehaviour
{

    void Start()
    {
        float ratioX, ratioY;
        if (1080f > Screen.width)
        {
            ratioX = Screen.width / 1080f;
        }
        else
        {
            ratioX = 1080f / Screen.width;
        }
        if (1920f > Screen.height)
        {
            ratioY = Screen.height / 1920f;
        }
        else
        {
            ratioY = 1920f / Screen.height;
        }
        foreach (RectTransform item in transform)
        {
            item.sizeDelta = new Vector2(item.sizeDelta.x * ratioX, item.sizeDelta.y * ratioX);
            item.anchoredPosition = new Vector2(item.anchoredPosition.x, item.anchoredPosition.y * ratioY);
        }

    }

}
