using UnityEngine;

public class AnimationEventBehaviour : MonoBehaviour
{
    /// <summary>
    /// 激活目标对象
    /// </summary>
    public void SetActiveTrue(GameObject game)
    {
        game.SetActive(true);
    }
    /// <summary>
    /// 隐藏目标对象
    /// </summary>
    public void SetActiveFales(GameObject game)
    {
        game.SetActive(false);
    }
    public Animator ani;
    public string[] clipNames;
    public void ActiveAnimation()
    {
        if(ani != null)
        {
            if (!ani.gameObject.activeSelf)
                ani.gameObject.SetActive(true);
            ani.enabled = true;
        }
    }
    /// <summary>
    /// 播放动画 
    /// </summary>
    /// <param name="index"> clipNames 的动画位置</param>
    public void PlayAnimationNumber(int index)
    {
        ActiveAnimation();
        ani.Play(clipNames[index]);
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="name">动画名称</param>
    public void PlayAnimationString(string name)
    {
        ActiveAnimation();
        ani.Play(name);
    }
}
