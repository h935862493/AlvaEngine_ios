using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
/// <summary>
/// �г�Ч��
/// </summary>
public class CutOut : MaterialEffectsBase
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
    /// ִ�к���
    /// </summary>
    public override void Execute()
    {
        if (!this.GetComponent<CutOut>().enabled)
            return;
        if (IsExecute)
            return;
        startPosition = transform.localPosition;
        gameObject.SetActive(true);
        //Recover();
        renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            //�����������
            if (IsOne)
            {
                backups_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < backups_materials.Length; i++)
                {
                    backups_materials[i] = Instantiate(renderer.materials[i]);
                }
                materialDictionary.Add(renderer, backups_materials);
            }
            //�滻Ŀ�����
            Material[] m_materials = new Material[renderer.materials.Length];
            for (int i = 0; i < m_materials.Length; i++)
            {
                m_materials[i] = material;
            }
            renderer.materials = m_materials;

            //������в��ʵ�������
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materials.Add(renderer.materials[i]);
            }
        }

        Bounds bounds = CalculateBounds(gameObject);
        moveDistance1 = bounds.size.x * transform.localScale.x;
        moveDistance2 = bounds.size.y * transform.localScale.y;
        moveDistance3 = bounds.size.z * transform.localScale.z;
        //moveDistance1 = bounds.size.x;
        //moveDistance2 = bounds.size.y;
        //moveDistance3 = bounds.size.z;
        MyTransform = bounds.center;

        //Debug.Log(bounds.size);
        if (direction)
        {
            switch (axis)
            {
                case SnapAxis.x:
                    //transform.position = transform.position - new Vector3(moveDistance1, 0, 0);
                    //Debug.Log(transform.position.x);
                    //Debug.Log(MyTransform.x);
                    //Debug.Log(moveDistance1);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(1, 0, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x + moveDistance1 / 2, MyTransform.y, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.y:
                    //transform.position = transform.position - new Vector3(0, moveDistance1, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, 1, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y + moveDistance2 / 2, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.z:
                    //transform.position = transform.position - new Vector3(0, 0, moveDistance1);
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
        else
        {
            switch (axis)
            {
                case SnapAxis.x:
                    //transform.position = transform.position + new Vector3(moveDistance1, 0, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(-1, 0, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x - moveDistance1 / 2, MyTransform.y, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.y:
                    //transform.position = transform.position + new Vector3(0, moveDistance1, 0);
                    foreach (Material material in materials)
                    {
                        material.SetVector("_PlaneNormal", new Vector4(0, -1, 0, 0));
                        material.SetVector("_PlanePosition", new Vector4(MyTransform.x, MyTransform.y - moveDistance2 / 2, MyTransform.z, 0));
                    }
                    break;
                case SnapAxis.z:
                    //transform.position = transform.position + new Vector3(0, 0, moveDistance1);
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
                        transform.position += new Vector3(0, 0, moveDistance3 / time * Time.deltaTime);
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
                //RecoverEvent?.Invoke();
            }
        }
    }

    public override void StopExecute()
    {
        if (!this.GetComponent<CutOut>().enabled)
            return;
        IsExecute = false;
        timeber = 0f;
        Recover();
        gameObject.SetActive(false);
        EndEvent?.Invoke();
    }

    public override void Recover()
    {
        if (!this.GetComponent<CutOut>().enabled)
            return;
        transform.localPosition = startPosition;
        IsExecute = false;
        foreach (var item in materialDictionary)
        {
            item.Key.materials = item.Value;
        }
    }

    /// <summary>
    /// ������޼���
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
            return new Bounds(model.transform.localPosition, Vector2.one * minBoundsSize);
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
