using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisionFileInfo : MonoBehaviour
{
    public string md5;
    public string type;
    public string name;
    public string snID;

    private Toggle toggle;

    private void Start()
    {
        toggle = transform.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            //Debug.Log(type + "         " + name);
            if (type.Equals(".glb") || type.Equals(".gltf"))
            {
                GlobalData.FileSelectAction?.Invoke(snID, name);
            }
        });
    }
}
