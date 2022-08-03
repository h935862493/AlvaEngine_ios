using Alva.Runtime.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Alva.Recognition
{
    public class ImageRecognition : DefaultTrackableEventHandler
    {
        public string p_path;

        //public List<GameObject> showChild = new List<GameObject>();
        Dictionary<GameObject, bool> showChild = new Dictionary<GameObject, bool>();
        public Transform editorRoot;
        public Transform folloeMe;
        void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            p_path = path;

            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            editorRoot = transform.Find("Quad");
            if (editorRoot)
            {
                GameObject go = new GameObject("FollowMe--");
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = editorRoot.localScale;
                transform.parent = go.transform;
                folloeMe = go.transform;

                if (editorRoot.GetComponent<Collider>())
                    Destroy(editorRoot.GetComponent<Collider>());
                if (editorRoot.GetComponent<Renderer>())
                    Destroy(editorRoot.GetComponent<Renderer>());
                if (editorRoot.GetComponent<MeshFilter>())
                    Destroy(editorRoot.GetComponent<MeshFilter>());

                //folloeMe.localScale *= 82.84272f;
                folloeMe.localScale *= 62;

                if (transform.childCount > 1)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var g = transform.GetChild(i).gameObject;
                        showChild.Add(g, g.activeSelf);
                        g.SetActive(false);
                    }

                }
            }

#if UNITY_ANDROID || UNITY_IOS
            Instantiate(Resources.Load<GameObject>("AlvaCore/AlvaCore_ImageTarget"));
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
            print("ʶ------editor found");
            foreach (var item in showChild.Keys)
            {
                item.SetActive(showChild[item]);
            }
        }
    }
}

