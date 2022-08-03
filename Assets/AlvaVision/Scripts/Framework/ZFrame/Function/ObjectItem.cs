using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectItem : MonoBehaviour
{
    public SourType mType;
    public EditableObjectsDataModel editable;

    public int AniCurrentIndex;
    public List<string> AniNameList;
    public bool isBeginForwAni = false;
}
