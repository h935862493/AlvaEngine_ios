using System.IO;
using UnityEditor;
using UnityEngine;

public class HxABbuild : MonoBehaviour
{
    [MenuItem("Custom Editor/Create Scene")]
    static void CreateSceneALL()
    {
        string[] SceneName = new string[]
       {
    "Alva_FDJ_Model",
    "Alva_FDJ_Image",
    "ALVA_FDJ_slam",
    "Alva_MT_Model"
       };
        int num = 0;
        foreach (var item in SceneName)
        {
            num++;
            //清空一下缓存  
            Caching.ClearCache();
            //获得用户选择的路径的方法，可以打开保存面板（推荐）
            //string Path = EditorUtility.SaveFilePanel("保存资源", "SS", "" + "Alva_FDJ_Model", "unity3d");
            //选择的要保存的对象 
            string[] levels = { "Assets/AlvaVision/Scenes/" + item + ".unity" };
            //打包场景  
            BuildPipeline.BuildPlayer(levels, @"/Users/alva/hx/ab/" + item + ".unity3d", BuildTarget.iOS, BuildOptions.BuildAdditionalStreamedScenes);
            print("打包完成：" + num + "/" + SceneName.Length);        }        print("打包结束****");
    }

    [MenuItem("Custom Editor/Create One Scene")]    static void CreateOneScene()    {
        //清空一下缓存  
        Caching.ClearCache();
        //获得用户选择的路径的方法，可以打开保存面板（推荐）
        string Path = EditorUtility.SaveFilePanel("保存资源", "SS", "" + "iOS", "unity3d");
        //选择的要保存的对象 
        string[] levels = { "Assets/AlvaVision/Scenes/ALVA_FDJ_slam.unity" };
        //打包场景  
        BuildPipeline.BuildPlayer(levels, Path, BuildTarget.iOS, BuildOptions.BuildAdditionalStreamedScenes);


        print("打包结束****");    }

}
