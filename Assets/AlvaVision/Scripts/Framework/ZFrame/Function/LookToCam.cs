using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCam : MonoBehaviour
{
    GameObject cam;
    private void Start()
    {
        cam = Camera.main.gameObject;
        if (GlobalData.ProjectSettingData.Type == "AreaTarget")
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        //transform.LookAt(new Vector3(cam.transform.position.x, transform.localPosition.y, cam.transform.position.z));
        transform.LookAt(cam.transform);
    }
}
