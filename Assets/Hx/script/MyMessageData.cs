using UnityEngine;

public class MyMessageData
{
    public static AssetBundle assetBundle;
    /// <summary>FDJ1场景用</summary>
    public static AssetBundle assetBundle_FDJ1;
    public static string curentBundleName;
    public static bool isFirstCheck = true;
    public static bool isFirstMainTop = true;
    public static AsyncOperation _asyncOperation;
    
    /// <summary>
    /// 第一次启动程序
    /// </summary>
    public static bool isFirstRun = true;
    /// <summary>
    /// 当前用户点击的界面
    /// </summary>
    static int _pageIndex = 0;

    public static int pageIndex
    {
        get
        {
            return _pageIndex;
        }
        set
        {
            _pageIndex = value;
        }
    }

    //当前界面滑动条到的位置
    public static float publicContentY, projectContentY, locationContentY;

    public static Vector2 parentCellSize;

    public static string BtnName;
    public static string ProjectPath;
    /// <summary>识别授权码</summary>
    public static string alvaLicense;
    /// <summary>识别类型1=image , 2=model</summary>
    public static string scanType;
    /// <summary>公司名</summary>
    public static string companyName = "AlvaSystems";
}

public enum AlvaType
{
    SLAM,
    ImageTarget,
    ModelTarget,
}
