using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Huey3DLine;

public class Hotspot : MonoBehaviour
{
    public GameObject HotObj;
    public GameObject PlaneObj;
    public GameObject LineObj;
    public Line myline;
    public float middleDistance = 0.1f;
    public string url;
    private float distance;
    private Vector3 point_size;
    private float point_size_changed;
    private Vector3 plane_size;
    private float plane_size_changed;
    private Vector3 gameobject_size;
    private float gameobject_size_changed;

    private List<GameObject> Lines;

    bool isInAvtive = false;

    private void Start()
    {
        gameobject_size = transform.position;
        distance = Vector3.Distance(HotObj.transform.position, PlaneObj.transform.position);
        point_size = HotObj.transform.localScale;
        point_size_changed = Vector3.Distance(HotObj.transform.localScale, point_size);
        plane_size = PlaneObj.transform.localScale;
        plane_size_changed = Vector3.Distance(PlaneObj.transform.localScale, plane_size);
        gameobject_size_changed = Vector3.Distance(gameobject_size, transform.position);

        GetLocation();
        if (!PlaneObj.activeSelf)
        {
            isInAvtive = true;
        }
    }

    public void InitData()
    {
        GetLocation();
    }


    private void Update()
    {
        if (HotObj == null || isInAvtive)
        {
            return;
        }
        HotObj.transform.localPosition = Vector3.zero;
        if (Vector3.Distance(HotObj.transform.position, PlaneObj.transform.position) != distance)
        {
            distance = Vector3.Distance(HotObj.transform.position, PlaneObj.transform.position);
            DrawLink();
        }
        if (Vector3.Distance(HotObj.transform.localScale, point_size) != point_size_changed)
        {
            point_size_changed = Vector3.Distance(HotObj.transform.localScale, point_size);
            DrawLink();
        }
        if (Vector3.Distance(PlaneObj.transform.localScale, plane_size) != plane_size_changed)
        {
            plane_size_changed = Vector3.Distance(PlaneObj.transform.localScale, plane_size);
            DrawLink();
        }
        if (Vector3.Distance(gameobject_size, transform.position) != gameobject_size_changed)
        {
            gameobject_size_changed = Vector3.Distance(gameobject_size, transform.position);
            DrawLink();
        }
    }

    private void DrawLink()
    {
        foreach (var item in Lines)
        {
            Destroy(item);
        }
        Lines.Clear();
        GetLocation();
    }

    public void InitObjectAvtive()
    {
        isInAvtive = !isInAvtive;
        PlaneObj.SetActive(!PlaneObj.activeSelf);
        if (PlaneObj.activeSelf)
        {
            GetLocation();
        }
        else
        {
            /*foreach (var item in Lines)
            {
                Destroy(item);
            }
            Lines.Clear();*/
            //Debug.Log("1111");
            OnDisable();
        }
    }

    private void OnDisable()
    {
        foreach (var item in Lines)
        {
            Destroy(item);
        }
        Lines.Clear();
    }

    public void OpenURL()
    {
        //Debug.Log("点击了obj222222");
        Application.OpenURL(url);
    }

