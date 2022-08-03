using Alva.Runtime.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Components.Manager
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    [RequireComponent(typeof(ToggleGroup))]
    public class GroupManager : MonoBehaviour
    {
        // [Header("所有的选项")]
        [SerializeField]
        public List<Toggle> toggleList = new List<Toggle>();
        [SerializeField]
        public ToggleGroup toggleGroup;
        [SerializeField]
        public List<UnityEvent> optionsChooseEvent = new List<UnityEvent>();
        // [Header("多选或者单选")]
        public bool IsMultChoose = false;     
        //[Header("单选正确答案")]
        //public int answer = 0;
        //[Header("多选正确答案")]
        //public int[] multyAnswer;      
        // Start is called before the first frame update
        void Awake()
        {
            toggleGroup = GetComponent<ToggleGroup>();
            if (IsMultChoose)
            {
                toggleGroup.enabled = false;
            }
            else
            {
                toggleGroup.enabled = true;
            }        
        }
     

        public void RestToggleList()
        {
            toggleList.Clear();
            optionsChooseEvent.Clear();
            if (!toggleGroup)
            {
                toggleGroup = GetComponent<ToggleGroup>();
            }
            CheckBoxAgent[] checkboxArray = transform.parent.GetComponentsInChildren<CheckBoxAgent>();
            for (int i = 0; i < checkboxArray.Length; i++)
            {
                if (checkboxArray[i].toggleGroup == toggleGroup)
                {
                    checkboxArray[i].transform.parent = transform;
                    Toggle toggle = checkboxArray[i].GetComponent<Toggle>();
                    if (!toggleList.Contains(toggle))
                    {
                        toggleList.Add(toggle);
                        optionsChooseEvent.Add(checkboxArray[i].chooseEvent);
                    }
                }
            }     
        }

      

        public void AddItem(Toggle item)
        {
            //RemoveEmptyItem();
            //if (!CheckIFContainItem(item))
            //{
            //    toggleList.Add(item);
            //}
        }

        private void RemoveEmptyItem()
        {
            for (int i = 0; i < toggleList.Count; i++)
            {
                if (toggleList[i] == null || toggleList[i].gameObject == null)
                {
                    toggleList.Remove(toggleList[i]);
                }
            }
        }
        bool CheckIFContainItem(Toggle item)
        {
            return toggleList.Contains(item);
        }
        /// <summary>
        /// 获取选中的项目,针对单选
        /// </summary>
        /// <returns>-1代表没有选中项，如果允许可以不选的话</returns>
        public int GetChooseToggleIndex()
        {
            int res = -1;
            RemoveEmptyItem();
            for (int i = 0; i < toggleList.Count; i++)
            {
                if (toggleList[i].isOn)
                {
                    res = i;
                    return res;
                }
            }
            return res;
        }
        /// <summary>
        /// 获取选中的项目,针对多选
        /// </summary>
        /// <returns>-1代表没有选中项，如果允许可以不选的话</returns>
        public List<int> GetChooseToggleIndexs()
        {
            List<int> res = new List<int>();
            RemoveEmptyItem();
            for (int i = 0; i < toggleList.Count; i++)
            {
                if (toggleList[i].isOn)
                {
                    res.Add(i);
                }
            }
            return res;
        }
    }
}
