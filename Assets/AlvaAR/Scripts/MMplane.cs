using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMplane : MonoBehaviour
{
    public Camera theCamera;

    //距离摄像机8.5米 用黄色表示
    public float upperDistance = 8.5f;

    private Transform tx;

    public GameObject plane;
    public Vector3[] vertices, v2, v3;
    Mesh mesh;
    void Start()
    {
        //if (!theCamera)
        //{
        //    theCamera = Camera.main;
        //}
        tx = theCamera.transform;
        AAA();
    }

    void AAA()
    {

        mesh = plane.GetComponent<MeshFilter>().mesh;

        vertices = mesh.vertices;
        //FindUpperCorners();
    }
    //public Transform arCA,model;
    void Update()
    {
     
        FindUpperCorners();
    }


    void FindUpperCorners()
    {
        Vector3[] corners = GetCorners(upperDistance);

        // for debugging
        Debug.DrawLine(corners[0], corners[1], Color.yellow); // UpperLeft -> UpperRight
        Debug.DrawLine(corners[1], corners[3], Color.yellow); // UpperRight -> LowerRight
        Debug.DrawLine(corners[3], corners[2], Color.yellow); // LowerRight -> LowerLeft
        Debug.DrawLine(corners[2], corners[0], Color.yellow); // LowerLeft -> UpperLeft
    }

    //public Transform p1, p2, p3, p4;

    Vector3[] GetCorners(float distance)
    {
        Vector3[] corners = new Vector3[4];

        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = theCamera.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        //p1.position = corners[0];
        //p2.position = corners[1];
        //p3.position = corners[2];
        //p4.position = corners[3];

        Vector3[] temp = new Vector3[4];
        temp[0] = new Vector3(corners[0].x, corners[0].y, vertices[0].z);
        temp[1] = new Vector3(corners[2].x, corners[2].y, vertices[1].z);//4
        temp[2] = new Vector3(corners[1].x, corners[1].y, vertices[2].z);//1
        temp[3] = new Vector3(corners[3].x, corners[3].y, vertices[3].z);//2
        v2 = temp;
        mesh.vertices = temp;
        //plane.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, plane.transform.localPosition.y, upperDistance);

        return corners;
    }
}
