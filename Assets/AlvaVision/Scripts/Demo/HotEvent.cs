using UnityEngine;

public class HotEvent : MonoBehaviour
{
    GameObject Line;
    private GameObject Plane;
    public GameObject Other;
    public int id;
    private void Start()
    {
        Line = transform.Find("line").gameObject;
        Plane = transform.Find("plane").gameObject;
    }

    public void SetObjActive()
    {
        
        Line.SetActive(!Line.activeSelf);
        Plane.SetActive(!Plane.activeSelf);
    }

    public void SetOtherActive()
    {
        Other.SetActive(!Other.activeSelf);
    }

    private void OnDisable()
    {
        Line.SetActive(false);
        Plane.SetActive(false);
    }
}
