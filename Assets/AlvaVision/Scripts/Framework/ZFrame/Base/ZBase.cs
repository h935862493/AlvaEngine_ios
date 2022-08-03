using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZBase : ZBaseExtension
{
    public abstract void OnInstance();
    public abstract void OnInitComp();
    public abstract void OnInitData();
    public abstract void OnInitFunc();
}
