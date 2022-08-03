using Alva.Runtime.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Recognition
{
    public class ModelRecognition : DefaultTrackableEventHandler
    {
        //[SerializeField]
        public string p_path;
        public GameObject dataModel;

        public List<GameObject> showGoChild = new List<GameObject>();
        void Awake()
        {
            p_path = path;
         
            dataModel = transform.Find("GLTF").gameObject;

            if (dataModel)
                dataModel.SetActive(false);

            print("////transform.childCount:" + transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    showGoChild.Add(transform.GetChild(i).gameObject);
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

#if UNITY_ANDROID || UNITY_IOS
            Instantiate(Resources.Load<GameObject>("AlvaCore/AlvaCore_ModelTarget"));
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
            foreach (var item in showGoChild)
            {
                item.SetActive(true);
            }
        }
    }
}

