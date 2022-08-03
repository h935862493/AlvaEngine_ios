using System.Collections.Generic;
using UnityEngine;

public class GameObjectContainer : MonoBehaviour
{
    [SerializeField]
    public List<Objects> list = new List<Objects>();
    [System.Serializable]
    public class Objects {
        [SerializeField]
        public List<GameObject> list = new List<GameObject>();
    }

    public void SetActiveFalse(int index)
    {
        if (index >= list.Count)
            return;
        foreach (var item in list[index].list)
        {
            if (null != item)
            {
                item.SetActive(false);
            }
        }
    }
    public void SetActiveTrue(int index)
    {
        if (index >= list.Count)
            return;
        foreach (var item in list[index].list)
        {
            if (null != item)
            {
                item.SetActive(true);
            }
        }
    }
    public void SetActiveAll(bool active)
    {
        foreach (var item in list)
        {
            foreach (var item1 in item.list)
            {
                if (null != item1)
                {
                    item1.SetActive(active);
                }
            }
        }
    }
}
