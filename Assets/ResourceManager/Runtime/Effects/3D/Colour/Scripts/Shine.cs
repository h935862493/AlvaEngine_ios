using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Shine : MaterialEffectsBase
{
    [System.Serializable]
    public class MaterialItem
    {
        public Material material;
        public Color initColor;
        public MaterialItem(Material material, Color initColor)
        {
            this.material = material;
            this.initColor = initColor;
        }
    }
    public Color color = new Color(1,0,0,1);
    [HideInInspector]
    public float alpha = 0.6f;
    [HideInInspector]
    public bool shine;
    //[ShowWhen("shine",true)]
    public float frequency = 3f;
    //[ShowWhen("shine", true)]
    public float time = 2f;
    //[ShowWhen("shine", true)]
    public bool IsCircle;
    [HideInInspector]
    public List<MaterialItem> materialItems = new List<MaterialItem>();

    private float Timeber;
    private float duration;
    
    public override void Execute()
    {
        if (!this.GetComponent<Shine>().enabled)
            return;
        if (IsExecute)
            return;
        base.Execute();
        materialItems.Clear();
        Timeber = 0;
        
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                MaterialItem materialItem = new MaterialItem(renderer.materials[i], renderer.materials[i].color);
                materialItems.Add(materialItem);
            }
        }
    }

    void Update()
    {
        if (IsExecute)
        {
            Timeber += Time.deltaTime;
            duration = 1 / frequency;
            var lerp = Mathf.PingPong(Time.time, duration) / duration;
            for (int i = 0; i < materialItems.Count; i++)
            {
                color.a = alpha;
                materialItems[i].material.color = Color.Lerp(materialItems[i].initColor, color, lerp);
            }
            if (Timeber > time)
            {
                StopExecute();
                if (IsCircle)
                {
                    Execute();
                }
            }
        }
    }

    public void StopExecute()
    {
        IsExecute = false;      
        Recover();
        EndEvent.Invoke();
    }

    public override void Recover()
    {
        base.Recover();
    }
}
