using UnityEngine;
using UnityEngine.UI;

public class HxMainManager : MonoBehaviour
{
    public static HxMainManager instance;
    public GameObject loadAni;

    public Text progressText;
    private void Awake()
    {
        instance = this;
    }
}
