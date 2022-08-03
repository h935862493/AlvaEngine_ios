using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Runtime.InteropServices;
using arsdk;
using System.IO;
using UnityEngine.UI;
using Assets.AlvaAR.arsdk;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Alva.Recognition;
using System.Collections.Generic;

public class AlvaImageTargetBehaviour : MonoBehaviour
{

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    public Material m_ClearMaterial;

    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
    public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    public bool enableAutoFocus = true;
    private string modelPath;
    public int found = 0;
    bool isHideLoadUI = true;
    AlvaARUnity.AlvaRotation rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_0;

    float scalex = 1.0f, scaley = 1.0f, offx = 0.0f, offy = 0.0f;

   // public GameObject model;

    public GameObject black, scan;

    public UnityEvent foundEvent;

    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private CommandBuffer m_VideoCommandBuffer;

    private bool bCommandBufferInitialized;

    private bool isFirstFound = true;

    private ImageRecognition imageRecognition;
    public GameObject imageTarget, imageARTarget;
    public List<string> alst = new List<string>();
    IntPtr guideimageIntP;
    bool isFirstGetInt = true;
    int preFound = -1;
    public void SetMaterialProperty(int imageWidth, int imageHeight)
    {
        //Debug.Log("scale Screen.width" + Screen.width);
        //Debug.Log("scale Screen.height" + Screen.height);
        if (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeRight)//横屏
        {
            float sx = Screen.width / (float)imageWidth, sy = Screen.height / (float)imageHeight;
            if (sx > sy)
            {
                var totalSize = imageHeight * sx;
                scaley = sy / sx;
                offy = (totalSize - Screen.height) / 2.0f / totalSize;
            }
            else
            {
                var totalSize = imageWidth * sy;
                scalex = sx / sy;
                offx = (totalSize - Screen.width) / 2.0f / totalSize;

            }
        }
        else
        {
            //竖屏情况 Screen.width=1080 Screen.height=2232    
            float sx = Screen.height / (float)imageWidth, sy = Screen.width / (float)imageHeight;
            if (sx > sy)
            {
                scaley = sy / sx;
                var totalSize = imageHeight * sx;
                offy = (totalSize - Screen.width) / 2.0f / totalSize;
                //Debug.Log("scale scaley" + scaley);
                //Debug.Log("scale offy" + offy);
            }
            else
            {
                scalex = sx / sy;
                var totalSize = imageWidth * sy;
                offx = (totalSize - Screen.height) / 2.0f / totalSize;
                //Debug.Log("scale scalex" + scalex);
                //Debug.Log("scale offx" + offx);
            }
        }
    }

    public ARKitWorldTrackingSessionConfiguration sessionConfiguration
    {
        get
        {
            ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
            //Debug.Log(config);
            config.planeDetection = planeDetection;
            // config.alignment = startAlignment;
            // config.getPointCloudData = getPointCloud;
            // config.enableLightEstimation = enableLightEstimation;
            config.enableAutoFocus = enableAutoFocus;
            config.environmentTexturing = environmentTexturing;
            return config;
        }
    }

