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

public class UnityARCameraManager : MonoBehaviour {

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    public Material m_ClearMaterial;

 
    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
    public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    public bool enableAutoFocus = true;
	
    private string modelPath;
    public string abFolderName = "motuo2_2";
    public string xmlName = "moto.xml";
    private IntPtr modeltarget = IntPtr.Zero;

    private int isAddAnchor = 0;
    public ColoredAnchor modelAnchor_ = new ColoredAnchor();
    private int anchorAddIndex = -1;

    public int found = 0;
    public int anchorAddisUse = 0;
    public int[] anchorIsDelete = { 0 };
    // public  int frameFound = 0;
    public float[] anchorMQT = { 0, 0, 0, 0, 0, 0, 0 };

    bool isFirstSetMt = true,isHideLoadUI=true;

    int  result, frameFound;

    private IntPtr[] backgrounddata = new IntPtr[2];
    // private IntPtr yBuffer = IntPtr.Zero;
    // private IntPtr uvBuffer = IntPtr.Zero;

    private IntPtr[] cameraData = new IntPtr[2];

    float scalex = 1.0f, scaley = 1.0f, offx = 0.0f, offy = 0.0f;
  
    public GameObject model;
    bool isFirst = true;
    public GameObject black,scan;
 
    float[] viewMatrix = new float[16];
    public UnityEvent foundEvent, returnEvent;
    

    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private CommandBuffer m_VideoCommandBuffer;
    
    private bool bCommandBufferInitialized;
    AlvaARUnity.AlvaRotation rotation = AlvaARUnity.AlvaRotation.Alva_ROTATION_0;

    public enum AlvaCamARStatus
    {
        Alva_ARTRACKING = 0, //tracking
        Alva_ARPAUSED = 1, //paused tracking
        Alva_ARSTOPPED = 2, //stopped tracking
        Alva_ARINVALID
    }

    public class ColoredAnchor
    {
        public UnityARUserAnchorData anchor;
        public int isHit;
    };

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

    private AlvaCamARStatus GetAlvaCamARStatusByTracking(ARTrackingState trackingState)
    {
        if (trackingState == ARTrackingState.ARTrackingStateNormal)
        {
            return AlvaCamARStatus.Alva_ARTRACKING;
        }
        return AlvaCamARStatus.Alva_ARSTOPPED;
    }


    public ARKitWorldTrackingSessionConfiguration sessionConfiguration
    {
        get
        {
            ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();
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
        black.SetActive(true);
    }

    private void Start () {

        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();


        bCommandBufferInitialized = false;
    

        Application.targetFrameRate = 30;
        
        var config = sessionConfiguration;
        if (config.IsSupported) {
            
            m_session.RunWithConfig (config);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
        }

        if (m_camera == null) {
            m_camera = Camera.main;
        }

        if (GlobalData.ProjectID == "3d")
        {
            modelPath = GlobalData.LocalPath + abFolderName + "/" + xmlName;
        }
        else
            modelPath = GlobalData.LocalPath + GlobalData.ProjectID + "/" + abFolderName + "/" + xmlName;


        //print("**************");
       // print("m_camera:"+m_camera.gameObject.name);
    }

    void OnDestroy()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
        AlvaARWrapper.Instance.UnitMT();
        m_session.Pause();
        if (m_VideoCommandBuffer != null)
        {
            if (m_camera)
            {
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
                m_camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_VideoCommandBuffer);
            }
        }
      
