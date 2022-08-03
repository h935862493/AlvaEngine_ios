using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public void SetActiveTrue()
    {
        gameObject.SetActive(true);
    }
    public void SetActiveFales()
    {
        gameObject.SetActive(false);
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
    public void PlayAnimation(int index)
    {
        ActiveAnimation();
        ani.Play(clipNames[index]);
    }
    public void PlayAnimation1(string name)
    {
        ActiveAnimation();
        ani.Play(name);
    }
}
