using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHighShaderManager : MonoBehaviour
{
    Vector3 scale, position, rotation;

    Material[] mats;

    private void Start()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            mats = GetComponent<MeshRenderer>().materials;
        }
        Invoke("SetInitData", 1f);
        GlobalData.IsModelAniPlayAction += OnAniPlay;
    }

    void SetInitData()
    {
        scale = transform.lossyScale;
        position = transform.position;
        rotation = transform.eulerAngles;
    }

    bool isPlay = false;
    bool isBegin = false;
    private void OnAniPlay(bool isPlay)
    {
        this.isPlay = isPlay;
        isBegin = true;
    }

    private void Update()
    {
        if (!isBegin)
        {
            return;
        }
        if (!isPlay)
        {
            OffHighLight();
        }
        if (isPlay)
        {
            if (Vector3.Distance(transform.lossyScale, scale) > 0)
            {
                scale = transform.lossyScale;
                //高亮
                OpenHighLight();
            }
            if (Vector3.Distance(transform.position, position) > 0)
            {
                position = transform.position;
                //高亮
                OpenHighLight();
            }
            if (Vector3.Distance(transform.eulerAngles, rotation) > 0)
            {
                rotation = transform.eulerAngles;
                //高亮
                OpenHighLight();
            }
        }
    }

    private void OpenHighLight()
    {
        isBegin = false;
        //Debug.Log("打开高亮" + gameObject.name);
        //高亮
        if (mats != null && mats.Length > 0)
        {
            Material[] itemMat = new Material[mats.Length];
            for (int i = 0; i < itemMat.Length; i++)
            {
                itemMat[i] = new Material(Shader.Find("Kaima/Depth/ForceField"));
            }
            GetComponent<MeshRenderer>().materials = itemMat;
        }
    }

    private void OffHighLight()
    {
        isBegin = false;
        //Debug.Log("关闭高亮");
        if (mats != null && mats.Length > 0)
        {
            GetComponent<MeshRenderer>().materials = mats;
        }
    }

    private void OnDestroy()
    {
        GlobalData.IsModelAniPlayAction -= OnAniPlay;
    }
}
