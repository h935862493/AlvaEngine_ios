using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alva.Common
{
    /// <summary>
    /// 用户运行数据
    /// </summary>
    public class PlayerData : MonoBehaviour
    {
        private PlayerData() { }
        private static PlayerData _instance;
        private static object _lock = new object();
        public static PlayerData Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = FindObjectOfType<PlayerData>();
                    if (_instance == null)
                    {
                        UnityEngine.GameObject objData = new GameObject();
                        _instance = objData.AddComponent<PlayerData>();
                        objData.name = "PlayerData";
                        objData.hideFlags = UnityEngine.HideFlags.NotEditable | UnityEngine.HideFlags.HideInHierarchy | UnityEngine.HideFlags.HideInInspector;
                    }
                }
            }
            return _instance;
        }

        [SerializeField]
        public double LicenseLimitDays = 11;
        /// <summary>
        /// 平台信息--接收调用
        /// </summary>
        /// <param name="unityMessageInfoJson">调用参数Json</param>
        public void UnityMessageProxy(string unityMessageInfoJson)
        {
            var unityMessageInfo = JsonUtility.FromJson<UnityMessageInfo>(unityMessageInfoJson);
            if (unityMessageInfo != null)
            {
                System.Collections.Generic.IEnumerable<Alva.Runtime.Components.Agent> agents = Resources.FindObjectsOfTypeAll<Alva.Runtime.Components.Agent>().Where(agent => agent.id.Equals(unityMessageInfo.GameObjectIDs));
                foreach (var agent in agents)
                {
                    agent.ExternalCall(unityMessageInfo.Args,unityMessageInfo.MethodName);
                }
            }

        }

        public delegate void SendRainbowSyncInfo(string syncMessage);
        public event SendRainbowSyncInfo SendRainbowSyncInfoEvent;
        public void SendSyncInfo(string syncMessage)
        {
            SendRainbowSyncInfoEvent?.Invoke(syncMessage);
        }

        public void OnClearTemplate()
        {
            UnityEngine.GameObject Template = UnityEngine.GameObject.FindGameObjectWithTag("Template");
            if (Template)
            {
                DestroyImmediate(Template);
            }
            else
            {
                return;
            }
            OnClearTemplate();
        }

        /// <summary>
        /// 机型适配
        /// </summary>
        public void OnDeviceModel()
        {
            UnityEngine.RectTransform TemplateTop = UnityEngine.GameObject.FindGameObjectWithTag("Template").transform.GetChild(0).transform.Find("Top").GetComponent<RectTransform>();
            Debug.Log(TemplateTop.gameObject.name);
            Debug.Log(SystemInfo.deviceModel);
            switch (SystemInfo.deviceModel)
            {
                case "H470SD4 (ONDA)":
                    TemplateTop.anchoredPosition = new Vector2(TemplateTop.anchoredPosition.x, TemplateTop.anchoredPosition.y - TemplateTop.sizeDelta.y);
                    break;
                default:
                    break;
            }
        }
        
        [SerializeField]
        public string[] jt_path;
        [SerializeField]
        public string prefab_directory;
    }
    /// <summary>
    /// 交互信息
    /// </summary>
    public class UnityMessageInfo
    {
        public string GameObjectIDs;
        public string MethodName;
        public string[] Args;

        public UnityMessageInfo(string id, string methodName, string[] args)
        {
            GameObjectIDs = id;
            MethodName = methodName;
            Args = args;
        }

        public string MessageToJson()
        {
            return Alva.Runtime.Toolbox.JsonTools.SerializeObject(this);
        }
    }


}
