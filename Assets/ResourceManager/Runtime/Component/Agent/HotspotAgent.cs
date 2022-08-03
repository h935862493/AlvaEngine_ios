using Alva.Runtime.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("myName/myProduct/v1/QuickSettings")]
public class HotspotAgent : Agent
{
    [SerializeField]
    private string URL;

    private void Start()
    {
        Hotspot hotspot = GetComponent<Hotspot>();
        if (hotspot != null)
        {
            hotspot.url = URL;
        }
    }

}
