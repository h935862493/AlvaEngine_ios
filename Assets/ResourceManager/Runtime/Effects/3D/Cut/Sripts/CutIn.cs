using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
/// <summary>
/// 切入效果
/// </summary>
public class CutIn : MaterialEffectsBase
{
    public SnapAxis axis = SnapAxis.x;
    public bool direction = true;
    public float time = 1f;

    private float currentSpeed;
    private Vector3 startPosition;
    private float speed1;
    private float speed2;
    private float timeber;
    private float moveDistance1;
    private float moveDistance2;
    private float moveDistance3;
    private Vector3 MyTransform;

    private void Awake()
    {
    }

    /// <summary>
    /// 执行函数
    /// </summary>
    public override void Execute()
    {
        if (!this.GetComponent<CutIn>().enabled)
            return;
        if (IsExecute)
            return;
        startPosition = transform.position;
        gameObject.SetActive(true);

        renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            //保存自身材质
            if (IsOne)
            {
                backups_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < backups_materials.Length; i++)
                {
                    backups_materials[i] = Instantiate(renderer.materials[i]);
                }
                materialDictionary.Add(renderer, backups_materials);
            }
            //替换目标材质
            Material[] m_materials = new Material[renderer.materials.Length];
            for (int i = 0; i < m_materials.Length; i++)
            {
                m_materials[i] = material;
                //m_materials[i] = Instantiate(renderer.materials[i]);             
            }
            renderer.materials = m_materials;

            //添加所有材质到集合中
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materials.Add(renderer.materials[i]);
            }
        }
        /*if (renderers.Length == 0)
        {
            skinedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinedRenderers)
            {
                //保存自身材质
                if (IsOne)
                {
                    backups_materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < backups_materials.Length; i++)
                    {
                        backups_materials[i] = Instantiate(renderer.materials[i]);
                    }
                    materialDictionary2.Add(renderer, backups_materials);
                }
                //替换目标材质
                Material[] m_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < m_materials.Length; i++)
                {
                    m_materials[i] = material;
                    //m_materials[i] = Instantiate(renderer.materials[i]);             
                }
                renderer.materials = m_materials;

                //添加所有材质到集合中
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials.Add(renderer.materials[i]);
                }
            }
        }
        else
        {
            foreach (var renderer in renderers)
            {
                //保存自身材质
                if (IsOne)
                {
                    backups_materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < backups_materials.Length; i++)
                    {
                        backups_materials[i] = Instantiate(renderer.materials[i]);
                    }
                    materialDictionary.Add(renderer, backups_materials);
                }
                //替换目标材质
                Material[] m_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < m_materials.Length; i++)
                {
                    m_materials[i] = material;
                    //m_materials[i] = Instantiate(renderer.materials[i]);             
                }
                renderer.materials = m_materials;

                //添加所有材质到集合中
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials.Add(renderer.materials[i]);
                }
            }
        }*/


        Bounds bounds = CalculateBounds(gameObject);
        moveDistance1 = bounds.size.x * transform.localScale.x;
        moveDistance2 = bounds.size.y * transform.localScale.y;
        moveDistance3 = bounds.size.z * transform.localScale.z;
        MyTransform = bounds.center;

        if (direction)
        {
            switch (axis)
            {
                case SnapAxis.x:
                    transform.position = transform.position - new Vector3(moveDistance1, 0, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(-1, 0, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x - moveDistance1 / 2, MyTransform.y, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.y:
                    transform.position = transform.position - new Vector3(0, moveDistance2, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, -1, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y - moveDistance2 / 2, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.z:
                    transform.position = transform.position - new Vector3(0, 0, moveDistance3);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, 0, -1, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y, MyTransform.z - moveDistance3 / 2, 0));
                    }
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
        }
        else
        {
            switch (axis)
            {
                case SnapAxis.x:
                    transform.position = transform.position + new Vector3(moveDistance1, 0, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(1, 0, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x + moveDistance1 / 2, MyTransform.y, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.y:
                    transform.position = transform.position + new Vector3(0, moveDistance2, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, 1, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y + moveDistance2 / 2, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.z:
                    transform.position = transform.position + new Vector3(0, 0, moveDistance3);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, 0, 1, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y, MyTransform.z + moveDistance3 / 2, 0));
                    }
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
        }

        IsOne = false;
        IsExecute = true;
        ExecuteEvent?.Invoke();
    }

    void Update()
    {
        if (IsExecute)
        {
            timeber += Time.deltaTime;
            if (direction)
            {
                switch (axis)
                {
                    case SnapAxis.x:
                        transform.position += new Vector3(moveDistance1 / time * Time.deltaTime, 0, 0);
                        break;
                    case SnapAxis.y:
                        transform.position += new Vector3(0, moveDistance2 / time * Time.deltaTime, 0);
                        break;
                    case SnapAxis.z:
                        transform.position += new Vector3(0, 0,  moveDistance3 / time * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
            }
            else
            {
                switch (axis)
                {
                    case SnapAxis.x:
                        transform.position -= new Vector3(moveDistance1 / time * Time.deltaTime, 0, 0);
                        break;
                    case SnapAxis.y:
                        transform.position -= new Vector3(0, moveDistance2 / time * Time.deltaTime, 0);
                        break;
                    case SnapAxis.z:
                        transform.position -= new Vector3(0, 0, moveDistance3 / time * Time.deltaTime);
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
            }
            if (timeber >= time)
            {
                StopExecute();
            }
        }
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<CutIn>().enabled)
            return;
        IsExecute = false;
        timeber = 0f;
        Recover();
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<CutIn>().enabled)
            return;
        transform.position = startPosition;
        foreach (var item in materialDictionary)
        {
            item.Key.materials = item.Value;
        }
        /*if (materialDictionary.Count == 0)
        {
            foreach (var item in materialDictionary2)
            {
                item.Key.materials = item.Value;
            }
        }
        else
        {
            foreach (var item in materialDictionary)
            {
                item.Key.materials = item.Value;
            }
        }*/
        RecoverEvent?.Invoke();
    }

    /// <summary>
    /// 顶点界限计算
    /// </summary>
    /// <param name="model"></param>
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


