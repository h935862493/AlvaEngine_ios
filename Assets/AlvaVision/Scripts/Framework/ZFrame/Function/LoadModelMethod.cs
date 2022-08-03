using System;
using System.IO;
using System.Text;
//using TriLibCore;
using UniGLTF;
using UnityEngine;

public class LoadModelMethod
{
    /// <summary>
    /// 加载模型
    /// </summary>
    /// <param name="path">模型本地地址</param>
    /// <returns></returns>
    public static GameObject LoadModel(string path)
    {
        GameObject tempGo = null;
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            Debug.Log("地址错误：" + path);
            return tempGo;
        }

        var ext = Path.GetExtension(path).ToLower();
        switch (ext)
        {
            case ".fbx":
                {
                    Debug.Log("fbx加载错误：" + path);
                    //tempGo = FBXfile(path);
                    break;
                }
            case ".gltf":
            case ".glb":
                {
                    var context = new UniGLTF.ImporterContext();
                    var file = File.ReadAllBytes(path);
                    if (ext == ".gltf")
                        context.ParseJson(Encoding.UTF8.GetString(file), new FileSystemStorage(Path.GetDirectoryName(path)));
                    else
                        context.ParseGlb(file);
                    context.Load();
                    context.ShowMeshes();
                    context.EnableUpdateWhenOffscreen();
                    context.ShowMeshes();
                    tempGo = context.Root;
                    break;
                }

            default:
                Debug.LogWarningFormat("unknown file type: {0}", path);
                break;
        }

        //MeshRenderer[] mrs = tempGo.GetComponentsInChildren<MeshRenderer>();
        //for (int i = 0; i < mrs.Length; i++)
        //{
        //    if (mrs[i] != null && mrs[i].material != null)
        //    {
        //        RendereringModelChange.Instance.SetMaterialRenderingMode(mrs[i].material, ALVARenderingMode.Fade);
        //    }
        //}

        return tempGo;
    }

    public static ImporterContext LoadGlbModel(string path)
    {
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            Debug.Log("地址错误：" + path);
            return null;
        }

        var ext = Path.GetExtension(path).ToLower();
        if (ext.Equals(".gltf") || ext.Equals(".gltf"))
        {
            var context = new UniGLTF.ImporterContext();
            var file = File.ReadAllBytes(path);
            if (ext == ".gltf")
                context.ParseJson(Encoding.UTF8.GetString(file), new FileSystemStorage(Path.GetDirectoryName(path)));
            else
                context.ParseGlb(file);
            context.Load();
            context.ShowMeshes();
            context.EnableUpdateWhenOffscreen();
            context.ShowMeshes();
            return context;
        }
        else
        {
            return null;
        }
    }
    //private static GameObject FBXfile(string path)
    //{
    //    GameObject tempGo = null;

    //    AssetLoaderOptions _assetLoaderOptions = null;
    //    var assetLoaderContext = AssetLoader.LoadModelFromFileNoThread(path, null, _assetLoaderOptions);

    //    tempGo = assetLoaderContext.RootGameObject;
    //    Debug.Log(tempGo.name);
    //    //using ()
    //    //{
    //    //    var assetLoader = new AssetLoader();
    //    //    try
    //    //    {
    //    //        var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
    //    //        assetLoaderOptions.RotationAngles = new Vector3(0, 0, 0f);
    //    //        assetLoaderOptions.AutoPlayAnimations = false;

    //    //        //assetLoader.LoadFromFile(path, assetLoaderOptions, null, delegate (GameObject loadedGameObject)
    //    //        //{
    //    //        //    tempGo = loadedGameObject;

    //    //        //});

    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        Debug.LogError(e.ToString());
    //    //    }
    //    //}
    //    return tempGo;
    //}

    public void Rest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void OnLost(GameObject go)
    {
        var rendererComponents = go.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = go.GetComponentsInChildren<Collider>(true);
        var canvasComponents = go.GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }
}
