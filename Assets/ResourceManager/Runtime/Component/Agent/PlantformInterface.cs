using System.Runtime.InteropServices;
using UnityEngine;

public static class PlantformInterface
{

#if UNITY_ANDROID
    public static void ReturnAppCalBack()
    {
        Debug.Log("===============调用Unity================");
        AndroidJavaObject mainActivity;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("ReturnAPP");
        Debug.Log("===============调用完成================");
    }

    public static void AlvaAddBrowserView(string url)
    {
        Debug.Log("===============调用Unity================");
        AndroidJavaObject mainActivity;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call<string>("AlvaAddBrowserView", url);
        Debug.Log("===============调用完成================");
    }

    public static void CloseAlvaBrowserView()
    {
        Debug.Log("===============调用Unity================");
        AndroidJavaObject mainActivity;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("CloseAlvaBrowserView");
        Debug.Log("===============调用完成================");
    }
#endif


#if UNITY_IOS
    //[DllImport("__Internal")]
    //private static extern void _returnApp();

    public static void ReturnAppCalBack()
    {
        Debug.Log("===============调用Unity================ReturnAppCalBack");
        //_returnApp();
        Debug.Log("===============调用完成================ReturnAppCalBack");
    }

    //[DllImport("__Internal")]
    //private static extern void _iosAlvaAddBrowserView(string url);

    public static void AlvaAddBrowserView(string url)
    {
        Debug.Log("===============调用Unity================AlvaAddBrowserView");
        //_iosAlvaAddBrowserView(url);
        Debug.Log("===============调用完成================AlvaAddBrowserView");
    }

    //[DllImport("__Internal")]
    //private static extern void _iosCloseAlvaBrowserView();

    public static void CloseAlvaBrowserView()
    {
        Debug.Log("===============调用Unity================CloseAlvaBrowserView");
        //_iosCloseAlvaBrowserView();
        Debug.Log("===============调用完成================CloseAlvaBrowserView");
    }

    //[DllImport("__Internal")]
    //private static extern void _iosAlvaHasLoadMe();

    public static void HasLaodMeScene()
    {
        Debug.Log("===============调用Unity================");
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            //_iosAlvaHasLoadMe();
        }
        Debug.Log("===============调用完成================");
    }
#endif
}
