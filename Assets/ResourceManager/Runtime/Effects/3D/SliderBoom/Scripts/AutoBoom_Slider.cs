using UnityEngine;
[AddComponentMenu("myName/myProduct/v1/Structural Decomposition_Slider")]
public class AutoBoom_Slider : MonoBehaviour
{
    Vector3 center;
    MeshRenderer[] renders;
    Vector3[] selfPosition;

    private float sliderArg;
    [HideInInspector]
    public float BeginValue = 0;
    [HideInInspector]
    public float EndValue = 1;
    [HideInInspector]
    public GameObject offsetGameObject;
    Vector3 offsetGameObjectStart;
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
            offsetGameObjectStart = offsetGameObject.transform.position;
        }

        selfPosition = new Vector3[renders.Length];
        for (int i = 0; i < selfPosition.Length; i++)
        {
            selfPosition[i] = renders[i].transform.position;
        }
    }
   
    float time = 0;
    /// <summary>
    /// º∏√Î÷¥––ÕÍ±œ
    /// </summary>
    /// <param name="time">√Î÷”</param>
    public void Execute(float arg)
    {
        if (!this.GetComponent<AutoBoom_Slider>().enabled)
            return;
        sliderArg = arg;
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
        for (int i = 0;i < renders.Length; i++)
        {
            if (offsetGameObject)
            {
                Vector3 offset = offsetGameObject.transform.position - offsetGameObjectStart;
                renders[i].transform.position = offsetQuaGameObject.transform.rotation * ( (selfPosition[i] - center) * (arg + 1) + center) + offset;
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
