using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonConfig
{
}

public class VersionData
{
    public VersionData()
    {

    }
    public string UserID;//暂用用户名作为唯一标识
    public List<VisionProjectInfo> UserProList;
}

public enum ProjectType
{
    ImageRecognition,
    ModelRecognition,
    SlamRecognition,
    AreaTarget,
    Capture,
    /// <summary>
    /// 多场景里的空白场景
    /// </summary>
    ManyScenes,
    Empty
}

public class SceneInfo
{
    public float resolutionRatioWidth;
    public float resolutionRatioHeight;
    public int pageNumber = 1;
    public bool light = true;
}

public enum Pro2DRecognition
{
    None,
    Image,
    VuMark
}


/// <summary>
/// 数据描述模型
/// </summary>
public class ProjectDescriptionDataModel
{
    public string ID;
    public string ProjectName;
    public string Type;
    public string Path;
    public string Time;
    public string PublishStatus;
    public string Version;
}

/// <summary>
/// 物体描述
/// </summary>
public class EditableObjectsDataModel
{
    public List<EditableObjects> editableObjectsList = new List<EditableObjects>();
    public string id = string.Empty;
    public string name = string.Empty;
    public bool active = true;
    public string type = "Model";
    public float[] position = new float[3];
    public float[] rotation = new float[3];
    public float[] scale = new float[3];
    public List<string[]> eventIndex = new List<string[]>();
    public string style = string.Empty;
    public string description = string.Empty;
    public string text = "新建文本";
    public int fontSize = 14;
    public string color = "FFFFFFFF";
    public string sourOriginalNames = string.Empty;
    public int task;
    public string scriptPath = string.Empty;
    public float gifSpeed = 5f;
    public bool awakeOnPlay = false;
    public bool isLoop = false;
    public string[] descriptions;
    public string serialId;
    public int viewPage = 1;
    public float[] aniSpeed;
    public int processIndex = 0;
    public int processGroup = 1;
    public string processdescription = string.Empty;
    public List<string[]> animationData;
    public string toggleImage = string.Empty;
    public bool m_switch;
    public float valueRangeMax = 0;
    public float valueRangeMin = 0;
}
[System.Serializable]
public class EditableObjects
{
    public List<EditableObjects> editableObjectsList = new List<EditableObjects>();
    public string name = "";
    public bool active = true;
    public float[] position = new float[3];
    public float[] rotation = new float[3];
    public float[] scale = new float[3];
    public int task = -1;
    public string scriptPath = string.Empty;
}
[System.Serializable]
public class AREAnimation
{
    public List<AREAnimationState> aREAnimationStates;
}
[System.Serializable]
public sealed class AREAnimationState
{
    public float weight;
    public float time;
    public float speed;
    public string name;
    public AREAnimationClip aREAnimationClip;
}
[System.Serializable]
public sealed class AREAnimationClip
{
    public bool legacy;
    public UnityEngine.WrapMode wrapMode;
    public float frameRate;
    public List<AREAnimationCurve> aREAnimationCurves;
}
[System.Serializable]
public class AREAnimationCurve
{
    public string relativePath;
    public string type;
    public string propertyName;
    public List<AREKeyframe> keyframes;
}
[System.Serializable]
public class AREKeyframe
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;
    public float inWeight;
    public float outWeight;
}

public class TaskJson
{
    public List<Task> tasks = new List<Task>();
}
public class Task
{
    public int number;
    public string title;
    public string type;
}


/// <summary>
/// 资源类型，用来判断面板刷新
/// </summary>
public enum SourType
{
    None,
    Model,
    Picture,
    Video,
    Audio,
    Image,
    Text,
    Button,
    Video2D,
    ParticleSystem,
    ImageTarget,
    ModelTarget,
    GroundPlane,
    Gif2D,
    Dashboard,
    Link2D,
    Hotspot,
    Gif3D,
    InputField3D,
    Button3D,
    Toggle3D,
    Group3D
}


