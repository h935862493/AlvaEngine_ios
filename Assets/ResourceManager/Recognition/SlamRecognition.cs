using Alva.Runtime.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Recognition
{
    public class SlamRecognition : DefaultTrackableEventHandler
    {
        public bool DisableModelScaling = false;
        public bool DisableModelMoving = false;
        public bool DisableModelRotating = false;

        private bool awakeDisableModelScaling;
        public List<GameObject> showGoChild = new List<GameObject>();
        void Awake()
        {
            awakeDisableModelScaling = DisableModelScaling;
            Transform SlamPlane = transform.Find("SlamPlane");
            if (SlamPlane)
            {
                if (SlamPlane.GetComponent<MeshFilter>())
                    Destroy(SlamPlane.GetComponent<MeshFilter>());
                if (SlamPlane.GetComponent<MeshRenderer>())
                    Destroy(SlamPlane.GetComponent<MeshRenderer>());
                if (SlamPlane.GetComponent<Collider>())
                    Destroy(SlamPlane.GetComponent<Collider>());
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    showGoChild.Add(transform.GetChild(i).gameObject);
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

#if UNITY_ANDROID || UNITY_IOS
            Instantiate(Resources.Load<GameObject>("AlvaCore/AlvaCore_PlaneTarget"));
#endif

        }

        protected override void OnTrackingFound()
        {

            base.OnTrackingFound();
        }

        protected override void OnTrackingLost()
        {
            base.OnTrackingLost();
        }

        public override void Found()
        {
            print("识别到啦------editor found 方法");
            foreach (var item in showGoChild)
            {
                item.SetActive(true);
            }
        }

        public override void SetDisableModelScaling(bool disableModelScaling)
        {
            if (disableModelScaling == true)
            {
                DisableModelScaling = true;
            }
            else
            {
                if (awakeDisableModelScaling == false)
                {
                    DisableModelScaling = false;
                }
            }
        }
    }
}

