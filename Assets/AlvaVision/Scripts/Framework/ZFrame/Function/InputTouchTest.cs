using Alva.Recognition;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputTouchTest : MonoBehaviour
{
    public SlamRecognition slamRecognition;

    private float minScale = 0f;
    private float maxScale = 100f;

    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  
    Touch newTouch1;
    Touch newTouch2;
    Transform relativeTrf;
    //Transform cam;
    Vector2 deltaposition;
    float oldDistance, newDistance, offset, dot, scaleFactor;
    Vector3 localScale, scale;
    private void Start()
    {
        //if (GlobalData.ProjectSettingData.Type == "AreaTarget")
        //{
        //    this.enabled = false;
        //}
        GameObject g = new GameObject("TouchMeGameObject");
        g.transform.position = Vector3.zero;
        g.transform.eulerAngles = Vector3.zero;
        relativeTrf = g.transform;
    }

    void Update()
    {
        //没有触摸  
        if (Input.touchCount <= 0)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))//点击在UI上
        {
            return;
        }

        relativeTrf.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

        if (slamRecognition)
        {
            //单点触摸/旋转
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !slamRecognition.DisableModelRotating)//允许旋转
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    transform.Rotate(0, -Input.GetTouch(0).deltaPosition.x * 0.5f, 0);
                //transform.Rotate(Vector3.down * Input.GetTouch(0).deltaPosition.x * 0.5f, Space.World);
            }

            //多点触摸
            newTouch1 = Input.GetTouch(0);
            newTouch2 = newTouch1;

            if (Input.touchCount == 2)
            {
                newTouch2 = Input.GetTouch(1);
                //第2点刚开始接触屏幕, 只记录，不做处理  
                if (newTouch2.phase == TouchPhase.Began)
                {
                    oldTouch2 = newTouch2;
                    oldTouch1 = newTouch1;
                    return;
                }

                //计算老的两点距离和新的两点间距离 
                oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

                //两个距离之差，为正表示放大手势， 为负表示缩小手势  
                offset = newDistance - oldDistance;
                dot = Vector2.Dot((newTouch1.position - oldTouch1.position), (newTouch2.position - oldTouch2.position));
                //print("///dot:" + dot);
                if (dot > 0 && !slamRecognition.DisableModelMoving)//允许移动
                {
                    deltaposition = Input.GetTouch(0).deltaPosition;
                    transform.Translate(deltaposition.x * 0.001f, deltaposition.y * 0.001f, 0f, relativeTrf);

                    //记住最新的触摸点，下次使用  
                    oldTouch1 = newTouch1;
                    oldTouch2 = newTouch2;
                }
                else
                {
                    if (!slamRecognition.DisableModelScaling)//允许缩放
                    {
                        //放大因子， 一个像素按 0.01倍来算(100可调整)  
                        scaleFactor = offset / 1000f;
                        localScale = transform.localScale;
                        scale = new Vector3(localScale.x + scaleFactor,
                                                    localScale.y + scaleFactor,
                                                    localScale.z + scaleFactor);
                        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                            && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
                        {
                            //最小缩放到 0.3 倍 ，最大放大到 3 倍
                            if (scale.x > minScale && scale.y > minScale && scale.z > minScale && scale.x < maxScale && scale.y < maxScale && scale.z < maxScale)
                            {
                                transform.localScale = scale;
                            }
                            //记住最新的触摸点，下次使用  
                            oldTouch1 = newTouch1;
                            oldTouch2 = newTouch2;
                        }
                    }

                }
            }

        }
        else
        {
            //单点触摸/旋转
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)//允许旋转
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    transform.Rotate(0, -Input.GetTouch(0).deltaPosition.x * 0.5f, 0);
            }

            //多点触摸
            newTouch1 = Input.GetTouch(0);
            newTouch2 = newTouch1;

            if (Input.touchCount == 2)
            {
                newTouch2 = Input.GetTouch(1);
                //第2点刚开始接触屏幕, 只记录，不做处理  
                if (newTouch2.phase == TouchPhase.Began)
                {
                    oldTouch2 = newTouch2;
                    oldTouch1 = newTouch1;
                    return;
                }

                //计算老的两点距离和新的两点间距离 
                oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

                //两个距离之差，为正表示放大手势， 为负表示缩小手势  
                offset = newDistance - oldDistance;
                dot = Vector2.Dot((newTouch1.position - oldTouch1.position), (newTouch2.position - oldTouch2.position));
                //print("///dot:" + dot);
                if (dot > 0)//允许移动
                {
                    deltaposition = Input.GetTouch(0).deltaPosition;
                    transform.Translate(deltaposition.x * 0.001f, deltaposition.y * 0.001f, 0f, relativeTrf);

                    //记住最新的触摸点，下次使用  
                    oldTouch1 = newTouch1;
                    oldTouch2 = newTouch2;
                }
                else
                {
                    //允许缩放

                    //放大因子， 一个像素按 0.01倍来算(100可调整)  
                    scaleFactor = offset / 1000f;
                    localScale = transform.localScale;
                    scale = new Vector3(localScale.x + scaleFactor,
                                                localScale.y + scaleFactor,
                                                localScale.z + scaleFactor);
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                        && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
                    {
                        //最小缩放到 0.3 倍 ，最大放大到 3 倍
                        if (scale.x > minScale && scale.y > minScale && scale.z > minScale && scale.x < maxScale && scale.y < maxScale && scale.z < maxScale)
                        {
                            transform.localScale = scale;
                        }
                        //记住最新的触摸点，下次使用  
                        oldTouch1 = newTouch1;
                        oldTouch2 = newTouch2;
                    }


                }
            }
        }
    }
}
