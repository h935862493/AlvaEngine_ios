using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����Ч��
/// </summary>
public enum Direct
{
    up,
    down,
    left,
    right
}
public class FadeIn : EffectsBase
{
    //[Header("ʱ��")]
    public float fadetime = 1f;

    private bool IsExecute;
    private float timeber = 0;
    private float total = 0;
    private List<Material> materials = new List<Material>();

    public void Execute()
    {
        if (!this.GetComponent<FadeIn>().enabled)
            return;
        if (IsExecute)
            return;
        IsExecute = true;
        gameObject.SetActive(true);
        MeshRenderer[] meshRenderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers)
        {
            Material[] materals = mr.materials;
            foreach (Material m in materals)
            {
                if (!materials.Contains(m))
                {
                    materials.Add(m);
                }
            }
        }
        for (int i = 0; i < materials.Count; i++)
        {
            Material m = materials[i];
            Color color = m.color;
            m.color = new Color(color.r, color.g, color.b, 0);
            setMaterialRenderingMode(m, RenderingMode.Fade);
        }
    }
    /*//����ģ�͵ĵ���Ч��
    public void HideModel()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material m = materials[i];
            Color color = m.color;
            m.color = new Color(color.r, color.g, color.b, 1);//����һ��Ҫ����������Fadeģʽ�µ�color aֵ Ϊ1 ��Ȼ ����һ����ʾ����һֱ��ʾΪ0
            setMaterialRenderingMode(m, RenderingMode.Fade);
            //m.DOColor(new Color(color.r, color.g, color.b, 0), fadeTime);
            m.color = Color.Lerp(new Color(color.r, color.g, color.b, 1), new Color(color.r, color.g, color.b, 0), fadeTime);
            Debug.Log(m.color);
        }
    }*/

    private void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            total = timeber * 1 / fadetime;
            for (int i = 0; i < materials.Count; i++)
            {
                Material m = materials[i];
                Color color = m.color;
                //m.color = new Color(color.r, color.g, color.b, 1);
                //setMaterialRenderingMode(m, RenderingMode.Opaque);
                m.color = new Color(color.r, color.g, color.b, total);
                //Debug.Log(m.color);
            }
            if (timeber > fadetime)
            {
                StopExecute();
                timeber = 0;
            }
        }
    }
    /*//���������������Ҫ���û��� ��Ȼ���´���ʾʹ�þ���͸��״̬
    public void ShowModel()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material m = materials[i];
            Color color = m.color;
            setMaterialRenderingMode(m, RenderingMode.Opaque);
        }
    }
*/
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }
    //���ò��ʵ���Ⱦģʽ 
    private void setMaterialRenderingMode(Material material, RenderingMode renderingMode)
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
                //material.SetFloat("" _Mode & quot;", 2); 
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



    public void StopExecute()
    {
        if (!this.GetComponent<FadeIn>().enabled)
            return;
        IsExecute = false;
        timeber = 0;
        //Recover();
        EndEvent?.Invoke();
    }

    public void Recover()
    {
        if (!this.GetComponent<FadeIn>().enabled)
            return;
        /*//this.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        foreach (var item in renderers)
        {
            item.material.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        }*/
    }

}
