using UnityEngine;
using UnityEngine.UI;

public class MainConfirm : MonoBehaviour
{
    float width = 0, height = 0;
    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        

        CanvasScaler scler = GetComponent<CanvasScaler>();
        scler.referenceResolution = new Vector2(width, height);      
    }
}
