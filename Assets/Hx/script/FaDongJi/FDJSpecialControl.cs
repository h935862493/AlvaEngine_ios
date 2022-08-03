using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FDJSpecialControl : MonoBehaviour
{
    public GameObject bangzi1, bangzi2, bangzi1_red, bangzi2_red;
    public List<GameObject> HideGos = new List<GameObject>();
    public List<Material> Mts = new List<Material>();

    private void OnEnable()
    {
        TrueMT();
    }
    private void OnDestroy()
    {
        RestMt();
    }

    public void HideBangziGG(bool bo, bool isError = false)
    {
        if (isError)
        {
            bangzi1.SetActive(true);
            bangzi1.GetComponent<MeshRenderer>().enabled = false;
            bangzi2.SetActive(true);
            bangzi2.GetComponent<MeshRenderer>().enabled = false;
            bangzi1_red.SetActive(true);
            bangzi2_red.SetActive(true);
        }
        else
        {
            bangzi1.GetComponent<MeshRenderer>().enabled = true;
            bangzi2.GetComponent<MeshRenderer>().enabled = true;
            bangzi1.SetActive(bo);
            bangzi2.SetActive(bo);
            bangzi1_red.SetActive(false);
            bangzi2_red.SetActive(false);

        }
    }

    //显示发动机真实材质
    public void TrueMT()
    {
        for (int i = 0; i < Mts.Count; i++)
        {
            SetMaterialRenderingMode(Mts[i], RenderingMode.Opaque);
        }

        for (int i = 0; i < HideGos.Count; i++)
        {
            HideGos[i].SetActive(false);
        }
    }

    void RestMt()
    {
        for (int i = 0; i < Mts.Count; i++)
        {
            SetMaterialRenderingMode(Mts[i], RenderingMode.Fade);
        }
    }
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
