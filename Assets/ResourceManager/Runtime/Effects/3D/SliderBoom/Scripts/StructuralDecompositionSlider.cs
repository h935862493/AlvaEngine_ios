using UnityEngine;
public class StructuralDecompositionSlider: MonoBehaviour
{
    Vector3 center;
    Vector3 Size;
    
    MeshRenderer[] renders;
    Vector3[] selfPosition;

    private float sliderArg;
    [HideInInspector]
    public float BeginValue = 0;
    [HideInInspector]
    public float EndValue = 1;
    [HideInInspector]
    GameObject offsetGameObject;
    Vector3 offsetGameObjectStart;
    GameObject offsetQuaGameObject;
    Vector3 originScale;

    private void Awake()
    {
        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        center = bound.center;
        Size = bound.size;
        offsetQuaGameObject = new GameObject();
        offsetQuaGameObject.transform.position = center;
        offsetQuaGameObject.transform.SetParent(transform);
        originScale = offsetQuaGameObject.transform.localScale;
        selfPosition = new Vector3[renders.Length];
        for (int i = 0; i < selfPosition.Length; i++)
        {
            selfPosition[i] = renders[i].transform.position;
        }
        if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>()!= null)
        {
            defaultTrackableEventHandler = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>();
            offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
            offsetGameObjectStart = offsetGameObject.transform.position;
            originScale = offsetGameObject.transform.localScale;
        }
        else
        {
            originScale = offsetQuaGameObject.transform.localScale; //初始Scale
        }
    }

    float time = 0;
    Alva.Runtime.Components.DefaultTrackableEventHandler defaultTrackableEventHandler;
    /// <summary>
    /// 几秒执行完毕
    /// </summary>
    /// <param name="time">秒钟</param>
    public void Execute(float arg)
    {
        if (!this.GetComponent<StructuralDecompositionSlider>().enabled)
            return;
        sliderArg = arg;
        if (defaultTrackableEventHandler != null && defaultTrackableEventHandler.type.Equals("SlamRecognition"))
        {
            if (sliderArg.Equals(0))
            {
                defaultTrackableEventHandler.SetDisableModelScaling(false);
            }
            else
            {
                defaultTrackableEventHandler.SetDisableModelScaling(true);
            }
        }
        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        if (offsetGameObject == null)
        {
            if (center == bound.center) //位置不变
                return;
            if (center != bound.center && Size == bound.size) //大小不变，位置改变
            {
                center = bound.center;
                Size = bound.size;
                offsetQuaGameObject = new GameObject();
                offsetQuaGameObject.transform.position = center;
                offsetQuaGameObject.transform.SetParent(transform);
                selfPosition = new Vector3[renders.Length];
                /*if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
                {
                    offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
                    offsetVecGameObjectStart = offsetGameObject.transform.position;
                }*/
                for (int i = 0; i < selfPosition.Length; i++)
                {
                    selfPosition[i] = renders[i].transform.position;
                }
            }
            return;
        }

        if (offsetGameObject.transform.localScale == originScale)
        {
            if (center == bound.center) //位置不变
                return;
            if (center != bound.center && Size == bound.size) //大小不变，位置改变
            {
                center = bound.center;
                offsetQuaGameObject = new GameObject();
                offsetQuaGameObject.transform.SetParent(transform);
                offsetQuaGameObject.transform.position = center;
                if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
                {
                    offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
                    offsetGameObjectStart = offsetGameObject.transform.position;
                }
                selfPosition = new Vector3[renders.Length];
                for (int i = 0; i < selfPosition.Length; i++)
                {
                    selfPosition[i] = renders[i].transform.position;
                }
            }
        }
        else
        {
            originScale = offsetGameObject.transform.localScale;
            center = bound.center;
            offsetQuaGameObject = new GameObject();
            offsetQuaGameObject.transform.SetParent(transform);
            offsetQuaGameObject.transform.position = center;
            selfPosition = new Vector3[renders.Length];
            if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
            {
                offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
                offsetGameObjectStart = offsetGameObject.transform.position;
                originScale = offsetGameObject.transform.localScale;
            }
            for (int i = 0; i < selfPosition.Length; i++)
            {
                selfPosition[i] = renders[i].transform.position;
            }
        }
    }

    float currentTime = 0;
    private void Update()
    {
        OnModelBoom(sliderArg * (EndValue - BeginValue));
    }

    float currentArg;
    private void OnModelBoom(float arg)
    {
        if (currentArg == arg)
        {
            return;
        }
        currentArg = arg;
        for (int i = 0; i < renders.Length; i++)
        {
            if (offsetGameObject)
            {
                Vector3 offset = offsetGameObject.transform.position - offsetGameObjectStart;
                renders[i].transform.position = offsetQuaGameObject.transform.rotation * ((selfPosition[i] - center) * (arg + 1) + center) + offset;
            }
            else
            {
                renders[i].transform.position = (selfPosition[i] - center) * (arg + 1) + center;
            }

        }
    }

    public Bounds CalculateBounds(GameObject model, float minBoundsSize = 0.1f)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        Vector3 scale = model.transform.localScale;

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

    private bool IsParent(GameObject go, Transform tr)
    {
        bool a = false;
        if (go.Equals(tr.gameObject))
        {
            return true;
        }
        if (tr.parent != null)
        {
            a = IsParent(go, tr);
        }
        return a;
    }
}
