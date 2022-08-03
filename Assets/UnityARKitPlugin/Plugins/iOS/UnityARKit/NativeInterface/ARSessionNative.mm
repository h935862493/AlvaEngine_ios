// Unity Technologies Inc (c) 2017
// ARSessionNative.mm
// Main implementation of ARKit plugin native parts

#import <CoreVideo/CoreVideo.h>
#import <arsdk/arsdk.h>
#include "stdlib.h"
#include "UnityAppController.h"
#include "ARKitDefines.h"

// These don't all need to be static data, but no other better place for them at the moment.
static id <MTLTexture> s_CapturedImageTextureY = NULL;
static id <MTLTexture> s_CapturedImageTextureCbCr = NULL;
//static UnityARMatrix4x4 s_CameraProjectionMatrix;

static float s_AmbientIntensity;
static int s_TrackingQuality;
static float s_ShaderScale;

static float s_unityCameraNearZ;
static float s_unityCameraFarZ;

//0-none 1- NatureTarget 2-ModelTarget 3-ImageTarget
static int s_arType =0;

static const int s_charSize=64;

static char* s_companyname=(char*)malloc(s_charSize);
static char* s_authString=(char*)malloc(s_charSize * 8);

static char* s_modelPath=(char*)malloc(s_charSize * 8);

static bool s_isInitARSDK =false;
static bool s_isQuit = false;
//static bool isUnityRender =false;

static alvacs s_ALVATrackingState = Alva_ARSTOPPED;//识别核心跟踪状态
static void* s_model =nullptr;
static alvaro s_rotation;

static unsigned char* s_cameraDataIn[2]={nullptr};
static unsigned char* s_cameraDataOut[2]={nullptr};
static CVPixelBufferRef s_outPixelBuffer = NULL;
static unsigned char* s_guideImage=nullptr;

static float  s_viewMatrixInArray[16]={0};
static float  s_viewMatrixOutArray[16]={0};
static float  s_projectionMatrixArray[16]={0};
static float  s_anchorMatrixArray[16]={0};
static float  s_modelMatrixArray[16]={0};

static matrix_float4x4 s_anchorMatrix = matrix_identity_float4x4; //默认值为单位阵
static int s_anchorIndex=-1; //默认值为-1

static float s_anchorTRSArr[8]={0};

static int fpsCount=0;

static int s_InitSucceed = -1;

int foundNum[1] = {0};
int foundInde[5] ={0};



static inline UnityARTrackingState GetUnityARTrackingStateFromARTrackingState(ARTrackingState trackingState)
{
    switch (trackingState) {
        case ARTrackingStateNormal:
            return UnityARTrackingStateNormal;
        case ARTrackingStateLimited:
            return UnityARTrackingStateLimited;
        case ARTrackingStateNotAvailable:
            return UnityARTrackingStateNotAvailable;
        default:
            [NSException raise:@"UnrecognizedARTrackingState" format:@"Unrecognized ARTrackingState: %ld", (long)trackingState];
            break;
    }
}

static inline UnityARTrackingReason GetUnityARTrackingReasonFromARTrackingReason(ARTrackingStateReason trackingReason)
{
    switch (trackingReason)
    {
        case ARTrackingStateReasonNone:
            return UnityARTrackingStateReasonNone;
        case ARTrackingStateReasonInitializing:
            return UnityARTrackingStateReasonInitializing;
        case ARTrackingStateReasonExcessiveMotion:
            return UnityARTrackingStateReasonExcessiveMotion;
        case ARTrackingStateReasonInsufficientFeatures:
            return UnityARTrackingStateReasonInsufficientFeatures;
        case ARTrackingStateReasonRelocalizing:
            return UnityARTrackingStateReasonRelocalizing;
        default:
            [NSException raise:@"UnrecognizedARTrackingStateReason" format:@"Unrecognized ARTrackingStateReason: %ld", (long)trackingReason];
            break;
    }
}

API_AVAILABLE(ios(12.0))
static inline UnityARWorldMappingStatus GetUnityARWorldMappingStatusFromARWorldMappingStatus(ARWorldMappingStatus worldMappingStatus)
{
    switch (worldMappingStatus) {
        case ARWorldMappingStatusNotAvailable:
            return UnityARWorldMappingStatusNotAvailable;
        case ARWorldMappingStatusLimited:
            return UnityARWorldMappingStatusLimited;
        case ARWorldMappingStatusExtending:
            return UnityARWorldMappingStatusExtending;
        case ARWorldMappingStatusMapped:
            return UnityARWorldMappingStatusMapped;
        default:
            [NSException raise:@"UnrecognizedARWorldMappingStatus" format:@"Unrecognized ARWorldMappingStatus: %ld", (long)worldMappingStatus];
            break;
    }
}


API_AVAILABLE(ios(12.0))
static inline AREnvironmentTexturing GetAREnvironmentTexturingFromUnityAREnvironmentTexturing(UnityAREnvironmentTexturing& unityEnvTexturing)
{
    switch (unityEnvTexturing)
    {
        case UnityAREnvironmentTexturingNone:
            return AREnvironmentTexturingNone;
        case UnityAREnvironmentTexturingManual:
            return AREnvironmentTexturingManual;
        case UnityAREnvironmentTexturingAutomatic:
            return AREnvironmentTexturingAutomatic;
    }
}



inline void GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(ARKitWorldTrackingSessionConfiguration& unityConfig, ARWorldTrackingConfiguration* appleConfig)
{
    appleConfig.planeDetection = GetARPlaneDetectionFromUnityARPlaneDetection(unityConfig.planeDetection);
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
    
    if (@available(iOS 12.0, *))
    {
        appleConfig.maximumNumberOfTrackedImages = unityConfig.maximumNumberOfTrackedImages;
    }
    
    if (@available(iOS 11.3, *))
    {
        appleConfig.autoFocusEnabled = (BOOL) unityConfig.enableAutoFocus;

        if (unityConfig.ptrVideoFormat != NULL)
        {
            appleConfig.videoFormat = (__bridge ARVideoFormat*) unityConfig.ptrVideoFormat;
        }
    }
    
    if (UnityIsARKit_2_0_Supported())
    {
        if (@available(iOS 12.0, *)) {
            appleConfig.initialWorldMap = (__bridge ARWorldMap*)unityConfig.ptrWorldMap;
            appleConfig.environmentTexturing = GetAREnvironmentTexturingFromUnityAREnvironmentTexturing(unityConfig.environmentTexturing);
        }
    }
}

inline void GetARSessionConfigurationFromARKitSessionConfiguration(ARKitSessionConfiguration& unityConfig, ARConfiguration* appleConfig)
{
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
}

#if ARKIT_USES_FACETRACKING
inline void GetARFaceConfigurationFromARKitFaceConfiguration(ARKitFaceTrackingConfiguration& unityConfig, ARConfiguration* appleConfig)
{
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
    
    if (@available(iOS 11.3, *))
    {
        if (unityConfig.ptrVideoFormat != NULL)
        {
            appleConfig.videoFormat = (__bridge ARVideoFormat*) unityConfig.ptrVideoFormat;
        }
    }
}
#endif

static inline void GetUnityARCameraDataFromCamera(UnityARCamera& unityARCamera, ARCamera* camera)
{
    CGSize nativeSize = GetAppController().rootView.bounds.size;
    matrix_float4x4 projectionMatrix = [camera projectionMatrixForOrientation:[[UIApplication sharedApplication] statusBarOrientation] viewportSize:nativeSize zNear:(CGFloat)s_unityCameraNearZ zFar:(CGFloat)s_unityCameraFarZ];
    
    //ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &s_CameraProjectionMatrix);
    ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &unityARCamera.projectionMatrix);
    
    unityARCamera.trackingState = GetUnityARTrackingStateFromARTrackingState(camera.trackingState);
    unityARCamera.trackingReason = GetUnityARTrackingReasonFromARTrackingReason(camera.trackingStateReason);
    
    unityARCamera.focalx = camera.intrinsics.columns[0].x;
    unityARCamera.focaly = camera.intrinsics.columns[1].y;
    unityARCamera.principalx = camera.intrinsics.columns[2].x;
    unityARCamera.principaly = camera.intrinsics.columns[2].y;
    //NSLog(@"%f",unityARCamera.focalx);
    //NSLog(@"%f",unityARCamera.focaly);
    //NSLog(@"%f",unityARCamera.principalx);
    //NSLog(@"%f",unityARCamera.principaly);
    
    
    
}

API_AVAILABLE(ios(11.3))
inline void UnityARPlaneGeometryFromARPlaneGeometry(UnityARPlaneGeometry& planeGeometry, ARPlaneGeometry *arPlaneGeometry)
{
    planeGeometry.vertexCount = arPlaneGeometry.vertexCount;
    planeGeometry.triangleCount = arPlaneGeometry.triangleCount;
    planeGeometry.textureCoordinateCount = arPlaneGeometry.textureCoordinateCount;
    planeGeometry.boundaryVertexCount = arPlaneGeometry.boundaryVertexCount;
    planeGeometry.vertices = (float *) arPlaneGeometry.vertices;
    planeGeometry.triangleIndices = (int *) arPlaneGeometry.triangleIndices;
    planeGeometry.textureCoordinates = (float *) arPlaneGeometry.textureCoordinates;
    planeGeometry.boundaryVertices = (float *) arPlaneGeometry.boundaryVertices;
    
}

