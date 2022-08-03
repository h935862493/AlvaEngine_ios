using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARSceneComonUI : MonoBehaviour
{
    public static ARSceneComonUI instance;
    public GameObject TipsBg, Panel_visions_main;
    public Text tip;
    Button SubmitButton, CancelButton;
    public Button ResetButton;

    /// <summary>
    /// 显示识别信息的按钮（专为测试版用）
    /// </summary>
    //public GameObject testInfo;
    private void Awake()
    {
        instance = this;
        var templateUIss = GameObject.FindGameObjectsWithTag("Template");
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
            //print("删除:" + templateUI.gameObject.name);
            Destroy(templateUI);
        }


        SubmitButton = transform.Find("TipsBg/Box/SubmitButton").GetComponent<Button>();
        CancelButton = transform.Find("TipsBg/Box/CancelButton").GetComponent<Button>();

        SubmitButton.onClick.AddListener(Return_btn);
        CancelButton.onClick.AddListener(Return_btn);

        if (ResetButton)
        {
            ResetButton.onClick.AddListener(Rest_btn);
        }

        TipsBg.SetActive(false);

        //if (GlobalData.isTestVersions && testInfo)
        //{
        //    testInfo.SetActive(true);
        //}
    }
    private void Update()
    {
        if (ResetButton && Input.GetMouseButtonDown(0))//Input.touchCount > 0)
        {
            if (EventSystem.current.currentSelectedGameObject && (EventSystem.current.currentSelectedGameObject.name == "Button_menu" || EventSystem.current.currentSelectedGameObject.name == "ResetButton" || EventSystem.current.currentSelectedGameObject.name == "Button_tip"))
                return;
            else
                ResetButton.gameObject.SetActive(false);
        }

    }

    public void Return_btn()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
            PlantformInterface.CloseAlvaBrowserView();
        print("Return_btn()Return_btn()Return_btn()");
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("AREngineMain");
        if (Application.platform != RuntimePlatform.WindowsEditor)
            PlantformInterface.ReturnAppCalBack();
    }

    public void Rest_btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowTip(string info)
    {
        tip.text = info;
        TipsBg.SetActive(true);
    }
    public void Btn_Menu(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }
}
