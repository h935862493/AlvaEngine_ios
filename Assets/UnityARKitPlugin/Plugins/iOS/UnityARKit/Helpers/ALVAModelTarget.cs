using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using arsdk;
using System.IO;
using Assets.AlvaAR.arsdk;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ALVAModelTarget : MonoBehaviour
{
    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    public Material m_ClearMaterial;


    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
    public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    public bool enableAutoFocus = true;

    private string modelPath;
    public string abFolderName = "Alva_MT_ios";
    public string xmlName = "moto.xml";


    public int found = 0;

    bool isHideLoadUI = true;

    AlvaARUnity.AlvaRotation rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_0;

    float scalex = 1.0f, scaley = 1.0f, offx = 0.0f, offy = 0.0f;

    public GameObject model;

    public GameObject black, scan;

    public UnityEvent foundEvent, lostEvent;

    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private CommandBuffer m_VideoCommandBuffer;

    private bool bCommandBufferInitialized;

    //private bool isFirstFound = true;
    IntPtr guideimageIntP;
    public Image guideImage;
    bool isFirstGetInt = true;
    bool isGuideImageOK = false;
    public GameObject TipsBg;
    public Text tip;
    int preFound = -1;
    public void SetMaterialProperty(int imageWidth, int imageHeight)
    {
        if (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeRight)//横屏
        {
            float sx = Screen.width / (float)imageWidth, sy = Screen.height / (float)imageHeight;
            if (sx > sy)
            {
                var totalSize = imageHeight * sx;
                scaley = sy / sx;
                offy = (totalSize - Screen.height) / 2.0f / totalSize;
                //Debug.Log("--scale scaley" + scaley);
                //Debug.Log("--scale offy" + offy);
            }
            else
            {
                var totalSize = imageWidth * sy;
                scalex = sx / sy;
                offx = (totalSize - Screen.width) / 2.0f / totalSize;
                //Debug.Log("--scale scalex" + scalex);
                //Debug.Log("--scale offx" + offx);
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
                //Debug.Log("++scale scaley" + scaley);
                //Debug.Log("++scale offy" + offy);
            }
            else
            {
                scalex = sx / sy;
                var totalSize = imageWidth * sy;
                offx = (totalSize - Screen.height) / 2.0f / totalSize;
                //Debug.Log("++scale scalex" + scalex);
                //Debug.Log("++scale offx" + offx);
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
        m_session.SetArType(2, (int)rotation);

        string auth = "";
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("AlvaModelTarget")))
        {
            auth = GlobalData.modelAuth;
        }
        else
        {
            auth = PlayerPrefs.GetString("AlvaModelTarget");
        }

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
            guideimageIntP = Marshal.AllocHGlobal(800 * 480 * 4);
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

        //Debug.Log("11");
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
        //Debug.Log("22");
        m_session.UnitMT();
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
                //Debug.Log("77");
            }
            //Debug.Log("88");
        }
        //Debug.Log("99");
        bCommandBufferInitialized = false;
        // Debug.Log("00");
    }

    private void OnEnable()
    {
        // Debug.Log("OnEnable called");
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
            Debug.Log("config.IsSupported");
            m_session.RunWithConfig(config);

            SetALVACoreInfo();
            // UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;

        }
    }

    private void Start()
    {
        //  Debug.Log("Start");

        bCommandBufferInitialized = false;
        //Application.targetFrameRate = 30;
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
            //print("-----------m_session.IsInitSucceed():" + n);
            if (n == -2146850802 || n == -2146981874)
            {
                if (TipsBg)
                {
                    tip.text = "特征版本号不匹配" + n;
                    TipsBg.SetActive(true);
                }
            }
        }
    }

    void FirstFrameUpdate(UnityARCamera unityARCamera)
    {
        //Debug.Log("FirstFrameUpdate");
        //sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;
        //Debug.Log("FirstFrameUpdate1");
        if (!bCommandBufferInitialized)
        {
            InitializeCommandBuffer();
        }
        //Debug.Log("FirstFrameUpdate11");
        SetMaterialProperty(unityARCamera.imageWidth, unityARCamera.imageHeight);
        //Debug.Log("FirstFrameUpdate111");
    }


    private float[] MatrixToFloatArray(Matrix4x4 mx)
    {
        float[] viewMatrix = {
                         mx.m00,  mx.m10,   mx.m20 , mx.m30,
                         mx.m01,  mx.m11,   mx.m21,  mx.m31,
                         mx.m02,  mx.m12,   mx.m22 , mx.m32,
                         mx.m03,  mx.m13,   mx.m23,  mx.m33};

        return viewMatrix;
    }


    private void ARFrameUpdate(UnityARCamera unityARCamera)
    {
        /*
        if (guideImage && !isGuideImageOK && m_session.IsInitARSDK())
        {
            isGuideImageOK = true;
            print("//////////////m_session.IsInitARSDK():获取缩略图");
            var tex = new Texture2D(800, 480, TextureFormat.ARGB32, false);
            tex.LoadRawTextureData(guideimageIntP, 800 * 480 * 4);
            tex.Apply();
            guideImage.sprite = GlobalData.Texture2DToSprite(tex);
            if (Screen.width > Screen.height)
            {
                guideImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
                guideImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.height / 480f * 800f);
            }
            else
            {
                guideImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
                guideImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width / 800f * 480f);

            }
            Marshal.FreeHGlobal(guideimageIntP);
            guideImage.gameObject.SetActive(true);

        }
        */

        if (!bCommandBufferInitialized)
        {
            InitializeCommandBuffer();
        }

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
            isHideLoadUI = false;
            black.SetActive(false);
        }

        if (unityARCamera.isFoundModel == 1)
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                //Debug.Log("SetTargetActive true preFound=" + preFound + "isFound=" + isFound);
                preFound = unityARCamera.isFoundModel;
                //if (guideImage)
                //guideImage.gameObject.SetActive(false);
                scan.SetActive(false);
                model.SetActive(true);
                if (foundEvent != null)
                {
                    foundEvent.Invoke();
                }
            }

            //if (isFirstFound)
            //{
            //    isFirstFound = false;
            //    scan.SetActive(false);
            //    //if (guideImage)
            //    //{
            //    //    guideImage.gameObject.SetActive(false);
            //    //}
            //    if (foundEvent != null)
            //    {
            //        foundEvent.Invoke();
            //    }
            //}
            UpdateModelByModeltarget(unityARCamera.viewMatrix, unityARCamera.projectionMatrix);
        }
        else
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                //if (guideImage)
                //guideImage.gameObject.SetActive(true);
                scan.SetActive(true);
                if (lostEvent != null)
                {
                    lostEvent.Invoke();
                }
                model.SetActive(false);
            }
        }

        _videoTextureY.UpdateExternalTexture(handles.TextureY);
        _videoTextureCbCr.UpdateExternalTexture(handles.TextureCbCr);
        //Debug.Log("finish");
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


    private void UpdateModelByModeltarget(UnityARMatrix4x4 viewMatrix, UnityARMatrix4x4 projectMatrix)
    {


        float[] modelMatrix = new float[]{
                1,0,0,0,
                0,1,0,0,
                0,0,1,0,
                0,0,0,1
            };



        Matrix4x4 newCameraViewMatrix = UnityARMatrixOps.GetMatrix(viewMatrix);//MatrixExtensions.RowMajorArrayToMatrix(cameraViewMatrix);
                                                                               //Debug.Log("newCameraViewMatrix" + newCameraViewMatrix);
                                                                               //Debug.Log("newCameraViewMatrix" + newCameraViewMatrix);
        Matrix4x4 modelViewMatrix = MatrixExtensions.ColMajorArrayToMatrix(modelMatrix);

        //Debug.Log("modelViewMatrix" + modelViewMatrix);

        Matrix4x4 newcameraProjectMatrix = UnityARMatrixOps.GetMatrix(projectMatrix);//MatrixExtensions.RowMajorArrayToMatrix(cameraProjectMatrix);

        //Debug.Log("newcameraProjectMatrix" + newcameraProjectMatrix);

        var mv = newCameraViewMatrix * modelViewMatrix;
        var newModelViewMatrix = m_camera.worldToCameraMatrix.inverse * mv;
        // Debug.Log("newModelViewMatrix"+ newModelViewMatrix);


        var rotation = MatrixExtensions.ExtractRotation(newModelViewMatrix);
        //  Debug.Log("rotation" + rotation);
        var position = MatrixExtensions.ExtractPosition(newModelViewMatrix);
        // Debug.Log("position" + position);
        var scale = MatrixExtensions.ExtractScale(newModelViewMatrix);
        //Debug.Log("scale" + scale);
        model.transform.rotation = rotation;
        model.transform.position = position;
        model.transform.localScale = scale;

        m_camera.projectionMatrix = newcameraProjectMatrix;
    }

}