inline void UnityARAnchorDataFromARAnchorPtr(UnityARAnchorData& anchorData, ARPlaneAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
    anchorData.alignment = nativeAnchor.alignment;
    anchorData.center.x = nativeAnchor.center.x;
    anchorData.center.y = nativeAnchor.center.y;
    anchorData.center.z = nativeAnchor.center.z;
    anchorData.extent.x = nativeAnchor.extent.x;
    anchorData.extent.y = nativeAnchor.extent.y;
    anchorData.extent.z = nativeAnchor.extent.z;
    
    if (@available(iOS 11.3, *))
    {
        UnityARPlaneGeometryFromARPlaneGeometry(anchorData.planeGeometry, nativeAnchor.geometry);
    }
}

inline void UnityARMatrix4x4FromCGAffineTransform(UnityARMatrix4x4& outMatrix, CGAffineTransform displayTransform, BOOL isLandscape)
{
    if (isLandscape)
    {
        outMatrix.column0.x = displayTransform.a;
        outMatrix.column0.y = displayTransform.c;
        outMatrix.column0.z = displayTransform.tx;
        outMatrix.column1.x = displayTransform.b;
        outMatrix.column1.y = -displayTransform.d;
        outMatrix.column1.z = 1.0f - displayTransform.ty;
        outMatrix.column2.z = 1.0f;
        outMatrix.column3.w = 1.0f;
    }
    else
    {
        outMatrix.column0.x = displayTransform.a;
        outMatrix.column0.y = -displayTransform.c;
        outMatrix.column0.z = 1.0f - displayTransform.tx;
        outMatrix.column1.x = displayTransform.b;
        outMatrix.column1.y = displayTransform.d;
        outMatrix.column1.z = displayTransform.ty;
        outMatrix.column2.z = 1.0f;
        outMatrix.column3.w = 1.0f;
    }
}

inline void UnityARUserAnchorDataFromARAnchorPtr(UnityARUserAnchorData& anchorData, ARAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
}

void SetALVAARTrackingState(ARFrame * arFrame)
{
    switch (arFrame.camera.trackingState) {
        case ARTrackingStateNotAvailable:
            s_ALVATrackingState =Alva_ARINVALID;
            break;
        case ARTrackingStateLimited:
            s_ALVATrackingState =Alva_ARPAUSED;
            break;
        case ARTrackingStateNormal:
            s_ALVATrackingState =Alva_ARTRACKING;
            break;
        default:
            break;
    }
}

int InitALVAModelTarget(ARFrame* arFrame,size_t cameraW,size_t cameraH,float screenW,float screenH)
{
    s_InitSucceed = -1;
    int ret=MT_init(s_companyname, s_authString);
    if(0!=ret){
        NSLog(@"MT_init error");
        return ret;
    }
    /*
    ret = MT_SetSimpleModel(2);
    if(0!=ret){
        NSLog(@"MT_SetSimpleModel error");
        return ret;
    }
    */
    ret=MT_setDataInfoMemory((int)cameraW, (int)cameraH, Alva_FMT_YUV_420sp);
    if(0!=ret){
        NSLog(@"MT_setDataInfoMemory error");
        return ret;
    }
    float focalx = arFrame.camera.intrinsics.columns[0].x;
    float focaly = arFrame.camera.intrinsics.columns[1].y;
    float principalx = arFrame.camera.intrinsics.columns[2].x;
    float principaly = arFrame.camera.intrinsics.columns[2].y;
    
    ret=MT_SetCameraInfo(cameraW, cameraH, focalx, focaly, principalx, principaly,  screenW,  screenH, s_rotation);
    if(0!=ret){
        NSLog(@"MT_SetCameraInfo error");
        return ret;
    }
    
    ret = initModelManager();
    
    if(0!=ret){
        NSLog(@"initModelManager error");
        return ret;
    }
    
    ret=addModel(s_modelPath, Alva_File);
    s_InitSucceed = ret;
    if(0!=ret){
        NSLog(@"initModelManager error %d",ret);
        return ret;
    }
   
    s_model=getModelByIndex(0);
    if(s_model==NULL){
        NSLog(@"getModelByIndex error");
        return ret;
    }
    if(s_guideImage!=nullptr){
        ret= getModelGuideImage(s_model,s_guideImage);
        if(0!=ret){
            NSLog(@"getModelGuideImage error %d",ret);
            return ret;
        }
    }
    ret = setModelStatus(s_model, 1);
    
    if(0!=ret){
        NSLog(@"setModelStatus error");
        return ret;
    }
    NSLog(@"initOK");
    return ret;
}

int InitALVAImageTarget(ARFrame* arFrame,size_t cameraW,size_t cameraH,float screenW,float screenH)
{
    s_InitSucceed = -1;
    int ret=IR_init((int)Alva_Memory,s_companyname, s_authString);
    if(0!=ret){
        NSLog(@"IR_init error %d",ret);
        return ret;
    }
    ret=IR_setDataInfoMemory((int)cameraW, (int)cameraH, Alva_FMT_YUV_420sp);
    if(0!=ret){
        NSLog(@"IR_setDataInfoMemory error %d",ret);
        return ret;
    }
    
    float focalx = arFrame.camera.intrinsics.columns[0].x;
    float focaly = arFrame.camera.intrinsics.columns[1].y;
    float principalx = arFrame.camera.intrinsics.columns[2].x;
    float principaly = arFrame.camera.intrinsics.columns[2].y;
    
    ret = IR_setCameraParamInfo(focalx, focaly, principalx, principaly, screenW, screenH, s_rotation);
    if (ret != 0) {
        NSLog(@"IR_setCameraParamInfo err.0x%x\n", ret);
        return ret;
    }
    
    IR_EnableTrack();

    //ret = initTrackerManager();
    
    //if(0!=ret){
        //NSLog(@"initTrackerManager error %d",ret);
        //return ret;
    //}
    //NSLog(@"s_modelPath=%s",s_modelPath);
    ret=addTrackable(s_modelPath, Alva_File);
    
    if(0!=ret){
        NSLog(@"addTrackable error %d",ret);
        return ret;
    }
    
    //int trackableNumber = getTrackableNumber();
    s_model=getTrackableByIndex(0);
    if(s_model==NULL){
        NSLog(@"getTrackableByIndex error %d",ret);
        return ret;
    }
    ret = setTrackableStatus(s_model, 1);
    
    if(0!=ret){
        NSLog(@"setModelStatus error %d",ret);
        return ret;
    }
    NSLog(@"initOK");
    
    return ret;
}

#if ARKIT_USES_FACETRACKING
inline void UnityARFaceGeometryFromARFaceGeometry(UnityARFaceGeometry& faceGeometry, ARFaceGeometry *arFaceGeometry)
{
    faceGeometry.vertexCount = arFaceGeometry.vertexCount;
    faceGeometry.triangleCount = arFaceGeometry.triangleCount;
    faceGeometry.textureCoordinateCount = arFaceGeometry.textureCoordinateCount;
    faceGeometry.vertices = (float *) arFaceGeometry.vertices;
    faceGeometry.triangleIndices = (int *) arFaceGeometry.triangleIndices;
    faceGeometry.textureCoordinates = (float *) arFaceGeometry.textureCoordinates;
}

inline void UnityARFaceAnchorDataFromARFaceAnchorPtr(UnityARFaceAnchorData& anchorData, ARFaceAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
    if (UnityIsARKit_2_0_Supported())
    {
        ARKitMatrixToUnityARMatrix4x4(nativeAnchor.leftEyeTransform, &anchorData.leftEyeTransform);
        ARKitMatrixToUnityARMatrix4x4(nativeAnchor.rightEyeTransform, &anchorData.rightEyeTransform);
        anchorData.lookAtPoint = UnityARVector3{nativeAnchor.lookAtPoint.x, nativeAnchor.lookAtPoint.y, nativeAnchor.lookAtPoint.z};
    }

    UnityARFaceGeometryFromARFaceGeometry(anchorData.faceGeometry, nativeAnchor.geometry);
    anchorData.blendShapes = (__bridge void *) nativeAnchor.blendShapes;
    anchorData.isTracked = (uint32_t) nativeAnchor.isTracked;
}
#endif

API_AVAILABLE(ios(11.3))
inline void UnityARImageAnchorDataFromARImageAnchorPtr(UnityARImageAnchorData& anchorData, ARImageAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
    anchorData.referenceImageName = (void*)[nativeAnchor.referenceImage.name UTF8String];
    anchorData.referenceImageSize = nativeAnchor.referenceImage.physicalSize.width;
    anchorData.isTracked = [nativeAnchor isTracked] ? 1 : 0;
}

