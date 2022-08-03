using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
//using RestSharp;
using UnityEngine;
using UnityEngine.Networking;

public class ZServer : ZBase
{
    public ZServer Instance;

    public override void OnInitComp()
    {
    }

    public override void OnInitData()
    {
    }

    public override void OnInitFunc()
    {
    }

    public override void OnInstance()
    {
        Instance = this;

    }

    private struct NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }
    private List<NoDelayedQueueItem> _actions = new List<NoDelayedQueueItem>();
    private List<NoDelayedQueueItem> _currentActions = new List<NoDelayedQueueItem>();

    private void Update()
    {
        if (_actions.Count > 0)
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            for (int i = 0; i < _currentActions.Count; i++)
            {
                _currentActions[i].action(_currentActions[i].param);
            }
        }
    }
    public void QueueOnMainThread(Action<object> taction, object param)
    {
        lock (_actions)
        {
            _actions.Add(new NoDelayedQueueItem { action = taction, param = param });
        }
    }
    /// <summary>
    /// 网络下载文件
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="fileName">文件名</param>
    /// <param name="info">项目下载所需信息</param>
    /// <param name="_isStop">是否停止</param>
    /// <param name="callBack">下载接口回调</param>
    /// <param name="ProcessAction">下载进度回调</param>
    public void DownloadFile(string url, string fileName, DownloadInfo info, bool _isStop, Action<string> callBack, Action<float> ProcessAction)
    {
        if (m_Urls.Contains(url))
        {
            return;
        }
        m_Urls.Add(url);
        DownLoadFile_HTTP(url, fileName, info, _isStop, callBack, ProcessAction);
    }
    public List<string> m_Urls = new List<string>();

    /// <summary>
    /// 网络下载文件
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="snID">云模型ID</param>
    /// <param name="fileName">文件名</param>
    /// <param name="info">项目下载所需信息</param>
    /// <param name="_isStop">是否停止</param>
    /// <param name="callBack">下载接口回调</param>
    /// <param name="ProcessAction">下载进度回调</param>
    public void DownloadFile(string url, string snID, string fileName, DownloadInfo info, bool _isStop, Action<string> callBack, Action<float> ProcessAction)
    {
        DownLoadModel_HTTP(url, snID, fileName, info, _isStop, callBack, ProcessAction);
    }
    /// <summary>
    /// 本地下载文件
    /// </summary>
    /// <param name="url">本地地址</param>
    /// <param name="callBack">结果回调</param>
    /// <param name="ProcessAction">加载进度</param>
    public void DownloadFileLocal(string url, Action<DownloadHandler> callBack, Action<float> ProcessAction)
    {
        StartCoroutine(DownloadFileLocal_Coroutine(url, callBack, ProcessAction));
    }

    //public void DownloadFile_Muilty(List<string> urllist, object jsonObject, bool _isStop, Action<DownloadHandler,string> callBack, Action<float> ProcessAction)
    //{
    //    Debug.Log("-----");
    //    StartCoroutine(DownloadFile_Muilty_Coroutine(urllist, jsonObject, _isStop, callBack, ProcessAction));
    //}

    //IEnumerator DownloadFile_Muilty_Coroutine(List<string> urllist, object jsonObject, bool _isStop, Action<DownloadHandler,string> callBack, Action<float> ProcessAction)
    //{
    //    foreach (var item in urllist)
    //    {
    //        yield return StartCoroutine(DownloadFile_Coroutine(item, jsonObject, _isStop, callBack, ProcessAction));
    //    } 
    //}
    #region unitywebquest

    ///// <summary>
    ///// 协程：下载文件
    ///// </summary>
    ///// <param name="url">请求的Web地址</param>
    ///// <param name="filePath">文件保存路径</param>
    ///// <param name="callBack">下载完成的回调函数</param>
    ///// <param name="_isStop">是否中断下载任务</param>
    ///// <returns></returns>
    //IEnumerator DownloadFile_Coroutine(string url,string fileName, DownloadInfo info, bool _isStop, Action<DownloadHandler,string> callBack,  Action<float> ProcessAction)
    //{
    //    bool downLoad = true;
    //    ulong length = 0;
    //    Stream ns;
    //    //long currentLength = 0;
    //    //下载进度
    //    float persent = 0;
    //    string savePath = GlobalData.LocalPath;

    //    string tempFileName = fileName + "_TMP" + ".tmp";

    //    string tempPath = savePath + tempFileName;

    //    string finalPath = savePath + fileName;

    //    string reName = fileName + ".zip";

    //    //long stationId = stationDic[stationNum];

    //    //string requestUrl = "http://dl.google.com/android/installer_r24.4.1-windows.exe";

    //    //string requestUrl = Consts.baseUrl + Consts.getModlesUrl + stationId;

    //    if (!Directory.Exists(savePath))
    //    {
    //        Directory.CreateDirectory(savePath);
    //    }
    //    Uri uri = new Uri(url);
    //    Debug.Log(uri.AbsoluteUri);

    //    WWWForm form = new WWWForm();
    //    form.AddField("ProjectID", info.ProjectID);
    //    // form.AddField("token", info.token);

    //    FileStream fs;

    //    //如果不存在，则下载
    //    if (!Directory.Exists(finalPath))
    //    {
    //        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!" + savePath + reName);
    //        if (!File.Exists(savePath + reName))
    //        {
    //            long lStartPos = 0;

    //            if (!File.Exists(tempPath))
    //            {
    //                fs = new FileStream(tempPath, FileMode.Create);
    //                lStartPos = 0;
    //            }
    //            else
    //            {
    //                //如果在临时文件路径下存在临时文件则继续下载临时文件
    //                fs = File.OpenWrite(tempPath);
    //                lStartPos = fs.Length;
    //                fs.Seek(lStartPos, SeekOrigin.Current);
    //            }
    //            UnityWebRequest uwr = UnityWebRequest.Post(uri.AbsoluteUri, form);// url method
    //                                                                              //uwr.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
    //            uwr.SetRequestHeader("token", info.token);
    //            uwr.SendWebRequest();//开始请求
    //            //long totillength = request.GetResponse().ContentLength;
    //            //if (lStartPos > 0)
    //            //{
    //            //    uwr.AddRange((int)lStartPos);
    //            //}
    //            length = uwr.downloadedBytes;
    //            byte[] nbytes = new byte[102400];
    //            int nReadSize = 0;
    //            nReadSize = ns.Read(nbytes, 0, 102400);
    //            while (nReadSize > 0)
    //            {
    //                persent = (float)currentLength / len;
    //                currentLength += nReadSize;
    //                fs.Write(nbytes, 0, nReadSize);
    //                nReadSize = ns.Read(nbytes, 0, 102400);
    //                //Debug.Log("下载进度：" + (float)currentLength / length);
    //                yield return null;
    //            }
    //            fs.Close();
    //            ns.Close();

    //            if (currentLength >= length)
    //            {
    //                try
    //                {
    //                    //下载完成重命名
    //                    File.Move(tempPath, savePath + "/" + reName);
    //                    //下载完立刻关闭
    //                    request.Abort();
    //                    currentLength = 0;
    //                    len = 0;
    //                }
    //                catch (System.Exception e)
    //                {

    //                    Debug.Log(e.ToString());
    //                    throw;
    //                }
    //            }


    //        }



    //    if (uwr.isNetworkError || uwr.isHttpError)
    //    {
    //        Debug.Log(uwr.error);
    //    }
    //    while (!uwr.isDone)
    //    {
    //        //Debug.LogError(www.downloadProgress);
    //        ProcessAction?.Invoke(uwr.downloadProgress);
    //        yield return null;
    //    }
    //    if (uwr.isDone)
    //    {
    //        if (uwr.responseCode.Equals(200))
    //        {
    //            Debug.Log(uwr.downloadHandler.text);
    //            SaveFile(GlobalData.LocalPath + fileName, uwr.downloadHandler.data);

    //            callBack?.Invoke(uwr.downloadHandler, GlobalData.LocalPath + fileName);
    //        }
    //        else
    //        {
    //            callBack?.Invoke(null, null);
    //        }
    //    }
    //    else
    //    {
    //        callBack?.Invoke(null, null);
    //    }

    //    uwr.Dispose();

    //    yield return null;
    //}
    #endregion

    FileStream fs;
    Stream ns;
    HttpWebRequest request;

    bool downLoad = false;
    long currentLength = 0;
    long len;
    float persent = 0;


    /// <summary>
    /// 使用http请求下载，并解压，适用于较大文件。支持断点续传
    /// </summary>
    /// <param name="stationNum">工位名称  eg:IP-01-CF</param>
    /// <param name="callBack">解压完回调</param>
    /// <returns></returns>
    public void DownLoadFile_HTTP(string url, string fileName, DownloadInfo info, bool _isStop, Action<string> callBack, Action<float> ProcessAction)
    {
        downLoad = true;
        long length = 0;
        //下载进度
        persent = 0;
        string tempFileName = Path.GetFileNameWithoutExtension(fileName) + "_TMP" + ".tmp";
        string tempPath = GlobalData.LocalPath + tempFileName;
        string finalPath = GlobalData.LocalPath + Path.GetFileNameWithoutExtension(fileName);
        string reName = fileName;

        string requestUrl = new Uri(url).AbsoluteUri;
        //requestUrl = requestUrl + "?ProjectID=" + info.ProjectID;

        if (!Directory.Exists(GlobalData.LocalPath))
        {
            Directory.CreateDirectory(GlobalData.LocalPath);
        }
        if (File.Exists(GlobalData.LocalPath + reName))
        {
            File.Delete(GlobalData.LocalPath + reName);
        }

        long lStartPos = 0;
        try
        {
            if (!File.Exists(tempPath))
            {
                fs = new FileStream(tempPath, FileMode.Create);
                lStartPos = 0;
            }
            else
            {
                //var now = DateTime.Now;
                //var logContent = string.Format("Tid: {0}{1} {2}.{3}\r\n", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString().PadRight(4), now.ToLongDateString(), now.ToLongTimeString(), now.Millisecond.ToString());

                //var logContentBytes = Encoding.Default.GetBytes(logContent);
                ////由于设置了文件共享模式为允许随后写入，所以即使多个线程同时写入文件，也会等待之前的线程写入结束之后再执行，而不会出现错误
                //using ( fs = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                //{
                //    lStartPos = fs.Length;
                //    fs.Seek(lStartPos, SeekOrigin.End);
                //    fs.Write(logContentBytes, 0, logContentBytes.Length);
                //}
                //如果在临时文件路径下存在临时文件则继续下载临时文件
                fs = File.OpenWrite(tempPath);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, SeekOrigin.Current);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
        //Debug.Log("requestUrl:" + requestUrl);
        //Debug.Log(info.token);
        //if (!string.IsNullOrEmpty(info.token))
        //{
        //    if (GlobalData.isRainbow)
        //        request.Headers["alva_author_token"] = info.token;
        //    else
        //        request.Headers["Authorization"] = info.token;
        //}

        request.Method = "Get";

        if (lStartPos > 0)
        {
            request.AddRange((int)lStartPos);
        }

        try
        {
            ns = request.GetResponse().GetResponseStream();
            length = request.GetResponse().ContentLength;
            if (len == 0)
            {
                len = length;
            }
            byte[] nbytes = new byte[102400];
            int nReadSize = 0;
            nReadSize = ns.Read(nbytes, 0, 102400);
            while (nReadSize > 0)
            {
                persent = (float)currentLength / len;
                currentLength += nReadSize;
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = ns.Read(nbytes, 0, 102400);
                ProcessAction?.Invoke(persent);
                //Debug.Log("下载进度：" + (float)currentLength / length);
            }
            fs.Close();
            ns.Close();

            if (currentLength >= length)
            {
                try
                {
                    //下载完成重命名
                    File.Move(tempPath, GlobalData.LocalPath + reName);
                    //下载完立刻关闭
                    request.Abort();
                    currentLength = 0;
                    len = 0;
                    callBack?.Invoke(GlobalData.LocalPath + reName);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                    callBack?.Invoke(null);
                }
            }

        }
        catch (Exception ex)
        {
            callBack?.Invoke(null);
            Debug.LogError(ex);
        }
    }
    public void HxDownLoadFile_HTTP(string url, string fileName, bool _isStop, Action<string> callBack, Action<float> ProcessAction)
    {
        downLoad = true;
        long length = 0;
        //下载进度
        persent = 0;
        string tempFileName = Path.GetFileNameWithoutExtension(fileName) + "_TMP" + ".tmp";
        string tempPath = GlobalData.LocalPath + tempFileName;
        string finalPath = GlobalData.LocalPath + Path.GetFileNameWithoutExtension(fileName);
        string reName = fileName;

        string requestUrl = new Uri(url).AbsoluteUri;

        if (!Directory.Exists(GlobalData.LocalPath))
        {
            Directory.CreateDirectory(GlobalData.LocalPath);
        }
        if (File.Exists(GlobalData.LocalPath + reName))
        {
            File.Delete(GlobalData.LocalPath + reName);
        }

        long lStartPos = 0;
        try
        {
            if (!File.Exists(tempPath))
            {
                fs = new FileStream(tempPath, FileMode.Create);
                lStartPos = 0;
            }
            else
            {
                //如果在临时文件路径下存在临时文件则继续下载临时文件
                fs = File.OpenWrite(tempPath);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, SeekOrigin.Current);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
        request.Method = "Get";

        if (lStartPos > 0)
        {
            request.AddRange((int)lStartPos);
        }

        try
        {
            ns = request.GetResponse().GetResponseStream();
            length = request.GetResponse().ContentLength;
            if (len == 0)
            {
                len = length;
            }
            byte[] nbytes = new byte[102400];
            int nReadSize = 0;
            nReadSize = ns.Read(nbytes, 0, 102400);
            while (nReadSize > 0)
            {
                persent = (float)currentLength / len;
                currentLength += nReadSize;
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = ns.Read(nbytes, 0, 102400);
                ProcessAction?.Invoke(persent);
                //Debug.Log("////下载进度：" + (float)currentLength / length);
            }
            fs.Close();
            ns.Close();

            if (currentLength >= length)
            {
                try
                {
                    //下载完成重命名
                    File.Move(tempPath, GlobalData.LocalPath + reName);
                    //下载完立刻关闭
                    request.Abort();
                    currentLength = 0;
                    len = 0;
                    callBack?.Invoke(GlobalData.LocalPath + reName);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                    callBack?.Invoke(null);
                }
            }

        }
        catch (Exception ex)
        {
            callBack?.Invoke(null);
            Debug.LogError(ex);
        }
    }
    public void DownLoadModel_HTTP(string url, string snID, string fileName, DownloadInfo info, bool _isStop, Action<string> callBack, Action<float> ProcessAction)
    {
        downLoad = true;
        long length = 0;
        //下载进度
        persent = 0;
        string tempFileName = Path.GetFileNameWithoutExtension(fileName) + "_TMP" + ".tmp";
        string tempPath = GlobalData.LocalPath + tempFileName;
        string finalPath = GlobalData.LocalPath + Path.GetFileNameWithoutExtension(fileName);
        string reName = fileName;

        string requestUrl = new Uri(url).AbsoluteUri;
        requestUrl = requestUrl + "?sn=" + snID;

        if (!Directory.Exists(GlobalData.LocalPath))
        {
            Directory.CreateDirectory(GlobalData.LocalPath);
        }
        if (File.Exists(GlobalData.LocalPath + reName))
        {
            File.Delete(GlobalData.LocalPath + reName);
        }

        long lStartPos = 0;
        try
        {
            if (!File.Exists(tempPath))
            {
                fs = new FileStream(tempPath, FileMode.Create);
                lStartPos = 0;
            }
            else
            {
                //var now = DateTime.Now;
                //var logContent = string.Format("Tid: {0}{1} {2}.{3}\r\n", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString().PadRight(4), now.ToLongDateString(), now.ToLongTimeString(), now.Millisecond.ToString());

                //var logContentBytes = Encoding.Default.GetBytes(logContent);
                ////由于设置了文件共享模式为允许随后写入，所以即使多个线程同时写入文件，也会等待之前的线程写入结束之后再执行，而不会出现错误
                //using ( fs = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                //{
                //    lStartPos = fs.Length;
                //    fs.Seek(lStartPos, SeekOrigin.End);
                //    fs.Write(logContentBytes, 0, logContentBytes.Length);
                //}
                //如果在临时文件路径下存在临时文件则继续下载临时文件
                fs = File.OpenWrite(tempPath);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, SeekOrigin.Current);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);

        request.Headers["alva_author_token"] = info.token;

        request.Method = "Get";

        if (lStartPos > 0)
        {
            request.AddRange((int)lStartPos);
        }

        try
        {
            ns = request.GetResponse().GetResponseStream();
            length = request.GetResponse().ContentLength;
            if (len == 0)
            {
                len = length;
            }
            byte[] nbytes = new byte[102400];
            int nReadSize = 0;
            nReadSize = ns.Read(nbytes, 0, 102400);
            while (nReadSize > 0)
            {
                persent = (float)currentLength / len;
                currentLength += nReadSize;
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = ns.Read(nbytes, 0, 102400);
                ProcessAction?.Invoke(persent);
                Debug.Log("下载进度：" + (float)currentLength / length);
            }
            fs.Close();
            ns.Close();

            if (currentLength >= length)
            {
                try
                {
                    //下载完成重命名
                    File.Move(tempPath, GlobalData.LocalPath + reName);
                    //下载完立刻关闭
                    request.Abort();
                    currentLength = 0;
                    len = 0;
                    callBack?.Invoke(GlobalData.LocalPath + reName);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                    callBack?.Invoke(null);
                }
            }

        }
        catch (Exception ex)
        {
            callBack?.Invoke(null);
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileBytes"></param>
    public void SaveFile(string filePath, byte[] fileBytes)
    {
        ///向服务器检查是否需要更新，若需要，下载覆盖，若不需要直接加载缓存信息。
        if (File.Exists(filePath))
            File.Delete(filePath);
        string dir = filePath.Replace(Path.GetFileName(filePath), "");
        Debug.Log("保存文件夹： " + dir);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileStream fs = new FileStream(filePath, FileMode.Create);
        //开始写入
        fs.Write(fileBytes, 0, fileBytes.Length);
        //清空缓冲区、关闭流
        fs.Flush();
        fs.Close();
        fs.Dispose();
    }

    /// <summary>
    /// 本地下载文件
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="callBack">下载结果回调</param>
    /// <param name="ProcessAction">下载进度</param>
    /// <returns></returns>
    IEnumerator DownloadFileLocal_Coroutine(string url, Action<DownloadHandler> callBack, Action<float> ProcessAction)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);

        UnityWebRequest uwr = UnityWebRequest.Get(uri.AbsoluteUri);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SendWebRequest();//开始请求
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请重试。");
        }
        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            ProcessAction?.Invoke(uwr.downloadProgress);
            yield return null;
        }
        if (uwr.isDone)
        {
            Debug.Log(uwr.downloadHandler.data.Length);
            callBack?.Invoke(uwr.downloadHandler);
        }
        else
        {
            callBack?.Invoke(null);
        }

        uwr.Dispose();

        yield return null;

    }


    /// <summary>
    /// Post请求服务器信息（json格式提交）
    /// </summary>
    /// <param name="url">接口地址</param>
    /// <param name="jsonObject">json对象</param>
    /// <param name="resultCallBack">接口结果回调</param>
    /// <returns></returns>
    public IEnumerator Post(string url, string[] head, object jsonObject, Action<DownloadHandler> resultCallBack = null, Action errorCallBack = null)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);

        UnityWebRequest uwr = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbPOST);// url method
        if (jsonObject != null)
        {
            string str = GlobalData.SerializeObject(jsonObject);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(str);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        if (head != null && head.Length > 1)
        {
            for (int i = 0; i < head.Length / 2; i = i + 2)
            {
                uwr.SetRequestHeader(head[i], head[i + 1]);
                Debug.Log("header --" + head[i] + ": " + head[i + 1]);
            }
        }
        uwr.useHttpContinue = false;

        yield return uwr.SendWebRequest();//开始请求

        //Debug.Log(uwr.uploadHandler.progress);

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络信息错误，请重试。");
            Debug.Log(uwr.uploadHandler.progress);
        }
        if (uwr.isDone)
        {
            //Debug.Log(uwr.uploadHandler.progress);
            //Debug.Log(uwr.downloadHandler.text);

            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            //Debug.Log("ContentType:  " + uwr.uploadHandler.contentType);
            Debug.Log("Post error----------:" + uwr.error);
            errorCallBack?.Invoke();
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请重试。");
        }

        uwr.Dispose();

        yield return null;
    }

    /// <summary>
    /// Put请求服务器信息（json格式提交）
    /// </summary>
    /// <param name="url">接口地址</param>
    /// <param name="jsonObject">json对象</param>
    /// <param name="resultCallBack">接口结果回调</param>
    /// <returns></returns>
    public IEnumerator Put(string url, string[] head, object jsonObject, Action<DownloadHandler> resultCallBack = null)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);

        UnityWebRequest uwr = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbPUT);// url method
        if (jsonObject != null)
        {
            string str = GlobalData.SerializeObject(jsonObject);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(str);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        if (head != null && head.Length > 1)
        {
            for (int i = 0; i < head.Length / 2; i = i + 2)
            {
                uwr.SetRequestHeader(head[i], head[i + 1]);
                Debug.Log("header --" + head[i] + ": " + head[i + 1]);
            }
        }
        uwr.useHttpContinue = false;

        yield return uwr.SendWebRequest();//开始请求

        Debug.Log(uwr.uploadHandler.progress);

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络信息错误，请重试。");
            Debug.Log(uwr.uploadHandler.progress);
        }
        if (uwr.isDone)
        {
            Debug.Log(uwr.uploadHandler.progress);
            Debug.Log(uwr.downloadHandler.text);
            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("ContentType:  " + uwr.uploadHandler.contentType);
            Debug.Log(uwr.error);
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请重试。");
        }

        uwr.Dispose();

        yield return null;
    }

    public IEnumerator ServerSimpleGet(string url, Action<DownloadHandler> resultCallBack = null)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);

        UnityWebRequest uwr = UnityWebRequest.Get(uri.AbsoluteUri);
        //UnityWebRequest uwr = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbGET);
        //uwr.downloadHandler = new DownloadHandlerBuffer();
        ////uwr.timeout = 5000000;
        //uwr.SetRequestHeader("alva_device", "1");

        yield return uwr.SendWebRequest();//开始请求

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络信息错误，请重试。");
            Debug.Log(uwr.uploadHandler.progress);
        }
        if (uwr.isDone)
        {
            //Debug.Log("////////////////////:"+uwr.downloadHandler.text);

            //StreamWriter streamWriter = new StreamWriter(@"C:\Users\Administrator\Desktop\vision新接口\public92.txt", true);
            //streamWriter.Write(uwr.downloadHandler.text);
            //streamWriter.Close();
            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请重试。");
        }

        uwr.Dispose();

        yield return null;
    }

    public IEnumerator GetServerModelList(string url, string token, Action<DownloadHandler> resultCallBack = null)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);

        UnityWebRequest uwr = UnityWebRequest.Get(uri.AbsoluteUri);
        //UnityWebRequest uwr = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbGET);// url method
        //uwr.useHttpContinue = false;
        uwr.SetRequestHeader("token", token);

        yield return uwr.SendWebRequest();//开始请求

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络信息错误，请重试。");
            Debug.Log(uwr.uploadHandler.progress);
        }
        if (uwr.isDone)
        {
            Debug.Log(uwr.downloadHandler.text);
            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络连接失败，请重试。");
        }

        uwr.Dispose();

        yield return null;
    }

    public IEnumerator Get(string url, string[] head, object jsonObject, Action<DownloadHandler> resultCallBack = null)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri.AbsoluteUri);
        UnityWebRequest uwr = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbGET);// url method
        if (jsonObject != null)
        {
            string str = GlobalData.SerializeObject(jsonObject);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(str);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        //uwr.SetRequestHeader("alva_device", "1");
        if (head != null && head.Length > 1)
        {
            for (int i = 0; i < head.Length / 2; i = i + 2)
            {
                uwr.SetRequestHeader(head[i], head[i + 1]);
                //Debug.Log("header --" + head[i] + ": " + head[i + 1]);
            }
        }
        uwr.useHttpContinue = false;

        yield return uwr.SendWebRequest();//开始请求

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData("网络信息错误，请重试。");
        }
        if (uwr.isDone)
        {
            //Debug.Log(uwr.downloadHandler.text);
            //#if UNITY_EDITOR
            //            StreamWriter streamWriter = new StreamWriter(@"C:\Users\Administrator\Desktop\vision新接口\w94.txt", true);
            //            streamWriter.Write(uwr.downloadHandler.text);
            //            streamWriter.Close();
            //#endif
            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            //Debug.Log("ContentType:  " + uwr.uploadHandler.contentType);
            Debug.Log("Get----:" + uwr.error);
            UI_MessageBoxPanel box = FindObjectOfType<UI_MessageBoxPanel>();
            box.SetTipData(uwr.error);
        }

        uwr.Dispose();

        yield return null;
    }

    public IEnumerator UploadFiles(string filePath, string url, Action<DownloadHandler> resultCallBack = null)
    {
        byte[] file = File.ReadAllBytes(filePath);
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(str);

        UnityWebRequest uwr = new UnityWebRequest(url, "Post");// url method
        uwr.uploadHandler = new UploadHandlerRaw(file);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "multipart/form-data");

        yield return uwr.SendWebRequest();//开始请求

        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            yield return null;
        }
        if (uwr.isDone)
        {
            Debug.Log(uwr.downloadHandler.text);
            resultCallBack?.Invoke(uwr.downloadHandler);
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("ContentType:  " + uwr.uploadHandler.contentType);
            Debug.Log(uwr.error);
        }
    }
}
