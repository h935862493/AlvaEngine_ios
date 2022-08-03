using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CustomButtonPanel : MonoBehaviour
{
    public Button btn_last;
    public Button btn_next;

    public Transform tf_parent;

    private void Start()
    {
        btn_next.onClick.AddListener(() =>
        {
            index++;
            if (index >= tf_parent.childCount)
                index = 0;
            ShowImage();
        });

        btn_last.onClick.AddListener(() =>
        {
            index--;
            if (index < 0)
                index = tf_parent.childCount - 1;
            ShowImage();
        });

        InitImages();
        ShowImage();
    }

    void InitImages()
    {
        foreach (Transform item in tf_parent)
        {
            item.gameObject.SetActive(false);
        }
    }

    int index = 0;

    void ShowImage()
    {
        InitImages();
        tf_parent.GetChild(index).gameObject.SetActive(true);
    }
}
