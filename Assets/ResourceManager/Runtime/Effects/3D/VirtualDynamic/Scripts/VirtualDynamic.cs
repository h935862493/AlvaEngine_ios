using UnityEngine;
[AddComponentMenu("myName/myProduct/v1/Transparent Bone")]
public class VirtualDynamic : MaterialEffectsBase
{
    public Color color = new Color(0.749f, 0.729f, 0.364f, 0.0f);
    public override void Execute()
    {
        if (!this.GetComponent<VirtualDynamic>().enabled)
            return;
        base.Execute();
        
    }
    private bool back;
    /// <summary>
    /// ÔËÐÐ
    /// </summary>
    private void Update()
    {
        if (IsExecute)
        {
            foreach (Material material in materials)
            {
                material.SetColor("_EmissionColor", color);
            }
            EndEvent?.Invoke();
        }
    }
}
