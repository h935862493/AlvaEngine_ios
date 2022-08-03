using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitScane());
    }

    IEnumerator InitScane()
    {
        yield return new WaitForSeconds(2f);
        //读取并加载识别信息
        //while (!VuforiaControler.GetInstance().GetTracker())
        //{
        //    Debug.Log("Found Vuforia Null");
        //    yield return new WaitForEndOfFrame();
        //}
        bool datesetResult = ZManager.instnace.zLoadARRes.OnLoadDatSet();
        if (!datesetResult)
        {
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("数据包读取识别或不存在，请检查更正后重新启动项目！");
        }
        else
        {
            RecognitionAction?.Invoke(GlobalData.RecognitionType);
        }
    }

    public Action<Pro2DRecognition> RecognitionAction;
}
