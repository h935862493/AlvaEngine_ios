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
        [Header("����ID")]
        [SerializeField]
        public string id;
        [Header("�������")]
        [SerializeField]
        public string title;
        [Header("��������")]
        [SerializeField]
        public string description;
        [Header("����ͼƬ�б�")]
        [SerializeField]
        public List<Texture2D> stepImages=new List<Texture2D> ();
        [Header("������Ƶ�б�")]
        [SerializeField]
        public List<VideoClip> stepVideos = new List<VideoClip>();
        [Header("����˳��")]
        [SerializeField]
        public int stepOrder =0; //��һ��0���ڶ���1��ֹ˳�����
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
