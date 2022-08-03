using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerControler:MonoBehaviour
{
    private GameObject[] StartAR(string url)
    {
        GameObject[] target = null;
        //VuforiaControler.GetInstance().ClearAll(true);
        //VuforiaControler.GetInstance().Load(url);
        //VuforiaControler.GetInstance().SetAllTrackablerObject();
        //VuforiaControler.GetInstance().GetAllTrackablerObject(out target);
        return target;
    }

    //public IEnumerator Architecture(Action<GameObject[]> callback, List<AssetInfo> assetInfos)
    //{
    //    yield return new WaitForSeconds(5f);
    //    foreach (var item in assetInfos)
    //    {
    //        if (item.mType == ResType.Json)
    //        {
    //            Debug.Log(item.mByte);
    //            string json = Tools.ResGameObject<string>(ResType.Json, item.mByte);
    //            Debug.Log("=============" + json + "==============");
    //            List<PageOfRes> pageInfo = Tools.DeserializeObject<List<PageOfRes>>(json);

    //            foreach (PageOfRes page in pageInfo)
    //            {
    //                foreach (ResInfo trsInfo in page.ResInfoList)
    //                {
    //                    foreach (AssetInfo res in Util.AssetInfoList)
    //                    {
    //                        Debug.Log(res.mType + "    " + trsInfo.ResType + "       " + res.resData.name + "      " + trsInfo.ResName + "    " + trsInfo.mTrans.PosX);
    //                        if (res.mType == trsInfo.ResType && res.resData.name.Equals(trsInfo.ResName))
    //                        {
    //                            GameObject obj = Instantiate(Resources.Load("ScanRes/Link") as GameObject);
    //                            //位置处理
    //                            switch (trsInfo.mTrans.PosX)
    //                            {
    //                                //左上
    //                                case 0:
    //                                    obj.transform.localPosition = new Vector3(-0.3f, 0.3f, 3);
    //                                    break;
    //                                //右上
    //                                case 1:
    //                                    obj.transform.localPosition = new Vector3(0.3f, 0.3f, 3);
    //                                    break;
    //                                //左下
    //                                case 2:
    //                                    obj.transform.localPosition = new Vector3(-0.3f, -0.3f, 3);
    //                                    break;
    //                                //右下
    //                                case 3:
    //                                    obj.transform.localPosition = new Vector3(0.3f, -0.3f, 3);
    //                                    break;
    //                                default:
    //                                    obj.gameObject.transform.localPosition = Vector3.zero;
    //                                    break;
    //                            }
    //                            float a;
    //                            a = 41f - Camera.main.fieldOfView;
    //                            a = a * 0.045f;
    //                            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z + a);

    //                            obj.gameObject.transform.localScale = Vector3.one;
    //                            obj.gameObject.transform.eulerAngles = new Vector3(-90, 0, 0);
    //                            switch (res.mType)
    //                            {
    //                                case ResType.Image:
    //                                    Debug.Log("加载图片");
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    Texture2D tex = new Texture2D(20, 20);
    //                                    tex.LoadImage(res.mByte);
    //                                    Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    //                                    RectTransform image = obj.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>();
    //                                    obj.GetComponentInChildren<Image>().sprite = sp;
    //                                    //缩放
    //                                    float tex_ratio = 1;
    //                                    if (tex.width > tex.height)
    //                                    {
    //                                        tex_ratio = tex.height / (float)tex.width;
    //                                        obj.transform.localScale = new Vector3(1, 1, tex_ratio);
    //                                    }
    //                                    else if (tex.width < tex.height)
    //                                    {
    //                                        tex_ratio = tex.width / (float)tex.height;
    //                                        obj.transform.localScale = new Vector3(tex_ratio, 1, 1);
    //                                    }
    //                                    IFullScreen fullScreen = GetComponent<IFullScreen>();
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { fullScreen.Play("image", sp); });
    //                                    break;
    //                                case ResType.Audio:
    //                                    Debug.Log("加载音频");
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    GameObject _obj = obj.GetComponentInChildren<Image>().gameObject;
    //                                    MediaPlayer player = _obj.AddComponent<MediaPlayer>();
    //                                    if (Application.platform == RuntimePlatform.WindowsEditor)
    //                                    {
    //                                        player.m_VideoPath = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.m_VideoPath, false);
    //                                    }
    //                                    else if (Application.platform == RuntimePlatform.Android)
    //                                    {
    //                                        player.PlatformOptionsAndroid.path = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.PlatformOptionsAndroid.path, false);
    //                                    }
    //                                    obj.AddComponent<DisplayUGUI>()._mediaPlayer = player;
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { if (player.Control.IsPlaying()) { player.Control.Pause(); } else { player.Control.Play(); } });
    //                                    break;
    //                                case ResType.Video:
    //                                    Debug.Log("加载视频");
    //                                    obj.GetComponent<TakeOffTheCard>().type = "video";
    //                                    Destroy(obj.transform.Find("Canvas/Image").gameObject);
    //                                    _obj = obj.transform.Find("Canvas/Video").gameObject;
    //                                    player = _obj.AddComponent<MediaPlayer>();
    //                                    if (Application.platform == RuntimePlatform.WindowsEditor)
    //                                    {
    //                                        player.m_VideoPath = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.m_VideoPath, false);
    //                                    }
    //                                    else if (Application.platform == RuntimePlatform.Android)
    //                                    {
    //                                        player.PlatformOptionsAndroid.path = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.PlatformOptionsAndroid.path, false);
    //                                    }
    //                                    _obj.GetComponent<DisplayUGUI>()._mediaPlayer = player;
    //                                    // player.Play();
    //                                    //player.Pause();
    //                                    fullScreen = GetComponent<IFullScreen>();
    //                                    _obj.GetComponent<Button>().onClick.AddListener(() => { fullScreen.Play("video", player); });
    //                                    break;
    //                                case ResType.Model:
    //                                    break;
    //                                case ResType.Link:
    //                                    Debug.Log("加载链接！");
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    string[] links = Tools.ResGameObject<string[]>(res.mType, res.mByte);
    //                                    Debug.Log("解析出的Link" + links.Length + "    " + links[0]);
    //                                    //实例化物体
    //                                    foreach (var link in links)
    //                                    {
    //                                        obj.transform.GetComponentInChildren<Button>().onClick.AddListener(
    //                                            () =>
    //                                            {
    //                                                Uri uri = new Uri(link);
    //                                                Application.OpenURL(uri.AbsoluteUri);
    //                                            }
    //                                            );
    //                                    }
    //                                    break;
    //                                case ResType.Txt:
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    string info = Tools.ResGameObject<string>(res.mType, res.mByte);
    //                                    obj.transform.Find("Canvas/Image/Text").GetComponent<Text>().text = info;
    //                                    obj.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
    //                                    fullScreen = GetComponent<IFullScreen>();
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { fullScreen.Play("txt", info); });
    //                                    break;
    //                                default:
    //                                    break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            break;
    //        }
    //    }
    //}

    //public IEnumerator GetARState(Action<GameObject[]> callback, List<AssetInfo> assetInfos)
    //{
    //    bool isCheck = true;
    //    while (isCheck)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        if (Vuforia.DeviceTrackerARController.Instance != null)
    //        {
    //            isCheck = false;
    //        }
    //    }
    //    GameObject[] target = null;
    //    Dictionary<string, string> imageTargetSize = new Dictionary<string, string>();
    //    foreach (var item in Util.AssetInfoList)
    //    {
    //        if (item.mType == ResType.Xml)
    //        {
    //            target = StartAR(item.localPath);
    //            XmlDocument xml = new XmlDocument();
    //            xml.Load(item.localPath);
    //            XmlNodeList xmlNodeList = xml.SelectSingleNode("QCARConfig/Tracking").ChildNodes;
    //            foreach (XmlElement xl1 in xmlNodeList)
    //            {
    //                imageTargetSize.Add(xl1.GetAttribute("name"), xl1.GetAttribute("size"));
    //            }
    //            break;
    //        }
    //    }
    //    bool isLoadGameobject = true;
    //    while (isLoadGameobject)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        if (target != null && target.Length > 0)
    //        {
    //            isLoadGameobject = false;
    //        }
    //    }
    //    callback(target);
    //    foreach (var item in assetInfos)
    //    {
    //        if (item.mType == ResType.Json)
    //        {
    //            Debug.Log(item.mByte);
    //            string json = Tools.ResGameObject<string>(ResType.Json, item.mByte);
    //            Debug.Log("=============" + json + "==============");
    //            List<PageOfRes> pageInfo = Tools.DeserializeObject<List<PageOfRes>>(json);
    //            InstantiateScaneObject(target, pageInfo, imageTargetSize);
    //            break;
    //        }
    //    }
    //}

    //void InstantiateScaneObject(GameObject[] target, List<PageOfRes> pageList, Dictionary<string, string> imageTargetSize)
    //{
    //    Debug.Log("开始实例化物体");
    //    foreach (PageOfRes page in pageList)
    //    {
    //        if (target == null)
    //        {
    //            return;
    //        }
    //        foreach (var itemTarget in target)
    //        {
    //            if (itemTarget == null)
    //            {
    //                continue;
    //            }
    //            if (itemTarget.name == page.PageName)
    //            {
    //                //识别图位置处理
    //                string size_string;
    //                imageTargetSize.TryGetValue(itemTarget.name, out size_string);
    //                float size_x = float.Parse(size_string.Split(' ')[0]);
    //                float size_y = float.Parse(size_string.Split(' ')[1]);
    //                float ratio = size_y / size_x;

    //                foreach (ResInfo trsInfo in page.ResInfoList)
    //                {
    //                    foreach (AssetInfo res in Util.AssetInfoList)
    //                    {
    //                        Debug.Log(res.mType + "    " + trsInfo.ResType + "       " + res.resData.name + "      " + trsInfo.ResName);
    //                        if (res.mType == trsInfo.ResType && res.resData.name.Equals(trsInfo.ResName))
    //                        {
    //                            GameObject obj = Instantiate(Resources.Load("ScanRes/Link") as GameObject, itemTarget.transform);
    //                            obj.GetComponent<TakeOffTheCard>().parent = itemTarget.transform;
    //                            obj.GetComponent<TakeOffTheCard>().point = (int)trsInfo.mTrans.PosX;
    //                            obj.GetComponent<TakeOffTheCard>().type = "image";
    //                            //位置处理
    //                            switch (trsInfo.mTrans.PosX)
    //                            {
    //                                //左上
    //                                case 0:
    //                                    obj.transform.localPosition = new Vector3(-0.5f, 0, 0.5f * ratio);
    //                                    break;
    //                                //右上
    //                                case 1:
    //                                    obj.transform.localPosition = new Vector3(0.5f, 0, 0.5f * ratio);
    //                                    break;
    //                                //左下
    //                                case 2:
    //                                    obj.transform.localPosition = new Vector3(-0.5f, 0, -0.5f * ratio);
    //                                    break;
    //                                //右下
    //                                case 3:
    //                                    obj.transform.localPosition = new Vector3(0.5f, 0, -0.5f * ratio);
    //                                    break;
    //                                default:
    //                                    obj.gameObject.transform.localPosition = Vector3.zero;
    //                                    break;
    //                            }
    //                            obj.transform.localRotation = Quaternion.identity;
    //                            switch (res.mType)
    //                            {
    //                                case ResType.Image:
    //                                    Debug.Log("加载图片");
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    Texture2D tex = new Texture2D(20, 20);
    //                                    tex.LoadImage(res.mByte);
    //                                    Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    //                                    RectTransform image = obj.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>();
    //                                    obj.GetComponentInChildren<Image>().sprite = sp;
    //                                    //缩放
    //                                    float tex_ratio = 1;
    //                                    if (tex.width > tex.height)
    //                                    {
    //                                        tex_ratio = tex.height / (float)tex.width;
    //                                        obj.transform.localScale = new Vector3(1, 1, tex_ratio);
    //                                    }
    //                                    else if (tex.width < tex.height)
    //                                    {
    //                                        tex_ratio = tex.width / (float)tex.height;
    //                                        obj.transform.localScale = new Vector3(tex_ratio, 1, 1);
    //                                    }
    //                                    //位置
    //                                    switch (trsInfo.mTrans.PosX)
    //                                    {
    //                                        //左上
    //                                        case 0:
    //                                            image.anchoredPosition = new Vector2(60, -60);
    //                                            obj.transform.Find("Canvas/Image/0").GetComponent<Image>().enabled = true;
    //                                            break;
    //                                        //右上
    //                                        case 1:
    //                                            image.anchoredPosition = new Vector2(-60, -60);
    //                                            obj.transform.Find("Canvas/Image/1").GetComponent<Image>().enabled = true;
    //                                            break;
    //                                        //左下
    //                                        case 2:
    //                                            image.anchoredPosition = new Vector2(60, 60);
    //                                            obj.transform.Find("Canvas/Image/2").GetComponent<Image>().enabled = true;
    //                                            break;
    //                                        //右下
    //                                        case 3:
    //                                            image.anchoredPosition = new Vector2(-60, 60);
    //                                            obj.transform.Find("Canvas/Image/3").GetComponent<Image>().enabled = true;
    //                                            break;
    //                                    }
    //                                    IFullScreen fullScreen = GetComponent<IFullScreen>();
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { fullScreen.Play("image", sp); });
    //                                    break;
    //                                case ResType.Audio:
    //                                    Debug.Log("加载音频");
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    GameObject _obj = obj.GetComponentInChildren<Image>().gameObject;
    //                                    MediaPlayer player = _obj.AddComponent<MediaPlayer>();
    //                                    Debug.Log(res.localPath);
    //                                    if (Application.platform == RuntimePlatform.WindowsEditor)
    //                                    {
    //                                        player.m_VideoPath = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.m_VideoPath, false);
    //                                    }
    //                                    else if (Application.platform == RuntimePlatform.Android)
    //                                    {
    //                                        player.PlatformOptionsAndroid.path = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.PlatformOptionsAndroid.path, false);
    //                                    }
    //                                    obj.AddComponent<DisplayUGUI>()._mediaPlayer = player;
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { if (player.Control.IsPlaying()) { player.Control.Pause(); } else { player.Control.Play(); } });
    //                                    RectTransform image1 = obj.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>();
    //                                    //位置
    //                                    switch (trsInfo.mTrans.PosX)
    //                                    {
    //                                        //左上
    //                                        case 0:
    //                                            image1.anchoredPosition = new Vector2(60, -60);
    //                                            break;
    //                                        //右上
    //                                        case 1:
    //                                            image1.anchoredPosition = new Vector2(-60, -60);
    //                                            break;
    //                                        //左下
    //                                        case 2:
    //                                            image1.anchoredPosition = new Vector2(60, 60);
    //                                            break;
    //                                        //右下
    //                                        case 3:
    //                                            image1.anchoredPosition = new Vector2(-60, 60);
    //                                            break;
    //                                    }
    //                                    break;
    //                                case ResType.Video:
    //                                    Debug.Log("加载视频");
    //                                    obj.GetComponent<TakeOffTheCard>().type = "video";
    //                                    Destroy(obj.transform.Find("Canvas/Image").gameObject);
    //                                    _obj = obj.transform.Find("Canvas/Video").gameObject;
    //                                    player = _obj.AddComponent<MediaPlayer>();
    //                                    Debug.Log(res.localPath);
    //                                    if (Application.platform == RuntimePlatform.WindowsEditor)
    //                                    {
    //                                        player.m_VideoPath = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.m_VideoPath, false);
    //                                    }
    //                                    else if (Application.platform == RuntimePlatform.Android)
    //                                    {
    //                                        player.PlatformOptionsAndroid.path = res.localPath;
    //                                        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, player.PlatformOptionsAndroid.path, false);
    //                                    }
    //                                    _obj.GetComponent<DisplayUGUI>()._mediaPlayer = player;
    //                                    //player.Play();
    //                                    //player.Pause();
    //                                    fullScreen = GetComponent<IFullScreen>();
    //                                    _obj.GetComponent<Button>().onClick.AddListener(() => { fullScreen.Play("video", player); });
    //                                    RectTransform image2 = obj.GetComponentInChildren<DisplayUGUI>().gameObject.GetComponent<RectTransform>();
    //                                    //位置
    //                                    switch (trsInfo.mTrans.PosX)
    //                                    {
    //                                        //左上
    //                                        case 0:
    //                                            image2.anchoredPosition = new Vector2(60, -60);
    //                                            break;
    //                                        //右上
    //                                        case 1:
    //                                            image2.anchoredPosition = new Vector2(-60, -60);
    //                                            break;
    //                                        //左下
    //                                        case 2:
    //                                            image2.anchoredPosition = new Vector2(60, 60);
    //                                            break;
    //                                        //右下
    //                                        case 3:
    //                                            image2.anchoredPosition = new Vector2(-60, 60);
    //                                            break;
    //                                    }
    //                                    break;
    //                                case ResType.Model:
    //                                    break;
    //                                case ResType.Link:
    //                                    Debug.Log("加载链接！");
    //                                    Destroy(obj);
    //                                    string[] links = Tools.ResGameObject<string[]>(res.mType, res.mByte);
    //                                    Debug.Log("解析出的Link" + links.Length + "    " + links[0]);
    //                                    if (GetComponent<IJumpToWebPage>() == null)
    //                                    {
    //                                        Debug.Log("IJumpToWebPage no exists");
    //                                        break;
    //                                    }

    //                                    IJumpToWebPage jumpToWebPage = GetComponent<IJumpToWebPage>();
    //                                    Uri uri = new Uri(links[0]);
    //                                    jumpToWebPage.Jump(uri.AbsoluteUri);
    //                                    //实例化物体
    //                                    //foreach (var item in links)
    //                                    //{
    //                                    //    obj.transform.GetComponentInChildren<Button>().onClick.AddListener(
    //                                    //        () =>
    //                                    //        {
    //                                    //            Uri uri = new Uri(item);
    //                                    //            Application.OpenURL(uri.AbsoluteUri);
    //                                    //        }
    //                                    //        );
    //                                    //}
    //                                    break;
    //                                case ResType.Txt:
    //                                    Destroy(obj.transform.Find("Canvas/Video").gameObject);
    //                                    string info = Tools.ResGameObject<string>(res.mType, res.mByte);
    //                                    obj.transform.Find("Canvas/Image/Text").GetComponent<Text>().text = info;
    //                                    obj.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("UI/TextBg");
    //                                    //obj.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
    //                                    RectTransform image3 = obj.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>();
    //                                    //位置
    //                                    switch (trsInfo.mTrans.PosX)
    //                                    {
    //                                        //左上
    //                                        case 0:
    //                                            image3.anchoredPosition = new Vector2(60, -60);
    //                                            break;
    //                                        //右上
    //                                        case 1:
    //                                            image3.anchoredPosition = new Vector2(-60, -60);
    //                                            break;
    //                                        //左下
    //                                        case 2:
    //                                            image3.anchoredPosition = new Vector2(60, 60);
    //                                            break;
    //                                        //右下
    //                                        case 3:
    //                                            image3.anchoredPosition = new Vector2(-60, 60);
    //                                            break;
    //                                    }
    //                                    fullScreen = GetComponent<IFullScreen>();
    //                                    obj.GetComponentInChildren<Button>().onClick.AddListener(() => { fullScreen.Play("txt", info); });
    //                                    break;
    //                                default:
    //                                    break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //    }
    //}

}