inline void UnityLightDataFromARFrame(UnityLightData& lightData, ARFrame *arFrame)
{
    if (arFrame.lightEstimate != NULL)
    {
#if ARKIT_USES_FACETRACKING
        if ([arFrame.lightEstimate class] == [ARDirectionalLightEstimate class])
        {
            lightData.arLightingType = DirectionalLightEstimate;
            ARDirectionalLightEstimate *dirLightEst = (ARDirectionalLightEstimate *) arFrame.lightEstimate;
            lightData.arDirectionalLightEstimate.sphericalHarmonicsCoefficients = (float *) dirLightEst.sphericalHarmonicsCoefficients.bytes;

            //[dirLightEst.sphericalHarmonicsCoefficients getBytes:lightData.arDirectionalLightEstimate.sphericalHarmonicsCoefficients length:sizeof(float)*27 ];

            UnityARVector4 dirAndIntensity;
            dirAndIntensity.x = dirLightEst.primaryLightDirection.x;
            dirAndIntensity.y = dirLightEst.primaryLightDirection.y;
            dirAndIntensity.z = dirLightEst.primaryLightDirection.z;
            dirAndIntensity.w = dirLightEst.primaryLightIntensity;
            lightData.arDirectionalLightEstimate.primaryLightDirectionAndIntensity = dirAndIntensity;
        }
        else
#endif
        {
            lightData.arLightingType = LightEstimate;
            lightData.arLightEstimate.ambientIntensity = arFrame.lightEstimate.ambientIntensity;
            lightData.arLightEstimate.ambientColorTemperature = arFrame.lightEstimate.ambientColorTemperature;
        }
    }
    
}


@interface UnityARAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorUpdatedCallback(data);
    }

@end

@interface UnityARUserAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARUserAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorUpdatedCallback(data);
    }

@end

@interface UnityARFaceAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_FACE_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_FACE_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_FACE_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARFaceAnchorCallbackWrapper

-(void)sendAnchorAddedEvent:(ARAnchor*)anchor
{
#if ARKIT_USES_FACETRACKING
    UnityARFaceAnchorData data;
    UnityARFaceAnchorDataFromARFaceAnchorPtr(data, (ARFaceAnchor*)anchor);
    _anchorAddedCallback(data);
#endif
}

-(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
{
#if ARKIT_USES_FACETRACKING
    UnityARFaceAnchorData data;
    UnityARFaceAnchorDataFromARFaceAnchorPtr(data, (ARFaceAnchor*)anchor);
    _anchorRemovedCallback(data);
#endif
}

-(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
{
#if ARKIT_USES_FACETRACKING
    UnityARFaceAnchorData data;
    UnityARFaceAnchorDataFromARFaceAnchorPtr(data, (ARFaceAnchor*)anchor);
    _anchorUpdatedCallback(data);
#endif
}

@end

@interface UnityARImageAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_IMAGE_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_IMAGE_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_IMAGE_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARImageAnchorCallbackWrapper

-(void)sendAnchorAddedEvent:(ARAnchor*)anchor
{
    UnityARImageAnchorData data;
    if (@available(iOS 11.3, *)) {
        UnityARImageAnchorDataFromARImageAnchorPtr(data, (ARImageAnchor*)anchor);
    }
    _anchorAddedCallback(data);
}

-(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
{
    UnityARImageAnchorData data;
    if (@available(iOS 11.3, *)) {
        UnityARImageAnchorDataFromARImageAnchorPtr(data, (ARImageAnchor*)anchor);
    }
    _anchorRemovedCallback(data);
}

-(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
{
    UnityARImageAnchorData data;
    if (@available(iOS 11.3, *)) {
        UnityARImageAnchorDataFromARImageAnchorPtr(data, (ARImageAnchor*)anchor);
    }
    _anchorUpdatedCallback(data);
}

@end

static UnityPixelBuffer s_UnityPixelBuffers;


@implementation UnityARSession

- (id)init
{
    if (self = [super init])
    {
        _textureCache = NULL;
        _classToCallbackMap = [[NSMutableDictionary alloc] init];
    }
    return self;
}

- (void)setupMetal
{
    if (_textureCache != NULL)
    {
        return;
    }
    _device = MTLCreateSystemDefaultDevice();
    CVMetalTextureCacheCreate(NULL, NULL, _device, NULL, &_textureCache);
}

- (void)teardownMetal
{
    if (_textureCache != NULL) {
        CFRelease(_textureCache);
        _textureCache = NULL;
    }
}

static CGAffineTransform s_CurAffineTransform;


- (void)session:(ARSession *)session didALVAModelTargetUpdateFrame:(ARFrame *)frame
{
    //NSLog(@"B1");
    CGRect nativeBounds = [[UIScreen mainScreen] nativeBounds];
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
   
    CVPixelBufferRef pixelBuffer = frame.capturedImage;
    
    size_t imageWidth = CVPixelBufferGetWidth(pixelBuffer);
    size_t imageHeight = CVPixelBufferGetHeight(pixelBuffer);
    //NSLog(@"B2");
    
    //默认竖屏
    int screenWidth=nativeBounds.size.width;
    int screenHeight=nativeBounds.size.height;
    
    if(s_rotation== Alva_ROTATION_0 || s_rotation== Alva_ROTATION_180){
        
    }else if(s_rotation== Alva_ROTATION_90 || s_rotation== Alva_ROTATION_270){
         screenWidth=nativeBounds.size.height;
         screenHeight=nativeBounds.size.width;
    }
   // NSLog(@"B3");
    
    int ret =0;
    
    UnityARCamera unityARCamera;
    unityARCamera.imageWidth = (int)imageWidth;
    unityARCamera.imageHeight = (int)imageHeight;
    
   // NSLog(@"B4");
    
    if (_getPointCloudData && frame.rawFeaturePoints != nullptr)
    {
        unityARCamera.ptrPointCloud = (__bridge_retained void *) frame.rawFeaturePoints;
    }
    else
    {
        unityARCamera.ptrPointCloud = nullptr;
    }
   // NSLog(@"B5");
    if(!s_isInitARSDK && s_InitSucceed == -1){
        ret=InitALVAModelTarget(frame, imageWidth,imageHeight,screenWidth,screenHeight);
        s_InitSucceed = ret;
        if(ret==0){
            s_isInitARSDK=true;
        }else{
            NSLog(@"InitALVAModelTarget error %d",ret);
            return;
        }
      //  NSLog(@"B7");
    }
   // NSLog(@"B8");
    if(s_isInitARSDK){
        CVPixelBufferLockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
        s_cameraDataIn[0] =(unsigned char*) CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,0);
        s_cameraDataIn[1]=(unsigned char*) CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,1);
        
        matrix_float4x4 viewMatrix=[frame.camera viewMatrixForOrientation:orientation];
        ARKitMatrixToFloatArray(viewMatrix,s_viewMatrixInArray);
        
        //锚点模型矩阵
        ARKitMatrixToFloatArray(s_anchorMatrix,s_anchorMatrixArray);
        
        SetALVAARTrackingState(frame);
        //因为ARKit没有锚点状态，所以锚点状态跟追踪状态一致
        
        int anchorStatus=s_ALVATrackingState;
        if(s_anchorIndex==-1){
            anchorStatus=Alva_ARINVALID;
        }
        ret = MT_MemoryChannel(s_cameraDataIn, s_viewMatrixInArray, s_anchorMatrixArray, s_ALVATrackingState, s_anchorIndex, anchorStatus);
        if(ret!=0){
            NSLog(@"MT_MemoryChannel error %d",ret);
            return;
        }
        CVPixelBufferUnlockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
        
        bool isNeedCreateBuffer=false;
        if(s_outPixelBuffer==NULL ){
            isNeedCreateBuffer=true;
        }else{
            size_t width = CVPixelBufferGetWidth(s_outPixelBuffer);
            size_t height = CVPixelBufferGetHeight(s_outPixelBuffer);
            if ((width != imageWidth) || (height != imageHeight)) {
                isNeedCreateBuffer =true;
            }
        }
      //  NSLog(@"B9");
        if(isNeedCreateBuffer){
        NSDictionary *pixelAttributes = @{(id)kCVPixelBufferIOSurfacePropertiesKey : @{}};
        CVReturn result = CVPixelBufferCreate(kCFAllocatorDefault,
                                              imageWidth,
                                              imageHeight,
                                              kCVPixelFormatType_420YpCbCr8BiPlanarFullRange,
                                              (__bridge CFDictionaryRef)(pixelAttributes),
                                              &s_outPixelBuffer);
            if(result!= kCVReturnSuccess){
                NSLog(@"CVPixelBufferCreate error %d",result);
                return;
            }
        }
     //   NSLog(@"B10");
        if(s_outPixelBuffer==NULL){
            NSLog(@"outPixelBuffer==NULL");
            return;
        }
        CVPixelBufferLockBaseAddress(s_outPixelBuffer, kCVPixelBufferLock_ReadOnly);
        if(s_cameraDataOut[0]==nullptr){
            s_cameraDataOut[0] = (unsigned char*)CVPixelBufferGetBaseAddressOfPlane(s_outPixelBuffer, 0);
        }
        if(s_cameraDataOut[1]==nullptr){
            s_cameraDataOut[1] = (unsigned char*)CVPixelBufferGetBaseAddressOfPlane(s_outPixelBuffer, 1);
        }
        ret=MT_GetBackGroundDataChannel(s_cameraDataOut);
        CVPixelBufferUnlockBaseAddress(s_outPixelBuffer, kCVPixelBufferLock_ReadOnly);
        if(ret!=0){
            NSLog(@"MT_GetBackGroundDataChannel error %d",ret);
        }else{
           
            ret=computeProjectMatrixFixRange(screenWidth,screenHeight, s_unityCameraNearZ,
                                         s_unityCameraFarZ,s_projectionMatrixArray);
            
            if(ret!=0){
                NSLog(@"computeProjectMatrixFixRange error %d",ret);
            }else{
                bool isFound=isModelFound(s_model);
                unityARCamera.isFoundModel=isFound;
                if(isFound==1){
                   
                    ret= getModelPoseMatrix(s_model, s_viewMatrixOutArray);
                    if(ret==0){
                        int isDelete=0;
                        int isAdd =0;
                        ret=getAnchorModelQT(s_model,s_anchorTRSArr,&isDelete,&isAdd);
                        if(ret==0){
                            matrix_float4x4 arkitViewMatrix = matrix_identity_float4x4;
                            FloatArrayToARKitMatrix(s_viewMatrixOutArray,&arkitViewMatrix);
                            ARKitMatrixToUnityARMatrix4x4(arkitViewMatrix, &unityARCamera.viewMatrix);
                            
                            matrix_float4x4 arkitProjectMatrix = matrix_identity_float4x4;
                            FloatArrayToARKitMatrix(s_projectionMatrixArray,&arkitProjectMatrix);
                            ARKitMatrixToUnityARMatrix4x4(arkitProjectMatrix, &unityARCamera.projectionMatrix);
                            
                           // dispatch_async(dispatch_get_main_queue(), ^{
                                
                           // });
                                
                            
                            if(isAdd==1 && (simd_equal(s_anchorMatrix,matrix_identity_float4x4))){
                                NSLog(@"add mao dian");
                                simd_quatf rotation=simd_quaternion(s_anchorTRSArr[0], s_anchorTRSArr[1], s_anchorTRSArr[2],s_anchorTRSArr[3]);
                                simd_float4x4 anchor_transform=simd_matrix4x4(rotation);
                                anchor_transform.columns[3]={s_anchorTRSArr[4],s_anchorTRSArr[5],s_anchorTRSArr[6],1};
                                ARAnchor *newAnchor = [[ARAnchor alloc] initWithTransform:anchor_transform];
                                [session addAnchor:newAnchor];
                                s_anchorMatrix=anchor_transform;
                                s_anchorIndex+=1;
                            }
                        }else{
                            NSLog(@"getAnchorModelQT errod");
                        }
                    }else{
                        NSLog(@"getAnchorModelQT errod");
                    }
                }
            }
        }
    }
    
    if(s_outPixelBuffer==nullptr) return;
    
    id<MTLTexture> textureY = nil;
    id<MTLTexture> textureCbCr = nil;
    
    // textureY
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(s_outPixelBuffer, 0);
        const size_t height = CVPixelBufferGetHeightOfPlane(s_outPixelBuffer, 0);
        MTLPixelFormat pixelFormat = MTLPixelFormatR8Unorm;
        
        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, s_outPixelBuffer, NULL, pixelFormat, width, height, 0, &texture);
        if(status == kCVReturnSuccess)
        {
            textureY = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }

    // textureCbCr
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(s_outPixelBuffer, 1);
        const size_t height = CVPixelBufferGetHeightOfPlane(s_outPixelBuffer, 1);
        MTLPixelFormat pixelFormat = MTLPixelFormatRG8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, s_outPixelBuffer, NULL, pixelFormat, width, height, 1, &texture);
        if(status == kCVReturnSuccess)
        {
            textureCbCr = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }
    
    
    
    if (textureY != nil && textureCbCr != nil) {
       // dispatch_async(dispatch_get_main_queue(), ^{
            // always assign the textures atomic
            s_CapturedImageTextureY = textureY;
            s_CapturedImageTextureCbCr = textureCbCr;
            if(_frameCallback!=nullptr){
                _frameCallback(unityARCamera);
                
                if (unityARCamera.ptrPointCloud != nullptr)
                {
                    CFRelease(unityARCamera.ptrPointCloud);
                }
            }
        //});
    }
}


