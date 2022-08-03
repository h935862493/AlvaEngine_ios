using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingErrorTips : MonoBehaviour
{
    public GameObject TipsBg, EventSystemGo;
    public Text tip;
    public Button SubmitButton, CancelButton;

    private void Start()
    {
        if (!FindObjectOfType<EventSystem>())
        {
            EventSystemGo.SetActive(true);
        }
        SubmitButton.onClick.AddListener(Return_btn);
        CancelButton.onClick.AddListener(Return_btn);

      
    }
    public void Return_btn()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        PlantformInterface.CloseAlvaBrowserView();
#endif
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("MainTemp");
    }

    public void ShowTip(string info)
    {
        tip.text = info;
        TipsBg.SetActive(true);
    }
}
