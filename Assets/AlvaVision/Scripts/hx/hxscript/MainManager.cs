using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public void GO()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        LoadingScreen.SceneToLoad = "SampleScene";
         UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
}
