using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetMouseButtonUp(0) && Input.GetTouch(0).phase != TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    //Debug.DrawLine(ray.origin, hitInfo.point);//scene視圖可看到 DrawLine(Vector3 origin,Vector3 end,Color col):衹有儅發生碰撞時，在Scene視圖才可以看到畫出的射綫。
                    GameObject Obj = hitInfo.collider.gameObject;
                    var ani = Obj.transform.root.GetComponentInChildren<Animation>();
                    if (ani)
                    {
                        ani.playAutomatically = false;
                        foreach (AnimationState itemani in ani)
                        {
                            ani.Play(itemani.name);
                            break;
                        }
                    }
                }
            }
        }

    }
}
