using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGroundPlane : MonoBehaviour
{
    public static FollowGroundPlane instance;
    bool isFirst = true;
    public Transform target;
    private void Awake()
    {
        instance = this;
        //UtilityHelper.EnableRendererColliderCanvas(gameObject, false);
    }

    public void Click()
    {
        if (isFirst)
        {
            isFirst = false;
            transform.position = target.position;
            transform.eulerAngles = target.eulerAngles;
            UtilityHelper.EnableRendererColliderCanvas(gameObject, true);
            //for (int i = 0; i < transform.childCount; i++)
            //{
            //    transform.GetChild(i).gameObject.SetActive(true);
            //}
        }
    }

    public void Back()
    {
        //LoadingScreen.SceneToLoad = "main";
        UnityEngine.SceneManagement.SceneManager.LoadScene("GroundPlane", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void Rest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnTrackingFound()
    {
        UtilityHelper.EnableRendererColliderCanvas(gameObject, true);
    }

    public void OnTrackingLost()
    {
        UtilityHelper.EnableRendererColliderCanvas(gameObject, false);
    }
}
