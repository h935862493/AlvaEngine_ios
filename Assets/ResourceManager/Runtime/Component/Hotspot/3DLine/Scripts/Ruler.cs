using UnityEngine;
using Huey3DLine;



public class Ruler : MonoBehaviour {
    public Line line;
    private GameObject ruler;
    private GameObject label;
    public GameObject labelPrefab;
    private Vector3[] TwoPointsOfLine = new Vector3[2];
    public LayerMask Layer;
    private void Start()
    {
        Debug.Log(Mathf.Atan2(4, 3));
        Debug.Log(Mathf.Atan2(4, 3) * Mathf.Rad2Deg);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point;
            if (GetPoint(out point))
            {
                line.Revoke();
                Destroy(label);

                line.OnBegin();
                line.OnCreatSpot(point);
                ruler = line.OnCreatLine(point, point);
                label = Instantiate(labelPrefab);
                TwoPointsOfLine[0] = TwoPointsOfLine[1] = point;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 point;
            if (GetPoint(out point))
            {
                Destroy(ruler);
                ruler = line.OnCreatLine(TwoPointsOfLine[0], point);
                label.transform.position = ruler.transform.position;
                label.transform.LookAt(TwoPointsOfLine[1]);
                label.transform.Rotate(Vector3.up, -90, Space.Self);
                label.GetComponent<TextMesh>().text = Vector3.Distance(TwoPointsOfLine[0], TwoPointsOfLine[1]).ToString();
                TwoPointsOfLine[1] = point;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(line.GetState() == Line.State.Drawing)
            {
                line.OnCreatSpot(TwoPointsOfLine[1]);
                line.OnEnd();
            }
        }
    }
    bool GetPoint(out Vector3 pos)
    {
        pos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Layer))
        {
            pos = hit.point;
            return true;
        }
        else
        {
            return false;
        }
    }
}
