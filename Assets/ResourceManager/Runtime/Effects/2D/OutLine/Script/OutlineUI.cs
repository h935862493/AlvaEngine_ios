using UnityEngine;
using UnityEngine.UI;

public class OutlineUI : MaterialEffectsBase
{
    //[Header("ÏßÌõ´ÖÏ¸")]
    public float edgeWidth = 0.05f;
    //[Header("ÑÕÉ«")]
    public Color color = new Color(0,1,0,1);

    private Material mat;
    private bool IsExcute;

    /*private void Start()
    {

    }*/
    public new void Execute()
    {
        if (!this.GetComponent<OutlineUI>().enabled)
            return;
        mat = this.GetComponent<Image>().material;
        Shader shader = Shader.Find("Custom/Edge");
        material = new Material(shader);
        this.GetComponent<Image>().material = material;
        IsExcute = true;
    }

    void Update()
    {
        if (IsExcute)
        {
            material.SetFloat("_Edge", edgeWidth);
            material.SetColor("_EdgeColor", color);
            EndEvent?.Invoke();
        }
    }
    public override void Recover()
    {
        if (!this.GetComponent<OutlineUI>().enabled)
            return;
        this.GetComponent<Image>().material = mat;
    }
}