    float Angle_360(Vector3 from_, Vector3 to_)
    {
        //两点的x、y值
        float x = from_.x - to_.x;
        float y = from_.y - to_.y;

        //斜边长度
        float hypotenuse = Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));

        //求出弧度
        float cos = x / hypotenuse;
        float radian = Mathf.Acos(cos);

        //用弧度算出角度    
        float angle = 180 / (Mathf.PI / radian);

        if (y < 0)
        {
            angle = -angle;
        }
        else if ((y == 0) && (x < 0))
        {
            angle = 180;
        }
        return angle;
    }

    private void GetLocation()
    {
        float angle = Angle_360(HotObj.transform.position, PlaneObj.transform.position);
        float width = PlaneObj.GetComponent<MeshRenderer>().bounds.size.x;
        float height = PlaneObj.GetComponent<MeshRenderer>().bounds.size.y;
        float anglePointLeft = Angle_360(HotObj.transform.position, new Vector3(PlaneObj.transform.position.x - width / 2, PlaneObj.transform.position.y, PlaneObj.transform.position.z));
        float anglePointRight = Angle_360(HotObj.transform.position, new Vector3(PlaneObj.transform.position.x + width / 2, PlaneObj.transform.position.y, PlaneObj.transform.position.z));

        float dis = Vector3.Distance(HotObj.transform.position, PlaneObj.transform.position);
        float c = Mathf.Sqrt(((width / 2) * (width / 2)) + ((height / 2) * (height / 2)));
        if (dis < (c + HotObj.GetComponent<MeshRenderer>().bounds.size.x/2))
        {
            return;
        }

        Vector3 begin = new Vector3(), end = new Vector3(), middle = new Vector3();

        bool isMiddle = false;
        float by = HotObj.transform.position.y + HotObj.GetComponent<MeshRenderer>().bounds.size.y / 2;
        begin = new Vector3(HotObj.transform.position.x, by, HotObj.transform.position.z);

        if ((angle <= -80f && angle >= -120f) || (HotObj.transform.position.x < PlaneObj.transform.position.x && anglePointLeft <= -80f && anglePointLeft >= -120f) || (HotObj.transform.position.x > PlaneObj.transform.position.x && anglePointRight <= -80f && anglePointRight >= -120f))
        {
            //上
            float ey = PlaneObj.transform.position.y - height / 2;
            end = new Vector3(PlaneObj.transform.position.x, ey, PlaneObj.transform.position.z);
        }
        else if (angle < -120f && angle > -160f)
        {
            //右上
            float ex = PlaneObj.transform.position.x - width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);

            middle = new Vector3(ex - middleDistance, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
            isMiddle = true;
        }
        else if ((angle <= -160f && angle >= -180f) || (angle <= 180f && angle >= 160f))
        {
            //右
            float ex = PlaneObj.transform.position.x - width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
        }
        else if ((angle <= 120f && angle >= 70f) || (HotObj.transform.position.x < PlaneObj.transform.position.x && anglePointLeft <= 120f && anglePointLeft >= 70f) || (HotObj.transform.position.x > PlaneObj.transform.position.x && anglePointRight <= 120f && anglePointRight >= 70f))
        {
            //下
            float ey = PlaneObj.transform.position.y + height / 2;
            end = new Vector3(PlaneObj.transform.position.x, ey, PlaneObj.transform.position.z);
        }
        else if (angle > 120f && angle < 160f)
        {
            //右下
            float ex = PlaneObj.transform.position.x - width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);

            middle = new Vector3(ex - middleDistance, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
            isMiddle = true;
        }
        else if (angle > 20f && angle < 70f)
        {
            //左下
            float ex = PlaneObj.transform.position.x + width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);

            middle = new Vector3(ex + middleDistance, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
            isMiddle = true;
        }
        else if (angle >= -20f && angle <= 20f)
        {
            //左
            float ex = PlaneObj.transform.position.x + width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
        }
        else if (angle > -80f && angle < -20f)
        {
            //左上
            float ex = PlaneObj.transform.position.x + width / 2;
            end = new Vector3(ex, PlaneObj.transform.position.y, PlaneObj.transform.position.z);

            middle = new Vector3(ex + middleDistance, PlaneObj.transform.position.y, PlaneObj.transform.position.z);
            isMiddle = true;
        }
        Lines = new List<GameObject>();
        myline.OnBegin();
        if (isMiddle)
        {
            Lines.Add(myline.OnCreatLine(begin, middle));
            Lines.Add(myline.OnCreatLine(middle, end));
        }
        else
        {
            Lines.Add(myline.OnCreatLine(begin, end));
        }
        myline.OnEnd();
    }

}
