using System.Linq;
using UnityEngine;

namespace Alva.Runtime.Android
{
    public class UnityEventCallback : AndroidJavaProxy
    {
        public UnityEventCallback() : base("com.alvasystems.rainbow.unity.eventhander.UnityEventListener") { }
        public void androidSendMessage(string[] parameter)
        {
            Debug.Log("alva editor " + parameter[0]);
            System.Collections.Generic.IEnumerable<Alva.Runtime.Components.Agent> agents = Resources.FindObjectsOfTypeAll<Alva.Runtime.Components.Agent>().Where(agent => agent.id == parameter[0]);
            foreach (var agent in agents)
            {
                agent.ExternalCall(parameter);
            }
        }
    }
    public class AndroidInterface
    {

        private static readonly AndroidInterface _instance = new AndroidInterface();
        private AndroidJavaObject AJO;
        public static AndroidInterface Instance
        {
            get
            {
                return _instance;
            }
        }
        public bool IsRainbow;
        private AndroidInterface()
        {
            if (IsRainbow)
            {
                RainbowInit();
            }
        }

        public void RainbowInit()
        {
            var context = new AndroidJavaObject("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            UnityEventCallback callback = new UnityEventCallback();
            AJO = new AndroidJavaObject("com.alvasystems.rainbow.unity.rendering.NewGLTexture", new object[] { callback });
        }

        public void SendUnityEventMessage(string[] parameter)
        {
            if (AJO != null)
            {
                AJO.Call("UnityEventMessage", parameter);
            }
        }
    }
}


