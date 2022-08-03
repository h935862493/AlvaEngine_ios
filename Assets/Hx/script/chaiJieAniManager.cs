using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaiJieAniManager : MonoBehaviour
{
    public EffectGO effectGO;
    public void AniShow5(float sp)
    {
        effectGO.AniShow5(sp);
    }
    //动画5行使结束隐藏模型
    public void AniHide5(float sp)
    {
        effectGO.AniHide5(sp);
    }
    public void AniShowAll()
    {
        effectGO.AniShowAll();
    }
}
