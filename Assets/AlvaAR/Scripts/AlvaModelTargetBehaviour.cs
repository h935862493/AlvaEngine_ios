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

public class AlvaModelTargetBehaviour : MonoBehaviour
{

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    public Material m_ClearMaterial;


    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
    public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    public bool enableAutoFocus = true;

    private string modelPath;

    private ModelRecognition modelRecognition;
    public GameObject modelTarget;


    public int found = 0;

    bool isHideLoadUI = true;

    AlvaARUnity.AlvaRotation rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_0;


    float scalex = 1.0f, scaley = 1.0f, offx = 0.0f, offy = 0.0f;


    public GameObject black, scan;
    public Image guideImage;
    IntPtr guideimageIntP;
    public UnityEvent foundEvent, returnEvent;


    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private CommandBuffer m_VideoCommandBuffer;

    private bool bCommandBufferInitialized;


    bool isFirstGetInt = true;
    private bool isFirstFound = true;

    public List<string> alst = new List<string>();
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
        var a = GameObject.FindObjectsOfType<Camera>();

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].transform.root.name != "AlvaCore_ModelTarget(Clone)")
            {
                //print("Destroy gameobject:" + a[i].name);
                Destroy(a[i].gameObject);
            }
        }
        var e = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 0; i < e.Length; i++)
        {
            if (e[i].transform.root.name != "AlvaCore_ModelTarget(Clone)")
            {
                //print("删除物体" + e[i].gameObject.name);
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
        m_session.SetArType(2, (int)rotation);

        if (File.Exists(modelPath) && MyMessageData.alvaLicense != "")
        {
            guideimageIntP = Marshal.AllocHGlobal(800 * 480 * 4);
            m_session.SetALVACoreInfo(MyMessageData.companyName, MyMessageData.alvaLicense, modelPath, guideimageIntP);
        }
        else
        {
            Debug.Log("SetALVACoreInfo error");
        }
    }

    private void OnDestroy()
    {    
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
        m_session.UnitMT();
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

    private void Start()
    {
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        bCommandBufferInitialized = false;

        if (m_camera == null)
        {
            m_camera = Camera.main;
        }

        modelRecognition = FindObjectOfType<ModelRecognition>();
        if (modelRecognition == null)
        {
            ARSceneComonUI.instance.ShowTip("没有挂载ModelRecognition的物体，请在编辑器确认");
            return;
        }

        modelTarget = modelRecognition.gameObject;
        if (modelTarget && modelRecognition.OnTargetLost != null)
        {
            modelRecognition.OnTargetLost.Invoke();
        }

        //找特征文件
        //string xmlTemp = GlobalData.LocalPath + GlobalData.ProjectID + "/" + Path.GetFileNameWithoutExtension(modelRecognition.path.Replace("\\", "/"));
        string xmlTemp = MyMessageData.ProjectPath + "/" + Path.GetFileNameWithoutExtension(modelRecognition.path.Replace("\\", "/"));
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
                //print("GetDirectories xml is null:" + GlobalData.LocalPath + GlobalData.ProjectID);
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
        var config = sessionConfiguration;
        if (config.IsSupported)
        {
            //Debug.Log("config.IsSupported");
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
            if (n == -2146850802 || n == -2146981874)
            {
                //print("特征版本号不匹配");
                ARSceneComonUI.instance.ShowTip("特征版本号不匹配" + n);
            }
        }
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


    public void GetFiles(string dir)
    {
        try
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                string exname = file.Substring(file.LastIndexOf(".") + 1);
                if (".xml".IndexOf(file.Substring(file.LastIndexOf(".") + 1)) > -1)
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
    bool isGuideImageOK = false;
    private void ARFrameUpdate(UnityARCamera unityARCamera)
    {
        /*
        if (!isGuideImageOK && m_session.IsInitARSDK())
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
        //Debug.Log("unityARCamera.isFoundModel =" + unityARCamera.isFoundModel );
        if (unityARCamera.isFoundModel == 1)
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                //Debug.Log("SetTargetActive true preFound=" + preFound + "isFound=" + isFound);
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(false);
                //if (guideImage)
                //guideImage.gameObject.SetActive(false);
                modelTarget.SetActive(true);
                if (modelRecognition != null && modelRecognition.OnTargetFound != null)
                {
                    modelRecognition.OnTargetFound.Invoke();
                }
            }

            if (isFirstFound)
            {
                isFirstFound = false;
                //if (guideImage)
                //{
                //guideImage.gameObject.SetActive(false);
                //}
                if (foundEvent != null)
                {
                    foundEvent.Invoke();
                }
                modelRecognition.Found();
                //if (modelRecognition.OnTargetFound != null)
                //{
                //    modelRecognition.OnTargetFound.Invoke();
                //}
            }
            UpdateModelByModeltarget(unityARCamera.viewMatrix, unityARCamera.projectionMatrix);
        }
        else
        {
            if (preFound != unityARCamera.isFoundModel)
            {
                preFound = unityARCamera.isFoundModel;
                scan.SetActive(true);
                
                //if (guideImage)
                //guideImage.gameObject.SetActive(true);

                if (modelRecognition != null && modelRecognition.OnTargetLost != null)
                {
                    modelRecognition.OnTargetLost.Invoke();
                }
                modelTarget.SetActive(false);
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
        modelTarget.transform.rotation = rotation;
        modelTarget.transform.position = position;
        modelTarget.transform.localScale = scale;

        m_camera.projectionMatrix = newcameraProjectMatrix;
    }
}
