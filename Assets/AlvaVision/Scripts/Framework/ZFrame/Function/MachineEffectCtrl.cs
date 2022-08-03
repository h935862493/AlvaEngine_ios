using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MachineEffectCtrl : MonoBehaviour
{
    private Material[] effectMaterial;
    Tweener[] effectTweener;
    public void Init()
    {
        effectMaterial = GetComponent<MeshRenderer>().materials;
        foreach (var item in effectMaterial)
        {
            item.DOFade(0f, 0f);
        }
        effectTweener = new Tweener[effectMaterial.Length];
        for (int i = 0; i < effectTweener.Length; i++)
        {
            Tweener item = effectTweener[i];
            item = effectMaterial[i].DOFade(1f, 0.8f);
            item.SetLoops(-1, LoopType.Yoyo);
            item.Play();
        }
      
    }

    public void PlayMachineEffect()
    {
        for (int i = 0; i < effectTweener.Length; i++)
        {
            effectTweener[i].Play();
        }
    }

    public void ResetMachineEffect()
    {
        for (int i = 0; i < effectTweener.Length; i++)
        {
            effectMaterial[i].DOFade(1f, 0f);
            Tweener item = effectTweener[i];
            item.Restart();
            item.Pause();
        }
    }

    public void SetMachineAlpha(float alpha)
    {
        if (effectMaterial != null)
        {
            foreach (var item in effectMaterial)
            {
                item.DOFade(alpha, 0f);
            }
        }
    }

    private void OnDisable()
    {
        if (effectMaterial != null)
        {
            foreach (var item in effectMaterial)
            {
                item.DOFade(1f, 0f);
            }
        }
    }

    public void StopMachineEffect()
    {
        ResetMachineEffect();
    }

}
