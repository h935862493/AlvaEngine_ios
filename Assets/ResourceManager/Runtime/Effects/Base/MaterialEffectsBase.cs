using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 材质特效基类
/// </summary>
public class MaterialEffectsBase : EffectsBase
{
    public Material material;

    [HideInInspector]
    public  bool IsOne = true;
    [HideInInspector]
    public List<Material> materials = new List<Material>();
    [HideInInspector]
    public Material[] backups_materials;
    [HideInInspector]
    public Dictionary<Renderer, Material[]> materialDictionary = new Dictionary<Renderer, Material[]>();
    /*[HideInInspector]
    public Dictionary<SkinnedMeshRenderer, Material[]> materialDictionary2 = new Dictionary<SkinnedMeshRenderer, Material[]>();*/
    [HideInInspector]
    public Renderer[] renderers;
    [HideInInspector]
    public SkinnedMeshRenderer[] skinedRenderers;
    [HideInInspector]
    public bool IsExecute;
    /// <summary>
    /// 执行
    /// </summary>
    public virtual void Execute()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            //保存自身材质
            if (IsOne)
            {
                backups_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < backups_materials.Length; i++)
                {
                    backups_materials[i] = Instantiate(renderer.materials[i]);
                }
                materialDictionary.Add(renderer, backups_materials);
            }

            //替换目标材质
            Material[] m_materials = new Material[renderer.materials.Length];
            for (int i = 0; i < m_materials.Length; i++)
            {
                m_materials[i] = material;
                //m_materials[i] = Instantiate(renderer.materials[i]);             
            }
            renderer.materials = m_materials;

            //添加所有材质到集合中
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materials.Add(renderer.materials[i]);
            }
        }

        /*foreach (var renderer in renderers)
        {
            Debug.Log(renderer.gameObject.name);
        }

        //GameObject[] b = GameObject.FindObjectsOfType<MeshRenderer>().;
        Debug.Log(renderers.Length);
        //MeshRenderer a = GameObject.FindObjectOfType<MeshRenderer>();
        //a.gameObject.SetActive(false);*/
        /*
        if (renderers.Length == 0)
        {
            skinedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            Debug.Log(skinedRenderers.Length);
            foreach (var renderer in skinedRenderers)
            {
                //保存自身材质
                if (IsOne)
                {
                    backups_materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < backups_materials.Length; i++)
                    {
                        backups_materials[i] = Instantiate(renderer.materials[i]);
                    }
                    materialDictionary.Add(renderer, backups_materials);
                }
                //替换目标材质
                Material[] m_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < m_materials.Length; i++)
                {
                    m_materials[i] = material;
                    //m_materials[i] = Instantiate(renderer.materials[i]);             
                }
                renderer.materials = m_materials;

                //添加所有材质到集合中
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials.Add(renderer.materials[i]);
                }
            }
        }
        else
        {
            foreach (var renderer in renderers)
            {
                //保存自身材质
                if (IsOne)
                {
                    backups_materials = new Material[renderer.materials.Length];
                    for (int i = 0; i < backups_materials.Length; i++)
                    {
                        backups_materials[i] = Instantiate(renderer.materials[i]);
                    }
                    materialDictionary.Add(renderer, backups_materials);
                }
                //替换目标材质
                Material[] m_materials = new Material[renderer.materials.Length];
                for (int i = 0; i < m_materials.Length; i++)
                {
                    m_materials[i] = material;
                    //m_materials[i] = Instantiate(renderer.materials[i]);             
                }
                renderer.materials = m_materials;

                //添加所有材质到集合中
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials.Add(renderer.materials[i]);
                }
            }
        }
        */

        IsOne = false;
        IsExecute = true;
        ExecuteEvent?.Invoke();
    }

    /// <summary>
    /// 停止
    /// </summary>
    public virtual void StopExecute()
    {
        EndEvent?.Invoke();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public virtual void Recover()
    {
        IsExecute = false;
        foreach (var item in materialDictionary)
        {
            item.Key.materials = item.Value;
        }
        /*if (materialDictionary.Count == 0)
        {
            foreach (var item in materialDictionary2)
            {
                item.Key.materials = item.Value;
            }
        }
        else
        {
            foreach (var item in materialDictionary)
            {
                item.Key.materials = item.Value;
            }
        }*/
        RecoverEvent?.Invoke();
    }
}
