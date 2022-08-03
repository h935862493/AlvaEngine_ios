using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTopConfirm : MonoBehaviour
{
    private void Start()
    {
        RectTransform rectParent = transform.parent.GetComponent<RectTransform>();
        rectParent.sizeDelta = new Vector2(rectParent.sizeDelta.x, rectParent.sizeDelta.y / 1920f * Screen.height);

        RectTransform rect = GetComponent<RectTransform>();
        //图片比例是360*120 
        rect.sizeDelta = new Vector2(0.935f * Screen.width, 120f * (0.935f * Screen.width / 3 / 360));
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y / 1920f * Screen.height);

        //foreach (RectTransform item in transform)
        //{
        //    //根据画布的大小（即屏幕像素）与UI比例改变
        //    item.sizeDelta = new Vector2(rect.sizeDelta.x/3,item.sizeDelta.y);
        //}
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rectitem = transform.GetChild(i).GetComponent<RectTransform>();
            rectitem.sizeDelta = new Vector2(rect.sizeDelta.x / 4, rectitem.sizeDelta.y);
            rectitem.anchoredPosition = new Vector2(rectitem.sizeDelta.x * i, rectitem.anchoredPosition.y);
        }

    }
}
