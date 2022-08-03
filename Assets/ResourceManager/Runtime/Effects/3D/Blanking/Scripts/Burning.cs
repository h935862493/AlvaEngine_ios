using UnityEngine;
public class Burning : MaterialEffectsBase
{
    public Texture texture;
    public Color color = new Color(1.0f, 0.498f, 0.0f, 1.0f);
    [HideInInspector]
    public bool IsRecover = true;
    public float time = 3f;
    [HideInInspector]
    public bool RunBack = true;
    //private float speed = 0.5f;
    private float DissolveTreshold = 1;

    /// <summary>
    /// 执行函数
    /// </summary>
    public override void Execute()
    {
        if (!this.GetComponent<Burning>().enabled)
            return;
        base.Execute();
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].SetFloat("_DissolveEdge", 0.3f);
                renderer.materials[i].SetFloat("_Treshold", -0.1f);
                renderer.materials[i].SetFloat("_DissolveTreshold", 0.0f);
                renderer.materials[i].SetFloat("_MinZ", 0.5f);
                renderer.materials[i].SetFloat("_MaxZ", 0.5f);
                renderer.materials[i].SetFloat("_UseDissolveEffect", 1);
                renderer.materials[i].SetFloat("_UseDissolve", 1);
                if (texture != null)
                {
                    renderer.materials[i].SetFloat("_usetexture", 1);
                    renderer.materials[i].SetTexture("_maintex", texture);
                }
                else
                {
                    renderer.materials[i].SetColor("_Albedo", color);
                }
                //renderer.materials[i].SetMatrix("SelfMatrix", transform.localToWorldMatrix);
                renderer.materials[i].SetVector("_CaptureLocation", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
            }
        }
        /*if (renderers.Length == 0)
        {
            foreach (var renderer in skinedRenderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].SetFloat("_DissolveEdge", 0.3f);
                    renderer.materials[i].SetFloat("_Treshold", -0.1f);
                    renderer.materials[i].SetFloat("_DissolveTreshold", 0.0f);
                    renderer.materials[i].SetFloat("_MinZ", 0.5f);
                    renderer.materials[i].SetFloat("_MaxZ", 0.5f);
                    renderer.materials[i].SetFloat("_UseDissolveEffect", 1);
                    renderer.materials[i].SetFloat("_UseDissolve", 1);
                    if (texture != null)
                    {
                        renderer.materials[i].SetFloat("_usetexture", 1);
                        renderer.materials[i].SetTexture("_maintex", texture);
                    }
                    else
                    {
                        renderer.materials[i].SetColor("_Albedo", color);
                    }
                    //renderer.materials[i].SetMatrix("SelfMatrix", transform.localToWorldMatrix);
                    renderer.materials[i].SetVector("_CaptureLocation", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
                }
            }
        }
        else
        {
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].SetFloat("_DissolveEdge", 0.3f);
                    renderer.materials[i].SetFloat("_Treshold", -0.1f);
                    renderer.materials[i].SetFloat("_DissolveTreshold", 0.0f);
                    renderer.materials[i].SetFloat("_MinZ", 0.5f);
                    renderer.materials[i].SetFloat("_MaxZ", 0.5f);
                    renderer.materials[i].SetFloat("_UseDissolveEffect", 1);
                    renderer.materials[i].SetFloat("_UseDissolve", 1);
                    if (texture != null)
                    {
                        renderer.materials[i].SetFloat("_usetexture", 1);
                        renderer.materials[i].SetTexture("_maintex", texture);
                    }
                    else
                    {
                        renderer.materials[i].SetColor("_Albedo", color);
                    }
                    //renderer.materials[i].SetMatrix("SelfMatrix", transform.localToWorldMatrix);
                    renderer.materials[i].SetVector("_CaptureLocation", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
                }
            }
        }*/
        if (RunBack)
            DissolveTreshold = 1;
        else
            DissolveTreshold = 0;
    }
    /// <summary>
    /// 运行
    /// </summary>
    private void Update()
    {
        if (IsExecute)
        {
            if (RunBack)
            {
                DissolveTreshold -= Time.deltaTime / time;
                //DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                if (DissolveTreshold <= 0)
                {
                    IsExecute = false;
                    IsOne = false;
                    EndEvent.Invoke();
                    if (IsRecover)
                    {
                        Recover();
                    }
                }
                foreach (Material material in materials)
                {
                    material.SetFloat("_DissolveThreshold", DissolveTreshold);
                }
            }
            else
            {
                DissolveTreshold += Time.deltaTime / time;
                DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                if (DissolveTreshold >= 1)
                {
                    IsExecute = false;
                    IsOne = false;
                    EndEvent.Invoke();
                    if (IsRecover)
                    {
                        Recover();
                    }
                }
                foreach (Material material in materials)
                {
                    material.SetFloat("_DissolveThreshold", DissolveTreshold);
                }
            }

            /*DissolveTreshold += Time.deltaTime / time;
            DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
            if (DissolveTreshold >= 1)
            {
                IsExecute = false;
                IsOne = false;
                EndEvent?.Invoke();
                if (IsRecover)
                {
                    Recover();
                }
            }
            foreach (Material material in materials)
            {
                material.SetFloat("_DissolveThreshold", DissolveTreshold);
            }
*/
            /*if (RunBack)
            {
                DissolveTreshold -= Time.deltaTime / time;
                DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                if (DissolveTreshold <= 0)
                {
                    IsExecute = false;
                    IsOne = false;
                    EndEvent.Invoke();
                    if (IsRecover)
                    {
                        Recover();
                    }
                }
                foreach (Material material in materials)
                {
                    material.SetFloat("_DissolveThreshold", DissolveTreshold);
                }
            }
            else
            {
                DissolveTreshold += Time.deltaTime / time;
                DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                if (DissolveTreshold >= 1)
                {
                    IsExecute = false;
                    IsOne = false;
                    EndEvent.Invoke();
                    if (IsRecover)
                    {
                        Recover();
                    }
                }
                foreach (Material material in materials)
                {
                    material.SetFloat("_DissolveThreshold", DissolveTreshold);
                }
            }*/
        }
    }
}
