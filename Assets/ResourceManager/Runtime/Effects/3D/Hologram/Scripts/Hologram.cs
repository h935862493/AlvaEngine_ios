using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("myName/myProduct/v1/3D Printing")]
public class Hologram : MaterialEffectsBase{

    public float time = 3f;
    private float timer = 0;
    public override void Execute()
    {
        if (!this.GetComponent<Hologram>().enabled)
            return;
        base.Execute();
        timer = -0.2f;
    }
    /// <summary>
    /// 运行
    /// </summary>
    private void Update () {
        if (IsExecute)
        {
            timer += Time.deltaTime / time;
            //timer += Time.deltaTime;
            if (timer > 1)
            {
                IsExecute = false;
                Recover();
                EndEvent?.Invoke();
            }
            foreach (Material material in materials)
            {
                material.SetFloat("_ShaderDisplacement", timer);
            }
        }
    }
}
