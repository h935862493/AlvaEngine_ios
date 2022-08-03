using Alva.Recognition;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;

public class AlvaPlaneTargetBehaviour : MonoBehaviour
{
    Touch touch;
    public PointCloudParticleExample pointCloudParticle;
    bool isFirst = true;
    SlamRecognition slamRecognition;
    public GameObject ScrollView_tip,planeTarget;
    public UnityEvent foundEvent;
 
    public bool isAllRightInit = false;
    private void Awake()
    {
        var a = GameObject.FindObjectsOfType<Camera>();

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].transform.root.name != "AlvaCore_PlaneTarget(Clone)")
            {
                //print("Destroy gameobject:" + a[i].name);
                Destroy(a[i].gameObject);
            }
        }
        var e = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 0; i < e.Length; i++)
        {
            if (e[i].transform.root.name != "AlvaCore_PlaneTarget(Clone)")
            {
                //print("Destroy gameobject:" + e[i].gameObject.name);
                Destroy(e[i].gameObject);
            }
        }
    }
    private void Start()
    {
        slamRecognition = FindObjectOfType<SlamRecognition>();

        if (slamRecognition == null)
        {
            ARSceneComonUI.instance.ShowTip("没有挂载slamRecognition的物体，请在编辑器确认");
            return;
        }

        if (slamRecognition.DisableModelScaling && slamRecognition.DisableModelMoving && slamRecognition.DisableModelRotating)
        {
            ScrollView_tip.SetActive(false);
        }
        else
        {
            ScrollView_tip.SetActive(true);
            ScrollView_tip.GetComponent<PlaneTargetTipUI>().InitData(slamRecognition.DisableModelMoving, slamRecognition.DisableModelRotating, slamRecognition.DisableModelScaling);
        }


        planeTarget = slamRecognition.gameObject;

        if (planeTarget && slamRecognition.OnTargetLost != null)
        {
            slamRecognition.OnTargetLost.Invoke();
        }
        planeTarget.AddComponent<InputTouchTest>().slamRecognition = slamRecognition;
        isAllRightInit = true;
    }

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                //Debug.Log("Got hit!");
                planeTarget.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                planeTarget.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                planeTarget.transform.Rotate(0, 180, 0, Space.Self);
                //Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                InitGo();
                return true;
            }
        }
        return false;
    }


    void Update()
    {
        if (!isFirst || !isAllRightInit)
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


        if (Input.touchCount > 0 && planeTarget != null)
        {
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                ARHitTestResultType[] resultTypes = {
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
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
        pointCloudParticle.enabled = false;
        pointCloudParticle.Stop();

        if (isFirst)
        {
            isFirst = false;
            if (foundEvent != null)
            {
                foundEvent.Invoke();
            }
            if (slamRecognition.OnTargetFound != null)
            {
                slamRecognition.OnTargetFound.Invoke();
            }
            slamRecognition.Found();
        }
    }
}
