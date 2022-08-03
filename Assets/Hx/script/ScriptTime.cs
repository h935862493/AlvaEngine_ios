using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTime : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestExeTime();
        }
    }
    void TestExeTime()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start(); //  开始监视代码运行时间
        TestFunc();
        stopwatch.Stop(); //  停止监视
        //  获取当前实例测量得出的总时间
        System.TimeSpan timespan = stopwatch.Elapsed;
        //   double hours = timespan.TotalHours; // 总小时
        //    double minutes = timespan.TotalMinutes;  // 总分钟
        //    double seconds = timespan.TotalSeconds;  //  总秒数
        double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
        //打印代码执行时间
        Debug.Log(milliseconds);
    }

    void TestFunc()
    {
        int m = 124;
        int n = 256;
        int mn = m + n;
        Debug.Log(mn);
    }

}
