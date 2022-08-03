using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildContentListConfirm : MonoBehaviour
{
    GridLayoutGroup Grid;
    public RectTransform ScrollViewRect;
    
    private void Awake()
    {
        float width = Screen.width;
        float height = Screen.height;

        ScrollViewRect.offsetMax = new Vector2(ScrollViewRect.offsetMax.x, ScrollViewRect.offsetMax.y / 1920f * height);

        Grid = GetComponent<GridLayoutGroup>();
        float x = width - Grid.padding.left - Grid.padding.right;
        Grid.cellSize = new Vector2(x, 0.49f * x);
        MyMessageData.parentCellSize = Grid.cellSize;
       
    }
}