- (void)session:(ARSession *)session didALVAImageTargetUpdateFrame:(ARFrame *)frame
{
   // NSLog(@"a1");
    CGRect nativeBounds = [[UIScreen mainScreen] nativeBounds];
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
   
    CVPixelBufferRef pixelBuffer = frame.capturedImage;
    size_t imageWidth = CVPixelBufferGetWidth(pixelBuffer);
    size_t imageHeight = CVPixelBufferGetHeight(pixelBuffer);
    int ret =0;
    
    UnityARCamera unityARCamera;
    unityARCamera.imageWidth = (int)imageWidth;
    unityARCamera.imageHeight = (int)imageHeight;
    
   // NSLog(@"a2");
    if (_getPointCloudData && frame.rawFeaturePoints != nullptr)
    {
        unityARCamera.ptrPointCloud = (__bridge_retained void *) frame.rawFeaturePoints;
    }
    else
    {
        unityARCamera.ptrPointCloud = nullptr;
    }
   // NSLog(@"a3");
    //默认竖屏
    int screenWidth=nativeBounds.size.width;
    int screenHeight=nativeBounds.size.height;
    
    if(s_rotation== Alva_ROTATION_0 || s_rotation== Alva_ROTATION_180){
        
    }else if(s_rotation== Alva_ROTATION_90 || s_rotation== Alva_ROTATION_270){
         screenWidth=nativeBounds.size.height;
         screenHeight=nativeBounds.size.width;
    }
    
    if(!s_isInitARSDK && s_InitSucceed == -1){
        ret=InitALVAImageTarget(frame, imageWidth,imageHeight,screenWidth,screenHeight);
        s_InitSucceed = ret;
        if(ret==0){
            s_isInitARSDK=true;
        }else{
            NSLog(@"InitALVAImageTarget error %d",ret);
            return;
        }
        
    }
  //  NSLog(@"a4");
    if(s_isInitARSDK){
        CVPixelBufferLockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
        s_cameraDataIn[0] =(unsigned char*) CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,0);
        s_cameraDataIn[1]=(unsigned char*) CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,1);
        
        matrix_float4x4 viewMatrix=[frame.camera viewMatrixForOrientation:orientation];
        ARKitMatrixToFloatArray(viewMatrix,s_viewMatrixInArray);
        
        float coord[2]={0};
        float dataMatrix[16]={0};
        ARKitMatrixToFloatArray(matrix_identity_float4x4,dataMatrix);
     
        ret = IR_MemoryChannel(s_cameraDataIn, coord,dataMatrix, s_viewMatrixInArray);
        if(ret!=0){
            NSLog(@"IR_MemoryChannel error %d",ret);
            return;
        }
        CVPixelBufferUnlockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
       // NSLog(@"a6");
        bool isNeedCreateBuffer=false;
        if(s_outPixelBuffer==NULL ){
            isNeedCreateBuffer=true;
        }else{
            size_t width = CVPixelBufferGetWidth(s_outPixelBuffer);
            size_t height = CVPixelBufferGetHeight(s_outPixelBuffer);
            if ((width != imageWidth) || (height != imageHeight)) {
                isNeedCreateBuffer =true;
            }
        }
        if(isNeedCreateBuffer){
         //   NSLog(@"a7");
        NSDictionary *pixelAttributes = @{(id)kCVPixelBufferIOSurfacePropertiesKey : @{}};
        CVReturn result = CVPixelBufferCreate(kCFAllocatorDefault,
                                              imageWidth,
                                              imageHeight,
                                              kCVPixelFormatType_420YpCbCr8BiPlanarFullRange,
                                              (__bridge CFDictionaryRef)(pixelAttributes),
                                              &s_outPixelBuffer);
            if(result!= kCVReturnSuccess){
                NSLog(@"CVPixelBufferCreate error %d",result);
                return;
            }
        }
        
        if(s_outPixelBuffer==NULL){
            NSLog(@"outPixelBuffer==NULL");
            return;
        }
      //  NSLog(@"a8");
        CVPixelBufferLockBaseAddress(s_outPixelBuffer, kCVPixelBufferLock_ReadOnly);
        if(s_cameraDataOut[0]==nullptr){
            s_cameraDataOut[0] = (unsigned char*)CVPixelBufferGetBaseAddressOfPlane(s_outPixelBuffer, 0);
        }
        if(s_cameraDataOut[1]==nullptr){
            s_cameraDataOut[1] = (unsigned char*)CVPixelBufferGetBaseAddressOfPlane(s_outPixelBuffer, 1);
        }
        ret=IR_GetBackGroundDataChannel(s_cameraDataOut);
        CVPixelBufferUnlockBaseAddress(s_outPixelBuffer, kCVPixelBufferLock_ReadOnly);
      //  NSLog(@"a9");
        if(ret!=0){
            NSLog(@"IR_GetBackGroundDataChannel error %d",ret);
            return;
        }else{
            
            getFoundNumIndexs(foundNum,foundInde);
            
            if(foundNum[0] > 0){
               // s_model = getTrackableByIndex(foundInde[0]);
                if(s_model!=nullptr){
                    unityARCamera.isFoundModel=1;
                    
                    ret=getTrackerPoseMatrix(s_model, s_viewMatrixOutArray);
                    
                    if(ret!=0){
                        NSLog(@"getTrackerPoseMatrix error %d",ret);
                    }else{
                        ret=IR_computeProjectMatrixFixRange(screenWidth,screenHeight, s_unityCameraNearZ,
                                                     s_unityCameraFarZ,s_projectionMatrixArray);
                        if(ret!=0){
                            NSLog(@"IR_computeProjectMatrixFixRange error %d",ret);
                            return;
                        }
                    }
                    matrix_float4x4 arkitViewMatrix = matrix_identity_float4x4;
                    FloatArrayToARKitMatrix(s_viewMatrixOutArray,&arkitViewMatrix);
                    ARKitMatrixToUnityARMatrix4x4(arkitViewMatrix, &unityARCamera.viewMatrix);
                    
                    matrix_float4x4 arkitProjectMatrix = matrix_identity_float4x4;
                    FloatArrayToARKitMatrix(s_projectionMatrixArray,&arkitProjectMatrix);
                    ARKitMatrixToUnityARMatrix4x4(arkitProjectMatrix, &unityARCamera.projectionMatrix);
                }
            }
            else{
                unityARCamera.isFoundModel = 0;
            }
        }
    }
    
    if(s_outPixelBuffer==nullptr) return;
    
    id<MTLTexture> textureY = nil;
    id<MTLTexture> textureCbCr = nil;

    // textureY
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(s_outPixelBuffer, 0);
        const size_t height = CVPixelBufferGetHeightOfPlane(s_outPixelBuffer, 0);
        MTLPixelFormat pixelFormat = MTLPixelFormatR8Unorm;
        
        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, s_outPixelBuffer, NULL, pixelFormat, width, height, 0, &texture);
        if(status == kCVReturnSuccess)
        {
            textureY = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }

    // textureCbCr
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(s_outPixelBuffer, 1);
        const size_t height = CVPixelBufferGetHeightOfPlane(s_outPixelBuffer, 1);
        MTLPixelFormat pixelFormat = MTLPixelFormatRG8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, s_outPixelBuffer, NULL, pixelFormat, width, height, 1, &texture);
        if(status == kCVReturnSuccess)
        {
            textureCbCr = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }
    
    
    
    if (textureY != nil && textureCbCr != nil) {
       // dispatch_async(dispatch_get_main_queue(), ^{
            // always assign the textures atomic
            s_CapturedImageTextureY = textureY;
            s_CapturedImageTextureCbCr = textureCbCr;
            if(_frameCallback!=nullptr){
                _frameCallback(unityARCamera);
               // NSLog(@"333");
                if (unityARCamera.ptrPointCloud != nullptr)
                {
                   // NSLog(@"444");
                    CFRelease(unityARCamera.ptrPointCloud);
                   // NSLog(@"4444");
                }
                //NSLog(@"555");
            }
        //});
    }
}

