using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UniGLTF;
using UnityEngine;
using UnityEngine.UI;

public class VRMtest : MonoBehaviour
{
    public string fileName;
    private void Start()
    {
        //print("1111111111111111111");
        //string path = "/storage/emulated/0/0testAPP/gltf/s15.glb";
        string path = @"C:\Users\Administrator\Desktop\" + fileName;

       LoadModel(path);
    }

    void LoadModel(string path)
    {
        if (!File.Exists(path))
        {
            print("path none");
            return;
        }

        Debug.LogFormat("{0}", path);
        var ext = Path.GetExtension(path).ToLower();
        switch (ext)
        {
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

                    //var animation = context.Root.GetComponent<Animation>();
                    //if (animation && animation.clip != null)
                    //{
                    //    animation.clip.wrapMode = WrapMode.Once;
                    //    animation.Play(animation.clip.name);

                    //    //animation.playAutomatically = false;
                    //    //foreach (AnimationState state in animation)
                    //    //{
                    //    //    Debug.Log(state.name);
                    //    //    state.layer = 10;
                    //    //    state.enabled = true;
                    //    //    state.weight = 1;
                    //    //}
                    //}

                    //go = context.Root;
                    //foreach (AnimationState animationState in animation)
                    //{
                    //    clips.Add(animationState.clip);
                    //}
                    GameObject go = context.Root;

                    break;
                }

            default:
                Debug.LogWarningFormat("unknown file type: {0}", path);
                break;
        }
    }

    // GameObject go;
    //public List<AnimationClip> clips = new List<AnimationClip>();
    // int num = 0;
    // public void PlayAnimation()
    // {
    //     num++;
    //     var ani = go.GetComponent<Animation>();
    //     ani.clip = clips[num];
    //     ani.Play();
    // }
}
