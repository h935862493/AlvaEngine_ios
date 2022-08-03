using UnityEngine;

public class RulerLabel : MonoBehaviour {
    private float a;
    private bool b;
    public TextMesh[] tm;
	void Update () {
        Vector3 relative = Camera.main.transform.position - transform.position;
        float angle = Mathf.Atan2(relative.y, relative.z) * Mathf.Rad2Deg;
       
        if (a != (180 - angle))
        {
            if((180 - angle) > 90 && (180 - angle) < 270)
            {
                if (b)
                {
                    b = false;
                    transform.Rotate(Vector3.right, 180, Space.Self);
                    transform.Rotate(Vector3.up, 180, Space.Self);
                }
                transform.Rotate(Vector3.left, (180 - angle) - a, Space.Self);
            }
            else
            {
                if (!b)
                {
                    b = true;
                    transform.Rotate(Vector3.right, 180, Space.Self);
                    transform.Rotate(Vector3.up, 180, Space.Self);
                }
                transform.Rotate(Vector3.right, (180 - angle) - a, Space.Self);
            }
            a = (180 - angle);
        }
    }
}
