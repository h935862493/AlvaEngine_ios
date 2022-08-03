using System.Collections.Generic;
using UnityEngine;

public class EffectGO : MonoBehaviour
{
    public MeshRenderer[] renderers;
    List<Material> m = new List<Material>();
    float Treshold = -0.1f;
    float DissolveTreshold = 0;
    bool UseDissolve = false;
    public float speed = 0.5f;
    bool Inscrease = true;
    //Transform prt;
    //转圈动画特有
    public bool isAni5 = false;
    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();

        //prt = transform.Find(".PRT3");
        //var a = prt.GetComponent<MeshRenderer>().materials;
        foreach (var item in renderers)
        {
            foreach (var b in item.materials)
            {
                m.Add(b as Material);
            }
        }


        foreach (Material s in m)
        {
            s.SetFloat("_Treshold", Treshold);
            s.SetFloat("_DissolveTreshold", DissolveTreshold);
        }

        if (isAni5)
        {
            foreach (Material s in m)
            {
                s.SetFloat("_DissolveEdge", 0.3f);
                s.SetFloat("_Treshold", Treshold);
                s.SetInt("_UseDissolveEffect", 1);
                //s.SetFloat("_EmissionStrenth", .3f);
                s.SetInt("_UseDissolve", 1);
                s.SetFloat("_Threshold", -0.1f);
                Treshold = -.1f;
            }
            foreach (Material h in m)
            {
                h.SetFloat("_DissolveThreshold", 1);
            }
            gameObject.GetComponent<EffectGO>().enabled = false;
        }
        foreach (Material s in m)
        {
            s.SetVector("_CaptureLocation", new Vector4(this.transform.parent.parent.position.z, 0, this.transform.parent.parent.position.x, 0));
        }

    }

    void Update()
    {
        if (m.Count != 0)
        {
            if (UseDissolve)
            {
                if (Inscrease)
                {
                    DissolveTreshold += Time.deltaTime * speed;
                    DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                    if (DissolveTreshold >= 1)
                    {
                        Inscrease = false;
                        gameObject.GetComponent<EffectGO>().enabled = false;
                    }
                }
                else
                {
                    DissolveTreshold -= Time.deltaTime * speed;
                    DissolveTreshold = Mathf.Clamp(DissolveTreshold, 0, 1);
                    if (DissolveTreshold <= 0)
                    {
                        Inscrease = true;
                        gameObject.GetComponent<EffectGO>().enabled = false;
                    }
                }


                foreach (Material a in m)
                {
                    a.SetFloat("_DissolveThreshold", DissolveTreshold);
                }
            }
            else
            {

                Shader.SetGlobalMatrix("SelfMatrix", transform.parent.parent.worldToLocalMatrix);
               
                if (Inscrease)
                {
                    Treshold += Time.deltaTime * speed;
                    Treshold = Mathf.Clamp(Treshold, -0.2f, 1.2f);
                    if (Treshold >= 1.2f)
                    {
                        Inscrease = false;
                    }
                }
                else
                {
                    Treshold -= Time.deltaTime * speed;
                    Treshold = Mathf.Clamp(Treshold, -0.2f, 1.2f);
                    if (Treshold <= -0.2f)
                    {
                        Inscrease = true;
                    }
                }
                foreach (Material a in m)
                {
                    a.SetFloat("_Threshold", Treshold);

                }
            }

        }
    }


    public void Flash()
    {
       
        foreach (Material s in m)
        {
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 0);
            s.SetInt("_UseDissolve", 0);
            s.SetFloat("_DissolveThreshold", 0f);
            DissolveTreshold = 0f;
        }
        UseDissolve = false;
    }
    public void ShowHide()
    {
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            s.SetFloat("_DissolveThreshold", 0f);
            Treshold = -.1f;
        }
        UseDissolve = true;
        Inscrease = true;
        gameObject.GetComponent<EffectGO>().enabled = false;
    }
    public void ShowHide2()
    {
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            s.SetFloat("_DissolveThreshold", 0f);
            Treshold = -.1f;
        }
        DissolveTreshold = 0;
        UseDissolve = true;
        Inscrease = true;
        gameObject.GetComponent<EffectGO>().enabled = false;
    }
    //动画5开始行使展示模型
    public void AniShow5(float sp)
    {
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            Treshold = -.1f;
        }
        foreach (Material h in m)
        {
            h.SetFloat("_DissolveThreshold", 1);
        }

        speed = sp;
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            Treshold = -.1f;
        }
        DissolveTreshold = 1;
        UseDissolve = true;
        Inscrease = false;
        gameObject.GetComponent<EffectGO>().enabled = true;
    }
    //动画5行使结束隐藏模型
    public void AniHide5(float sp)
    {
        speed = sp;
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            Treshold = -.1f;
        }
        DissolveTreshold = 0;
        UseDissolve = true;
        Inscrease = true;
        gameObject.GetComponent<EffectGO>().enabled = true;
    }


    public void AniShowAll()
    {
        gameObject.GetComponent<EffectGO>().enabled = false;
        foreach (Material s in m)
        {
            s.SetFloat("_DissolveEdge", 0.3f);
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 1);
            //s.SetFloat("_EmissionStrenth", .3f);
            s.SetInt("_UseDissolve", 1);
            s.SetFloat("_Threshold", -0.1f);
            Treshold = -.1f;
        }
        foreach (Material h in m)
        {
            h.SetFloat("_DissolveThreshold", 0);
        }
    }
    void OnDestroy()
    {
        foreach (Material s in m)
        {
            s.SetFloat("_Treshold", Treshold);
            s.SetInt("_UseDissolveEffect", 0);
            s.SetInt("_UseDissolve", 0);
            s.SetFloat("_DissolveThreshold", 0f);
            DissolveTreshold = 0f;
        }
    }

}
