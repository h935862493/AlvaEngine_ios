using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentListConfirm : MonoBehaviour
{
    private GridLayoutGroup Grid;

    // Start is called before the first frame update
    void Awake()
    {
        Grid = GetComponent<GridLayoutGroup>();
        float width = Screen.width;
        float height = Screen.height;
        Grid.cellSize = new Vector2(width, height);
        RectTransform rect = Grid.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width * 4, rect.sizeDelta.y);
    }
}
