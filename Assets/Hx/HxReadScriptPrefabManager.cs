//using Battlehub.RTCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HxReadScriptPrefabManager : MonoBehaviour
{
    public static HxReadScriptPrefabManager instance;

    private void Start()
    {
        instance = this;
    }

    public void Goo()
    {
        var go = GameObject.Find("Cube");
        AddScript(go, "Script");
    }

    /// <summary>
    /// 添加脚本
    /// </summary>
    /// <param name="gameobjectName"></param>
    /// <param name="scriptName"></param>
    public void AddScript(GameObject go, string scriptName)
    {

        //if (go)
        //{
        //    Type[] editableTypes = IOC.Resolve<Battlehub.RTEditor.IEditorsMap>().GetEditableTypes();
        //    print
        //        ("****************************editableTypes.Length:" + editableTypes.Length);
        //    foreach (var item in editableTypes)
        //    {
        //        if (item.Name == scriptName)
        //        {
        //            print("****************************AddComponent(item):" + item.Name);
        //            go.AddComponent(item);
        //            return;
        //        }
        //    }
        //}
    }
}
