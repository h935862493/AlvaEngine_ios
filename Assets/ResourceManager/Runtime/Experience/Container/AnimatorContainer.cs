using System.Collections.Generic;
using UnityEngine;

public class AnimatorContainer : MonoBehaviour
{
    [SerializeField]
    public List<Objects> AnimatorList = new List<Objects>();
    [System.Serializable]
    public class Objects
    {
        [SerializeField]
        public List<AnimatorObject> list = new List<AnimatorObject>();
    }
    [System.Serializable]
    public class AnimatorObject
    {
        [SerializeField]
        public string stateName;
        [SerializeField]
        public Animator animator = new Animator();
    }
    public void OnPlay(int index)
    {
        if (index >= AnimatorList.Count)
            return;
        foreach (AnimatorObject animatorObject in AnimatorList[index].list)
        {
            animatorObject.animator.Play(animatorObject.stateName);
        }
    }
    public void OnSetBoolFalse(string name)
    {
        foreach (var item in AnimatorList)
        {
            foreach (AnimatorObject animatorObject in item.list)
            {
                animatorObject.animator.SetBool(name, false);
            }
        }
    }
    public void OnSetBoolTrue(string name)
    {
        foreach (var item in AnimatorList)
        {
            foreach (AnimatorObject animatorObject in item.list)
            {
                animatorObject.animator.SetBool(name, true);
            }
        }
    }
}
