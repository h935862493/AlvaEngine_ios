using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTouchMe : MonoBehaviour {

    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  

    Vector3 lastmosPos;
    public float rotespeed = 2.5f;

    Vector3 modelScale;

    Vector3 BiggestScale = Vector3.one;
    float BiggestDistance = 1;

    Vector3 MinnestScale = Vector3.zero;

    bool IsReset = true;

    // Use this for initialization
    void Start()
    {
        modelScale = transform.localScale;
        CalcDistance();
        transform.localScale = modelScale;
        MinnestScale = modelScale * 0.5f;
    }

    Vector3 MeshCenter()
    {
        Vector3 vecCenter = Vector3.zero;
        int times = 0;
        foreach (var item in transform.GetComponentsInChildren<MeshRenderer>())
        {
            vecCenter += item.bounds.center;
            times++;
        }
        return vecCenter / times;
    }

    void CalcDistance()
    {
        BiggestDistance = Vector3.Distance(MeshCenter(), Camera.main.transform.position) - Camera.main.nearClipPlane - 0.02f;
        Debug.Log(BiggestDistance);
        //float minDis = 0;/*Vector3.Distance(transform.GetComponent<BoxCollider>().center, transform.GetComponent<BoxCollider>().bounds.size / 2);*/
        float a = 0;
        float b = 0;
        float c = 0;
        float distance = 0;
        BoxCollider bc = transform.GetComponent<BoxCollider>();

        transform.localScale = modelScale;

        for (float i = 0.01f; i > 10; i += 0.01f)
        {
            transform.localScale += Vector3.one * i;
            a = bc.bounds.size.x;
            //Debug.Log();
            b = bc.bounds.size.y;
            c = bc.bounds.size.z;
            //模型对角线长度
            distance = Mathf.Sqrt(a * a + c * c + b * b);

            if (BiggestDistance - distance <= 0)
            {
                BiggestScale = transform.localScale;
                Debug.Log(BiggestScale);
                return;
            }
        }

        transform.localScale = modelScale;
    }

    RaycastHit hit;
    private void Update()
    {
        if (IsReset)
        {
            IsReset = false;
            Debug.Log("实例化参数重置=================");
            transform.localScale = modelScale;
        }

        if (Input.touchCount <= 0)
        {
            return;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //Debug.Log(hit.distance);
            if (transform == hit.transform)
            {
                //Debug.Log("放大缩小！=================");
                //Debug.Log(hit.distance);
                OnModelTouch();
            }
        }
    }
    
    private void OnModelTouch()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(1))
        {
            transform.Translate(Vector3.up);
        }
        
#endif
        ////没有触摸  
        //if (Input.touchCount <= 0)
        //{
        //    return;
        //}

        //单点触摸， 水平上下旋转
        if (1 == Input.touchCount)
        {
            Vector3 aimPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.transform.position.y));
            Vector3 dir = (aimPos - transform.position).normalized;
            if (lastmosPos != Vector3.zero)
            {
                Vector3 m = (Input.mousePosition - lastmosPos).normalized * rotespeed;
                transform.Rotate(new Vector3(m.y, -m.x, m.z), Space.World);
                //Debug.Log("旋转");
            }

            lastmosPos = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                lastmosPos = Vector3.zero;
            }

            //Touch touch = Input.GetTouch(0);
            //Vector2 deltaPos = touch.deltaPosition;
            //transform.Rotate(Vector3.down * deltaPos.x * 0.5f, Space.World);
            //transform.Rotate(Vector3.right * deltaPos.y, Space.World);
            //transform.Rotate(Vector3.down, -Input.GetTouch(0).deltaPosition.x * 0.25f);
        }

        //if (Input.touchCount > 1)
        //{
        //    return;
        //}

        //多点触摸, 放大缩小  
        Touch newTouch1 = Input.GetTouch(0);
        Touch newTouch2 = Input.GetTouch(1);

        //第2点刚开始接触屏幕, 只记录，不做处理  
        if (newTouch2.phase == TouchPhase.Began)
        {
            oldTouch2 = newTouch2;
            oldTouch1 = newTouch1;
            return;
        }

        //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
        float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
        float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

        //两个距离之差，为正表示放大手势， 为负表示缩小手势  
        float offset = newDistance - oldDistance;

        if (offset > 0)//放大
        {
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 0.1f))
            //{
            //    return;
            //}
            //BoxCollider bc = transform.GetComponent<BoxCollider>();
            //if (BiggestDistance  >
            //    Vector3.Distance(bc.center, bc.bounds.size / 2))
            transform.localScale += transform.localScale * Time.deltaTime * offset / 5;

            transform.localScale = transform.localScale.x > BiggestScale.x 
                || transform.localScale.y > BiggestScale.y 
                || transform.localScale.z > BiggestScale.z ? BiggestScale : transform.localScale;
        }
        else
        {
            transform.localScale += transform.localScale * Time.deltaTime * offset / 5;

            transform.localScale = transform.localScale.x < MinnestScale.x
              || transform.localScale.y < MinnestScale.y
              || transform.localScale.z < MinnestScale.z ? MinnestScale : transform.localScale;

        }
        //记住最新的触摸点，下次使用
        oldTouch1 = newTouch1;
        oldTouch2 = newTouch2;

    }
}