public class StepBind
{
    /// <summary>
    /// 步骤
    /// </summary>
    public int step;
    /// <summary>
    /// 步骤名称
    /// </summary>
    public List<GameObject> objList;
    /// <summary>
    /// 步骤描述
    /// </summary>
    public string desc;

    public StepBind(List<GameObject> objs, int step, string desc)
    {
        this.objList = objs;
        this.step = step;
        this.desc = desc;
    }
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    public string token { get; set; }
    public UserProfiles profiles { get; set; }
    public UserPermission permission { get; set; }

}
public class UserPermission
{
    public string appName { get; set; }
    public string expireTimeStamp { get; set; }
    public DateTime expireTime { get; set; }
    public double remainingDay { get; set; }
    public List<UserFunctions> functions { get; set; }
}
public class UserFunctions
{
    public string functionId { get; set; }
    public string functionName { get; set; }
    public string functionValue { get; set; }
    public string description { get; set; }
    public string id { get; set; }
}
public class UserProfiles
{
    //public string avatar { get; set; }
    //public string deptInfo { get; set; }
    //public string email { get; set; }
    //public long id { get; set; }
    //public string mobile { get; set; }
    //public string nickname { get; set; }
    //public string realname { get; set; }
    //public int rowVersion { get; set; }
    //public int userKind { get; set; }
    //public string username { get; set; }
    //public string orgId { get; set; }
    //public bool administrator { get; set; }

    public string avatar { get; set; }
    public string deptInfo { get; set; }
    public string email { get; set; }
    public long id { get; set; }
    public string mobile { get; set; }
    public string nickName { get; set; }
    public string realname { get; set; }
    public int rowVersion { get; set; }
    public int userKind { get; set; }
    public int kind { get; set; }
    public string userName { get; set; }
    public long orgId { get; set; }
    public int gender { get; set; }
    public int level { get; set; }
    public bool locked { get; set; }
    public string ssoOrg { get; set; }
    public bool administrator { get; set; }
}
public class LoginUserRoles
{
    public string id;//"id": "1",
    public string serialId;//   "serialId": "e4025ddbfe854486a53626421185a0aa",
    public string name;//    "name": "Guider-Employee",
    public string description;//    "description": "Guider-Employee",
    public string alias;//    "alias": "3dc7199582901f93",
    public string code;//     "code": "214487b14fd89012",
    public string path;//     "path": "e4025ddbfe854486a53626421185a0aa,",
    public int level;//     "level": 1,
    public string createdUserId;//     "createdUserId": "0",
    public string createdTime;//    "createdTime": "2020-08-17 11:08:46",
    public int updatedUserId;//     "updatedUserId": 0,
    public string updatedTime;//     "updatedTime": "2020-08-17 11:08:46",
    public int rowVersion;//     "rowVersion": 1
}

public class LoginUserGroups
{
    public string id;//"id": "1",
    public string serialId;//   "serialId": "e4025ddbfe854486a53626421185a0aa",
    public string name;//    "name": "Guider-Employee",
    public string description;//    "description": "Guider-Employee",
    public string alias;//    "alias": "3dc7199582901f93",
    public string code;//     "code": "214487b14fd89012",
    public string path;//     "path": "e4025ddbfe854486a53626421185a0aa,",
    public int level;//     "level": 1,
    public string createdUserId;//     "createdUserId": "0",
    public string createdTime;//    "createdTime": "2020-08-17 11:08:46",
    public string updatedUserId;//     "updatedUserId": 0,
    public string updatedTime;//     "updatedTime": "2020-08-17 11:08:46",
    public int rowVersion;//     "rowVersion": 1
    public int rowState;//"rowState": 1,
}

public class LoginUserSetting
{
    public LoginUserSettingInfo model;
    public LoginUserSettingInfo project;
}

public class LoginUserSettingInfo
{
    public int scope;
}

