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


public class ImageTargetCameraManager : MonoBehaviour {

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    public Material m_ClearMaterial;


    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
    public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    public bool enableAutoFocus = true;

    private string modelPath;
    public string abFolderName = "Alva_FDJ_ios";
    public string xmlName = "test.xml";


    public int found = 0;

    bool isHideLoadUI = true;

    AlvaARUnity.AlvaRotation rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_0;




    float scalex = 1.0f, scaley = 1.0f, offx = 0.0f, offy = 0.0f;

    public GameObject model;

    public GameObject black, scan;


    public UnityEvent foundEvent, LostEvent;


    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private CommandBuffer m_VideoCommandBuffer;

    private bool bCommandBufferInitialized;



    private bool isFirstFound = true;
    IntPtr guideimageIntP;
    bool isFirstGetInt = true;
    public GameObject TipsBg;
    public Text tip;
    int preFound = 0;
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
        //Debug.Log("Awake");
        black.SetActive(true);
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

        string auth = PlayerPrefs.GetString("AlvaImageTarget");
     

        if (GlobalData.ProjectID == "3d")
        {
            modelPath = GlobalData.LocalPath + abFolderName + "/" + xmlName;
        }
        else
            modelPath = GlobalData.LocalPath + GlobalData.ProjectID + "/" + abFolderName + "/" + xmlName;


        //print("modelPath=" + modelPath);
        //print("auth=" + auth);

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
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
       // Debug.Log("11");
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
       // Debug.Log("22");
        m_session.UnitIR();
       // Debug.Log("33");

        m_session.Pause();
       // Debug.Log("44");
        if (m_VideoCommandBuffer != null)
        {
           // Debug.Log("55");
            if (m_camera != null)
            {
               // Debug.Log("66");
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_VideoCommandBuffer);
               // Debug.Log("77");
            }
           // Debug.Log("88");
        }
       // Debug.Log("99");
        bCommandBufferInitialized = false;
       // Debug.Log("00");
    }

    private void OnEnable()
    {
      //  Debug.Log("OnEnable called");
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       // Debug.Log("OnSceneLoaded: " + scene.name);
       // Debug.Log(mode);
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        var config = sessionConfiguration;
        if (config.IsSupported)
        {
           // Debug.Log("config.IsSupported");
            m_session.RunWithConfig(config);

            SetALVACoreInfo();
            // UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;

        }
    }

    private void Start()
    {
       // Debug.Log("Start");

        bCommandBufferInitialized = false;
        Application.targetFrameRate = 30;
        if (m_camera == null)
        {
            m_camera = Camera.main;
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
                if (TipsBg)
                {
                    tip.text = "特征版本号不匹配" + n;
                    TipsBg.SetActive(true);
                }
            }
        }
    }
    


    private void ARFrameUpdate(UnityARCamera unityARCamera)
    {

        if (!bCommandBufferInitialized)
        {
            InitializeCommandBuffer();
        }


        // Debug.Log("SetMaterialProperty");
        // Debug.Log("2");
        ARTextureHandles handles = m_session.GetARVideoTextureHandles();

        if (handles.IsNull())
        {
            Debug.Log("ARTextureHandlesIsNull");
            return;

        }

        // Texture Y
        if (_videoTextureY == null)
        {
            _videoTextureY = Texture2D.CreateExternalTexture(unityARCamera.imageWidth, unityARCamera.imageHeight,
                TextureFormat.R8, false, false, handles.TextureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
        }
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

        // Debug.Log("ARFrameUpdate");
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

        //Debug.Log(cam.isFoundModel);
        if (unityARCamera.isFoundModel == 1)
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(false);
                model.SetActive(true);
                if (foundEvent != null)
                {
                    foundEvent.Invoke();
                }
            }

            //if (isFirstFound)
            //{
            //    //Debug.Log("isFirstFound");
            //    isFirstFound = false;

            //    scan.SetActive(false);
            //    if (foundEvent != null)
            //    {
            //        foundEvent.Invoke();
            //    }
            //}

            //SetModelByMatrix(UnityARMatrixOps.GetMatrix(unityARCamera.modelMatrix));
            UpdateModelByImagetarget(unityARCamera.viewMatrix, unityARCamera.projectionMatrix);
        }
        else
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(true);
                if (LostEvent != null)
                {
                    LostEvent.Invoke();
                }
                model.SetActive(false);
            }
        }
        
        _videoTextureY.UpdateExternalTexture(handles.TextureY);
        _videoTextureCbCr.UpdateExternalTexture(handles.TextureCbCr);
    }
    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public string GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString();

    }

    void InitializeCommandBuffer()
    {
        m_VideoCommandBuffer = new CommandBuffer();
        m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, m_ClearMaterial);
        m_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
        m_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_VideoCommandBuffer);
        bCommandBufferInitialized = true;

    }

    private void UpdateModelByImagetarget(UnityARMatrix4x4 viewMatrix, UnityARMatrix4x4 projectMatrix)
    {


        float[] modelMatrix = new float[]{
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                0,0,0,1
            };



        Matrix4x4 newCameraViewMatrix = UnityARMatrixOps.GetMatrix(viewMatrix);


        Matrix4x4 modelViewMatrix = MatrixExtensions.ColMajorArrayToMatrix(modelMatrix);

        //Debug.Log("modelViewMatrix" + modelViewMatrix);

        Matrix4x4 newcameraProjectMatrix = UnityARMatrixOps.GetMatrix(projectMatrix);//MatrixExtensions.RowMajorArrayToMatrix(cameraProjectMatrix);

        //Debug.Log("newcameraProjectMatrix" + newcameraProjectMatrix);

        var mv = newCameraViewMatrix * modelViewMatrix;
        var newModelViewMatrix = m_camera.worldToCameraMatrix.inverse * mv;
        // Debug.Log("newModelViewMatrix"+ newModelViewMatrix);


        var rotation = MatrixExtensions.ExtractRotation(newModelViewMatrix);
        //Debug.Log("aaarotation" + rotation);
        var position = MatrixExtensions.ExtractPosition(newModelViewMatrix);
        //Debug.Log("aaaposition" + position);
        var scale = MatrixExtensions.ExtractScale(newModelViewMatrix);
        //Debug.Log("aaascale" + scale);
        model.transform.rotation = rotation;
        model.transform.position = position;
        model.transform.localScale = scale;

        // m_camera.worldToCameraMatrix = newCameraViewMatrix;
        m_camera.projectionMatrix = newcameraProjectMatrix;
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

        model.transform.position = MatrixExtensions.ExtractPosition(final);
        model.transform.rotation = MatrixExtensions.ExtractRotation(final);
        model.transform.localScale = MatrixExtensions.ExtractScale(final);
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


    
}

