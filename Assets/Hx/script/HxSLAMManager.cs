
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;

public class HxSLAMManager : MonoBehaviour
{
    Touch touch;
    public PointCloudParticleExample pointCloudParticle;
    public UnityARGeneratePlane m_arplaneManager;
    bool isFirst = true;
    public Transform m_HitTransform;
    public UnityEvent foundEvent;

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                Debug.Log("Got hit!");
                m_HitTransform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                m_HitTransform.transform.Rotate(0, 180, 0, Space.Self);
                //Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                InitGo();
                return true;
            }
        }
        return false;
    }


    void Update()
    {
        if (!isFirst)
        {
            return;
        }

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }
 
        
        if (Input.touchCount > 0 && m_HitTransform != null)
        {
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                // prioritize reults types
                ARHitTestResultType[] resultTypes = {
						//ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingGeometry,
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        //ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                    };

                foreach (ARHitTestResultType resultType in resultTypes)
                {
                    if (HitTestWithResultType(point, resultType))
                    {
                        
                        return;
                    }
                }
                
            }
        }
    }

    private void InitGo()
    {
        //Debug.Log("点击了************");
        pointCloudParticle.enabled = false;
        pointCloudParticle.Stop();
        //curentGo = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
        //curentGo.transform.position = hitpose.position;
        //curentGo.transform.rotation = hitpose.rotation;
        

        if (isFirst)
        {
            isFirst = false;
            if (foundEvent != null)
            {
                foundEvent.Invoke();
            }
        }
    }

  
}
