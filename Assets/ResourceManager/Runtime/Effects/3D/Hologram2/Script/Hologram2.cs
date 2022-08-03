using UnityEngine;
[AddComponentMenu("myName/myProduct/v1/Holographic Grid")]
public class Hologram2 : MaterialEffectsBase
{
    public float time = 1f;
    //public float speed = 0.5f;
    private float timer = 0;
    public override void Execute()
    {
        if (!this.GetComponent<Hologram2>().enabled)
            return;
        base.Execute();
        timer = -0.2f;
    }
    /// <summary>
    /// ÔËÐÐ
    /// </summary>
    private void Update()
    {
        if (IsExecute)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                timer = 0;
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
