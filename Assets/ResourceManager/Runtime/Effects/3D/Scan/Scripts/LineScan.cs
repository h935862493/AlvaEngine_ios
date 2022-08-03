using UnityEngine;

public class LineScan : MaterialEffectsBase
{
    public Texture texture;
    public Color color = new Color(1.0f, 0.498f, 0.0f, 1.0f);
    public float time = 3f;
    [HideInInspector]
    public Axis axis;
    public float width = 0.1f;  
    [HideInInspector]
    public bool loop = false;
    [HideInInspector]
    public bool IsRecover = true;
    [HideInInspector]
    private float Treshold;
    private float _edge;
    //private float speed;
    /// <summary>
    /// 执行函数
    /// </summary>
    public override void Execute()
    {
        _edge = width;
        if (!this.GetComponent<LineScan>().enabled)
            return;
        float min_z = 0.5f;
        //float _edge = 0.08f;
        Bounds bounds = CalculateBounds(gameObject);
        switch (axis)
        {
            case Axis.x:
                min_z = bounds.size.x / 2.0f;
                _edge = _edge / bounds.size.x;  
                break;
            case Axis.y:
                min_z = bounds.size.y / 2.0f;
                _edge = _edge / bounds.size.y;
                break;
            case Axis.z:
                min_z = bounds.size.z / 2.0f;
                _edge = _edge / bounds.size.z;
                break;
        }
        base.Execute();
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].SetFloat("_Treshold", -0.1f);
                renderer.materials[i].SetFloat("_DissolveTreshold", 0.0f);
                renderer.materials[i].SetFloat("_MinZ", min_z);
                renderer.materials[i].SetFloat("_MaxZ", min_z);
                renderer.materials[i].SetFloat("_Edge", _edge);

                renderer.materials[i].SetFloat("_UseDissolveEffect", 1);
                renderer.materials[i].SetFloat("_UseDissolve", 0);
                renderer.materials[i].SetFloat("_AxisChoose", (float)axis);
                
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


    void Update()
    {
        if (IsExecute)
        {
            Treshold += Time.deltaTime / time;
            Treshold = Mathf.Clamp(Treshold, -0.2f, 1.2f);

            if (Treshold >= 1.2f)
            {
                Treshold = -0.2f;
                IsOne = false;
                if (!loop)
                {
                    IsExecute = false;
                    if (IsRecover)
                    {
                        Recover();
                        EndEvent?.Invoke();
                    }
                }
            }
            foreach (Material material in materials)
            {
                material.SetFloat("_Threshold", Treshold);
            }
        }
    }

    /// <summary>
    /// 计算模型边界
    /// </summary>
    /// <param name="model">目标</param>
    /// <param name="minBoundsSize"></param>
    /// <returns></returns>
    private Bounds CalculateBounds(GameObject model, float minBoundsSize = 0.1f)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        Vector3 scale = model.transform.localScale;
        model.transform.localScale = Vector3.one;

        if (renderers.Length == 0)
        {
            return new Bounds(model.transform.position, Vector2.one * minBoundsSize);
        }
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        model.transform.localScale = scale;
        return bounds;
    }
}
