using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanel : MonoBehaviour
{
    public GameObject Hengping;
    public GameObject Shuping;

    public GameObject CustomPanel;


    private void Awake()
    {
        //加载场景资源
        if (System.IO.File.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/" + "Library/SceneInfo.json"))
        {
            string jsonSceneInfo = System.IO.File.ReadAllText(GlobalData.LocalPath + GlobalData.ProjectID + "/" + "Library/SceneInfo.json");
            SceneInfo sceneInfo = GlobalData.DeserializeObject<SceneInfo>(jsonSceneInfo);
            Debug.Log(sceneInfo.resolutionRatioHeight + "  ");
            if (sceneInfo.resolutionRatioHeight > sceneInfo.resolutionRatioWidth)
            {
                //Screen.orientation = ScreenOrientation.Portrait;
                Shuping.SetActive(true);
                CustomPanel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                //Screen.orientation = ScreenOrientation.LandscapeLeft;
                Hengping.SetActive(true);
            }
        }
        else
        {
            Shuping.SetActive(true);
        }
    }
}