- (void)session:(ARSession *)session didDefaultUpdateFrame:(ARFrame *)frame
{
  //  NSLog(@"C1");
    s_AmbientIntensity = frame.lightEstimate.ambientIntensity;
    s_TrackingQuality = (int)frame.camera.trackingState;
   // NSLog(@"C2");
    CGRect nativeBounds = [[UIScreen mainScreen] nativeBounds];
    CGSize nativeSize = GetAppController().rootView.bounds.size;
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
    s_CurAffineTransform = CGAffineTransformInvert([frame displayTransformForOrientation:orientation viewportSize:nativeSize]);
   // NSLog(@"C3");
    UnityARCamera unityARCamera;

    GetUnityARCameraDataFromCamera(unityARCamera, frame.camera);

    if (_getPointCloudData && frame.rawFeaturePoints != nullptr)
    {
        unityARCamera.ptrPointCloud = (__bridge_retained void *) frame.rawFeaturePoints;
    }
    else
    {
        unityARCamera.ptrPointCloud = nullptr;
    }
  //  NSLog(@"C4");
    CVPixelBufferRef pixelBuffer = frame.capturedImage;
    
    size_t imageWidth = CVPixelBufferGetWidth(pixelBuffer);
    size_t imageHeight = CVPixelBufferGetHeight(pixelBuffer);
    
    float imageAspect = (float)imageWidth / (float)imageHeight;
    float screenAspect = nativeBounds.size.height / nativeBounds.size.width;
    unityARCamera.videoParams.texCoordScale =  screenAspect / imageAspect;
    s_ShaderScale = screenAspect / imageAspect;
  //  NSLog(@"C5");
    unityARCamera.getLightEstimation = _getLightEstimation;
    if (_getLightEstimation)
    {
        UnityLightDataFromARFrame(unityARCamera.lightData, frame);
    }
  //  NSLog(@"C6");
    unityARCamera.videoParams.yWidth = (uint32_t)imageWidth;
    unityARCamera.videoParams.yHeight = (uint32_t)imageHeight;
    unityARCamera.videoParams.cvPixelBufferPtr = (void *) pixelBuffer;
    UnityARMatrix4x4 displayTransform;
    memset(&displayTransform, 0, sizeof(UnityARMatrix4x4));
    UnityARMatrix4x4FromCGAffineTransform(displayTransform, s_CurAffineTransform, UIInterfaceOrientationIsLandscape(orientation));
    unityARCamera.displayTransform = displayTransform;
   // NSLog(@"C7");
    if (UnityIsARKit_2_0_Supported())
    {
        if (@available(iOS 12.0, *))
        {
            unityARCamera.worldMappingStatus = GetUnityARWorldMappingStatusFromARWorldMappingStatus(frame.worldMappingStatus);
        }
    }
  //  NSLog(@"C8");
    if (_frameCallback != NULL)
    {
        
        matrix_float4x4 rotatedMatrix = matrix_identity_float4x4;
        unityARCamera.videoParams.screenOrientation = 3;

        // rotation  matrix
        // [ cos    -sin]
        // [ sin     cos]
        switch (orientation) {
            case UIInterfaceOrientationPortrait:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = 1;
                rotatedMatrix.columns[1][0] = -1;
                rotatedMatrix.columns[1][1] = 0;
                unityARCamera.videoParams.screenOrientation = 1;
                break;
            case UIInterfaceOrientationLandscapeLeft:
                rotatedMatrix.columns[0][0] = -1;
                rotatedMatrix.columns[0][1] = 0;
                rotatedMatrix.columns[1][0] = 0;
                rotatedMatrix.columns[1][1] = -1;
                unityARCamera.videoParams.screenOrientation = 4;
                break;
            case UIInterfaceOrientationPortraitUpsideDown:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = -1;
                rotatedMatrix.columns[1][0] = 1;
                rotatedMatrix.columns[1][1] = 0;
                unityARCamera.videoParams.screenOrientation = 2;
                break;
            default:
                break;
        }

        matrix_float4x4 matrix = matrix_multiply(frame.camera.transform, rotatedMatrix);

        ARKitMatrixToUnityARMatrix4x4(matrix, &unityARCamera.worldTransform);

        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
            _frameCallback(unityARCamera);
            if (unityARCamera.ptrPointCloud != nullptr)
            {
                CFRelease(unityARCamera.ptrPointCloud);
            }
        });
      //  NSLog(@"C9");
    }

    
    if (CVPixelBufferGetPlaneCount(pixelBuffer) < 2 || CVPixelBufferGetPixelFormatType(pixelBuffer) != kCVPixelFormatType_420YpCbCr8BiPlanarFullRange) {
        return;
    }
    
    if (s_UnityPixelBuffers.bEnable)
    {
        
        CVPixelBufferLockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
        
        if (s_UnityPixelBuffers.pYPixelBytes)
        {
            unsigned long numBytes = CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 0) * CVPixelBufferGetHeightOfPlane(pixelBuffer,0);
            void* baseAddress = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,0);
            memcpy(s_UnityPixelBuffers.pYPixelBytes, baseAddress, numBytes);
        }
        if (s_UnityPixelBuffers.pUVPixelBytes)
        {
            unsigned long numBytes = CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 1) * CVPixelBufferGetHeightOfPlane(pixelBuffer,1);
            void* baseAddress = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,1);
            memcpy(s_UnityPixelBuffers.pUVPixelBytes, baseAddress, numBytes);
        }
        
        CVPixelBufferUnlockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
    }
    
    id<MTLTexture> textureY = nil;
    id<MTLTexture> textureCbCr = nil;

    // textureY
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
        MTLPixelFormat pixelFormat = MTLPixelFormatR8Unorm;
        
        
        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 0, &texture);
        if(status == kCVReturnSuccess)
        {
            textureY = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }

    // textureCbCr
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
        MTLPixelFormat pixelFormat = MTLPixelFormatRG8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 1, &texture);
        if(status == kCVReturnSuccess)
        {
            textureCbCr = CVMetalTextureGetTexture(texture);
        }
        if (texture != NULL)
        {
            CFRelease(texture);
        }
    }

    if (textureY != nil && textureCbCr != nil) {
        dispatch_async(dispatch_get_main_queue(), ^{
            // always assign the textures atomic
            s_CapturedImageTextureY = textureY;
            s_CapturedImageTextureCbCr = textureCbCr;
        });
    }
    //NSLog(@"C10");
}

- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame
{
   // NSLog(@"session1");
    if((s_arType==1|| s_arType==2) && s_isQuit) return;
   // NSLog(@"session2");
    if(fpsCount<1){
        fpsCount++;
        return;;
    }else{
        fpsCount=0;
    }
    switch(s_arType){
        case 1:
            [self session:session didALVAImageTargetUpdateFrame:frame];
            break;
        case 2:
            [self session:session didALVAModelTargetUpdateFrame:frame];
            break;
       // case 3:
         //   [self session:session didDefaultUpdateFrame:frame];
         //   break;
        default:
            [self session:session didDefaultUpdateFrame:frame];
            break;
    }
}


- (void)session:(ARSession *)session didFailWithError:(NSError *)error
{
    if (_arSessionFailedCallback != NULL)
    {
        _arSessionFailedCallback(static_cast<const void*>([[error localizedDescription] UTF8String]));
    }
}

- (void)session:(ARSession *)session didAddAnchors:(NSArray<ARAnchor*>*)anchors
{
    [self sendAnchorAddedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didUpdateAnchors:(NSArray<ARAnchor*>*)anchors
{
   [self sendAnchorUpdatedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didRemoveAnchors:(NSArray<ARAnchor*>*)anchors
{
   [self sendAnchorRemovedEventToUnity:anchors];
}

- (void) sendAnchorAddedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorAddedEvent:anchorPtr];
    }
}

- (void)session:(ARSession *)session cameraDidChangeTrackingState:(ARCamera *)camera
{
    if (_arSessionTrackingChanged != NULL)
    {
        UnityARCamera unityCamera;
        GetUnityARCameraDataFromCamera(unityCamera, camera);
        _arSessionTrackingChanged(unityCamera);
    }
}

- (void)sessionWasInterrupted:(ARSession *)session
{
    if (_arSessionInterrupted != NULL)
    {
        _arSessionInterrupted();

    }
}

- (void)sessionInterruptionEnded:(ARSession *)session
{
    if (_arSessionInterruptionEnded != NULL)
    {
        _arSessionInterruptionEnded();
    }
}

- (BOOL)sessionShouldAttemptRelocalization:(ARSession *)session
{
    if (_arSessionShouldRelocalize != NULL)
    {
        return _arSessionShouldRelocalize();
    }
    return NO;
}

- (void) sendAnchorRemovedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorRemovedEvent:anchorPtr];
    }
}

- (void) sendAnchorUpdatedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorUpdatedEvent:anchorPtr];
    }
}

@end

/// Create the native mirror to the C# ARSession object

extern "C" void* unity_CreateNativeARSession()
{
    UnityARSession *nativeSession = [[UnityARSession alloc] init];
    nativeSession->_session = [ARSession new];
    nativeSession->_session.delegate = nativeSession;
    s_unityCameraNearZ = .01;
    s_unityCameraFarZ = 30;
    s_UnityPixelBuffers.bEnable = false;
    return (__bridge_retained void*)nativeSession;
}



extern "C" void session_SetSessionCallbacks(const void* session, UNITY_AR_FRAME_CALLBACK frameCallback,
                                            UNITY_AR_SESSION_FAILED_CALLBACK sessionFailed,
                                            UNITY_AR_SESSION_VOID_CALLBACK sessionInterrupted,
                                            UNITY_AR_SESSION_VOID_CALLBACK sessionInterruptionEnded,
                                            UNITY_AR_SESSION_RELOCALIZE_CALLBACK sessionShouldRelocalize,
                                            UNITY_AR_SESSION_TRACKING_CHANGED trackingChanged,
                                            UNITY_AR_SESSION_WORLD_MAP_COMPLETION_CALLBACK worldMapCompletionHandler,
                                            UNITY_AR_SESSION_REF_OBJ_EXTRACT_COMPLETION_CALLBACK refObjExtractCompletionHandler)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    nativeSession->_frameCallback = frameCallback;
    nativeSession->_arSessionFailedCallback = sessionFailed;
    nativeSession->_arSessionInterrupted = sessionInterrupted;
    nativeSession->_arSessionInterruptionEnded = sessionInterruptionEnded;
    nativeSession->_arSessionShouldRelocalize = sessionShouldRelocalize;
    nativeSession->_arSessionTrackingChanged = trackingChanged;
    nativeSession->_arSessionWorldMapCompletionHandler = worldMapCompletionHandler;
    nativeSession->_arSessionRefObjExtractCompletionHandler = refObjExtractCompletionHandler;
}

extern "C" void session_SetPlaneAnchorCallbacks(const void* session, UNITY_AR_ANCHOR_CALLBACK anchorAddedCallback,
                                            UNITY_AR_ANCHOR_CALLBACK anchorUpdatedCallback,
                                            UNITY_AR_ANCHOR_CALLBACK anchorRemovedCallback)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    UnityARAnchorCallbackWrapper* anchorCallbacks = [[UnityARAnchorCallbackWrapper alloc] init];
    anchorCallbacks->_anchorAddedCallback = anchorAddedCallback;
    anchorCallbacks->_anchorUpdatedCallback = anchorUpdatedCallback;
    anchorCallbacks->_anchorRemovedCallback = anchorRemovedCallback;
    [nativeSession->_classToCallbackMap setObject:anchorCallbacks forKey:(id)[ARPlaneAnchor class]];
}

extern "C" void session_SetUserAnchorCallbacks(const void* session, UNITY_AR_USER_ANCHOR_CALLBACK userAnchorAddedCallback,
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorUpdatedCallback,
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorRemovedCallback)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    UnityARUserAnchorCallbackWrapper* userAnchorCallbacks = [[UnityARUserAnchorCallbackWrapper alloc] init];
    userAnchorCallbacks->_anchorAddedCallback = userAnchorAddedCallback;
    userAnchorCallbacks->_anchorUpdatedCallback = userAnchorUpdatedCallback;
    userAnchorCallbacks->_anchorRemovedCallback = userAnchorRemovedCallback;
    [nativeSession->_classToCallbackMap setObject:userAnchorCallbacks forKey:(id)[ARAnchor class]];
}

extern "C" void session_SetFaceAnchorCallbacks(const void* session, UNITY_AR_FACE_ANCHOR_CALLBACK faceAnchorAddedCallback,
                                               UNITY_AR_FACE_ANCHOR_CALLBACK faceAnchorUpdatedCallback,
                                               UNITY_AR_FACE_ANCHOR_CALLBACK faceAnchorRemovedCallback)
{
#if ARKIT_USES_FACETRACKING
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    UnityARFaceAnchorCallbackWrapper* faceAnchorCallbacks = [[UnityARFaceAnchorCallbackWrapper alloc] init];
    faceAnchorCallbacks->_anchorAddedCallback = faceAnchorAddedCallback;
    faceAnchorCallbacks->_anchorUpdatedCallback = faceAnchorUpdatedCallback;
    faceAnchorCallbacks->_anchorRemovedCallback = faceAnchorRemovedCallback;
    [nativeSession->_classToCallbackMap setObject:faceAnchorCallbacks forKey:[ARFaceAnchor class]];
#endif
}

extern "C" void session_SetImageAnchorCallbacks(const void* session, UNITY_AR_IMAGE_ANCHOR_CALLBACK imageAnchorAddedCallback,
                                                UNITY_AR_IMAGE_ANCHOR_CALLBACK imageAnchorUpdatedCallback,
                                                UNITY_AR_IMAGE_ANCHOR_CALLBACK imageAnchorRemovedCallback)
{
    if (@available(iOS 11.3, *))
    {
        UnityARSession* nativeSession = (__bridge UnityARSession*)session;
        UnityARImageAnchorCallbackWrapper* imageAnchorCallbacks = [[UnityARImageAnchorCallbackWrapper alloc] init];
        imageAnchorCallbacks->_anchorAddedCallback = imageAnchorAddedCallback;
        imageAnchorCallbacks->_anchorUpdatedCallback = imageAnchorUpdatedCallback;
        imageAnchorCallbacks->_anchorRemovedCallback = imageAnchorRemovedCallback;
        [nativeSession->_classToCallbackMap setObject:imageAnchorCallbacks forKey:(id)[ARImageAnchor class]];
    }
}

extern "C" void* session_GetARKitSessionPtr(const void* session)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    return (__bridge void*)nativeSession->_session;
}

