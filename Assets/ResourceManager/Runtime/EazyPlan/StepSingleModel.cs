using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alva.EazyPlan
{
    public class StepSingleModel : MonoBehaviour
    {
        [Header("²½ÖèID")]
        [SerializeField]
        public string id;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetStepID(string _ID)
        {
            id = _ID;
        }
        public void OnSetRotate()
        {
            if (transform .childCount>0)
            {
                transform.GetChild(0).localEulerAngles =new Vector3 (90,0,0);
            }
        }
    }
}
