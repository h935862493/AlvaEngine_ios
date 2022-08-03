using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_MenuSelection : MonoBehaviour
{
    public List<btn_Show_go> btn_goList;


    void Start()
    {

        foreach (var item in btn_goList)
        {
            item.btn.onClick.AddListener(() =>
            {
                SetPlaneHide();
                item.go.SetActive(true);
            });
        }


        SetPlaneHide();
        btn_goList[0].btn.onClick.Invoke();
        btn_goList[0].btn.Select();

    }

    void SetPlaneHide()
    {
        foreach (btn_Show_go item in btn_goList)
        {
            item.go.SetActive(false);
        }
    }
}

[Serializable]
public class btn_Show_go
{
    public Button btn;
    public GameObject go;
}