extern "C" void* session_GetARKitFramePtr(const void* session)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    return (__bridge void*)nativeSession->_session.currentFrame;
}

extern "C" void StartWorldTrackingSessionWithOptions(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARWorldTrackingConfiguration* config = [ARWorldTrackingConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    session->_getLightEstimation = (BOOL) unityConfig.enableLightEstimation;
    
    if(UnityIsARKit_1_5_Supported() && unityConfig.referenceImagesResourceGroup != NULL && strlen(unityConfig.referenceImagesResourceGroup) > 0)
    {
        NSString *strResourceGroup = [[NSString alloc] initWithUTF8String:unityConfig.referenceImagesResourceGroup];
        if (@available(iOS 11.3, *)) {
            NSSet<ARReferenceImage *> *referenceImages = [ARReferenceImage referenceImagesInGroupNamed:strResourceGroup bundle:nil];
            config.detectionImages = referenceImages;
        }
    }

    if(UnityIsARKit_2_0_Supported())
    {
        if (@available(iOS 12.0, *))
        {
            NSMutableSet<ARReferenceObject *> *referenceObjects = nullptr;
            if (unityConfig.referenceObjectsResourceGroup != NULL && strlen(unityConfig.referenceObjectsResourceGroup) > 0)
            {
                NSString *strResourceGroup = [[NSString alloc] initWithUTF8String:unityConfig.referenceObjectsResourceGroup];
                [referenceObjects setByAddingObjectsFromSet:[ARReferenceObject referenceObjectsInGroupNamed:strResourceGroup bundle:nil]];
            }
            
            if (unityConfig.ptrDynamicReferenceObjects != nullptr)
            {
                NSSet<ARReferenceObject *> *dynamicReferenceObjects = (__bridge NSSet<ARReferenceObject *> *)unityConfig.ptrDynamicReferenceObjects;
                if (referenceObjects != nullptr)
                {
                    [referenceObjects setByAddingObjectsFromSet:dynamicReferenceObjects];
                }
                else
                {
                    referenceObjects = dynamicReferenceObjects;
                }
            }
            
            config.detectionObjects = referenceObjects;
            config.autoFocusEnabled=false;
            config.videoFormat = [ARWorldTrackingConfiguration supportedVideoFormats].lastObject;
        }
    }
   // NSLog(@"config:framesPerSecond=%ld,width=%d,height=%d",config.videoFormat.framesPerSecond,config.videoFormat.imageResolution.width,config.videoFormat.imageResolution.height);
    if (runOptions == UnityARSessionRunOptionsNone)
        [session->_session runWithConfiguration:config];
    else
        [session->_session runWithConfiguration:config options:runOpts];
    
    [session setupMetal];
}


extern "C" void StartWorldTrackingSession(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig)
{
    StartWorldTrackingSessionWithOptions(nativeSession, unityConfig, UnityARSessionRunOptionsNone);
}

extern "C" void StartSessionWithOptions(void* nativeSession, ARKitSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARConfiguration* config = [AROrientationTrackingConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    session->_getLightEstimation = (BOOL) unityConfig.enableLightEstimation;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
}

extern "C" void StartSession(void* nativeSession, ARKitSessionConfiguration unityConfig)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARConfiguration* config = [AROrientationTrackingConfiguration new];
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    session->_getLightEstimation = (BOOL) unityConfig.enableLightEstimation;
    [session->_session runWithConfiguration:config];
    [session setupMetal];
}

extern "C" void StartFaceTrackingSessionWithOptions(void* nativeSession, ARKitFaceTrackingConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
#if ARKIT_USES_FACETRACKING
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARConfiguration* config = [ARFaceTrackingConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARFaceConfigurationFromARKitFaceConfiguration(unityConfig, config);
    session->_getLightEstimation = (BOOL) unityConfig.enableLightEstimation;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
#else
    [NSException raise:@"UnityARKitPluginFaceTrackingNotEnabled" format:@"UnityARKitPlugin: Trying to start FaceTracking session without enabling it in settings."];
#endif
}

extern "C" void StartFaceTrackingSession(void* nativeSession, ARKitFaceTrackingConfiguration unityConfig)
{
    StartFaceTrackingSessionWithOptions(nativeSession, unityConfig, UnityARSessionRunOptionsNone);
}

extern "C" void PauseSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session->_session pause];
    [session teardownMetal];
}

extern "C" void StopSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session teardownMetal];
}

extern "C" UnityARUserAnchorData SessionAddUserAnchor(void* nativeSession, UnityARUserAnchorData anchorData)
{
    // create a native ARAnchor and add it to the session
    // then return the data back to the user that they will
    // need in case they want to remove it
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    
    matrix_float4x4 anchor_transform = matrix_identity_float4x4;
    UnityARMatrix4x4ToARKitMatrix(anchorData.transform, &anchor_transform);
    ARAnchor *newAnchor = [[ARAnchor alloc] initWithTransform:anchor_transform];
    
    [session->_session addAnchor:newAnchor];
    UnityARUserAnchorData returnAnchorData;
    UnityARUserAnchorDataFromARAnchorPtr(returnAnchorData, newAnchor);
    return returnAnchorData;
}

extern "C" void SessionRemoveUserAnchor(void* nativeSession, const char * anchorIdentifier)
{
    // go through anchors and find the right one
    // then remove it from the session
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    for (ARAnchor* a in session->_session.currentFrame.anchors)
    {
        if ([[a.identifier UUIDString] isEqualToString:[NSString stringWithUTF8String:anchorIdentifier]])
        {
            [session->_session removeAnchor:a];
            return;
        }
    }
}

extern "C" void SessionSetWorldOrigin(void* nativeSession, UnityARMatrix4x4 worldMatrix)
{
    if (@available(iOS 11.3, *))
    {
        UnityARSession* session = (__bridge UnityARSession*)nativeSession;
        matrix_float4x4 arWorldMatrix;
        UnityARMatrix4x4ToARKitMatrix(worldMatrix, &arWorldMatrix);
        [session->_session setWorldOrigin:arWorldMatrix];
    }
}

extern "C" void SetCameraNearFar (float nearZ, float farZ)
{
    s_unityCameraNearZ = nearZ;
    s_unityCameraFarZ = farZ;
}

extern "C" void CapturePixelData (uint32_t enable, void* pYPixelBytes, void *pUVPixelBytes)
{
    s_UnityPixelBuffers.bEnable = (BOOL) enable;
    if (s_UnityPixelBuffers.bEnable)
    {
        s_UnityPixelBuffers.pYPixelBytes = pYPixelBytes;
        s_UnityPixelBuffers.pUVPixelBytes = pUVPixelBytes;
    } else {
        s_UnityPixelBuffers.pYPixelBytes = NULL;
        s_UnityPixelBuffers.pUVPixelBytes = NULL;
    }
}

extern "C" struct HitTestResult
{
    void* ptr;
    int count;
};

// Must match ARHitTestResult in ARHitTestResult.cs
extern "C" struct UnityARHitTestResult
{
    ARHitTestResultType type;
    double distance;
    UnityARMatrix4x4 localTransform;
    UnityARMatrix4x4 worldTransform;
    void* anchorPtr;
    bool isValid;
};

// Must match ARTextureHandles in UnityARSession.cs
extern "C" struct UnityARTextureHandles
{
    void* textureY;
    void* textureCbCr;
};

// Cache results locally
static NSArray<ARHitTestResult *>* s_LastHitTestResults;

// Returns the number of hits and caches the results internally
extern "C" int HitTest(void* nativeSession, CGPoint point, ARHitTestResultType types)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    point = CGPointApplyAffineTransform(CGPointMake(point.x, 1.0f - point.y), CGAffineTransformInvert(CGAffineTransformInvert(s_CurAffineTransform)));
    s_LastHitTestResults = [session->_session.currentFrame hitTest:point types:types];

    return (int)[s_LastHitTestResults count];
}
extern "C" UnityARHitTestResult GetLastHitTestResult(int index)
{
    UnityARHitTestResult unityResult;
    memset(&unityResult, 0, sizeof(UnityARHitTestResult));

    if (s_LastHitTestResults != nil && index >= 0 && index < [s_LastHitTestResults count])
    {
        ARHitTestResult* hitResult = s_LastHitTestResults[index];
        unityResult.type = hitResult.type;
        unityResult.distance = hitResult.distance;
        ARKitMatrixToUnityARMatrix4x4(hitResult.localTransform, &unityResult.localTransform);
        ARKitMatrixToUnityARMatrix4x4(hitResult.worldTransform, &unityResult.worldTransform);
        unityResult.anchorPtr = (void*)[hitResult.anchor.identifier.UUIDString UTF8String];
        unityResult.isValid = true;
    }

    return unityResult;
}

extern "C" UnityARTextureHandles GetVideoTextureHandles()
{
    UnityARTextureHandles handles;
    handles.textureY = (__bridge_retained void*)s_CapturedImageTextureY;
    handles.textureCbCr = (__bridge_retained void*)s_CapturedImageTextureCbCr;

    return handles;
}