public class LoginDeviceInfo
{
    public string id;// "id": "2",
    public string serialId;//"serialId": "7a648ce6415e42e29b60ff575f390447",
    public string name; // "name": "Guider-Capture",
    public string description; //   "description": "Guider-Capture",
    public LoginSetting setting; //    "setting": {
    public string createdUserId; //  "createdUserId": "0",
    public string createdTime; //   "createdTime": "2020-08-17 10:04:26",
    public string updatedUserId; //   "updatedUserId": "0",
    public string updatedTime; // "updatedTime": "2020-08-17 10:04:26",
    public int rowState; //  "rowState": 1,
    public int rowVersion; //   "rowVersion": 1
}

public class LoginSetting
{
    public bool repeatLogin;
}


/// <summary>
/// 登录信息
/// </summary>
public class LoginJson
{
    public string loginId;
    public string loginPassword;
    public string deviceSerialId;
}
/// <summary>
/// 服务器返回结果通用解析结构
/// </summary>
public class ServerMessages<T>
{
    public int code;
    public string message;
    public List<T> data;
}
/// <summary>
/// 获取用户信息
/// </summary>
public class GetUserInfo
{
    public string name;
    public string token;
}
/// <summary>
/// 获取项目列表
/// </summary>
public class GetProjectInfo
{
    public string userName;
    public string token;
}

public class PageProjectInfo
{
    public List<VisionProjectInfo> items;
    public int rows;
    public int pages;
}
/// <summary>
/// 项目信息
/// </summary>
public class VisionProjectInfo
{
    public string id { get; set; }
    public string serialId { get; set; }
    public string name { get; set; }
    public string category { get; set; }
    public bool visibleState { get; set; }
    public bool hide { get; set; }
    public bool down { get; set; }
    public bool collect { get; set; }
    public string pVersion { get; set; }
    public bool top { get; set; }
    public string actionState { get; set; }
    public List<ItemsThumbnail> thumbnail { get; set; }
    public string scope { get; set; }
    public string shareCode { get; set; }
    public bool release { get; set; }
    public string createdUserId { get; set; }
    public DateTime createdTime { get; set; }
    public string updatedUserId { get; set; }
    public DateTime updatedTime { get; set; }
    public int rowVersion { get; set; }
    public List<ResPackages> resPackages { get; set; }
    public int platform { get; set; }

