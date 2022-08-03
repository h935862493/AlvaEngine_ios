using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempToMain : MonoBehaviour
{
    IEnumerator Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        //QualitySettings.SetQualityLevel(2, true);
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        //print("过度场景---------------3333");
    }

}