extern "C" void ReleaseVideoTextureHandles(UnityARTextureHandles handles)
{
    if (handles.textureY != NULL)
    {
        CFRelease(handles.textureY);
    }
    if (handles.textureCbCr != NULL)
    {
        CFRelease(handles.textureCbCr);
    }
}

//extern "C" UnityARMatrix4x4 GetCameraProjectionMatrix()
//{
  //  return s_CameraProjectionMatrix;
//}

extern "C" float GetAmbientIntensity()
{
    return s_AmbientIntensity;
}

extern "C" int GetTrackingQuality()
{
    return s_TrackingQuality;
}

extern "C" bool IsARKitWorldTrackingSessionConfigurationSupported()
{
    return ARWorldTrackingConfiguration.isSupported;
}

extern "C" bool IsARKitSessionConfigurationSupported()
{
    return AROrientationTrackingConfiguration.isSupported;
}

extern "C" void EnumerateVideoFormats(UNITY_AR_VIDEOFORMAT_CALLBACK videoFormatCallback)
{
    if (@available(iOS 11.3, *))
    {
        for(ARVideoFormat* arVideoFormat in ARWorldTrackingConfiguration.supportedVideoFormats)
        {
            UnityARVideoFormat videoFormat;
            videoFormat.ptrVideoFormat = (__bridge void *)arVideoFormat;
            videoFormat.imageResolutionWidth = 1280;//arVideoFormat.imageResolution.width;
            videoFormat.imageResolutionHeight = 720;//arVideoFormat.imageResolution.height;
            videoFormat.framesPerSecond = arVideoFormat.framesPerSecond;
            videoFormatCallback(videoFormat);
        }
    }
}

extern "C" void EnumerateFaceTrackingVideoFormats(UNITY_AR_VIDEOFORMAT_CALLBACK videoFormatCallback)
{
#if ARKIT_USES_FACETRACKING
    if (@available(iOS 11.3, *))
    {
        for(ARVideoFormat* arVideoFormat in ARFaceTrackingConfiguration.supportedVideoFormats)
        {
            UnityARVideoFormat videoFormat;
            videoFormat.ptrVideoFormat = (__bridge void *)arVideoFormat;
            videoFormat.imageResolutionWidth = 1280;//arVideoFormat.imageResolution.width;
            videoFormat.imageResolutionHeight = 720;//arVideoFormat.imageResolution.height;
            videoFormat.framesPerSecond = arVideoFormat.framesPerSecond;
            videoFormatCallback(videoFormat);
        }
    }
#else
    [NSException raise:@"UnityARKitPluginFaceTrackingNotEnabled" format:@"UnityARKitPlugin: Checking FaceTracking video formats without enabling it in settings."];
#endif
}

extern "C" bool Native_IsARKit_1_5_Supported()
{
    return UnityIsARKit_1_5_Supported();
}

extern "C" bool Native_IsARKit_2_0_Supported()
{
    return UnityIsARKit_2_0_Supported();
}

extern "C" bool IsARKitFaceTrackingConfigurationSupported()
{
#if ARKIT_USES_FACETRACKING
    return ARFaceTrackingConfiguration.isSupported;
#else
    [NSException raise:@"UnityARKitPluginFaceTrackingNotEnabled" format:@"UnityARKitPlugin: Checking FaceTracking device support without enabling it in settings."];
    return false;
#endif
}

extern "C" void GetBlendShapesInfo(void* ptrDictionary, void (*visitorFn)(const char* key, const float value))
{
#if ARKIT_USES_FACETRACKING
    // Get your NSDictionary
    NSDictionary<ARBlendShapeLocation, NSNumber*> * dictionary = (__bridge NSDictionary<ARBlendShapeLocation, NSNumber*> *) ptrDictionary;
    
    for(NSString* key in dictionary)
    {
        NSNumber* value = [dictionary objectForKey:key];
        visitorFn([key UTF8String], [value floatValue]);
    }
#endif
}

extern "C" void setArType(int unityArType,int unityRotation)
{
    s_arType=unityArType;
    s_isInitARSDK=false;
    s_rotation=(alvaro)unityRotation;
   // NSLog(@"rotation=%d",s_rotation);
}

extern "C" void setALVACoreInfo(char* unityCompanyName,char* unityAuthString,char* unityModelPath,unsigned char* guideImage)
{
    s_InitSucceed = -1;
    s_isQuit=false;
    memset(s_companyname,0,s_charSize);
    memset(s_authString,0,s_charSize * 8);
    memset(s_modelPath,0,s_charSize * 8);
    strcpy(s_companyname, unityCompanyName);
    strcpy(s_authString, unityAuthString);
    strcpy(s_modelPath, unityModelPath);
    s_guideImage=guideImage;
}

extern "C" void unitMT()
{
    s_isQuit=true;
    //s_isInitARSDK=false;
    int ret=MT_unit();
    if(ret!=0){
        NSLog(@"MT_unit error %d",ret);
    }
    if(s_outPixelBuffer!=nullptr){
        CVPixelBufferRelease(s_outPixelBuffer);
        s_outPixelBuffer=nullptr;
        s_cameraDataIn[0]=nullptr;
        s_cameraDataIn[1]=nullptr;
        s_cameraDataOut[0]=nullptr;
        s_cameraDataOut[1]=nullptr;
        
    }
}

extern "C" bool isInitARSDK()
{
    return s_isInitARSDK;
}

extern "C" int isInitSucceed()
{
    return s_InitSucceed;
}

extern "C" void unitIR()
{
    s_isQuit=true;
   // s_isInitARSDK=false;
    int ret=IR_unit();
    if(ret!=0){
        NSLog(@"IR_unit error %d",ret);
    }
    
    if(s_outPixelBuffer!=nullptr){
        CVPixelBufferRelease(s_outPixelBuffer);
        s_outPixelBuffer=nullptr;
        s_cameraDataIn[0]=nullptr;
        s_cameraDataIn[1]=nullptr;
        s_cameraDataOut[0]=nullptr;
        s_cameraDataOut[1]=nullptr;
    }
}

#ifdef __cplusplus
extern "C" {
#endif
    
void session_GetCurrentWorldMap(void* sessionPtr, const void* callbackPtr)
{
    if (sessionPtr == nullptr)
        return;
    
    UnityARSession* nativeSession = (__bridge UnityARSession*)sessionPtr;
    if (!UnityAreFeaturesSupported(kUnityARKitSupportedFeaturesWorldMap))
    {
        // If 2.0 is not supported, then invoke callback immediately with a null world map
        nativeSession->_arSessionWorldMapCompletionHandler(callbackPtr, nullptr);
        return;
    }
    
    if (@available(iOS 12.0, *))
    {
        [nativeSession->_session getCurrentWorldMapWithCompletionHandler:^(ARWorldMap* worldMap, NSError* error)
         {
             if (error)
                 NSLog(@"%@", error);
             
             nativeSession->_arSessionWorldMapCompletionHandler(callbackPtr, (__bridge_retained void*)worldMap);
         }];
    }
    else
    {
        // Fallback on earlier versions
        nativeSession->_arSessionWorldMapCompletionHandler(callbackPtr, nullptr);
        return;
    }
}

void session_ExtractReferenceObject(void * sessionPtr, UnityARMatrix4x4 unityTransform, UnityARVector3 unityCenter, UnityARVector3 unityExtent, const void* callbackPtr)
{
    if (sessionPtr == nullptr)
        return;
    
    UnityARSession* nativeSession = (__bridge UnityARSession*)sessionPtr;

    if (!UnityAreFeaturesSupported(kUnityARKitSupportedFeaturesReferenceObject))
    {
        // If 2.0 is not supported, then invoke callback immediately with a null reference object
        nativeSession->_arSessionRefObjExtractCompletionHandler(callbackPtr, nullptr);
        return;
    }
    
    matrix_float4x4 transform;
    UnityARMatrix4x4ToARKitMatrix(unityTransform, &transform);
    
    const vector_float3 center{unityCenter.x, unityCenter.y, unityCenter.z};
    const vector_float3 extent{unityExtent.x, unityExtent.y, unityExtent.z};

    if (@available(iOS 12.0, *))
    {
        [nativeSession->_session createReferenceObjectWithTransform:transform center:center extent:extent completionHandler:^(ARReferenceObject * referenceObject, NSError * error)
         {
             if (error)
                 NSLog(@"%@", error);
             nativeSession->_arSessionRefObjExtractCompletionHandler(callbackPtr, (__bridge_retained void*)referenceObject);
             
         }];
    }
    else
    {
        // Fallback on earlier versions
        nativeSession->_arSessionRefObjExtractCompletionHandler(callbackPtr, nullptr);
        return;
    }
}
    
bool sessionConfig_IsEnvironmentTexturingSupported()
{
    if (@available(iOS 12.0, *)) {
        if ([AREnvironmentProbeAnchor class])
        {
            return true;
        }
        else
        {
            return  false;
        }
    }
    else
    {
        // Fallback on earlier versions
        return  false;
    }
}
  

#ifdef __cplusplus
}
#endif


