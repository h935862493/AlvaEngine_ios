using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class ZBindPermissions : ZBase
{
    public override void OnInitComp()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            //{
            //    The user authorized use of the microphone.
            //   Debug.Log("============有IMEI权限============");
            //}
            //else
            //{
            //    StartCoroutine(AllowPhoneState());
            //    Permission.RequestUserPermission(Permission.FineLocation);
            //    Debug.Log("===========申请IMEI权限=============");
            //}
            //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");//com.unity3d.player.UnityPlayer;
            //AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");// jc.GetStatic<AndroidJavaObject> currentActivity
            ////AndroidJavaObject jo = new AndroidJavaObject("com.alva.vision.MainActivity");
            //string result = jo.Call<string>("getDeviceId");
            //Debug.Log("IMEI获取结果" + result);
        }
    }

    public override void OnInitData()
    {
    }

    public override void OnInitFunc()
    {
    }

    public override void OnInstance()
    {
    }
    string[] strs = new string[] {
       "android.permission.INTERNET",
        "android.permission.READ_PHONE_STATE",
        "android.permission.READ_EXTERNAL_STORAGE",
        "android.permission.WRITE_EXTERNAL_STORAGE",
        "android.permission.ACCESS_WIFI_STATE",
        "android.permission.ACCESS_NETWORK_STATE",
        "android.permission.CAMERA",
        "android.permission.DISABLE_KEYGUARD",
        "android.permission.RECORD_AUDIO",
        "android.permission.SYSTEM_ALERT_WINDOW",
        //"android.permission.CHANGE_WIFI_STATE",
        //"android.permission.CHANGE_NETWORK_STATE",
        //"android.permission.ACCESS_COARSE_LOCATION",
        //"android.permission.ACCESS_FINE_LOCATION",
        //"android.permission.SYSTEM_OVERLAY_WINDOW",
        //"android.permission.ACCESS_COARSE_UPDATES",
        "android.permission.WRITE_SETTINGS",
        //"android.permission.BATTERY_STATS",
        //"android.permission.MOUNT_UNMOUNT_FILESYSTEMS"
    };

    string imei = "android.permission.READ_PHONE_STATE";

    public IEnumerator AllowPhoneState()
    {
        for (int i = 0; i < strs.Length; i++)
        {
            Permission.RequestUserPermission(strs[i]);
            while (!Permission.HasUserAuthorizedPermission(strs[i]))
            {
                yield return new WaitForEndOfFrame();
            }
        }
       
    }
}
