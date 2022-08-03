using UnityEngine;
using UnityEngine.UI;

public class FDJUIControl : MonoBehaviour
{
    CanvasScaler scler;
   
    private void Start()
    {
        scler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {
        if (Screen.orientation != ScreenOrientation.Landscape)
        {
            scler.matchWidthOrHeight = 0.5f;
        }
        else
            scler.matchWidthOrHeight = 1;
    }
}