    #region 老vision
    //public DateTime createTime;
    //public DateTime updateTime;
    public int isSet;
    public string thumbnailMD5;
    public string thumbnailExtension;
    public string fileMD5;
    public string webUrl;
    /// <summary>true=vuforia核心，false=Alva核心</summary>
    public bool vuforiaType;
    public string thumbnailCloudSerialId;
    public string fileCloudSerialId;
    #endregion
}
public class ResPackages
{
    public string id { get; set; }
    public string serialId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int category { get; set; }
    public string categoryText { get; set; }
    public List<string> thumbnail { get; set; }
    public ResPackage resPackage { get; set; }
    public string createdUserId { get; set; }
    public DateTime createdTime { get; set; }
    public string updatedUserId { get; set; }
    public DateTime updatedTime { get; set; }
    public int rowVersion { get; set; }
    public int packetsQty { get; set; }
}
public class ItemsThumbnail
{
    public string name { get; set; }
    public string description { get; set; }
    public string bucketSerialId { get; set; }
    public bool ownershipPrivate { get; set; }
    public int mediumType { get; set; }
    public string mediumTypeText { get; set; }
    public string diskFileSerialId { get; set; }
    public string cTag { get; set; }
    public string cProperty { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public DateTime createTime { get; set; }
    public DateTime updateTime { get; set; }
    public int rowState { get; set; }
    public int rowVersion { get; set; }
    public ItemsThumbnailFile file { get; set; }
}
public class ItemsThumbnailFile
{
    public string name { get; set; }
    public string hash { get; set; }
    public string extension { get; set; }
    public string size { get; set; }
    public int referenceQty { get; set; }
    public int uploadState { get; set; }
    public string uploadStateMessage { get; set; }
    public string downloadUrl { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public DateTime createTime { get; set; }
    public DateTime updateTime { get; set; }
    public int rowState { get; set; }
    public int rowVersion { get; set; }
}
public class ResPackage
{
    public string name { get; set; }
    public string description { get; set; }
    public string bucketSerialId { get; set; }
    public bool ownershipPrivate { get; set; }
    public int mediumType { get; set; }
    public string mediumTypeText { get; set; }
    public string diskFileSerialId { get; set; }
    public string cTag { get; set; }
    public string cProperty { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public DateTime createTime { get; set; }
    public DateTime updateTime { get; set; }
    public int rowState { get; set; }
    public int rowVersion { get; set; }
    public ResPackageFile file { get; set; }
}
public class ResPackageFile
{
    public string name { get; set; }
    public string hash { get; set; }
    public string extension { get; set; }
    public string size { get; set; }
    public int referenceQty { get; set; }
    public int uploadState { get; set; }
    public string uploadStateMessage { get; set; }
    public string downloadUrl { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public DateTime createTime { get; set; }
    public DateTime updateTime { get; set; }
    public int rowState { get; set; }
    public int rowVersion { get; set; }
}

/// <summary>
/// 项目下载所需信息
/// </summary>
public class DownloadInfo
{
    public string ProjectID;
    public string token;
}
/// <summary>
/// 置顶所需参数
/// </summary>
public class SetTopInfo
{
    public bool top;
}

public class NewServerMessage<T>
{
    public T data;
    public int code;
    public string message;
}


public class ServerMgsList<T>
{
    public List<T> items;
    public int rows;
    public int pages;
}

public class ModelServerItem
{
    public int id;
    public string serialId;
    public string name;
    public string description;
    public string projectId;
    public string projectSN;
    public int createdUserId;
    public string createdTime;
    public int updatedUserId;
    public string updatedTime;
    public int rowState;
    public int rowVersion;
    public bool isDownload;
}

public class AnimationEventJsons
{
    public List<AnimationEventJson> animationEventJsons = new List<AnimationEventJson>();
}
public class AnimationEventJson
{
    public string ID;
    public string modelID;
    public string animationClipName;
    public string animationEvent;
    public int function;
    public string parameter;
}

[System.Serializable]
public class directoryData
{
    public string path;
    public bool isForlder;
    public List<directoryData> directoryDatas = new List<directoryData>();
    public List<FileData> files = new List<FileData>();
}

[System.Serializable]
public class FileData
{
    public string path;
    public bool isForlder;
}

//文件树
[Serializable]
public class FileBranch
{
    public string branchName;
    public List<FileBranch> fileBranch = new List<FileBranch>();
    public List<FileDescribe> childFiles = new List<FileDescribe>(); //子级文件 
}
//文件描述
[Serializable]
public class FileDescribe
{
    public string md5; //文件ID
    public string name;
    public string extension;  //文件后缀
    public string serialId;
}

public class RainbowFile
{
    public string name { get; set; }
    public string hash { get; set; }
    public string extension { get; set; }
    public string size { get; set; }
    public int referenceQty { get; set; }
    public int uploadState { get; set; }
    public string uploadStateMessage { get; set; }
    public string downloadUrl { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public string createTime { get; set; }
    public string updateTime { get; set; }
    public int rowState { get; set; }
}

public class RainbowData
{
    public string name { get; set; }
    public string description { get; set; }
    public string bucketSerialId { get; set; }
    public bool ownershipPrivate { get; set; }
    public int storageMediumType { get; set; }
    public string expires { get; set; }
    public string diskFileSerialId { get; set; }
    public RainbowFile file { get; set; }
    public string cTag { get; set; }
    public string id { get; set; }
    public string serialId { get; set; }
    public string createTime { get; set; }
    public string updateTime { get; set; }
    public int rowState { get; set; }
}
