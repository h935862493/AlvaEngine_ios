using Alva.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Alva.Runtime.Components
{
    [AddComponentMenu("myName/myProduct/v1/QuickSettings")]
    public class LabelAgent : Agent
    {
        bool isInit = false;
        TextMeshProUGUI textMesh;
        [ConditionalHide("is3D", true)]
        public bool ShowBillboard = false;
        Camera camera;
        [SerializeField, HideInInspector]
        bool is3D = true;
        private void Awake()
        {
            MyInit();
        }
        void LateUpdate()
        {
            if (ShowBillboard && is3D)
            {
                if (camera == null)
                {
                    camera = FindObjectOfType<Camera>();
                    if (camera == null)
                    {
                        return;
                    }
                }
                if (camera)
                {
                    // �����ҵĽ�ɫ�����UI�������෴�ģ����ֱ����LookAt()����Ҫ��ÿ��UIԪ����ת������
                    // Ϊ�˼򵥣��������������������ʵ������һ��������ת�����Լ����Ϊ��������������
                    transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
                 //  transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                }
            }
        }
        public void MyInit()
        {
            if (isInit)
            {
                return;
            }
            textMesh = GetComponent<TextMeshProUGUI>();
            isInit = true;
        }
        public void SetContent(string content)
        {
            textMesh.text = content;
        }
        public string GeContent()
        {
            return textMesh.text;
        }
        public override bool GetShowStyle()
        {
            return true;
        }
        public override void Init()
        {
            defaultElement = "style1";
            is3D = componentPrefabType == ComponentPrefabType.Label;
#if UNITY_EDITOR
            Toolbox.TimerTools.TimerInvoke(0.2F, ShrinkageAllExcludeAgent);
#endif
        }
        public override string[] GetStyle()
        {
            return new string[1] { "style1" };
        }
        public override void OnStyleValueSelected(object value)
        {
            Debug.Log(value.ToString());
        }
    }
}

