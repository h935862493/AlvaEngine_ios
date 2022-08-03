using Alva.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Alva.EazyPlan
{
    [RequireComponent(typeof(AnimationPlayer))]
    [RequireComponent(typeof(Animator))]
    public class OperationStepModels : MonoBehaviour
    {
        [Header("步骤ID")]
        [SerializeField]
        public string id;
        [Header("步骤标题")]
        [SerializeField]
        public string title;
        [Header("步骤描述")]
        [SerializeField]
        public string description;
        [Header("步骤图片列表")]
        [SerializeField]
        public List<Texture2D> stepImages=new List<Texture2D> ();
        [Header("步骤视频列表")]
        [SerializeField]
        public List<VideoClip> stepVideos = new List<VideoClip>();
        [Header("步骤顺序")]
        [SerializeField]
        public int stepOrder =0; //第一步0，第二步1防止顺序错乱
        Animator animator;
        AnimationPlayer animationPlayer;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void PlayAnimation()
        {
            if (animator==null)
            {
                animator = GetComponent<Animator>();
            }
            if (animationPlayer == null)
            {
                animationPlayer = transform.gameObject.GetOrAddComponent<AnimationPlayer>();
            }
            if (animator != null&& animator.runtimeAnimatorController&& animator.runtimeAnimatorController.animationClips!=null)
            {            
                if (animator.runtimeAnimatorController.animationClips.Length>0)
                {
                    animationPlayer.PlayOnceAnimation(animator.runtimeAnimatorController.animationClips[0].name);
                }   
            }
        }
        public void SetStepID(string _ID)
        {
            id = _ID;
        }
        public void SetStepTitle(string _title)
        {
            title = _title;
        }
        public void SetStepDescription(string _description)
        {
            description = _description;
        }
        public void SetStepImages(List<Texture2D> _stepImages) {
            stepImages = _stepImages;
        }
        public void SetStepVideos(List<VideoClip> _stepVideos)
        {
            stepVideos = _stepVideos;
        }
        public void SetStepOrders(int _stepOrder)
        {
            stepOrder  = _stepOrder;
        }
   
    }
}
