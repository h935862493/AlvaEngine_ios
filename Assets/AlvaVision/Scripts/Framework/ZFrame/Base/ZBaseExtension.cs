using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ZBaseExtension : MonoBehaviour
{
    protected int Add(int i, int j)
    {
        return i + j;
    }

    protected TaskJson LoadTask()
    {
        string[] paths = System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectTaskStr);
        if (paths.Length < 1)
        {
            return null;
        }
        for (int i = 0; i < paths.Length; i++)
        {
            if (System.IO.File.Exists(paths[i]))
            {
                string json = System.IO.File.ReadAllText(paths[i]);
                TaskJson task = GlobalData.DeserializeObject<TaskJson>(json);
                return task;
            }
            else
            {
                return null;
            }
        }
        return null;

    }

    protected bool LoadDatSet(string[] PlayerPrefs3DPaths)
    {
        GlobalData.ImageTargetPathStrList.Clear();
        bool isHave = false;
        foreach (string item in PlayerPrefs3DPaths)
        {
            if (System.IO.File.Exists(item) && item.Contains("target"))
            {
                string json3D = System.IO.File.ReadAllText(item);
                EditableObjectsDataModel editableObjectsDataModel3D = GlobalData.DeserializeObject<EditableObjectsDataModel>(json3D);
                if (editableObjectsDataModel3D.type == "ImageTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        GlobalData.ImageTargetPathStrList.Add(editableObjectsDataModel3D.style, editableObjectsDataModel3D.id);
                        //GlobalData.ProjectConfigDatStr = editableObjectsDataModel3D.style;
                        GlobalData.RecognitionType = Pro2DRecognition.Image;
                        isHave = true;
                    }
                }
                else if (editableObjectsDataModel3D.type == "ModelTarget")
                {

                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        GlobalData.ProjectConfigDatStr = editableObjectsDataModel3D.style;
                        isHave = true;
                    }
                }
                else if (editableObjectsDataModel3D.type == "VuMarkTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        GlobalData.ProjectConfigDatStr = Path.GetDirectoryName(editableObjectsDataModel3D.style) + "/" + Path.GetFileNameWithoutExtension(editableObjectsDataModel3D.sourOriginalNames) + ".xml";
                        GlobalData.RecognitionType = Pro2DRecognition.VuMark;
                        isHave = true;
                    }
                }
                else if (editableObjectsDataModel3D.type == "AreaTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        GlobalData.ProjectConfigDatStr = editableObjectsDataModel3D.style;
                        isHave = true;
                    }
                }
            }
        }
        return isHave;
    }

    private List<ItemCloudModelInfo> CloudModelIDList;

    protected void LoadModel(string[] PlayerPrefs3DPaths, Transform modelParent, Transform uiParent)
    {
        CloudModelIDList = new List<ItemCloudModelInfo>();
        foreach (string item in PlayerPrefs3DPaths)
        {
            if (System.IO.File.Exists(item))
            {
                string json3D = System.IO.File.ReadAllText(item);
                EditableObjectsDataModel editableObjectsDataModel3D = GlobalData.DeserializeObject<EditableObjectsDataModel>(json3D);
                if (editableObjectsDataModel3D.type == "Model")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.serialId))
                    {
                        ItemCloudModelInfo info = new ItemCloudModelInfo();
                        info.fileName = editableObjectsDataModel3D.serialId;
                        info.modelID = editableObjectsDataModel3D.serialId;
                        info.info = new DownloadInfo();
                        info.info.token = GlobalData.getUserInfo.token;
                        info.editableObjectsDataModel3D = editableObjectsDataModel3D;
                        CloudModelIDList.Add(info);
                        //StartCoroutine(ZManager.instnace.zServer.GetServerModelList(GlobalData.BaseUrl + GlobalData.ModelListUrl,GlobalData.getUserInfo.token));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                        {
                            string modelPath = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectResStr + "/" + editableObjectsDataModel3D.style;
                            //Debug.Log("modelPath:" + modelPath);
                            LoadModelDataInfo(editableObjectsDataModel3D, modelPath, modelParent);
                        }
                    }

                }
                else if (editableObjectsDataModel3D.type == "Video")
                {
                    GameObject video = Instantiate(Resources.Load("Prefab/Video")) as GameObject;
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        System.Uri uri = new System.Uri(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style);
                        video.GetComponentInChildren<UnityEngine.Video.VideoPlayer>().url = uri.AbsoluteUri;
                    }
                    video.transform.SetParent(modelParent);

                    OnSetObjectData(video, editableObjectsDataModel3D);

                    video.transform.name = editableObjectsDataModel3D.name;

                    Add3DObjectItemScript(video, editableObjectsDataModel3D);
                    //video.AddComponent<LookToCam>();

                    OnBindEditableDataModelWithTask(video, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(video, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(video.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Audio")
                {
                    GameObject audio = new GameObject();
                    AudioSource source = audio.AddComponent<AudioSource>();
                    StartCoroutine(GlobalData.LoadAudio(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style, source));
                    audio.transform.SetParent(modelParent);

                    OnSetObjectData(audio, editableObjectsDataModel3D);

                    audio.transform.name = editableObjectsDataModel3D.name;

                    Add3DObjectItemScript(audio, editableObjectsDataModel3D);

                    if (editableObjectsDataModel3D.active == false)
                    {
                        source.Stop();
                    }
                    OnBindEditableDataModelWithTask(audio, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(audio, path);
                        }
                    }


                    //if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    //{
                    //    SetModelTargetOfObjectTrs(audio.transform);
                    //}
                }
                else if (editableObjectsDataModel3D.type == "Picture")
                {
                    GameObject picture = Instantiate(Resources.Load("Prefab/Picture")) as GameObject;
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        picture.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style);
                        picture.GetComponent<MeshRenderer>().material.mainTexture = GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style);
                    }
                    picture.transform.SetParent(modelParent);

                    OnSetObjectData(picture, editableObjectsDataModel3D);

                    picture.transform.name = editableObjectsDataModel3D.name;

                    Add3DObjectItemScript(picture, editableObjectsDataModel3D);

                    //picture.AddComponent<LookToCam>();

                    OnBindEditableDataModelWithTask(picture, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(picture, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(picture.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Dashboard")
                {
                    GameObject dashboard = Instantiate(Resources.Load("Prefab/Dashboard")) as GameObject;
                    dashboard.transform.SetParent(modelParent);

                    OnSetObjectData(dashboard, editableObjectsDataModel3D);
                    dashboard.GetComponentInChildren<Dashboard>().InitAllData(float.Parse(editableObjectsDataModel3D.descriptions[0]), float.Parse(editableObjectsDataModel3D.descriptions[1]), int.Parse(editableObjectsDataModel3D.descriptions[2]), int.Parse(editableObjectsDataModel3D.descriptions[3]), float.Parse(editableObjectsDataModel3D.descriptions[4]));

                    Add3DObjectItemScript(dashboard, editableObjectsDataModel3D);

                    //picture.AddComponent<LookToCam>();

                    OnBindEditableDataModelWithTask(dashboard, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(dashboard, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(dashboard.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Hotspot")
                {
                    GameObject hot = Instantiate(Resources.Load("Prefab/Hotspot")) as GameObject;
                    hot.transform.SetParent(modelParent);

                    OnSetObjectData(hot, editableObjectsDataModel3D);
                    OnSetModelData(hot.transform, editableObjectsDataModel3D.editableObjectsList);

                    Add3DObjectItemScript(hot, editableObjectsDataModel3D);

                    //hot.GetComponent<Hotspot>().InitData(modelParent);
                    if (editableObjectsDataModel3D.descriptions != null && editableObjectsDataModel3D.descriptions.Length > 0)
                    {
                        hot.GetComponent<Hotspot>().url = editableObjectsDataModel3D.descriptions[0];
                    }

                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        hot.transform.Find("plane").GetComponent<MeshRenderer>().material.mainTexture = GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style);
                    }

                    hot.GetComponent<Hotspot>().InitObjectAvtive();


                    //picture.AddComponent<LookToCam>();

                    OnBindEditableDataModelWithTask(hot, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(hot, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(hot.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Gif3D")
                {
                    GameObject gif3D = Instantiate(Resources.Load("Prefab/Gif3D"), modelParent) as GameObject;

                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        Gif2Textures.GifFrames mframes = gif3D.AddComponent<GIFPlayer>().LoadGif(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectResStr + "/" + editableObjectsDataModel3D.style);
                        Material mat = gif3D.GetComponentInChildren<MeshRenderer>().material;
                        GIFPlayer.PLAYSTATE state = GIFPlayer.PLAYSTATE.ONCE;
                        gif3D.GetComponentInChildren<GIFPlayer>().isPlayOnAwake = editableObjectsDataModel3D.awakeOnPlay;
                        if (editableObjectsDataModel3D.isLoop)
                        {
                            state = GIFPlayer.PLAYSTATE.LOOP;
                        }
                        if (editableObjectsDataModel3D.awakeOnPlay)
                        {
                            gif3D.GetComponentInChildren<GIFPlayer>().PlayGif(0, editableObjectsDataModel3D.gifSpeed, mframes, mat, state);
                        }
                    }
                    OnSetObjectData(gif3D, editableObjectsDataModel3D);

                    Add3DObjectItemScript(gif3D, editableObjectsDataModel3D);

                    //picture.AddComponent<LookToCam>();

                    OnBindEditableDataModelWithTask(gif3D, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(gif3D, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(gif3D.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Light")
                {
                    GameObject objLight = new GameObject();
                    objLight.transform.SetParent(modelParent);
                    OnSetObjectData(objLight, editableObjectsDataModel3D);
                    Light light = objLight.AddComponent<Light>();

                    if (editableObjectsDataModel3D.descriptions.Length > 0 && editableObjectsDataModel3D.descriptions[0] != null)
                    {
                        //print("//////////1111editableObjectsDataModel3D.descriptions.Length:" + editableObjectsDataModel3D.descriptions.Length);
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[0]))
                        {
                            bool isColor = ColorUtility.TryParseHtmlString("#" + editableObjectsDataModel3D.descriptions[0], out Color prizeColor);
                            if (isColor)
                            {
                                light.color = prizeColor;
                            }
                        }
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[1]))
                        {
                            light.type = (LightType)Enum.Parse(typeof(LightType), editableObjectsDataModel3D.descriptions[1]);
                        }
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[2]))
                        {
                            light.intensity = float.Parse(editableObjectsDataModel3D.descriptions[2]);
                        }
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[3]))
                        {
                            light.bounceIntensity = float.Parse(editableObjectsDataModel3D.descriptions[3]);
                        }
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[4]))
                        {
                            light.shadows = (LightShadows)Enum.Parse(typeof(LightShadows), editableObjectsDataModel3D.descriptions[4]);
                        }
                        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.descriptions[5]))
                        {
                            if (GlobalData.ProjectSettingData.Type == "ModelTarget" || GlobalData.ProjectSettingData.Type == "ImageTarget")
                            {
                                light.range = float.Parse(editableObjectsDataModel3D.descriptions[5]) * 0.1f;
                            }
                            else
                            {
                                light.range = float.Parse(editableObjectsDataModel3D.descriptions[5]);
                            }
                        }
                    }
                    else
                    {
                        //print("//////////2222:" + editableObjectsDataModel3D.descriptions);
                        light.type = LightType.Directional;
                    }
                }
                else if (editableObjectsDataModel3D.type == "ParticleSystem")
                {
                    GameObject particleSystemGameObject = Instantiate(Resources.Load("Prefab/ParticleSystem"), modelParent) as GameObject;

                    particleSystemGameObject.name = editableObjectsDataModel3D.name;

                    Add3DObjectItemScript(particleSystemGameObject, editableObjectsDataModel3D);

                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        List<GameObject[]> particleSystemList = GlobalData.LoadParticleSystem(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style);
                        //压缩包
                        foreach (var particleSystemItem in particleSystemList)
                        {
                            //资源包
                            GameObject go = new GameObject();
                            go.transform.position = Vector3.zero;
                            go.transform.eulerAngles = Vector3.zero;
                            go.transform.localScale = Vector3.one;
                            foreach (var particleSystem in particleSystemItem)
                            {
                                //特效物体
                                GameObject particleSystemObject = Instantiate(particleSystem);
                                particleSystemObject.transform.SetParent(go.transform);
                                ResetShader(particleSystemObject);
                                //particleSystemObject.AddComponent<BoxCollider>();
                            }
                            go.transform.SetParent(particleSystemGameObject.transform);
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localEulerAngles = Vector3.zero;
                        }
                        if (particleSystemGameObject.GetComponent<ParticleSystem>() != null)
                        {
                            Destroy(particleSystemGameObject.GetComponent<ParticleSystem>());
                        }

                    }
                    if (particleSystemGameObject.GetComponent<BoxCollider>() != null)
                    {
                        Destroy(particleSystemGameObject.GetComponent<BoxCollider>());
                    }
                    OnSetObjectData(particleSystemGameObject, editableObjectsDataModel3D);
                    OnBindEditableDataModelWithTask(particleSystemGameObject, editableObjectsDataModel3D);

                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(particleSystemGameObject, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        ParticleSystem[] particleSystems = particleSystemGameObject.GetComponentsInChildren<ParticleSystem>();
                        foreach (var itemP in particleSystems)
                        {
                            itemP.scalingMode = ParticleSystemScalingMode.Hierarchy;
                        }
                        SetModelTargetOfObjectTrs(particleSystemGameObject.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Button3D")
                {
                    GameObject button = Instantiate(Resources.Load("Prefab/Button3D"), modelParent) as GameObject;

                    OnSetObjectData(button, editableObjectsDataModel3D);
                    Add3DObjectItemScript(button, editableObjectsDataModel3D);
                    OnBindEditableDataModelWithTask(button, editableObjectsDataModel3D);
                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(button, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(button.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Toggle3D")
                {
                    GameObject toggle = Instantiate(Resources.Load("Prefab/Toggle3D"), modelParent) as GameObject;
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.style))
                    {
                        toggle.transform.Find("Canvas/Toggle/Background").GetComponent<UnityEngine.UI.Image>().sprite = GlobalData.Texture2DToSprite(GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.style));
                    }
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.toggleImage))
                    {
                        toggle.transform.Find("Canvas/Toggle/Background/Checkmark").GetComponent<UnityEngine.UI.Image>().sprite = GlobalData.Texture2DToSprite(GlobalData.LoadImage(GlobalData.LocalPath + GlobalData.ProjectID + "/Asset/" + editableObjectsDataModel3D.toggleImage));
                    }
                    //开关颜色
                    ColorUtility.TryParseHtmlString("#" + editableObjectsDataModel3D.color, out Color prizeColor);
                    toggle.transform.Find("Canvas/Toggle/Background").GetComponent<UnityEngine.UI.Image>().color = prizeColor;
                    toggle.transform.Find("Canvas/Toggle/Background/Checkmark").GetComponent<UnityEngine.UI.Image>().color = prizeColor;
                    toggle.GetComponentInChildren<UnityEngine.UI.Toggle>().isOn = editableObjectsDataModel3D.m_switch;

                    OnSetObjectData(toggle, editableObjectsDataModel3D);
                    Add3DObjectItemScript(toggle, editableObjectsDataModel3D);
                    OnBindEditableDataModelWithTask(toggle, editableObjectsDataModel3D);
                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(toggle, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(toggle.transform);
                    }
                }
                else if (editableObjectsDataModel3D.type == "Group3D")
                {
                    GameObject group = Instantiate(Resources.Load("Prefab/Group3D"), modelParent) as GameObject;

                    OnSetObjectData(group, editableObjectsDataModel3D);
                    if (GlobalData.ProjectSettingData.Type == "ImageTarget")
                    {
                        group.transform.localScale = group.transform.localScale * 10f;
                    }
                    Add3DObjectItemScript(group, editableObjectsDataModel3D);
                    OnBindEditableDataModelWithTask(group, editableObjectsDataModel3D);
                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(group, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(group.transform);
                    }

                    if (Directory.Exists(item + "/../" + editableObjectsDataModel3D.id))
                    {
                        LoadModel(System.IO.Directory.GetFiles(item + "/../" + editableObjectsDataModel3D.id), group.transform, uiParent);
                    }
                }
                else if (editableObjectsDataModel3D.type == "InputField3D")
                {
                    GameObject inputField3D = Instantiate(Resources.Load("Prefab/InputField3D"), modelParent) as GameObject;
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.text))
                    {
                        inputField3D.GetComponentInChildren<TMPro.TMP_InputField>().text = editableObjectsDataModel3D.text;
                    }
                    ColorUtility.TryParseHtmlString("#" + editableObjectsDataModel3D.color, out Color prizeColor);
                    inputField3D.GetComponentInChildren<TMPro.TMP_InputField>().GetComponentInChildren<TMPro.TextMeshProUGUI>().color = prizeColor;
                    inputField3D.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = editableObjectsDataModel3D.fontSize;

                    OnSetObjectData(inputField3D, editableObjectsDataModel3D);

                    Transform inputfield = inputField3D.GetComponentInChildren<TMPro.TMP_InputField>().transform;
                    Vector3 standard_scale = new Vector3(1.5f, 0.27f, 1) * 0.1f;

                    Transform textInput = inputField3D.GetComponentInChildren<TMPro.TMP_InputField>().GetComponentInChildren<TMPro.TextMeshProUGUI>().transform;
                    Vector3 m_scale = inputField3D.transform.localScale;
                    Vector3 adaptationTar_scale = textInput.transform.localScale;
                    Vector2 adaptationTar_size = textInput.GetComponent<RectTransform>().sizeDelta;


                    float difference_x = standard_scale.x / m_scale.x;
                    float difference_y = standard_scale.y / m_scale.y;
                    textInput.transform.localScale = new Vector3(difference_x * adaptationTar_scale.x, difference_y * adaptationTar_scale.y, inputfield.transform.localScale.z);

                    float difference_rtx = m_scale.x / standard_scale.x;
                    float difference_rty = m_scale.y / standard_scale.y;
                    textInput.GetComponent<RectTransform>().sizeDelta = new Vector2(difference_rtx * adaptationTar_size.x, difference_rty * adaptationTar_size.y);

                    Add3DObjectItemScript(inputField3D, editableObjectsDataModel3D);
                    OnBindEditableDataModelWithTask(inputField3D, editableObjectsDataModel3D);
                    ///添加动态脚本
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
                    {
                        string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
                        foreach (var path in paths)
                        {
                            AddLoadScripts(inputField3D, path);
                        }
                    }

                    if (GlobalData.ProjectSettingData.Type == "ModelTarget")
                    {
                        SetModelTargetOfObjectTrs(inputField3D.transform);
                    }


                }
                else if (editableObjectsDataModel3D.type == "ImageTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.id) && System.IO.Directory.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id))
                    {
                        modelParent = GameObject.Find(editableObjectsDataModel3D.id).transform;
                        if (modelParent != null)
                        {
                            GameObject imageTarget = new GameObject();
                            imageTarget.transform.SetParent(modelParent);
                            imageTarget.transform.rotation = Quaternion.identity;
                            imageTarget.transform.localScale = Vector3.one;/*new Vector3(editableObjectsDataModel3D.scale[0], editableObjectsDataModel3D.scale[1], editableObjectsDataModel3D.scale[2]);*/
                            imageTarget.transform.localPosition = Vector3.zero;
                            imageTarget.name = editableObjectsDataModel3D.name;//修改名字为ID

                            Add3DObjectItemScript(imageTarget, editableObjectsDataModel3D);

                            LoadModel(System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id), imageTarget.transform, uiParent);

                        }
                    }
                }
                else if (editableObjectsDataModel3D.type == "ModelTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.id) && System.IO.Directory.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id))
                    {
                        GameObject modelTarget = new GameObject();
                        modelTarget.transform.SetParent(modelParent);
                        modelTarget.transform.rotation = Quaternion.identity;
                        modelTarget.transform.localScale = Vector3.one;/*new Vector3(editableObjectsDataModel3D.scale[0], editableObjectsDataModel3D.scale[1], editableObjectsDataModel3D.scale[2]);*/
                        modelTarget.transform.localPosition = Vector3.zero;
                        modelTarget.name = editableObjectsDataModel3D.name;//修改名字为ID

                        LoadModel(System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id), modelTarget.transform, uiParent/*imagetarget.transform*/);
                    }
                }
                else if (editableObjectsDataModel3D.type == "AreaTarget")
                {
                    if (!string.IsNullOrEmpty(editableObjectsDataModel3D.id) && System.IO.Directory.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id))
                    {
                        GameObject modelTarget = new GameObject();
                        modelTarget.transform.SetParent(modelParent);
                        modelTarget.transform.rotation = Quaternion.identity;
                        modelTarget.transform.localScale = Vector3.one;/*new Vector3(editableObjectsDataModel3D.scale[0], editableObjectsDataModel3D.scale[1], editableObjectsDataModel3D.scale[2]);*/
                        modelTarget.transform.localPosition = Vector3.zero;
                        modelTarget.name = editableObjectsDataModel3D.name;//修改名字为ID

                        LoadModel(System.IO.Directory.GetFiles(GlobalData.LocalPath + GlobalData.ProjectID + "/Library/3D/" + editableObjectsDataModel3D.id), modelTarget.transform, uiParent/*imagetarget.transform*/);
                    }
                }
            }
        }
        StartCoroutine(DownLoadCloudModelList(uiParent, modelParent));
    }

    class ItemCloudModelInfo
    {
        public string modelID;
        public string fileName;
        public DownloadInfo info;
        public EditableObjectsDataModel editableObjectsDataModel3D;
    }

    IEnumerator DownLoadCloudModelList(Transform uiParent, Transform modelParent)
    {
        // ZManager.instnace.zServer.

        foreach (var item in CloudModelIDList)
        {
            string json = GlobalData.ReadCachedJson(GlobalData.ModelVersionPath);
            if (!string.IsNullOrEmpty(json))
            {
                //遍历本地对比版本
                List<ItemCloudModelInfo> localModelList = GlobalData.DeserializeObject<List<ItemCloudModelInfo>>(json);
                foreach (var info in localModelList)
                {
                    if (item.modelID.Equals(info.modelID))
                    {

                    }
                }
            }
            else
            {
                //下载并存入缓存
                DownLoadCloudModel(item, uiParent, modelParent);
                //遍历本地对比版本
                List<ItemCloudModelInfo> localModelList = new List<ItemCloudModelInfo>();
                localModelList.Add(item);
                GlobalData.WriteCachedJson(GlobalData.SerializeObject(localModelList), GlobalData.ModelVersionPath);
            }
            while (!IsDownLoadFinsh)
            {
                yield return new WaitForEndOfFrame();
            }
            IsDownLoadFinsh = false;
        }
        //Debug.Log("===================================");
    }

    bool IsDownLoadFinsh = false;

    private void DownLoadCloudModel(ItemCloudModelInfo modelInfo, Transform uiParent, Transform modelParent)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("LoadPanel"), uiParent.root);
        UI_DownLoadPanel t = go.GetComponent<UI_DownLoadPanel>();
        modelInfo.fileName = modelInfo.fileName + ".zip";
        t.DownLoadModelsFromServer(GlobalData.BaseUrl + GlobalData.DownLoadModelIDUrl, modelInfo.modelID, modelInfo.fileName, modelInfo.info,
            (path) =>
            {
                //Debug.Log(modelInfo.modelID + "下载模型");
                bool IsZip = ZipHelper.UnzipFile(path, GlobalData.LocalPath + Path.GetFileNameWithoutExtension(path));
                File.Delete(path);
                string[] paths = Directory.GetFiles(GlobalData.LocalPath + Path.GetFileNameWithoutExtension(path));
                foreach (var item in paths)
                {
                    //Debug.Log(item);
                    LoadModelDataInfo(modelInfo.editableObjectsDataModel3D, item, modelParent);
                }
                IsDownLoadFinsh = true;
            });
        DestroyImmediate(go);
    }

    private void LoadModelDataInfo(EditableObjectsDataModel editableObjectsDataModel3D, string modelPath, Transform modelParent)
    {
        GameObject model = Instantiate(Resources.Load("Prefab/Cube")) as GameObject;

        if (model.GetComponent<MeshRenderer>())
        {
            DestroyImmediate(model.GetComponent<MeshRenderer>());
        }

        LoadModelMethod.LoadModel(modelPath).transform.SetParent(model.transform);

        if (File.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectResStr + "/" + Path.GetFileNameWithoutExtension(editableObjectsDataModel3D.style) + ".json"))
        {
            try
            {
                string pathItem = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectResStr + "/" + Path.GetFileNameWithoutExtension(editableObjectsDataModel3D.style) + ".json";
                AREAnimation mAni = GlobalData.DeserializeObject<AREAnimation>(File.ReadAllText(pathItem));
                if (mAni != null)
                {
                    LoadAREAnimation(model, mAni);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("动画加载失败：" + ex.ToString());
            }

        }

        model.transform.name = editableObjectsDataModel3D.name;

        model.transform.SetParent(modelParent);

        if (model.GetComponent<BoxCollider>() != null)
        {
            Destroy(model.GetComponent<BoxCollider>());
        }
        OnSetObjectData(model, editableObjectsDataModel3D);
        OnSetModelData(model.transform, editableObjectsDataModel3D.editableObjectsList);

        //float x = 0, y = 0, z = 0;
        //CalcHeight(model, ref x, ref y, ref z);
        //BoxCollider box = model.AddComponent<BoxCollider>();
        //box.size = new Vector3(x, y, z);

        //GameObject line = Instantiate(Resources.Load("Prefab/scan_line")) as GameObject;
        //line.transform.SetParent(model.transform);
        //line.transform.localPosition = Vector3.zero;
        //line.transform.localRotation = Quaternion.identity;
        //line.transform.localScale = Vector3.one;
        //line.AddComponent<LineToLookAt>();
        //line.name = "scan_line";
        //line.SetActive(false);

        Add3DObjectItemScript(model, editableObjectsDataModel3D);

        OnBindEditableDataModelWithTask(model, editableObjectsDataModel3D);

        if (model.GetComponentInChildren<Animation>() != null)
        {
            Animation ani = model.GetComponentInChildren<Animation>();
            ani.playAutomatically = false;
            foreach (AnimationState itemani in ani)
            {
                itemani.wrapMode = WrapMode.Once;
            }
        }
        if (GlobalData.ProjectSettingData.Type == "ModelTarget")
        {
            SetModelTargetOfObjectTrs(model.transform);
        }

        ///添加动态脚本
        if (!string.IsNullOrEmpty(editableObjectsDataModel3D.scriptPath))
        {
            string[] paths = editableObjectsDataModel3D.scriptPath.Split('|');
            foreach (var path in paths)
            {
                AddLoadScripts(model, path);
            }
        }
    }


    /// <summary>
    /// 加载ARE动画
    /// </summary>
    /// <param name="model"></param>
    /// <param name="aREAnimation"></param>
    public void LoadAREAnimation(GameObject model, AREAnimation aREAnimation)
    {
        Animation animation = model.GetComponentInChildren<Animation>();
        foreach (AREAnimationState aREAnimationState in aREAnimation.aREAnimationStates)
        {
            AnimationClip animationClip = new AnimationClip();
            animationClip.name = aREAnimationState.name;
            animationClip.wrapMode = aREAnimationState.aREAnimationClip.wrapMode;
            animationClip.legacy = aREAnimationState.aREAnimationClip.legacy;
            animationClip.frameRate = aREAnimationState.aREAnimationClip.frameRate;
            foreach (AREAnimationCurve aREAnimationCurve in aREAnimationState.aREAnimationClip.aREAnimationCurves)
            {
                AnimationCurve animationCurve = new AnimationCurve();
                foreach (AREKeyframe aREKeyframe in aREAnimationCurve.keyframes)
                {
                    Keyframe keyframe = new Keyframe(aREKeyframe.time, aREKeyframe.value);
                    animationCurve.AddKey(keyframe);
                }
                animationClip.SetCurve(aREAnimationCurve.relativePath, Type.GetType(aREAnimationCurve.type), aREAnimationCurve.propertyName, animationCurve);
            }
            if (animation == null)
            {
                animation = model.transform.GetChild(0).gameObject.AddComponent<Animation>();
            }
            animation.AddClip(animationClip, aREAnimationState.name);
        }
    }

    /// <summary>
    /// 设置物体绑定的动态脚本
    /// </summary>
    /// <param name="go">物体</param>
    /// <param name="path">脚本地址</param>
    private void AddLoadScripts(GameObject go, string path)
    {
        HxReadScriptPrefabManager.instance.AddScript(go, path);
    }

    private void Add3DObjectItemScript(GameObject go, EditableObjectsDataModel editableO)
    {
        if (go.GetComponent<ObjectItem>())
        {
            return;
        }
        ObjectItem o = go.AddComponent<ObjectItem>();
        o.mType = (SourType)System.Enum.Parse(typeof(SourType), editableO.type);
        o.editable = editableO;
    }

    protected void LoadUI(Transform uiParent, Transform modelParent)
    {
        try
        {
            //加载场景资源
            if (System.IO.File.Exists(GlobalData.LocalPath + GlobalData.ProjectID + "/" + "Library/SceneInfo.json"))
            {
                string jsonSceneInfo = System.IO.File.ReadAllText(GlobalData.LocalPath + GlobalData.ProjectID + "/" + "Library/SceneInfo.json");
                SceneInfo sceneInfo = GlobalData.DeserializeObject<SceneInfo>(jsonSceneInfo);
        //        Debug.Log(sceneInfo.resolutionRatioHeight + "  ");
                GlobalData.ResolutionRatio = new Vector2(sceneInfo.resolutionRatioWidth, sceneInfo.resolutionRatioHeight);

                //设置场景内置灯光
                Light[] lights = Camera.main.transform.GetComponentsInChildren<Light>();
                foreach (var item in lights)
                {
                    item.gameObject.SetActive(sceneInfo.light);
                }
            }
            //2D
            string path = GlobalData.LocalPath + GlobalData.ProjectID + "/" + GlobalData.ProjectConfig2DStr + GlobalData.ConfigJson2D;
            if (System.IO.File.Exists(path))
            {
                string json2D = System.IO.File.ReadAllText(path);
                List<EditableObjectsDataModel> editableObjectsDataModelsList2D = GlobalData.DeserializeObject<List<EditableObjectsDataModel>>(json2D);
                if (editableObjectsDataModelsList2D.Count != 0)
                {
                    bool isPort = true;
            //        Debug.LogError(editableObjectsDataModelsList2D.Count);
                    //设置屏幕分辨率为UI父模板
                    RectTransform bg_rt = uiParent.GetComponent<RectTransform>();
                    if (GlobalData.ResolutionRatio.x > GlobalData.ResolutionRatio.y)
                    {
                        bg_rt.sizeDelta = new Vector2(Screen.height, Screen.width);
                        isPort = false;
                    }
                    else
                    {
                        bg_rt.sizeDelta = new Vector2(Screen.width, Screen.height);
                    }


                    foreach (EditableObjectsDataModel editableObjectsDataModel in editableObjectsDataModelsList2D)
                    {
                        GameObject uiObj = Instantiate(Resources.Load("UIPrefab/Component/" + editableObjectsDataModel.type), uiParent.transform) as GameObject;

                        OnSetUIBaseData(editableObjectsDataModel, uiObj.GetComponent<RectTransform>(), isPort);

                        OnBindEditableDataModelWithTask(uiObj, editableObjectsDataModel);

                        uiObj.AddComponent<UIItem>().OnSetData(editableObjectsDataModel, modelParent, uiParent);

                    }

                    if (GlobalData.ResolutionRatio.x > GlobalData.ResolutionRatio.y)
                    {
                        bg_rt.pivot = Vector2.one * 0.5f;
                        bg_rt.anchorMin = Vector2.one * 0.5f;
                        bg_rt.anchorMax = Vector2.one * 0.5f;
                        bg_rt.localEulerAngles = new Vector3(0, 0, 270);
                    }

                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void OnSetObjectData(GameObject model, EditableObjectsDataModel editableObjectsDataModel)
    {
        model.transform.localPosition = new Vector3(editableObjectsDataModel.position[0], editableObjectsDataModel.position[1], editableObjectsDataModel.position[2]);
//        Debug.Log(model.name + "==========" + editableObjectsDataModel.rotation[1] + "==========");
        model.transform.localRotation = Quaternion.Euler(editableObjectsDataModel.rotation[0], editableObjectsDataModel.rotation[1], editableObjectsDataModel.rotation[2]);
        model.transform.localScale = new Vector3(editableObjectsDataModel.scale[0], editableObjectsDataModel.scale[1], editableObjectsDataModel.scale[2]);
        model.SetActive(editableObjectsDataModel.active);
        if (GlobalData.ProjectSettingData.Type == "ImageTarget")
        {
            model.transform.localScale = model.transform.localScale * 0.1f;
            model.transform.localPosition = model.transform.localPosition * 0.1f;
        }
    }
    private void OnSetModelData(Transform parent, List<EditableObjects> editableObjectsList)
    {
        if (editableObjectsList.Count != 0)
        {
            for (int i = 0; i < editableObjectsList.Count; i++)
            {
                parent.transform.GetChild(i).localPosition = new Vector3(editableObjectsList[i].position[0], editableObjectsList[i].position[1], editableObjectsList[i].position[2]);
                parent.transform.GetChild(i).localEulerAngles = new Vector3(editableObjectsList[i].rotation[0], editableObjectsList[i].rotation[1], editableObjectsList[i].rotation[2]);
                parent.transform.GetChild(i).localScale = new Vector3(editableObjectsList[i].scale[0], editableObjectsList[i].scale[1], editableObjectsList[i].scale[2]);
                parent.transform.GetChild(i).name = editableObjectsList[i].name;
                parent.transform.GetChild(i).gameObject.SetActive(editableObjectsList[i].active);
                //if (parent.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>())
                //{
                //    //parent.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Legacy Shaders/Self-Illumin/Bumped Diffuse");
                //}
                OnBindEditableDataWithTask(parent.transform.GetChild(i).gameObject, editableObjectsList[i]);

                //parent.transform.GetChild(i).gameObject.AddComponent<ModelHighLightManager>();
                //parent.transform.GetChild(i).gameObject.AddComponent<ModelHighShaderManager>();
                ///添加动态脚本
                if (!string.IsNullOrEmpty(editableObjectsList[i].scriptPath))
                {
                    string[] paths = editableObjectsList[i].scriptPath.Split('|');
                    foreach (var path in paths)
                    {
                        AddLoadScripts(parent.transform.GetChild(i).gameObject, path);
                    }
                }

                if (parent.transform.GetChild(i).childCount == editableObjectsList[i].editableObjectsList.Count)
                {
                    OnSetModelData(parent.transform.GetChild(i), editableObjectsList[i].editableObjectsList);
                }

            }
        }
    }
    private void OnSetUIBaseData(EditableObjectsDataModel editableObjectsDataModel, RectTransform selfTransform, bool isPort)
    {

        // gameObject.name = editableObjectsDataModel.name;
        //string id = "";
        //GlobalData.CreateID(ref id, 6);
        //selfTransform.gameObject.name = id + "ui";
        selfTransform.gameObject.name = editableObjectsDataModel.id;
        //根据类型判断进入那个类型识别,图片识别和模型识别叠加了一层父物体
        if (isPort)
        {
            // Rect rect = new Rect(editableObjectsDataModel.position[0], editableObjectsDataModel.position[1], editableObjectsDataModel.scale[0], editableObjectsDataModel.scale[1]);
            selfTransform.anchoredPosition = new Vector2(editableObjectsDataModel.position[0] * Screen.width, editableObjectsDataModel.position[1] * Screen.height);
            selfTransform.localEulerAngles = new Vector3(editableObjectsDataModel.rotation[0], editableObjectsDataModel.rotation[1], editableObjectsDataModel.rotation[2]);
            if (Screen.width < Screen.height)
            {
                selfTransform.sizeDelta = new Vector2(editableObjectsDataModel.scale[0] * Screen.width, editableObjectsDataModel.scale[1] * GlobalData.ResolutionRatio.y * (Screen.width / GlobalData.ResolutionRatio.x));
            }
            else
            {
                selfTransform.sizeDelta = new Vector2(editableObjectsDataModel.scale[0] * GlobalData.ResolutionRatio.x * (Screen.height / GlobalData.ResolutionRatio.y), editableObjectsDataModel.scale[1] * Screen.height);
            }
        }
        else
        {
            // Rect rect = new Rect(editableObjectsDataModel.position[0], editableObjectsDataModel.position[1], editableObjectsDataModel.scale[0], editableObjectsDataModel.scale[1]);
            selfTransform.anchoredPosition = new Vector2(editableObjectsDataModel.position[0] * Screen.height, editableObjectsDataModel.position[1] * Screen.width);
            selfTransform.localEulerAngles = new Vector3(editableObjectsDataModel.rotation[0], editableObjectsDataModel.rotation[1], editableObjectsDataModel.rotation[2]);
            if (Screen.width < Screen.height)
            {
                selfTransform.sizeDelta = new Vector2(editableObjectsDataModel.scale[0] * Screen.height, editableObjectsDataModel.scale[1] * GlobalData.ResolutionRatio.y * (Screen.height / GlobalData.ResolutionRatio.x));
            }
            else
            {
                selfTransform.sizeDelta = new Vector2(editableObjectsDataModel.scale[0] * GlobalData.ResolutionRatio.x * (Screen.width / GlobalData.ResolutionRatio.y), editableObjectsDataModel.scale[1] * Screen.width);
            }
        }
        ///添加动态脚本
        if (!string.IsNullOrEmpty(editableObjectsDataModel.scriptPath))
        {
            string[] paths = editableObjectsDataModel.scriptPath.Split('|');
            foreach (var path in paths)
            {
                AddLoadScripts(selfTransform.gameObject, path);
            }
        }

        selfTransform.gameObject.SetActive(editableObjectsDataModel.active);
    }

    /// <summary>
    /// 重置Shader
    /// </summary>
    /// <param name="obj"></param>
    private void ResetShader(UnityEngine.Object obj)
    {
        List<Material> listMat = new List<Material>();
        listMat.Clear();
        if (obj is Material)
        {
            Material m = obj as Material;
            listMat.Add(m);
        }
        else if (obj is GameObject)
        {
            GameObject go = obj as GameObject;
            Renderer[] rends = go.GetComponentsInChildren<Renderer>();
            if (null != rends)
            {
                foreach (Renderer item in rends)
                {
                    Material[] materialsArr = item.sharedMaterials;
                    foreach (Material m in materialsArr)
                        listMat.Add(m);
                }
            }
        }
        for (int i = 0; i < listMat.Count; i++)
        {
            Material m = listMat[i];
            if (null == m)
                continue;
            var shaderName = m.shader.name;
            var newShader = Shader.Find(shaderName);
            if (newShader != null)
                m.shader = newShader;
        }
    }
    /// <summary>
    /// 设置物体大小
    /// </summary>
    /// <param name="trs"></param>
    private void SetModelTargetOfObjectTrs(Transform trs)
    {
        trs.localScale = trs.localScale * 0.1f;
        trs.localPosition = trs.localPosition * 0.1f;
    }

    /// <summary>
    /// 绑定任务
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="editable"></param>
    private void OnBindEditableDataWithTask(GameObject obj, EditableObjects editable)
    {
        EditableData ed = obj.GetComponent<EditableData>();
        if (ed == null)
            ed = obj.AddComponent<EditableData>();
        ed.eoData = editable;

        GlobalData.EditableDataTaskObjs.Add(obj);
    }
    /// <summary>
    /// 绑定任务
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="editableModel"></param>
    private void OnBindEditableDataModelWithTask(GameObject obj, EditableObjectsDataModel editableModel)
    {
        EditableData ed = obj.GetComponent<EditableData>();
        if (ed == null)
            ed = obj.AddComponent<EditableData>();
        ed.eoModel = editableModel;

        GlobalData.EditableDataTaskObjs.Add(obj);

    }
    /// <summary>
    /// 估算模型高度绑定BoxCollider
    /// </summary>
    /// <param name="go"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    private void CalcHeight(GameObject go, ref float x, ref float y, ref float z)
    {
        float xmax = float.MinValue; float xmin = float.MaxValue;
        float ymax = float.MinValue; float ymin = float.MaxValue;
        float zmax = float.MinValue; float zmin = float.MaxValue;
        foreach (var item in go.GetComponentsInChildren<MeshRenderer>())
        {
            if (item.bounds.max.x > xmax)
                xmax = item.bounds.max.x;
            if (item.bounds.min.x < xmin)
                xmin = item.bounds.min.x;
            if (item.bounds.max.y > ymax)
                ymax = item.bounds.max.y;
            if (item.bounds.min.y < ymin)
                ymin = item.bounds.min.y;
            if (item.bounds.max.z > zmax)
                zmax = item.bounds.max.z;
            if (item.bounds.min.z < zmin)
                zmin = item.bounds.min.z;
        }
        x = xmax - xmin;//x
        z = zmax - zmin;
        y = ymax - ymin;//z
    }
    private float GetDecimal(float f)
    {
        float result = 1;
        if (f < 1 && f > 0)
        {
            while (f * 10 < 10)
            {
                result = result * 10;
                f = f * 10;
            }
            return result;
        }
        else if (f > 1)
        {
            while (f / 10 > 1)
            {
                result = result / 10;
                f = f / 10;
            }
            return result;
        }
        return 0;
    }
}
