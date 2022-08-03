using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIItem : MonoBehaviour
{

    private EditableObjectsDataModel editableObjectsDataModel;    //数据
    private SourType sourType;

    private void OnEnable()
    {
        if (editableObjectsDataModel == null)
        {
            return;
        }
        if ((SourType)Enum.Parse(typeof(SourType), editableObjectsDataModel.type) == SourType.Video2D)
        {
            UnityEngine.Video.VideoPlayer video = GetComponent<UnityEngine.Video.VideoPlayer>();
            if (video == null)
            {
                Debug.Log("视频加载失败");
                return;
            }
            video.isLooping = true;
            video.Play();
            transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 初始化设置UI的历史数据
    /// </summary>
    /// <param name="_editableObjectsDataModel"></param>
    public void OnSetData(EditableObjectsDataModel _editableObjectsDataModel,Transform modelParent,Transform uiParent)
    {
        editableObjectsDataModel = _editableObjectsDataModel;

        sourType = (SourType)Enum.Parse(typeof(SourType), editableObjectsDataModel.type);
        switch (sourType)
        {
            case SourType.None:
                break;
            case SourType.Image:
                OnSetSpriteStyle();
                break;
            case SourType.Text:
                OnSetText();
                break;
            case SourType.Button:
                OnSetSpriteStyle();
                OnSetText();
                for (int i = 0; i < editableObjectsDataModel.eventIndex.Count; i++)
                {
                    OnSetButtonData(modelParent,uiParent, editableObjectsDataModel.eventIndex[i]);
                }
                break;
            case SourType.Video2D:
                OnSetVideoButtonClick();
                break;
            case SourType.Gif2D:
                OnPlayGif();
                break;
            case SourType.Link2D:
                OnLink2D();
                break;
        }

        if (editableObjectsDataModel.viewPage.Equals(1))
        {
            gameObject.SetActive(editableObjectsDataModel.active);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (_editableObjectsDataModel.id.Equals("af23ccea") || _editableObjectsDataModel.id.Equals("8f7b48cc"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnLink2D()
    {
        if (!string.IsNullOrEmpty(editableObjectsDataModel.description))
        {
            transform.Find("Text").GetComponent<Text>().color = new Color(44/255f,144/255f,1f);
            transform.Find("Text").GetComponent<Text>().text = editableObjectsDataModel.description;
            transform.Find("Text").GetComponent<Text>().fontSize = editableObjectsDataModel.fontSize;
            transform.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                Application.OpenURL(editableObjectsDataModel.description);
            });
            transform.GetComponentInChildren<Button>().GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            transform.Find("Text").GetComponent<Text>().text = "";
        }
    }

    private void OnPlayGif()
    {
        GIFPlayer player = transform.GetComponentInChildren<GIFPlayer>();
        Gif2Textures.GifFrames mframes = player.LoadGif(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectResStr + "/" + editableObjectsDataModel.style);
        RawImage raw = GetComponent<RawImage>();
        GIFPlayer.PLAYSTATE state = GIFPlayer.PLAYSTATE.ONCE;
        if (editableObjectsDataModel.isLoop)
        {
            state = GIFPlayer.PLAYSTATE.LOOP;
        }
        if (editableObjectsDataModel.awakeOnPlay)
        {
            player.PlayGif(0, editableObjectsDataModel.gifSpeed, mframes, raw, state);
        }
       
        transform.GetComponentInChildren<Button>().onClick.AddListener(
            ()=> 
            {
                bool isPlay = player.IsPlayer();
                if (isPlay)
                {
                    player.Stop();
                }
                else
                {
                    player.PlayGif(0, editableObjectsDataModel.gifSpeed, mframes, raw, state);
                }
            });
    }

    /// <summary>
    /// 视频事件
    /// </summary>
    /// <param name="trans"></param>
    private void OnSetVideoButtonClick()
    {
        UnityEngine.Video.VideoPlayer videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url = GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel.style;
        videoPlayer.prepareCompleted += OnVideoPrepareCompleted;

        Button btn = transform.Find("PlayButton").GetComponent<Button>();
        Image PauseImage = transform.Find("PauseImage").GetComponent<Image>();
       
        UnityEngine.Video.VideoPlayer video = transform.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (btn == null || video == null)
        {
            Debug.Log("视频加载失败");
            return;
        }
        video.isLooping = true;

        btn.onClick.AddListener(() => 
        {
            if (video.isPlaying)
            {
                PauseImage.gameObject.SetActive(true);
                video.Pause();
            }
            else
            {
                PauseImage.gameObject.SetActive(false);
                video.Play();
            }
        });

        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(
           () =>
           {
               transform.localScale = Vector3.zero;
               video.Stop();
           });
    }
    /// <summary>
    /// 按钮事件
    /// </summary>
    /// <param name="_editableObjectsDataModel"></param>
    /// <param name="modelParent"></param>
    void OnSetButtonData(Transform modelParent,Transform uiParent,string[] eventIndex)
    {
        if (string.IsNullOrEmpty(eventIndex[0]))
        {
            Debug.Log("UI在编辑器中未绑定数据");
            return;
        }
        if (eventIndex[0].Equals(1))
        {
            SetPageTagEvent(int.Parse(eventIndex[2]));
        }
        else
        {
            SetModelEvent(modelParent, uiParent, eventIndex);
        }

    }
    /// <summary>
    /// 多View事件
    /// </summary>
    /// <param name="page">页码</param>
    void SetPageTagEvent(int page)
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            UIItem[] uiItemList = transform.parent.GetComponentsInChildren<UIItem>(true);
            foreach (var item in uiItemList)
            {
                if (item.editableObjectsDataModel.viewPage.Equals(page))
                {
                    item.gameObject.SetActive(item.editableObjectsDataModel.active);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        });


    }

    /// <summary>
    /// 模型事件
    /// </summary>
    /// <param name="modelParent"></param>
    void SetModelEvent(Transform modelParent, Transform uiParent, string[] eventIndex)
    {
        Debug.Log(editableObjectsDataModel.eventIndex[0] + " objname " + modelParent.name);

        //Dictionary<GameObject, bool> dic = GetObjAvtive(modelParent);

        //foreach (Transform item in modelParent)
        //{
        //    item.gameObject.SetActive(true);
        //}
        Debug.Log(modelParent.GetChild(0).gameObject.activeSelf + modelParent.GetChild(0).gameObject.name);
        //从3D里面找
        ObjectItem[] objs = modelParent.GetComponentsInChildren<ObjectItem>(true);
        Transform obj = null;
        foreach (var o in objs)
        {
            Debug.Log(o.editable.name + " " + o.editable.id);
            if (o.editable.id.Equals(eventIndex[0].Trim()))
            {
                obj = o.transform;
                break;
            }
        }
        //3D的没找到  从2D的物体里面找
        if (obj == null)
        {
            UIItem[] objsUI = uiParent.GetComponentsInChildren<UIItem>(true);
            Debug.LogWarning("=====================" + objsUI.Length);
            foreach (var o in objsUI)
            {
                Debug.Log(o.editableObjectsDataModel.name + " " + o.editableObjectsDataModel.id);
                if (o.editableObjectsDataModel.id.Equals(eventIndex[0].Trim()))
                {
                    obj = o.transform;
                    break;
                }
            }
        }
        // Transform obj = modelParent.Find(editableObjectsDataModel.eventIndex[0].ToString().Trim());
        // Debug.LogError(obj.name);
        if (obj == null)
        {
            Debug.LogError("UI绑定数据失败 " + eventIndex[0] + " 未找到！");
            return;
        }

        Animation ani = obj.GetComponentInChildren<Animation>();
        if (ani != null)
        {
            Debug.LogError("=======================================================================");
            EditableObjectsDataModel editableObjectsDataModel3D = obj.GetComponentInChildren<ObjectItem>().editable;
            InitModelAni(ani, editableObjectsDataModel3D,obj.gameObject,eventIndex);
            if (obj.GetComponent<ModelAniManager>() == null )
            {
                obj.gameObject.AddComponent<ModelAniManager>();
            }
        }

        Button btn = GetComponent<Button>();
        if (!eventIndex[1].Equals("0"))
        {
            btn.onClick.AddListener(delegate { OnButtonClick(obj.gameObject, eventIndex); });
        }
    }


    Dictionary<GameObject,bool> GetObjAvtive(Transform parent)
    {
        Dictionary<GameObject, bool> dic = new Dictionary<GameObject, bool>();
        foreach (Transform item in parent)
        {
            dic.Add(item.gameObject, item.gameObject.activeSelf); 
        }
        return dic;
    }


    private void OnButtonClick(GameObject obj,string[] eventIndex)
    {
        if (string.IsNullOrEmpty(eventIndex[1]) || string.IsNullOrEmpty(eventIndex[2]))
        {
            Debug.LogError("没有事件可绑定！");
            return;
        }

        ObjectItem item = obj.GetComponentInChildren<ObjectItem>();
        if (item == null)
        {
            Debug.LogError("物体获取类型错误！");
            UIItem itemUI = obj.GetComponentInChildren<UIItem>();
            if (itemUI == null)
            {
                Debug.LogError("UI物体获取类型错误！");
            }
            else
            {
                OnSetGameObjectActiveEvent(obj, eventIndex);
            }
        }
        else
        {
            if (eventIndex[1] == "1")
            {
                OnSetGameObjectActiveEvent(obj, eventIndex);
            }
            else
            {
                switch (item.mType)
                {
                    case SourType.None:
                        break;
                    case SourType.Model:
                        OnSetModelDataEvent(obj, eventIndex);
                        break;
                    //case SourType.Picture:
                    //case SourType.Gif3D:
                    //case SourType.Hotspot:
                    //case SourType.ParticleSystem:
                    //case SourType.ImageTarget:  ///只有显示隐藏所以行为一样
                    //    OnSetGameObjectActiveEvent(obj, eventIndex);
                    //    break;
                    case SourType.Video:
                        OnSetVideo3DDataEvent(obj, eventIndex);
                        break;
                    case SourType.Image:
                        break;
                    case SourType.Text:
                        break;
                    case SourType.Button:
                        break;
                    case SourType.Video2D:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    bool isPlay = false;
    /// <summary>
    /// 模型行为
    /// </summary>
    /// <param name="_editableObjectsDataModel"></param>
    /// <param name="obj"></param>
    void OnSetModelDataEvent(GameObject obj,string[] eventIndex)
    {
        string index = eventIndex[1];
        if (index.Equals("1"))
        {
            if ((int.Parse(eventIndex[2]) == 1))
            {
                obj.SetActive(true);
            }
            else if((int.Parse(eventIndex[2]) == 2))
            {
                obj.SetActive(false);
            }
            else if((int.Parse(eventIndex[2]) == 3))
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
        else
        {
            GlobalData.IsInitAniPlayAction?.Invoke();

            Animation ani = obj.GetComponentInChildren<Animation>();
            int indexAni = int.Parse(eventIndex[2]);
            if (indexAni.Equals(1))
            {
                ani.Play(eventIndex[1]);
                obj.GetComponentInChildren<ObjectItem>().AniCurrentIndex = ReturnAniIndex(obj.GetComponentInChildren<ObjectItem>().AniNameList, eventIndex[1]);
            }
            else if(indexAni.Equals(2))
            {
                ani.Stop();
            }
            else if(indexAni.Equals(3))
            {
                if (isPlay)
                {
                    isPlay = false;
                    AnimationState state = ani[eventIndex[1]];
                    ani.Play(eventIndex[1]);
                    state.time = 0;
                    ani.Sample();
                    state.enabled = false;
                }
                else
                {
                    isPlay = true;
                    ani.Play(eventIndex[1]);
                    obj.GetComponentInChildren<ObjectItem>().AniCurrentIndex = ReturnAniIndex(obj.GetComponentInChildren<ObjectItem>().AniNameList, eventIndex[1]);
                }
            }
            else if (indexAni.Equals(4))
            {
                PlayForWordOrReverse(true, ani,obj,eventIndex[1]);
            }
            else if(indexAni.Equals(5))
            {
                PlayForWordOrReverse(false, ani,obj, eventIndex[1]);
            }
        }
    }

    private int ReturnAniIndex(List<string> aniNames,string name)
    {
        for (int i = 0; i < aniNames.Count; i++)
        {
            if (aniNames[i].Equals(name))
            {
                return i;
            }
        }
        return -1;
    }

    bool isBeginForwAni = false;
    /// <summary>
    /// 轮播动画
    /// </summary>
    /// <param name="isForward"></param>
    /// <param name="ani"></param>
    private void PlayForWordOrReverse(bool isForward,Animation ani,GameObject obj,string aniName)
    {
        ObjectItem it = obj.GetComponentInChildren<ObjectItem>();
        int currentIndex = 0;
        if (!it.isBeginForwAni)
        {
            currentIndex = ReturnAniIndex(it.AniNameList, aniName);
            it.AniCurrentIndex = currentIndex;
            it.isBeginForwAni = true;
        }
        else
        {
            if (isForward)
            {
                it.AniCurrentIndex++;
            }
            else
            {
                it.AniCurrentIndex--;
            }
            if (it.AniCurrentIndex < 0)
            {
                it.AniCurrentIndex = ani.GetClipCount() - 1;/////////////////////////xiaoyu  -1
            }
            if (it.AniCurrentIndex > ani.GetClipCount() - 1)/////////////////////////xiaoyu  -1
            {
                it.AniCurrentIndex = 0;
            }
            currentIndex = it.AniCurrentIndex;
        }

        ani.Play(it.AniNameList[currentIndex]);

        Debug.LogError(it.AniCurrentIndex);
    }

    private void InitModelAni(Animation ani, EditableObjectsDataModel editableObjectsDataModel3D,GameObject obj, string[] eventIndex)
    {
        ani.playAutomatically = true;
        if (eventIndex[1] != null && eventIndex[1] != "1")
        {
            ObjectItem objItem = obj.GetComponentInChildren<ObjectItem>();

            objItem.AniNameList = new List<string>();
            foreach (AnimationState item in ani)
            {
                objItem.AniNameList.Add(item.name);
            }
        }

        if (editableObjectsDataModel3D.animationData == null)
        {
            return;
        }
        foreach (var item in editableObjectsDataModel3D.animationData)
        {
            if (item.Length > 2)
            {
                if (int.TryParse(item[1],out int speed))
                {
                    ani[item[0]].speed = speed;
                }
                if (bool.TryParse(item[2],out bool loop))
                {
                    ani[item[0]].wrapMode = loop == true ? WrapMode.Loop : WrapMode.Once;
                }
            }
        }
    }

    IEnumerator OpenHightLight(GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        //obj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.AddComponent<HighlightableObject>().FlashingOn();
        yield return new WaitForSeconds(6f);
        UnityEngine.Video.VideoPlayer player = obj.transform.root.GetChild(0).GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
        Debug.Log(obj.name + player.gameObject.name);
        obj.transform.root.GetChild(0).gameObject.SetActive(true);
        player.Play();
    }

    /// <summary>
    /// 3D视频行为
    /// </summary>
    /// <param name="_editableObjectsDataModel"></param>
    /// <param name="obj"></param>
    void OnSetVideo3DDataEvent(GameObject obj,string[] eventIndex)
    {
        int index = int.Parse(eventIndex[1]);
        if (index == 1)
        {
            if ((int.Parse(eventIndex[2]) == 1))
            {
                obj.SetActive(true);
            }
            else if((int.Parse(eventIndex[2]) == 2))
            {
                obj.SetActive(false);
            }
            else if ((int.Parse(eventIndex[2]) == 3))
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
        else
        {
            UnityEngine.Video.VideoPlayer player = obj.GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
            int indexV = int.Parse(eventIndex[2]);
            if (indexV == 1)
            {
                player.Play();
            }
            else if (indexV == 2)
            {
                player.Pause();
            }
            else if(indexV.Equals(3))
            {
                player.Stop();
            }
            else
            {
                if (player.isPlaying)
                {
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }
        }

    }
    /// <summary>
    /// 设置物体显示隐藏腥味
    /// </summary>
    /// <param name="obj">物体本体</param>
    /// <param name="eventIndex">事件信息</param>
    void OnSetGameObjectActiveEvent(GameObject obj,string[] eventIndex)
    {
        int index = int.Parse(eventIndex[1]);
        Debug.Log(index + obj.name);
        if (index == 1)
        {
            if ((int.Parse(eventIndex[2]) == 1))
            {
                obj.SetActive(true);
            }
            else if((int.Parse(eventIndex[2]) == 2))
            {
                obj.SetActive(false);
            }
            else if((int.Parse(eventIndex[2]) == 3))
            {
                Debug.LogError("kaiguan" + obj.activeSelf);
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
    /// <summary>
    /// 设置UI的图片显示
    /// </summary>
    void OnSetSpriteStyle()
    {
        if (!string.IsNullOrEmpty(editableObjectsDataModel.style))
        {
            GetComponent<UnityEngine.UI.Image>().sprite = GlobalData.Texture2DToSprite(GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel.style));
        }
    }
    /// <summary>
    /// 设置UI的文字内容
    /// </summary>
    void OnSetText()
    {
        if (!string.IsNullOrEmpty(editableObjectsDataModel.description))
        {
            transform.Find("Text").GetComponent<Text>().text = editableObjectsDataModel.description;
            transform.Find("Text").GetComponent<Text>().fontSize = editableObjectsDataModel.fontSize;
        }
        else
        {
            transform.Find("Text").GetComponent<Text>().text = "";
        }       
    }
    /// <summary>
    /// 视频加载完成调用
    /// </summary>
    /// <param name="videoPlayer"></param>
    private void OnVideoPrepareCompleted(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        videoPlayer.gameObject.GetComponent<UnityEngine.UI.RawImage>().texture = videoPlayer.texture;
    }

    private void OnDestroy()
    {
        //HighlightableObject[] highlightableObjects = FindObjectsOfType<HighlightableObject>();
        //foreach (var item in highlightableObjects)
        //{
        //    item.FlashingOff();
        //}
        GlobalData.AniCurrentIndex = -1;
        GlobalData.AniNameList.Clear();
    }
}
