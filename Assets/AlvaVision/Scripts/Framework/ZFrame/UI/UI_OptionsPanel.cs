using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class UI_OptionsPanel : MonoBehaviour
{
    public Button Btn_Close;
    public Button Btn_Options;

    public List<Transform> ProItemList;

    public List<GameObject> ProjectItemContentList;

    public List<OptionItem> OptionList;

    private void Start()
    {
        Btn_Close.onClick.AddListener(DoMoveUp);
        Btn_Options.onClick.AddListener(DoMoveDown);
        transform.localScale = new Vector3(1, 0, 1);

        foreach (var item in OptionList)
        {
            item.btn.onClick.AddListener(delegate { OnSetItemIndex(item.index); });
        }

        ProItemList = new List<Transform>();

        GlobalData.SetOptionTypeObject += OnSetOptionObjectAction;
        GlobalData.IsResetSceneAction += OnResetSceneAction;
    }

    private void OnSetOptionObjectAction(List<GameObject> objs)
    {

        ProjectItemContentList = objs;
        CheckMissingGameObject(ProItemList);
        CheckMissingGameObject(ProjectItemContentList);
        //for (int j = 0; j < ProItemList.Count; j++)
        //{
        //    if (!ProItemList[j])
        //    {
        //        ProItemList.RemoveAt(j);
        //    }
        //}
        //for (int k = 0; k < ProjectItemContentList.Count; k++)
        //{
        //    if (!ProjectItemContentList[k])
        //    {
        //        ProjectItemContentList.RemoveAt(k);
        //    }
        //}
        if (ProjectItemContentList != null && ProjectItemContentList.Count > 0)
        {
            for (int i = 0; i < ProjectItemContentList.Count; i++)
            {
                if (!ProItemList.Exists(t => t.gameObject == ProjectItemContentList[i]))
                {
                    ProItemList.Add(ProjectItemContentList[i].transform);
                }
            }
        }
        OnSetItemIndex(curentIndex);
    }

    public void CheckMissingGameObject<T>(List<T> gos, int k = 0) where T : UnityEngine.Object
    {
        for (int j = k; j < gos.Count; j++)
        {
            if (gos[j] == null)
            {
                gos.RemoveAt(j);
                CheckMissingGameObject(gos, j);

            }
        }
    }
    public int curentIndex = 0;
    private void OnSetItemIndex(int index)
    {
        curentIndex = index;
        switch (index)
        {
            case 0://全部
                foreach (var item in ProItemList)
                {
                    if (item)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                break;
            case 1://图片识别
                foreach (var item in ProItemList)
                {
                    if (item.GetComponent<ProjectUIItem>().projectType == ProjectType.ImageRecognition)
                    {
                        item.gameObject.SetActive(true);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                break;
            case 2://物体识别
                foreach (var item in ProItemList)
                {
                    if (item.GetComponent<ProjectUIItem>().projectType == ProjectType.ModelRecognition)
                    {
                        item.gameObject.SetActive(true);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                break;
            case 3://地面识别
                foreach (var item in ProItemList)
                {
                    if (item.GetComponent<ProjectUIItem>().projectType == ProjectType.SlamRecognition)
                    {
                        item.gameObject.SetActive(true);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                break;
        }
        DoMoveUp();
    }

    private void DoMoveDown()
    {
        //transform.DOMoveY(Screen.height, 0.3f);

        transform.DOScaleY(1, 0.3f);
    }

    private void DoMoveUp()
    {
        //Debug.Log((transform as RectTransform).sizeDelta.y);
        //transform.DOMoveY(Screen.height, 0.3f);
        //(transform as RectTransform).DOMove(new Vector3(0, Screen.height + (transform as RectTransform).sizeDelta.y,0), 0.3f, false);
        transform.DOScaleY(0, 0.3f);
        //(transform as RectTransform).DOMoveY(Screen.height + (transform as RectTransform).sizeDelta.y, 0.3f);
    }
    private void OnResetSceneAction(bool obj)
    {
        if (!obj)
        {
            return;
        }
        ProjectItemContentList.Clear();
        ProItemList.Clear();
        curentIndex = 0;
    }

    private void OnDestroy()
    {
        GlobalData.SetOptionTypeObject -= OnSetOptionObjectAction;
        GlobalData.IsResetSceneAction -= OnResetSceneAction;
    }
}

[Serializable]
public class OptionItem
{
    public Button btn;
    public int index;
}
