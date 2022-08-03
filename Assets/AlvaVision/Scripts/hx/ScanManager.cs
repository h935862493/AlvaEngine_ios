using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScanManager : MonoBehaviour
{
    public static ScanManager instance;
    //当前状态
    public bool cur_state = false;

    //public AnchorBehaviour anchorBehaviour;
    private void Start()
    {
        instance = this;
    }


}
