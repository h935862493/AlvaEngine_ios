using UnityEngine;
using UnityEngine.UI;

public class TablePanel : MonoBehaviour
{
    private Button TableButton;
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        TableButton = transform.Find("TableButton").GetComponent<Button>();
        rect = transform.Find("TableBg").GetComponent<RectTransform>();

        //if (GlobalData.ProjectGameObjects.Count > 0)
        //{
        //    for (int i = 0; i < GlobalData.ProjectGameObjects.Count; i++)
        //    {
        //        if (GlobalData.ProjectGameObjects[i].ItemInfo.serialId == GlobalData.ProjectID)
        //        {
        //            continue;
        //        }
        //        GameObject obj = Instantiate(Resources.Load("UI/TableItem") as GameObject);
        //        obj.transform.SetParent(rect);
        //        TableObjectItem item = obj.AddComponent<TableObjectItem>();
        //        item.OnSetData(GlobalData.ProjectGameObjects[i].ItemInfo, GlobalData.ProjectGameObjects[i].IsLocal);
        //    }           
        //}

        TableButton.onClick.AddListener(OnTableButtonClick);
    }

    private void OnTableButtonClick()
    {
        if (!rect.gameObject.activeSelf)
        {
            rect.gameObject.SetActive(true);
            TableButton.GetComponent<Image>().sprite = SpriteManager.Instance.折叠Table;
        }
        else
        {
            rect.gameObject.SetActive(false);
            TableButton.GetComponent<Image>().sprite = SpriteManager.Instance.展开Table;
        }
    }
}
