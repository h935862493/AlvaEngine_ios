using Assets.AlvaAR.arsdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToLookAt : MonoBehaviour
{
    public Vector3 getCameraPosByWorldToCameraMatrix(Matrix4x4 worldToCameraMatrix)
    {
        var mx = Matrix4x4.Inverse(worldToCameraMatrix);

        mx.m02 *= -1f;
        mx.m12 *= -1f;
        mx.m22 *= -1f;
        mx.m32 *= -1f;
        var pos = MatrixExtensions.ExtractPosition(mx);

        return pos;
    }

    private void OnEnable()
    {

        //GameObject obj = new GameObject();
        //obj.transform.SetParent(transform.parent);
        //obj.transform.localScale = transform.localScale;
        //obj.transform.position = transform.position;
        //obj.transform.rotation = transform.rotation;
        //obj.transform.LookAt(getCameraPosByWorldToCameraMatrix(Camera.main.worldToCameraMatrix));
        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, obj.transform.eulerAngles.y, 0);

        //Destroy(obj);
    }
}
