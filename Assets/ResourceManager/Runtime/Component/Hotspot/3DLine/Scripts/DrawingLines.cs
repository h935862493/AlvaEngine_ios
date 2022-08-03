using UnityEngine;

public class DrawingLines : MonoBehaviour {
    public bool active;
    public LayerMask Layer;
    public KeyCode RevokeKey;
    public KeyCode RevokeAllKey;
    private int frameNum = 0;
    public Huey3DLine.Line line;
    private Vector3 spot;

    public void Update()
    {
        if (!active) return;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point;
            if (GetPoint(out point))
            {
                line.OnBegin();
                line.OnCreatSpot(point);
                spot = point;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            frameNum++;
            if (frameNum % line.LineSmoothingValue != 0) return;
            Vector3 point;
            if (GetPoint(out point))
            {
                line.OnCreatLine(spot, point);
                line.OnCreatSpot(point);
                spot = point;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            line.OnEnd();
        }
#elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
        {
            Vector3 point;
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    if (GetPoint(out point))
                    {
                        line.OnBegin();
                        line.OnCreatSpot(point);
                    }
                    break;
                case TouchPhase.Moved:
                    frameNum++;
                    if (frameNum % line.LineSmoothingValue != 0) return;
                    if (GetPoint(out point))
                    {
                        line.OnCreatLine(spot, point);
                        line.OnCreatSpot(point);
                    }
                    break;
                case TouchPhase.Ended:
                    line.OnEnd();
                    break;
            }
        } 
#endif
        if (Input.GetKeyDown(RevokeKey)) line.Revoke();
        if (Input.GetKeyDown(RevokeAllKey)) line.RevokeAll();
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
