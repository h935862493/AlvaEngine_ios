using UnityEngine;
using Alva.Runtime.Components;
public class StructuralDecompositionButton: MonoBehaviour
{
    Vector3 center;
    Vector3 Size;
    Vector3 oneSize;
    Vector3 flagPosition;
    GameObject slamGameObject;
    int flag = 0;
    bool IsBoom;
    bool IsRecover;
    Vector3 originPosition;
    MeshRenderer[] renders;
    Vector3[] selfPosition;
    public ulong Time = 2;
    public ulong BeginValue = 0;
    public ulong EndValue = 1;
    Vector3 originScale;
    private bool IsExcute = false;
    [HideInInspector]
    public GameObject offsetGameObject;
    Vector3 offsetVecGameObjectStart;
    GameObject offsetQuaGameObject;

    private void Awake()
    {
        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        Size = bound.size;
        oneSize = bound.size;
        //Debug.Log("onesize" + oneSize);
        center = bound.center;  //默认中心点
        offsetQuaGameObject = new GameObject();
        offsetQuaGameObject.transform.SetParent(transform);
        offsetQuaGameObject.transform.position = center;  //生成一个标记点
        selfPosition = new Vector3[renders.Length];
        if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
        {
            defaultTrackableEventHandler = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>();
            offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
            offsetVecGameObjectStart = offsetGameObject.transform.position;
            originScale = offsetGameObject.transform.localScale;
        }
        else
        {
            originScale = offsetQuaGameObject.transform.localScale; //初始Scale
        }
        for (int i = 0; i < selfPosition.Length; i++)
        {
            selfPosition[i] = renders[i].transform.position;
        }
        
    }
    Alva.Runtime.Components.DefaultTrackableEventHandler defaultTrackableEventHandler;
    public void Execute()
    {
        if (!this.GetComponent<StructuralDecompositionButton>().enabled)
            return;
        IsExcute = true;
        isRecover = false;
        IsBoom = true;
        currentTime = 0;
        if (defaultTrackableEventHandler != null && defaultTrackableEventHandler.type.Equals("SlamRecognition"))
        {
            defaultTrackableEventHandler.SetDisableModelScaling(true);
        }

        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        //if (offsetGameObject == null && IsParent(offsetGameObject,transform))
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
                    offsetVecGameObjectStart = offsetGameObject.transform.position;
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
                offsetVecGameObjectStart = offsetGameObject.transform.position;
                originScale = offsetGameObject.transform.localScale;
            }
            for (int i = 0; i < selfPosition.Length; i++)
            {
                selfPosition[i] = renders[i].transform.position;
            }
        }
    }

    bool isRecover = false;
    public void Recover()
    {
        if (!this.GetComponent<StructuralDecompositionButton>().enabled)
            return;
        if (!IsBoom)
            return;
        Invoke("RecoverWait", Time);
        IsExcute = true;
        isRecover = true;
        IsRecover = true;
        currentTime = 0;
        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        //Size = bound.size;
        //if (offsetGameObject == null && IsParent(offsetGameObject, transform))
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
                selfPosition = new Vector3[renders.Length];
                if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
                {
                    offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
                    offsetVecGameObjectStart = offsetGameObject.transform.position;
                }
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
            Size = bound.size;
            selfPosition = new Vector3[renders.Length];
            if (defaultTrackableEventHandler != null)
            {
                offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
                offsetVecGameObjectStart = offsetGameObject.transform.position;
            }
            for (int i = 0; i < selfPosition.Length; i++)
            {
                selfPosition[i] = renders[i].transform.position;
            }
        }
        IsBoom = false;
    }

    float currentTime = 0;
    private void Update()
    {
        if (!IsExcute)
        {
            return;
        }
        if (this.Time > 0 && currentTime < this.Time)
        {
            currentTime += UnityEngine.Time.deltaTime;
            if (!isRecover)
            {
                OnModelBoom(Mathf.Lerp(BeginValue, EndValue, currentTime / this.Time));
            }
            else
            {
                OnModelBoom(Mathf.Lerp(EndValue , BeginValue, currentTime / this.Time));
            }
        }
        else
        {
            IsExcute = false;
        }
        if (this.Time == 0 && isRecover)
        {
            OnModelBoom(BeginValue);
            IsExcute = false;
        }
        if (this.Time == 0 && !isRecover)
        {
            OnModelBoom(EndValue);
            IsExcute = false;
        }
    }

    private void OnModelBoom(float arg)
    {
        for (int i = 0; i < renders.Length; i++)
        {
            if (offsetGameObject)
            {        
                Vector3 offset = offsetGameObject.transform.position - offsetVecGameObjectStart;
                //renders[i].transform.position = (offsetQuaGameObject.transform.rotation * ((selfPosition[i] - center) * (arg + 1 / originScale.x + originScale.x / (23F)) + center) + offset;
                renders[i].transform.position = offsetQuaGameObject.transform.rotation * ((selfPosition[i] - center)  * (arg + 1) + center) + offset;
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
    public void RecoverWait() {
        if (defaultTrackableEventHandler != null && defaultTrackableEventHandler.type.Equals("SlamRecognition"))
        {
            defaultTrackableEventHandler.SetDisableModelScaling(false);
        }
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
