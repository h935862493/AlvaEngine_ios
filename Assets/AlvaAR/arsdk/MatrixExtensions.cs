using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AlvaAR.arsdk
{
    using UnityEngine;

    public static class MatrixExtensions
    {
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        public static Matrix4x4 ColMajorArrayToMatrix(float[] rowMajorMatrix)
        {
            Matrix4x4 mx = new Matrix4x4();
            mx.m00 = rowMajorMatrix[0]; mx.m01 = rowMajorMatrix[4]; mx.m02 = rowMajorMatrix[8]; mx.m03 = rowMajorMatrix[12];
            mx.m10 = rowMajorMatrix[1]; mx.m11 = rowMajorMatrix[5]; mx.m12 = rowMajorMatrix[9]; mx.m13 = rowMajorMatrix[13];
            mx.m20 = rowMajorMatrix[2]; mx.m21 = rowMajorMatrix[6]; mx.m22 = rowMajorMatrix[10]; mx.m23 = rowMajorMatrix[14];
            mx.m30 = rowMajorMatrix[3]; mx.m31 = rowMajorMatrix[7]; mx.m32 = rowMajorMatrix[11]; mx.m33 = rowMajorMatrix[15];

            return mx;
        }
    }
}
