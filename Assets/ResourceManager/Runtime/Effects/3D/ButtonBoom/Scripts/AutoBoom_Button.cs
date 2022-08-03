using UnityEngine;
[AddComponentMenu("myName/myProduct/v1/Structural Decomposition_Button")]
public class AutoBoom_Button : MonoBehaviour
{
    Vector3 center;
    MeshRenderer[] renders;
    Vector3[] selfPosition;

    public ulong Time = 0;
    public ulong BeginValue = 0;
    public ulong EndValue = 1;

    private bool IsExcute = false;
    [HideInInspector]
    public GameObject offsetGameObject;
    Vector3 offsetVecGameObjectStart;
    GameObject offsetQuaGameObject;

    private void Awake()
    {
        renders = GetComponentsInChildren<MeshRenderer>();
        Bounds bound = CalculateBounds(gameObject);
        center = bound.center;
        offsetQuaGameObject = new GameObject();
        offsetQuaGameObject.transform.position = center;
        offsetQuaGameObject.transform.SetParent(transform);
        if (FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>() != null)
        {
            offsetGameObject = FindObjectOfType<Alva.Runtime.Components.DefaultTrackableEventHandler>().gameObject;
            offsetVecGameObjectStart = offsetGameObject.transform.position;
            //offsetQuaGameObjectStart = offsetGameObject.transform.eulerAngles;
            //x = offsetQuaGameObjectStart.x;
            //y = offsetQuaGameObjectStart.y;
        }
        selfPosition = new Vector3[renders.Length];
        for (int i = 0; i < selfPosition.Length; i++)
        {
            selfPosition[i] = renders[i].transform.position;
        }
    }

    public void Execute()
    {
        if (!this.GetComponent<AutoBoom_Button>().enabled)
            return;
        IsExcute = true;
        isRecover = false;
        currentTime = 0;
    }

    bool isRecover = false;
    public void Recover()
    {
        if (!this.GetComponent<AutoBoom_Button>().enabled)
            return;
        IsExcute = true;
        isRecover = true;
        currentTime = 0;
    }

    float currentTime = 0;
    private void Update()
    {
        if (!IsExcute)
        {
            return;
        }
        if (this.Time > 0 && currentTime < this.Time )
        {
            currentTime += UnityEngine.Time.deltaTime;
            if (!isRecover)
            {
                OnModelBoom(Mathf.Lerp(BeginValue, EndValue, currentTime / this.Time));
            }
            else
            {
                OnModelBoom(Mathf.Lerp(EndValue, BeginValue, currentTime / this.Time));
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
        //Debug.Log(arg);
        for(int i = 0;i < renders.Length; i++)
        {
            if (offsetGameObject)
            {
                Vector3 offset = offsetGameObject.transform.position - offsetVecGameObjectStart;
                //Vector3 quaOffset = offsetGameObject.transform.rotation * 
                renders[i].transform.position = offsetQuaGameObject.transform.rotation *((selfPosition[i] - center) * (arg + 1) + center) + offset;
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
