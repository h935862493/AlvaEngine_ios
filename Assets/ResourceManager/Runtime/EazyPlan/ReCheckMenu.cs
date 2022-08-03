using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ReCheckMenu : MonoBehaviour
{
    public void Return()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
