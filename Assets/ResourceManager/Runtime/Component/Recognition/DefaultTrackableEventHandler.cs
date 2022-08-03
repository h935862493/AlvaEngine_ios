using Alva.Common;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Alva.Runtime.Components
{
    public class DefaultTrackableEventHandler : Agent
    {
        [HideInInspector]
        public string path;

        [HideInInspector]
        public string type;

        //public GameObject dataModel;

        public UnityEvent OnTargetFound;
        public UnityEvent OnTargetLost;

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    OnTrackingFound();
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    OnTrackingLost();
            //}
        }
        public virtual void SetDisableModelScaling(bool disableModelScaling)
        {

        }
#if UNITY_EDITOR
        public void AddListener(object target, string name)
        {
            UnityAction unityAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, "Execute");
            UnityEventTools.AddVoidPersistentListener(OnTargetFound, unityAction);
        }
#endif
        public virtual void Found()
        {

        }

        protected virtual void OnTrackingFound()
        {
            if (OnTargetFound != null)
                OnTargetFound.Invoke();
            //平台交互信息同步---发送
            //PlayerData.Instance().SendSyncInfo(new UnityMessageInfo(id, "ExternalCall", new string[0]).MessageToJson());
        }
        
        protected virtual void OnTrackingLost()
        {
            if (OnTargetLost != null)
                OnTargetLost.Invoke();
            //平台交互信息同步---发送
            //PlayerData.Instance().SendSyncInfo(new UnityMessageInfo(id, "ExternalCall", new string[0]).MessageToJson());
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI(SerializedObject so)
        {
            base.OnInspectorGUI(so);
            OnDrawing(so, "OnTargetFound");
            OnDrawing(so, "OnTargetLost");
        }
#endif
        public override void ExternalCall(string[] parameter, string methodName)
        {
            base.ExternalCall(parameter);
            switch (methodName)
            {
                case "OnTrackingFound":
                    if (OnTargetFound != null)
                        OnTargetFound.Invoke();
                    break;
                case "OnTrackingLost":
                    if (OnTargetLost != null)
                        OnTargetLost.Invoke();
                    break;
            }
        }
    }

}

