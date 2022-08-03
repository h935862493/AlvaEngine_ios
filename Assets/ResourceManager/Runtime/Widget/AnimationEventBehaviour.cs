using UnityEngine;

public class AnimationEventBehaviour : MonoBehaviour
{
    /// <summary>
    /// ����Ŀ�����
    /// </summary>
    public void SetActiveTrue(GameObject game)
    {
        game.SetActive(true);
    }
    /// <summary>
    /// ����Ŀ�����
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
    /// ���Ŷ��� 
    /// </summary>
    /// <param name="index"> clipNames �Ķ���λ��</param>
    public void PlayAnimationNumber(int index)
    {
        ActiveAnimation();
        ani.Play(clipNames[index]);
    }
    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="name">��������</param>
    public void PlayAnimationString(string name)
    {
        ActiveAnimation();
        ani.Play(name);
    }
}