        bCommandBufferInitialized = false;
    }

    void FirstFrameUpdate(UnityARCamera cam)
    {
        //sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;
        int totalSize = cam.videoParams.yWidth * cam.videoParams.yHeight;
        //Debug.Log("1");
        cameraData[0]=Marshal.AllocHGlobal(totalSize);
        //Debug.Log(cameraData[0]);
       // Debug.Log("11");
        cameraData[1] = Marshal.AllocHGlobal(totalSize/2);
        backgrounddata[0]= Marshal.AllocHGlobal(totalSize);
        backgrounddata[1]= Marshal.AllocHGlobal(totalSize / 2);

        m_session.SetCapturePixelData(true, cameraData[0], cameraData[1]);


        if (!bCommandBufferInitialized)
        {
            InitializeCommandBuffer();
        }

        SetMaterialProperty(cam.videoParams.yWidth, cam.videoParams.yHeight);

      
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

    bool InitMT(UnityARCamera cam)
    {
        //print("初始化alva核心环境");
        // cam.videoParams.yWidth;
        string auth="";
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("AlvaModelTarget")))
        {
             auth = "8B66471AEC15B8D9173512343B6229D13F404AA1CC2425C8075111B4ED35A053468380291126065C0A3D20D146981926342CC0B602A542A18958013AE8D5EEE5BE12CD4791C5FBB3938CC45EA9CBFCB31A4BB0C5FA522199FD03F23658D5D400DEFFAD36046608092442A10826645209421204892644129124188589246252180212059002665A0106480111E6DBFDF56EFFBFFEFFBB9FEF6EF9A57E669DBBEFFFFFAFDA6EFD99A567FBF7FA77DDFBBF7EF9FFFACF57DBEF07812DE9437FD52F2FCE0DF4AD1A8518402050991643986A4A0B326888032BF42F2968376A9F146DD40776D9DB016FF14E19F595D3046C9A68C64B758ECE033E4E7C6EE6C52ED6DD";
        }
        else
        {
            auth = PlayerPrefs.GetString("AlvaModelTarget");
        }
       // print("auth:" + auth);
        int result = AlvaARWrapper.Instance.InitMT(MyMessageData.companyName, auth);

      //  Debug.Log("arsdk1 MT_Init " + result);

        if (0 == result)
        {
           
            var imageSize = new Vector2(cam.videoParams.yWidth, cam.videoParams.yHeight);
           // print("imageSize.x" + (int)imageSize.x+ "    imageSize.y" + (int)imageSize.y);
            result = AlvaARWrapper.Instance.SetMTDataInfoMemory((int)imageSize.x, (int)imageSize.y, AlvaARUnity.AlvaFormat.Alva_FMT_YUV_420sp);

           // Debug.Log("arsdk MT_setDataInfoMemory " + result);
            if (0 == result)
            {
                //Matrix4x4 mx = m_session.GetCameraProjection();

                //result = AlvaARWrapper.Instance.SetMTCameraInfo(imageSize.x, imageSize.y, cam.focalx, cam.focaly, cam.principalx, cam.principaly, Screen.width, Screen.height, MatrixToFloatArray(mx));

              
              //  Debug.Log("Screen.orientation" + Screen.orientation);
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
                result = AlvaARWrapper.Instance.SetMTCameraInfo(imageSize.x, imageSize.y, cam.focalx, cam.focaly, cam.principalx, cam.principaly, Screen.width, Screen.height, rotation);
               // result = AlvaARWrapper.Instance.SetMTCameraInfo(imageSize.x, imageSize.y, cam.focalx, cam.focaly, cam.principalx, cam.principaly, Screen.width, Screen.height, MatrixToFloatArray(mx));

                //Debug.Log("arsdk1 MT_SetCameraInfo " + result);

                if (0 == result)
                {
                    result = AlvaARWrapper.Instance.InitModelManager();
                  //  Debug.Log("arsdk1 initModelManager " + result);

                    if (0 == result)
                    {
                        if (File.Exists(modelPath))
                        {
                           // print("路径正确：" + modelPath);
                            try
                            {
                                result = AlvaARWrapper.Instance.AddModel(modelPath);
                               // Debug.Log("arsdk1 addModel " + result);
                                if (0 == result)
                                {
                                    modeltarget = AlvaARWrapper.Instance.GetModelByIndex(0);
                                    
                                    //Debug.Log("arsdk1 getModelByIndex " + (modeltarget == IntPtr.Zero ? "NULL" : "OK"));

                                }
                            }
                            catch (Exception ex)
                            {
                               // print("*******:" + ex.ToString());
                            }
                        }
                        else
                        {
                            print("路径错误;" + modelPath);
                        }
                    }
                }
            }
            else
            {
                print("初始化失败");
            }
        }
        return false;
    }


    private void SetMatrixBySlam(float[] viewMatrix)
    {
        string ss = "";
        foreach (var item in viewMatrix)
        {
            ss += ("//" + item);
        }

        float[] cameraViewMatrix = new float[]{
                                    viewMatrix[0],  viewMatrix[1],  viewMatrix[2],  viewMatrix[3],
                                    viewMatrix[4],  viewMatrix[5],  viewMatrix[6],  viewMatrix[7],
                                    viewMatrix[8],  viewMatrix[9],  viewMatrix[10], viewMatrix[11],
                                    viewMatrix[12], viewMatrix[13], viewMatrix[14], viewMatrix[15] };

        float[] modelMatrix = new float[]{1,0,0,0,
            0,1,0,0,
            0,0,1,0,
            0,0,0,1
        };

        //计算投影矩阵
        float[] cameraProjectMatrix = new float[16];
        AlvaARWrapper.Instance.ComputeProjectMatrixFixRange(Screen.width, Screen.height, m_camera.nearClipPlane, m_camera.farClipPlane, cameraProjectMatrix);

        Matrix4x4 newCameraViewMatrix = FloatArrayToMatrix(cameraViewMatrix);
        Matrix4x4 modelViewMatrix = FloatArrayToMatrix(modelMatrix);
        Matrix4x4 newCameraProjectMatrix = FloatArrayToMatrix(cameraProjectMatrix);

        var mv = newCameraViewMatrix * modelViewMatrix;

        UpdateModelMatrix(mv);
        m_camera.projectionMatrix = newCameraProjectMatrix;
    }

    /*根据识别核心调整模型位置*/
    public void UpdateModelMatrix(Matrix4x4 mv)
    {

        var newModelViewMatrix = m_camera.worldToCameraMatrix.inverse * mv;
        model.transform.localScale = MatrixExtensions.ExtractScale(newModelViewMatrix);
        var rotation = MatrixExtensions.ExtractRotation(newModelViewMatrix);
        var position = MatrixExtensions.ExtractPosition(newModelViewMatrix);
        model.transform.rotation = rotation;
        model.transform.position = position;
        //transshow.text = model.transform.position.ToString()
        //+ "--" + model.transform.eulerAngles.ToString()
        //+ "--" + model.transform.localScale.ToString();
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


    private void addPoseAnchor(float[] qt)
    {
        if (qt != null)
        {
            //print("16-16-16-isAddAnchor:" + isAddAnchor);
            if (isAddAnchor == 0)
            {

                 
                var q = new Quaternion(qt[0], qt[1], qt[2], qt[3]);
                var t = new Vector3(qt[4], qt[5], qt[6]);

                GameObject gameObject = new GameObject();
                gameObject.transform.rotation = q;
                gameObject.transform.position = t;
                UnityARUserAnchorData aranchor= UnityARUserAnchorData.UnityARUserAnchorDataFromGameObject(gameObject);
                m_session.AddUserAnchor(aranchor);
               
                modelAnchor_.isHit = 1;
                anchorAddIndex++;
                modelAnchor_.anchor = aranchor;
                isAddAnchor = 1;

            }
        }
        else
        {
            //print("18-18-18-qt=null");
        }
    }

    public bool ByteToFile(byte[] byteArray, string fileName)
    {
        bool result = false;
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
                result = true;
            }
        }
        catch
        {
            result = false;
        }
        return result;
    }

    private void ARFrameUpdate(UnityARCamera cam)
    {
       

        if (isFirstSetMt)
        {
            isFirstSetMt = false;
            InitMT(cam);
            
            return;
        }
     //  print("111111");
        AlvaCamARStatus anchorStateIndex = AlvaCamARStatus.Alva_ARSTOPPED;

        float[] anchorModel = new float[]{
                    1,0,0,0,
                    0,1,0,0,
                    0,0,1,0,
                    0,0,0,1
            };

        bool isFound = false;

        if (modelAnchor_.anchor.identifierStr!="" && isAddAnchor == 1)
        {
            //目前得不到锚点状态 暂时用相机状态代替
            anchorStateIndex = GetAlvaCamARStatusByTracking(cam.trackingState);

            if (anchorStateIndex == AlvaCamARStatus.Alva_ARTRACKING)
            {
                // Render object only if the tracking state is AR_TRACKING_STATE_TRACKING.

                var trans = UnityARMatrixOps.GetMatrix(modelAnchor_.anchor.transform);
              
                anchorModel = MatrixToFloatArray(trans);
             
            }
        }

        //Debug.Log("cam.videoParams.yWidth"+ cam.videoParams.yWidth);
        //Debug.Log("cam.videoParams.yHeight" + cam.videoParams.yHeight);
        int totalBufferSize = cam.videoParams.yWidth * cam.videoParams.yHeight ;

       // if (cameraImage == null)
       // {
          //  cameraImage = new byte[totalBufferSize*2];
      //  }

      //  Marshal.Copy(yBuffer, cameraImage, 0, totalBufferSize);
        //Debug.Log("cyBuffer");
      //  Marshal.Copy(uvBuffer, cameraImage, totalBufferSize, totalBufferSize);

        //ByteToFile(cameraImage,Application.persistentDataPath+"/aa.yuv");
        AlvaCamARStatus status = GetAlvaCamARStatusByTracking(cam.trackingState);
       

        //print("status:" + status);
        //print("cam.worldTransform" + cam.worldTransform);
        var mx = UnityARMatrixOps.GetMatrix(cam.worldTransform).inverse;
        //print("mx:" + mx);

        float[] cameraViewMatrix = MatrixToFloatArray(mx);
        //print("cameraViewMatrix:" + cameraViewMatrix);

        //print("anchorModel:" + anchorModel);
        //Debug.Log("----0000----MTMemory:" + GetTimeStamp());


        result = AlvaARWrapper.Instance.MTMemoryChannel(cameraData,  cameraViewMatrix, anchorModel, (int)status, anchorAddIndex, (int)anchorStateIndex);

     //   Debug.Log("arsdk MTMemory result " + result);

        
        if (0 == result)
        {
            
            //float[] viewMatrix = new float[66];

            result = AlvaARWrapper.Instance.GetMTBackGroundDataChannel(backgrounddata);
          //  Debug.Log("backgrounddata");

            if (_videoTextureY == null)
            {
               // Debug.Log("_videoTextureY");
                _videoTextureY = new Texture2D(cam.videoParams.yWidth, cam.videoParams.yHeight,TextureFormat.R8,false);
               
                _videoTextureY.filterMode = FilterMode.Bilinear;
               // Debug.Log("_videoTextureY.filterMode");
                _videoTextureY.wrapMode = TextureWrapMode.Repeat;
             //   Debug.Log("m_ClearMaterial");
               
              //  Debug.Log(_videoTextureY == null);
                m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
              //  Debug.Log("m_ClearMaterial end");
            }
            //Debug.Log("_videoTextureY"+ _videoTextureY == null);
            // Texture CbCr
            if (_videoTextureCbCr == null)
            {
               // Debug.Log("_videoTextureCbCr");                
                _videoTextureCbCr = new Texture2D(cam.videoParams.yWidth/2, cam.videoParams.yHeight/2,TextureFormat.RG16, false);
              //  Debug.Log(_videoTextureCbCr == null);
                _videoTextureCbCr.filterMode = FilterMode.Bilinear;
                _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
             //   Debug.Log("_videoTextureCbCr wrapMode");
             //   Debug.Log(cameraData[1]);
             //   Debug.Log(totalBufferSize / 2);
               // byte[] aaa = new byte[totalBufferSize / 2];
               // Marshal.Copy(cameraData[1], aaa, 0, totalBufferSize / 2);

               
                m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
            }

            _videoTextureY.LoadRawTextureData(backgrounddata[0], totalBufferSize);
            _videoTextureY.Apply();
            _videoTextureCbCr.LoadRawTextureData(backgrounddata[1], totalBufferSize / 2);
            _videoTextureCbCr.Apply();
            //Debug.Log("----1111----GetMTBackGroundData:" + GetTimeStamp());
            //Debug.Log("arsdk GetMTBackGroundData result " + result);
            //Debug.Log("arsdk totalBufferSize: " + totalBufferSize);
            // Debug.Log("arsdk backgrounddata: " + backgrounddata);
            // byte[] yuvData = new byte[totalBufferSize*2];
            // Marshal.Copy(backgrounddata, yuvData, 0, totalBufferSize*2);
            // if (loadYUV == null)
            // {
            //     loadYUV = GetComponent<LoadYUV>();
            // }
            //Debug.Log("arsdk yWidth: " + cam.videoParams.yWidth+ "  yHeight:" + cam.videoParams.yHeight);
            //Debug.Log("arsdk yuvData: " + yuvData);

            // loadYUV.SetYUV(material,yuvData, cam.videoParams.yWidth, cam.videoParams.yHeight);


            m_ClearMaterial.SetFloat("_ScaleX", scalex);
            m_ClearMaterial.SetFloat("_ScaleY", scaley);
            m_ClearMaterial.SetFloat("_OffsetX", offx);
            m_ClearMaterial.SetFloat("_OffsetY", offy);
            m_ClearMaterial.SetInt("rotation", (int)rotation);
            // Debug.Log("arsdk SetFloat over");
            //_videoTextureY.UpdateExternalTexture(cameraData[0]);
            // Debug.Log("UpdateExternalTexture");
            // _videoTextureCbCr.UpdateExternalTexture(cameraData[1]);



            if (isHideLoadUI)
            {
               // Debug.Log("isHideLoadUI");
                isHideLoadUI = false;
                black.SetActive(false);
                scan.SetActive(true);
            }
            //Debug.Log("arsdk BackgroundMaterial");
            if (0 == result)
            {
                
                frameFound = AlvaARWrapper.Instance.IsModelFound(modeltarget);
                
               // Debug.Log("arsdk IsModelFound -->" + frameFound);

                if (frameFound == 1)
                {
                    isFound = true;
                    //if (text)
                    //{
                    //    text.text = "found ";
                    //}

                    //Debug.Log("arsdk11 Model is Found 1111viewMatrix.Length:" + viewMatrix.Length);
                    
                    AlvaARWrapper.Instance.GetModelPoseMatrix(modeltarget, viewMatrix);
                  //  anchorAddisUse = AlvaARWrapper.Instance.GetAnchorModelQT(modeltarget, anchorMQT, anchorIsDelete);

                    //SetMatrixByCAD(viewMatrix);
                    SetMatrixBySlam(viewMatrix);
                  
                    if (isFirst)
                    {
                        isFirst = false;
                        scan.SetActive(false);
                        if (foundEvent != null)
                        {
                            foundEvent.Invoke();
                        }
                    }
                }
                else
                {
                    isFound = false;
                }

                if (isFound)
                {
                    //添加锚点
                    if (isAddAnchor == 1)
                    {

                       /* ARTrackingState tracking_state = ARTrackingState.ARTrackingStateNotAvailable;
                        print("11-11-11-modelAnchor_.anchor != null:" + modelAnchor_.anchor != null);

                        if (modelAnchor_.anchor != null)
                        {
                            tracking_state = modelAnchor_.anchor.;
                        }
                        print("12-12-12-anchorAddisUse:" + anchorAddisUse);
                        if (tracking_state == TrackingState.Tracking)
                        {
                            print("13-13-13-");
                        }
                        else if (anchorAddisUse == 0)
                        {
                            aRAnchorManager.RemoveAnchor(modelAnchor_.anchor);

                            modelAnchor_.anchor = null;
                            isAddAnchor = 0;
                            addPoseAnchor(anchorMQT);
                        }*/
                       
                    }
                    else
                    {
                        //print("14-14-14-anchorAddisUse:" + anchorAddisUse);
                        if (anchorAddisUse == 0)
                        {
                            addPoseAnchor(anchorMQT);
                        }
                    }
                }
            }
            
        }

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

    

}
