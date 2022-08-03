using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePositon : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera theCamera;
    public float upperDistance = 8.5f;
    private Transform tx;

    public GameObject plane;
    public Vector3[] vertices;
    Mesh mesh;
    void Start()
    {
      
        tx = theCamera.transform;
        mesh = plane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }
    private void Update()
    {
        //GetCorners(upperDistance);
    }

    public void GetCorners(float distance)
    {
        Vector3[] corners = new Vector3[4];
        //plane.GetComponent<MeshRenderer>().bounds
        //corners[0] = theCamera.ScreenToWorldPoint(pos);
        /* float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
         // theCamera.aspect =  480.0f/640.0f;
         //  float aspect = 2.5f;// theCamera.aspect;
         float aspect =  theCamera.aspect;
         print("aspect" + aspect);

         float height = distance * Mathf.Tan(halfFOV);
         float width = height * aspect;
         float height2 = width / 1.33f;
         height = height2;
         print(height2);
         print(height);
         print(width);
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

         Vector3[] temp = new Vector3[4];
         temp[0] = new Vector3(corners[2].x, corners[2].y, vertices[0].z);
         temp[1] = new Vector3(corners[3].x, corners[3].y, vertices[1].z);//4
         temp[2] = new Vector3(corners[0].x, corners[0].y, vertices[2].z);//1
         temp[3] = new Vector3(corners[1].x, corners[1].y, vertices[3].z);//2

         mesh.vertices = temp;
         plane.transform.localPosition = new Vector3(plane.transform.localPosition.x, plane.transform.localPosition.y, distance);*/

    }
}
