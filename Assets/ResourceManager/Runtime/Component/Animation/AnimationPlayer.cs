
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Alva.Animations
{
    [System.Serializable]
    public class AnimationItem
    {
        public string name;
        public float delayTime;
        public bool needPlay;
        public bool isLoop;
        [HideInInspector, SerializeField]
        public AnimationClip animationClip;
        [SerializeField]
        public UnityEvent finishEvent;
        [SerializeField]
        public List<KeyAnimation> keyAnimations = new List<KeyAnimation>();

        public AnimationItem(string _name)
        {
            name = _name;
            delayTime = 0;
            needPlay = false;
            isLoop = false;
        }
    }
    [System.Serializable]
    public class KeyAnimation
    {
        public int keyAnimationIndex;
        [SerializeField]
        public UnityEvent keyEvent;
    }
    public class AnimationClipInfo
    {
        public string name;
        public float delayTime;
        public bool isLoop;
        [HideInInspector, SerializeField]
        public AnimationClip animationClip;
        [SerializeField]
        public UnityEvent finishEvent;
        public AnimationClipInfo(string name, float delayTime)
        {
            this.name = name;
            this.delayTime = delayTime;
            isLoop = false;
        }
        public AnimationClipInfo(string name, float delayTime, bool isLoop)
        {
            this.name = name;
            this.delayTime = delayTime;
            this.isLoop = isLoop;
        }
        public AnimationClipInfo(string name, float delayTime, bool isLoop, UnityEvent finishEvent)
        {
            this.name = name;
            this.delayTime = delayTime;
            this.isLoop = isLoop;
            this.finishEvent = finishEvent;
        }

    }
    //[DisallowMultipleComponent] 
    [RequireComponent(typeof(Animator))]
    public class AnimationPlayer : MonoBehaviour
    {

        Animator animator;
        Queue<AnimationClipInfo> animationClipInfosQueue = new Queue<AnimationClipInfo>();
        bool isCompletPlayingAnimation = true;
        [SerializeField]
        public List<AnimationItem> animationItemList = new List<AnimationItem>();
        public Button playButton;
        public Button stopButton;
        [SerializeField, HideInInspector]
        public Action startAction;
        AnimationClipInfo currentPlayAnimationClipInfo;
        void Awake()
        {
            animator = GetComponent<Animator>();
            //  AddAndPlayEmptyAnimation();                        
            animator.enabled = false;
            //for (int i = 0; i < animationItemList.Count; i++)
            //{
            //    AddAnimationClipEndUnityEvent(animationItemList[i]);
            //}
        }
        private void Start()
        {
            startAction?.Invoke();
            if (playButton)
            {
                playButton.onClick.AddListener(PlayAnimations);
            }
            if (stopButton)
            {
                stopButton.onClick.AddListener(StopPlayAnimation);
            }

        }
        public void ResetAnimationItems()
        {
            animator = GetComponent<Animator>();
            animationItemList.Clear();
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = animator.runtimeAnimatorController.animationClips[i].name;
                AnimationItem item = new AnimationItem(name);
                item.animationClip = GetAnimationClipByName(name);
                animationItemList.Add(item);
                //  AddAnimationClipEndUnityEvent(item);
            }
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
        #region 暂时解决不了添加空动画，先放弃
        void AddAndPlayEmptyAnimation()
        {
            AnimationClip animationClip = new AnimationClip();
            animationClip.name = "empty";
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;
            PropertyInfo[] infos = typeof(RuntimeAnimatorController).GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                Debug.Log("infoname--" + infos[i].Name);
            }
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo animationClips = typeof(RuntimeAnimatorController).GetProperty("animationClips", flag);
            AnimationClip[] newAnimationClips = GetNewAnimationClips(animator.runtimeAnimatorController.animationClips, animationClip);
            if (animationClips == null)
            {
                Debug.Log("animationClips==null");
            }
            // animationClips.SetValue(controller,newAnimationClips ,null);
            MethodInfo setAnimationClips = animationClips.GetSetMethod(true);
            if (setAnimationClips == null)
            {
                Debug.Log("setAnimationClips==null");
            }
            setAnimationClips.Invoke(controller, newAnimationClips);
            PlayEmptyAnimation();
        }

        private void PlayEmptyAnimation()
        {
            animator.Play("empty");
        }

        AnimationClip[] GetNewAnimationClips(AnimationClip[] animationClips, AnimationClip clip)
        {
            AnimationClip[] clips = new AnimationClip[animationClips.Length + 1];
            clips[0] = clip;
            for (int i = 0; i < animationClips.Length; i++)
            {
                clips[i + 1] = animationClips[i];
            }
            return clips;
        }
        #endregion
        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// 队列播放动画，没有延迟
        /// </summary>
        /// <param name="animationNames"></param>
        public void PlayAnimationQueue(string animationNames)
        {
            Debug.Log("PlayAnimationQueue");
            string[] info = animationNames.Split(',');
            for (int i = 0; i < info.Length; i++)
            {
                if (CheckIfContainAnimationClip(info[i]))
                {
                    AnimationClipInfo clipInfo = new AnimationClipInfo(info[i], 0);
                    animationClipInfosQueue.Enqueue(clipInfo);
                }
            }
            if (animationClipInfosQueue.Count > 0)
            {
                isCompletPlayingAnimation = false;
                AnimationClipInfo clipInfo = animationClipInfosQueue.Dequeue();
                StartCoroutine(PlayOnceAnimation(clipInfo.name, clipInfo.delayTime));
            }
            //    }           
            //}
        }
        /// <summary>
        /// 队列播放动画，有延迟
        /// </summary>
        /// <param name="animationNamesAndDelays"></param>
        public void PlayAnimationQueueWithDelay(string animationNamesAndDelays)
        {
            string[] info = animationNamesAndDelays.Split('|');
            for (int i = 0; i < info.Length; i++)
            {
                string[] clipInfos = info[i].Split(',');
                string name = clipInfos[0];
                float delayTime = 0f;
                if (info.Length >= 2)
                {
                    bool isFlag = float.TryParse(clipInfos[1], out delayTime);
                    if (!isFlag)
                    {
                        delayTime = 0f;
                    }
                }
                if (CheckIfContainAnimationClip(name))
                {
                    AnimationClipInfo clipInfo = new AnimationClipInfo(name, delayTime);
                    animationClipInfosQueue.Enqueue(clipInfo);
                }
            }
            if (animationClipInfosQueue.Count > 0)
            {
                isCompletPlayingAnimation = false;
                AnimationClipInfo clipInfo = animationClipInfosQueue.Dequeue();
                StartCoroutine(PlayOnceAnimation(clipInfo.name, clipInfo.delayTime));
            }
        }
        public void PlayAnimations()
        {
            animationClipInfosQueue.Clear();
            for (int i = 0; i < animationItemList.Count; i++)
            {
                if (animationItemList[i].needPlay && CheckIfContainAnimationClip(animationItemList[i].name))
                {
                    AnimationClipInfo clipInfo = new AnimationClipInfo(animationItemList[i].name, animationItemList[i].delayTime, animationItemList[i].isLoop);
                    //  AnimationClipInfo clipInfo = new AnimationClipInfo(animationItemList[i].name, animationItemList[i].delayTime, animationItemList[i].isLoop, animationItemList[i].finishEvent);
                    animationClipInfosQueue.Enqueue(clipInfo);
                }
            }
            if (animationClipInfosQueue.Count > 0)
            {
                isCompletPlayingAnimation = false;
                AnimationClipInfo clipInfo = animationClipInfosQueue.Dequeue();
                WrapMode wrapMode = clipInfo.isLoop ? WrapMode.Loop : WrapMode.Once;
                currentPlayAnimationClipInfo = clipInfo;
                StartCoroutine(PlayOnceAnimation(clipInfo.name, clipInfo.delayTime, wrapMode));
                //  StartCoroutine(PlayOnceAnimation(clipInfo));
            }
        }
        public void PlayAnimationClipKeyEvent(int keyIndex)
        {
            Debug.LogWarning("PlayAnimationClipKeyEvent++" + keyIndex);
            if (keyIndex < 0)
            {
                return;
            }
            if (currentPlayAnimationClipInfo != null)
            {
                currentPlayAnimationClipInfo.animationClip = GetAnimationClipByName(currentPlayAnimationClipInfo.name);
                AnimationClip animationClip = currentPlayAnimationClipInfo.animationClip;
                AnimationItem animationItem = null;
                for (int i = 0; i < animationItemList.Count; i++)
                {
                    if (animationItemList[i] != null && animationItemList[i].animationClip != null && animationClip == animationItemList[i].animationClip)
                    {
                        animationItem = animationItemList[i];
                        break;
                    }
                }
                if (animationItem != null)
                {
                    List<KeyAnimation> needExcuteKeyAnimationList = new List<KeyAnimation>();
                    for (int i = 0; i < animationItem.keyAnimations.Count; i++)
                    {
                        if (animationItem.keyAnimations[i] != null && animationItem.keyAnimations[i].keyAnimationIndex == keyIndex)
                        {
                            needExcuteKeyAnimationList.Add(animationItem.keyAnimations[i]);
                        }
                    }
                    for (int i = 0; i < needExcuteKeyAnimationList.Count; i++)
                    {
                        if (needExcuteKeyAnimationList[i].keyEvent != null)
                        {
                            needExcuteKeyAnimationList[i].keyEvent.Invoke();
                            Debug.LogWarning("PlayAnimationClipKeyEvenneedExcuteKeyAnimationList++");
                        }
                    }
                }
            }
        }
        private bool CheckIfContainAnimationClip(string clipName)
        {
            animator = GetComponent<Animator>();
            bool res = false;
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                if (animator.runtimeAnimatorController.animationClips[i].name == clipName)
                {
                    res = true;
                    return res;
                }
            }
            return res;
        }

        /// <summary>
        /// 延迟播放动画
        /// </summary>
        /// <param name="animationNameAndDelayTime"></param>
        public void DelayPlayLoopAnimation(string animationNameAndDelayTime)
        {
            string[] info = animationNameAndDelayTime.Split(',');
            string animationName = info[0];
            float delayTime = 1f;
            if (info.Length >= 2)
            {
                bool isFlag = float.TryParse(info[1], out delayTime);
                if (!isFlag)
                {
                    delayTime = 1f;
                }
            }
            if (CheckIfContainAnimationClip(animationName))
            {
                StartCoroutine(PlayLoopAnimation(animationName, delayTime));
            }
        }
        public IEnumerator PlayLoopAnimation(string animationName, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(animationName);
            if (animationClip)
            {
                animator.enabled = true;
                animationClip.wrapMode = WrapMode.Loop;
                animator.Play(animationClip.name);
                animator.speed = 1;
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", animationName);
            }

        }
        /// <summary>
        /// 延迟播放动画
        /// </summary>
        /// <param name="animationNameAndDelayTime"></param>
        public void DelayPlayOnceAnimation(string animationNameAndDelayTime)
        {
            string[] info = animationNameAndDelayTime.Split(',');
            string animationName = info[0];
            float delayTime = 1f;
            if (info.Length >= 2)
            {
                bool isFlag = float.TryParse(info[1], out delayTime);
                if (!isFlag)
                {
                    delayTime = 1f;
                }
            }
            if (CheckIfContainAnimationClip(animationName))
            {
                StartCoroutine(PlayOnceAnimation(animationName, delayTime));
            }
        }
        public IEnumerator PlayOnceAnimation(string animationName, float delayTime, WrapMode wrapMode = WrapMode.Once)
        {
            Debug.Log("PlayOnceAnimation");
            yield return new WaitForSeconds(delayTime);
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(animationName);
            if (animationClip)
            {
                animator.enabled = true;
                animationClip.wrapMode = wrapMode;
                animator.Play(animationClip.name);
                animator.speed = 1;
                if (animationClip.wrapMode == WrapMode.Once)
                {
                    AddPlayAnimationFinishEvent(animationClip);
                }
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", animationName);
            }

        }
        public IEnumerator PlayOnceAnimation(AnimationClipInfo clipInfo)
        {
            Debug.Log("PlayOnceAnimation");
            yield return new WaitForSeconds(clipInfo.delayTime);
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(clipInfo.name);
            clipInfo.animationClip = animationClip;
            if (animationClip)
            {
                animator.enabled = true;
                WrapMode wrapMode = clipInfo.isLoop ? WrapMode.Loop : WrapMode.Once;
                animationClip.wrapMode = wrapMode;
                animator.Play(animationClip.name);
                animator.speed = 1;
                if (animationClip.wrapMode == WrapMode.Once)
                {
                    AddPlayAnimationEndEvent(clipInfo);
                }
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", clipInfo.name);
            }

        }
        /// <summary>
        /// 播放一次动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayOnceAnimation(string animationName)
        {
            StopPlayAnimation();
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(animationName);
            if (animationClip)
            {
                animator.enabled = true;
                animationClip.wrapMode = WrapMode.Once;
                animator.speed = 1;
                animator.recorderStartTime = 0;
                animator.Play(animationClip.name);              
                AddPlayAnimationFinishEvent(animationClip);
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", animationName);
            }

        }
       
        public void PlayOnceAnimationRerverse(string animationName)
        {
            StopPlayAnimation();
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(animationName);
            if (animationClip)
            {
                animator.enabled = true;
                animationClip.wrapMode = WrapMode.Once;
                animator.StartPlayback();
                animator.speed = -1;              
                animator.Play(animationClip.name,0,1);
               // AddPlayAnimationFinishEvent(animationClip);
               
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", animationName);
            }

        }

        /// <summary>
        /// 循环播放动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayLoopAnimation(string animationName)
        {
            CancelInvoke("StopPlayAnimation");
            AnimationClip animationClip = GetAnimationClipByName(animationName);
            if (animationClip)
            {
                animator.enabled = true;
                animationClip.wrapMode = WrapMode.Loop;
                animator.Play(animationClip.name);
                animator.speed = 1;
            }
            else
            {
                Debug.LogErrorFormat("动画片段{0}不存在", animationName);
            }
        }
        void AddPlayAnimationFinishEvent(AnimationClip animationClip)
        {
            if (CheckContainFinishEvent(animationClip))
            {
                return;
            }
            AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.time = animationClip.length;
            animationEvent.functionName = "PlayAnimationFinishEvent";
            animationEvent.objectReferenceParameter = animationClip;
            animationClip.AddEvent(animationEvent);
        }
        void AddAnimationClipEndUnityEvent(AnimationItem animationItem)
        {
            AnimationClip animationClip = GetAnimationClipByName(animationItem.name);
            AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.time = animationClip.length;
            animationEvent.functionName = "BindAnimationClipEndUnityEvent";
            animationEvent.objectReferenceParameter = animationClip;
            animationClip.AddEvent(animationEvent);
        }

        void BindAnimationClipEndUnityEvent(UnityEngine.Object param)
        {
            Debug.LogError("BindAnimationClipEndUnityEvent");
            AnimationClip animationClip = param as AnimationClip;
            AnimationItem animationItem = null;
            for (int i = 0; i < animationItemList.Count; i++)
            {
                if (animationItemList[i].animationClip && animationItemList[i].animationClip == animationClip)
                {
                    animationItem = animationItemList[i];
                }
            }
            if (animationItem != null)
            {
                if (animationItem.finishEvent != null)
                {
                    Debug.LogError("animationItem.finishEvent.Invoke()");
                    animationItem.finishEvent.Invoke();
                }
            }
        }

        void AddPlayAnimationEndEvent(AnimationClipInfo clipInfo)
        {
            if (CheckContainFinishEvent(clipInfo.animationClip))
            {
                return;
            }
            AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.time = clipInfo.animationClip.length;
            animationEvent.functionName = "PlayAnimationEndEvent";
            animationEvent.objectReferenceParameter = clipInfo.animationClip;
            clipInfo.animationClip.AddEvent(animationEvent);
        }

        bool CheckContainFinishEvent(AnimationClip animationClip)
        {
            bool res = false;
            AnimationEvent[] events = animationClip.events;
            if (events == null)
            {
                return res;
            }
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].functionName == "PlayAnimationFinishEvent")
                {
                    res = true;
                    return res;
                }
            }
            return res;
        }
        void PlayAnimationFinishEvent(UnityEngine.Object param)
        {
            AnimationClip animationClip = param as AnimationClip;

            //if (currentPlayAnimationClipInfo != null)
            //{
            //    UnityEvent finishEvent = currentPlayAnimationClipInfo.finishEvent;
            //      finishEvent?.Invoke();
            //}
            AnimationItem animationItem = null;
            for (int i = 0; i < animationItemList.Count; i++)
            {
                if (animationItemList[i].animationClip && animationItemList[i].animationClip == animationClip)
                {
                    animationItem = animationItemList[i];
                }
            }
            if (animationItem != null)
            {
                if (animationItem.finishEvent != null)
                {
                    //  Debug.LogError("animationItem.finishEvent.Invoke");
                    animationItem.finishEvent.Invoke();
                }
            }
            if (animationClip.wrapMode == WrapMode.Once)
            {
                animator.enabled = false;
                if (animationClipInfosQueue.Count > 0)
                {
                    AnimationClipInfo clipInfo = animationClipInfosQueue.Dequeue();
                    currentPlayAnimationClipInfo = clipInfo;
                    WrapMode wrapMode = clipInfo.isLoop ? WrapMode.Loop : WrapMode.Once;
                    StartCoroutine(PlayOnceAnimation(clipInfo.name, clipInfo.delayTime, wrapMode));
                }
            }

        }
        void PlayAnimationEndEvent(UnityEngine.Object param)
        {
            AnimationClip animationClip = param as AnimationClip;
            //if (currentPlayAnimationClipInfo!=null)
            //{
            //    UnityEvent finishEvent = currentPlayAnimationClipInfo.finishEvent;
            //  //  finishEvent?.Invoke();
            //}

            if (animationClip.wrapMode == WrapMode.Once)
            {
                animator.enabled = false;
                if (animationClipInfosQueue.Count > 0)
                {
                    AnimationClipInfo clipInfo = animationClipInfosQueue.Dequeue();
                    currentPlayAnimationClipInfo = clipInfo;
                    WrapMode wrapMode = clipInfo.isLoop ? WrapMode.Loop : WrapMode.Once;
                    StartCoroutine(PlayOnceAnimation(clipInfo));
                }
            }

        }
        /// <summary>
        /// 停止播放动画
        /// </summary>
        public void StopPlayAnimation()
        {
            animator.StopPlayback();
            animator.enabled = false;
            animationClipInfosQueue.Clear();
            StopAllCoroutines();
         //   Debug.Log("StopPlayAnimation");
        }
        AnimationClip GetAnimationClipByName(string animationName)
        {
            AnimationClip clip = null;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == animationName)
                {
                    clip = clips[i];
                    return clip;
                }
            }
            return clip;
        }
    }
}
