using System;
using UnityEngine;

public class LoadYUV : MonoBehaviour
{

    private int videoWidth = 1280;
    private int videoHeight = 720;
    private byte[] bufY = null;
    private byte[] bufU = null;
    private byte[] bufV = null;
 

    private Texture2D texY = null;
    private Texture2D texU = null;
    private Texture2D texV = null;


    void Start()
    {

        if (texY == null)
        {
            texY = new Texture2D(videoWidth, videoHeight, TextureFormat.Alpha8, false);
        }

        //U分量和V分量分别存放在两张贴图中
        if (texU == null)
        {
            texU = new Texture2D(videoWidth >> 1, videoHeight >> 1, TextureFormat.Alpha8, false);
        }
        if (texV == null)
        {
            texV = new Texture2D(videoWidth >> 1, videoHeight >> 1, TextureFormat.Alpha8, false);
        }

        bufY = new byte[videoWidth * videoHeight];
        bufU = new byte[videoWidth * videoHeight >> 2];
        bufV = new byte[videoWidth * videoHeight >> 2];

    }

    static Texture2D Rotate90Texture2D(Texture2D tex)
    {
        Color[] texColor = tex.GetPixels();
        int width = tex.width;
        int height = tex.height;
        Color[] newTexColor = new Color[width * height];
        try
        {
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    newTexColor[width * height - height - height * i + j] = texColor[j * width + i];
                }
            }
        }
        catch (Exception exc)
        {
            Debug.Log("Exception.Message--===" + exc.Message);
        }
        Texture2D NewTex = new Texture2D(height, width);
        NewTex.SetPixels(newTexColor);
        NewTex.Apply();
        return NewTex;
    }
    public void SetYUV(Material material, byte[] buff,int imageWidth,int imageHeight)
    {
        //Debug.Log("===接收到的Byte长度" + buff.Length + "===");
        LoadYUVNV12(buff, imageWidth, imageHeight);
        texY.LoadRawTextureData(bufY);
        texU.LoadRawTextureData(bufU);
        texV.LoadRawTextureData(bufV);
        //Debug.Log("===转化图片成功===");
        texY.Apply();
        texU.Apply();
        texV.Apply();

        // texUV.LoadRawTextureData(bufUV);
        //texUV.Apply();

        material.SetTexture("_MainTex", texY);
        material.SetTexture("_UTex", texU);
        material.SetTexture("_VTex", texV);

        //mMaterial.SetTexture("_UVTex", texUV);
        //Debug.Log("===Shader赋值成功===");

        
    }

    void LoadYUVNV12(byte[] YUVimage, int width, int height)
    {
        if(videoWidth!=width || videoHeight!=height)
        {
            videoWidth = width;
            videoHeight = height;
            texY = new Texture2D(videoWidth, videoHeight, TextureFormat.Alpha8, false);
            texU = new Texture2D(videoWidth >> 1, videoHeight >> 1, TextureFormat.Alpha8, false);
            texV = new Texture2D(videoWidth >> 1, videoHeight >> 1, TextureFormat.Alpha8, false);

            bufY = new byte[videoWidth * videoHeight];
            bufU = new byte[videoWidth * videoHeight >> 2];
            bufV = new byte[videoWidth * videoHeight >> 2];
        }

       // Array.Copy(YUVimage,bufY,)

        for (int i = 0; i < width * height; i++)
        {
            bufY[i] = YUVimage[i];
        }

        for (int i = 0; i < width * height / 4; i++)
        {
            bufU[i] = YUVimage[(width * height) + 2 * i];
            bufV[i] = YUVimage[(width * height) + 2 * i + 1];

        }
    }

    /*void LoadYUVimage(byte[] buff)
    {
        int firstFrameEndIndex = (int)(videoH * videoW * 1.5f);

        int yIndex = firstFrameEndIndex * 4 / 6;
        int uIndex = firstFrameEndIndex * 5 / 6;
        int vIndex = firstFrameEndIndex;

        bufY = new byte[videoW * videoH];
        bufU = new byte[videoW * videoH >> 2];
        bufV = new byte[videoW * videoH >> 2];
        // bufUV = new byte[videoW * videoH >> 1];

        bool isSingle = false;
        int indexV = 0;
        int indexU = 0;

        for (int i = 0; i < firstFrameEndIndex; i++)
        {

            if (i < yIndex)
            {
                bufV[i] = buff[i];
                continue;
            }

            if (isSingle)
            {
                bufU[indexV] = buff[i];
                indexV++;
                isSingle = false;
            }
            else
            {
                bufU[indexU] = buff[i];
                indexU++;
                isSingle = true;
            }

        }

        //如果是把UV分量一起写入到一张RGBA4444的纹理中时，byte[]
        //里的字节顺序应该是  UVUVUVUV....
        //这样在shader中纹理采样的结果 U 分量就存在r、g通道。
        //V 分量就存在b、a通道。

        for (int i = 0; i < bufUV.Length; i += 2)
        {
            bufUV[i] = bufU[i >> 1];
            bufUV[i + 1] = bufV[i >> 1];
        }

        //如果不反转数组，得到的图像就是上下颠倒的
        //建议不在这里反转，因为反转数组还是挺耗性能的，
        //应该到shader中去反转一下uv坐标即可
        //Array.Reverse(bufY);
        //Array.Reverse(bufU);
        //Array.Reverse(bufV);
        //Array.Reverse(bufUV);
    }*/

}


