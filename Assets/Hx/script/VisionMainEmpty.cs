using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class VisionMainEmpty : MonoBehaviour
{
    public GameObject  EventSystemGo;
    public GameObject[] templateUIss;
    void Start()
    {
        if (!FindObjectOfType<EventSystem>())
        {
            EventSystemGo.SetActive(true);
        }

        templateUIss = GameObject.FindGameObjectsWithTag("Template");
        GameObject templateUI = null;
        //print("///templateUIss.Length:" + templateUIss.Length);
        foreach (var item in templateUIss)
        {
            if (item.name == "Template(Clone)")
            {
                templateUI = item;
                break;
            }
        }
        if (templateUI)
        {
            //print("É¾³ý:" + templateUI.gameObject.name);
            Destroy(templateUI);
        }
    }
    public void Return_btn()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("MainTemp");
    }

    public void Rest_btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Btn_Menu(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }
}