    private void Awake()
    {
       // Debug.Log("Awake");
        imageARTarget.transform.parent = null;
        black.SetActive(true);
        var a = GameObject.FindObjectsOfType<Camera>();

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].transform.root.name != "AlvaCore_ImageTarget(Clone)")
            {
               // print("Destroy gameobject:" + a[i].name);
                Destroy(a[i].gameObject);
            }
        }
        var e = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 0; i < e.Length; i++)
        {
            if (e[i].transform.root.name != "AlvaCore_ImageTarget(Clone)")
            {
               // print("删除物体" + e[i].gameObject.name);
                Destroy(e[i].gameObject);
            }
        }
    }

    private void SetALVACoreInfo()
    {

        switch (Screen.orientation)
        {
            case ScreenOrientation.Portrait:
                break;
            case ScreenOrientation.LandscapeLeft:
                rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_90;
                break;
            case ScreenOrientation.PortraitUpsideDown:
                rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_180;
                break;
            case ScreenOrientation.LandscapeRight:
                rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_270;
                break;
        }

        //设置识别类型
        m_session.SetArType(1, (int)rotation);

        string auth = MyMessageData.alvaLicense;

        if (File.Exists(modelPath) && auth != "")
        {
            m_session.SetALVACoreInfo(MyMessageData.companyName, auth, modelPath, guideimageIntP);
        }
        else
        {
            Debug.Log("SetALVACoreInfo error");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
        m_session.UnitIR();
        m_session.Pause();
        if (m_VideoCommandBuffer != null)
        {
            if (m_camera != null)
            {
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_VideoCommandBuffer);
            }
        }
        bCommandBufferInitialized = false;
    }

    private void OnEnable()
    {
       // Debug.Log("OnEnable called");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void Start()
    {
        bCommandBufferInitialized = false;
        Application.targetFrameRate = 30;
        if (m_camera == null)
        {
            m_camera = Camera.main;
        }

        imageRecognition = FindObjectOfType<ImageRecognition>();

        if (imageRecognition == null)
        {
            ARSceneComonUI.instance.ShowTip("没有挂载ImageRecognition的物体，请在编辑器确认");
            return;
        }

        imageTarget = imageRecognition.transform.parent.gameObject;
        imageTarget.transform.parent = imageARTarget.transform.GetChild(0);
        imageTarget.transform.localPosition = Vector3.zero;
        imageTarget.transform.localEulerAngles = Vector3.zero;

        if (imageTarget && imageRecognition.OnTargetLost != null)
        {
            imageRecognition.OnTargetLost.Invoke();
        }

        //找特征文件
        string xmlTemp = MyMessageData.ProjectPath + "/" + Path.GetFileNameWithoutExtension(imageRecognition.path.Replace("\\", "/"));
        Debug.Log("xmlTemp=" + xmlTemp);

        if (Directory.Exists(xmlTemp))
        {
            GetFiles(xmlTemp);
        }
        else
        {
            var t = Directory.GetDirectories(MyMessageData.ProjectPath);
            if (t.Length > 0)
            {
                GetFiles(t[0]);
            }
            else
            {
                //print("没找到特征文件夹啊" + GlobalData.LocalPath + GlobalData.ProjectID);
                ARSceneComonUI.instance.ShowTip("没找到特征文件夹，请在编辑器确认");
                return;
            }
        }

        if (alst.Count == 0)
        {
            ARSceneComonUI.instance.ShowTip("没找到特征文件，请在编辑器确认");
            return;
        }

        modelPath = alst[0];
        Debug.Log("modelPath=" + modelPath);
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        var config = sessionConfiguration;
        if (config.IsSupported)
        {
            m_session.RunWithConfig(config);

            SetALVACoreInfo();
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;

        }

       
    }



    
    private void Update()
    {
        if (isFirstGetInt && m_session.IsInitSucceed() != -1)
        {

            isFirstGetInt = false;
            int n = m_session.IsInitSucceed();
            if (n == -2147047410 || n == -2146981874 || n == -2146916338 || n == -2146850802)
            {
                //print("特征版本号不匹配");
                ARSceneComonUI.instance.ShowTip("特征版本号不匹配"+n);
            }
        }
    }
    
    private void ARFrameUpdate(UnityARCamera unityARCamera)
    {
        
        

        if (!bCommandBufferInitialized)
        {
            InitializeCommandBuffer();
        }


       //  Debug.Log("SetMaterialProperty");
       //  Debug.Log("2");
        ARTextureHandles handles = m_session.GetARVideoTextureHandles();

        if (handles.IsNull())
        {
            Debug.Log("ARTextureHandlesIsNull");
            return;

        }

       // Debug.Log("3");

        // Texture Y
        if (_videoTextureY == null)
        {
            _videoTextureY = Texture2D.CreateExternalTexture(unityARCamera.imageWidth, unityARCamera.imageHeight,
                TextureFormat.R8, false, false, handles.TextureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
        }
      //  Debug.Log("4");
        //Debug.Log("_videoTextureY"+ _videoTextureY == null);
        // Texture CbCr
        if (_videoTextureCbCr == null)
        {
            _videoTextureCbCr = Texture2D.CreateExternalTexture(unityARCamera.imageWidth, unityARCamera.imageHeight,
                TextureFormat.RG16, false, false, (System.IntPtr)handles.TextureCbCr);
            _videoTextureCbCr.filterMode = FilterMode.Bilinear;
            _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
            m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
        }

         //Debug.Log("ARFrameUpdate");
        // Debug.Log("unityARCamera.imageWidth " + unityARCamera.imageWidth);
        SetMaterialProperty(unityARCamera.imageWidth, unityARCamera.imageHeight);

        m_ClearMaterial.SetFloat("_ScaleX", scalex);
        m_ClearMaterial.SetFloat("_ScaleY", scaley);
        m_ClearMaterial.SetFloat("_OffsetX", offx);
        m_ClearMaterial.SetFloat("_OffsetY", offy);
        m_ClearMaterial.SetInt("rotation", (int)rotation);

        if (isHideLoadUI)
        {
            //Debug.Log("isHideLoadUI");
            isHideLoadUI = false;
            black.SetActive(false);
            scan.SetActive(true);
        }

        //print("*****unityARCamera.isFoundModel:" + unityARCamera.isFoundModel);
        if (unityARCamera.isFoundModel == 1)
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(false);
                imageARTarget.SetActive(true);
                if (imageRecognition.OnTargetFound != null)
                    imageRecognition.OnTargetFound.Invoke();
            }

            if (isFirstFound)
            {
                isFirstFound = false;

                if (foundEvent != null)
                {
                    foundEvent.Invoke();
                }
                imageRecognition.Found();
                
            }
            //  SetModelByMatrix(UnityARMatrixOps.GetMatrix(unityARCamera.modelMatrix));

            UpdateModelByImagetarget(unityARCamera.viewMatrix, unityARCamera.projectionMatrix);
        }
        else
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(true);
                if (imageRecognition.OnTargetLost != null)
                    imageRecognition.OnTargetLost.Invoke();
                imageARTarget.SetActive(false);
            }

        }
        _videoTextureY.UpdateExternalTexture(handles.TextureY);
        _videoTextureCbCr.UpdateExternalTexture(handles.TextureCbCr);
       // Debug.Log("finish");
    }
  
    void InitializeCommandBuffer()
    {
        m_VideoCommandBuffer = new CommandBuffer();
        m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, m_ClearMaterial);
        m_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
        m_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_VideoCommandBuffer);
        bCommandBufferInitialized = true;

    }

    Matrix4x4  newModelViewMatrixUp;

    private void UpdateModelByImagetarget(UnityARMatrix4x4 viewMatrix, UnityARMatrix4x4 projectMatrix)
    {


        float[] modelMatrix = new float[]{
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                0,0,0,1
            };


        //Debug.Log("newcameraProjectMatrix" + newcameraProjectMatrix);

         newModelViewMatrixUp = m_camera.worldToCameraMatrix.inverse * UnityARMatrixOps.GetMatrix(viewMatrix) * MatrixExtensions.ColMajorArrayToMatrix(modelMatrix);
        // Debug.Log("newModelViewMatrix"+ newModelViewMatrix);


        imageARTarget.transform.rotation = MatrixExtensions.ExtractRotation(newModelViewMatrixUp);
        //Debug.Log("aaarotation" + rotation);
        imageARTarget.transform.position = MatrixExtensions.ExtractPosition(newModelViewMatrixUp);
        //Debug.Log("aaaposition" + position);
        imageARTarget.transform.localScale = MatrixExtensions.ExtractScale(newModelViewMatrixUp);
        
       // m_camera.worldToCameraMatrix = newCameraViewMatrix;
        m_camera.projectionMatrix = UnityARMatrixOps.GetMatrix(projectMatrix);

        //print(imageARTarget.name+ ": localScale:" + imageARTarget.transform.localScale);
    }

    private Matrix4x4 FloatArrayToMatrix(float[] viewMatrix)
    {
        Matrix4x4 mx = new Matrix4x4();

        mx.m00 = viewMatrix[0]; mx.m01 = viewMatrix[4]; mx.m02 = viewMatrix[8]; mx.m03 = viewMatrix[12];
        mx.m10 = viewMatrix[1]; mx.m11 = viewMatrix[5]; mx.m12 = viewMatrix[9]; mx.m13 = viewMatrix[13];
        mx.m20 = viewMatrix[2]; mx.m21 = viewMatrix[6]; mx.m22 = viewMatrix[10]; mx.m23 = viewMatrix[14];
        mx.m30 = viewMatrix[3]; mx.m31 = viewMatrix[7]; mx.m32 = viewMatrix[11]; mx.m33 = viewMatrix[15];

        return mx;
    }

    private Matrix4x4 RowFloatArrayToMatrix(float[] viewMatrix)
    {
        Matrix4x4 mx = new Matrix4x4();

        mx.m00 = viewMatrix[0]; mx.m01 = viewMatrix[4]; mx.m02 = viewMatrix[8]; mx.m03 = viewMatrix[12];
        mx.m10 = viewMatrix[1]; mx.m11 = viewMatrix[5]; mx.m12 = viewMatrix[9]; mx.m13 = viewMatrix[13];
        mx.m20 = viewMatrix[2]; mx.m21 = viewMatrix[6]; mx.m22 = viewMatrix[10]; mx.m23 = viewMatrix[14];
        mx.m30 = viewMatrix[3]; mx.m31 = viewMatrix[7]; mx.m32 = viewMatrix[11]; mx.m33 = viewMatrix[15];

        return mx;
    }
    public void SetModelByMatrix(Matrix4x4 mat)
    {
        Matrix4x4 dd = Matrix4x4.identity;
        dd.m00 = 0;
        dd.m01 = -1;
        dd.m10 = 1;
        dd.m11 = 0;

        Matrix4x4 abcd = Matrix4x4.identity;
        abcd.m00 = -1;

        var final = MatrixZZ(mat * dd * abcd);

        imageARTarget.transform.position = MatrixExtensions.ExtractPosition(final);
        imageARTarget.transform.rotation = MatrixExtensions.ExtractRotation(final);
        imageARTarget.transform.localScale = MatrixExtensions.ExtractScale(final);
        //text.text = m_camera.transform.parent.localPosition.ToString()
        //+ "--" + m_camera.transform.parent.localEulerAngles.ToString()
        //+ "--" + m_camera.transform.parent.localScale.ToString()+"****"+m_camera.worldToCameraMatrix;

    }
    private Matrix4x4 MatrixZZ(Matrix4x4 viewMatrix)
    {
        Matrix4x4 mx = new Matrix4x4();

        mx.m00 = viewMatrix.m00; mx.m01 = viewMatrix.m10; mx.m02 = viewMatrix.m20; mx.m03 = viewMatrix.m30;
        mx.m10 = viewMatrix.m01; mx.m11 = viewMatrix.m11; mx.m12 = viewMatrix.m21; mx.m13 = viewMatrix.m31;
        mx.m20 = viewMatrix.m02; mx.m21 = viewMatrix.m12; mx.m22 = viewMatrix.m22; mx.m23 = viewMatrix.m32;
        mx.m30 = viewMatrix.m03; mx.m31 = viewMatrix.m13; mx.m32 = viewMatrix.m23; mx.m33 = viewMatrix.m33;


        return mx;
    }

    public void GetFiles(string dir)
    {
       // print("GetFiles:" + dir);
        try
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                string exname = file.Substring(file.LastIndexOf(".") + 1);//
                if (".xml".IndexOf(file.Substring(file.LastIndexOf(".") + 1)) > -1)//
                {
                    FileInfo fi = new FileInfo(file);
                    alst.Add(fi.FullName);
                }
            }
        }
        catch
        {
        }
    }

}

