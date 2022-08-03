
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UGUI/拖拽UI对象在画布(Canvas)内部移动
/// </summary>
public class UIDragMe : MonoBehaviour,IBeginDragHandler ,IDragHandler,IEndDragHandler {
    
    private Vector2 originalLocalPointerPosition;   //记录开始拖拽时鼠标指针的本地坐标
    private Vector3 originalPanelLocalPosition;     //记录开始拖拽时目标对象本地坐标
    private RectTransform panelRectTransform;       //目标实例(RectTransform类型)
    private RectTransform canvasRectTransform;      //画布实例(RectTransform类型)
    // Use this for initialization
    void Start () {
        panelRectTransform = transform as RectTransform;
        //canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)//开始拖拽时发生事件
    {
        originalPanelLocalPosition = panelRectTransform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
    }
    public void OnDrag(PointerEventData eventData)//拖拽中发生事件
    {
        if (panelRectTransform == null || canvasRectTransform == null)
            return;

        Vector2 localPointerPosition;//拖拽时鼠标指针的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;     //鼠标指针相对位移
            panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;   //目标对象位移补偿
        }
        ClampToWindow();
    }
    public void OnEndDrag(PointerEventData eventData)//结束拖拽时发生事件
    {
        Debug.Log("Completed!");
    }
    //限制窗口于画布内部
    public void ClampToWindow()
    {
        Vector2 pos = Vector2.zero, maxPosition = Vector2.zero,minPosition = Vector2.zero;

        minPosition = new Vector2(panelRectTransform.sizeDelta.x / 2, panelRectTransform.sizeDelta.y / 2);
        maxPosition = new Vector2(Screen.width - panelRectTransform.sizeDelta.x/2, Screen.height - panelRectTransform.sizeDelta.y/2);

        //在min与max之间取值给panelRectTransform.localPosition
        pos.x = Mathf.Clamp(panelRectTransform.anchoredPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(panelRectTransform.anchoredPosition.y, minPosition.y, maxPosition.y);

        panelRectTransform.anchoredPosition = pos;//最终赋值
    }
}
