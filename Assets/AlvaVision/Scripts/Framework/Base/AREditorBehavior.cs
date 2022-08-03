using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class AREditorBehavior : MonoBehaviour
{
    private GameObject objectRoot;
    private GameObject uiRoot;
    public static AREditorBehavior instance;

    private void Awake()
    {
        instance = this;
    }
    public GameObject ObjectRoot
    {
        get => objectRoot ? objectRoot : GameObject.Find("ModelObjects");
    }
    public GameObject UiRoot
    {
        get => uiRoot ? uiRoot : GameObject.Find("CustomButtonPanel");
    }
}
