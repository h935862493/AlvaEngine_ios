using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlvaEventFSM : MonoBehaviour
{
    [SerializeField]
    public int currentState = 0;
    [SerializeField]
    public List<Objects> list = new List<Objects>();
    [System.Serializable]
    public class Objects
    {
        [SerializeField]
        public float state = 0.0f;
        [SerializeField]
        public List<UnityEngine.Events.UnityEvent> list = new List<UnityEngine.Events.UnityEvent>();
    }

    private void OnFSM(int state)
    {
        foreach (var item in list)
        {
            if (state == item.state)
            {
                foreach (UnityEngine.Events.UnityEvent m_event in item.list)
                {
                    m_event?.Invoke();
                }
            }
        }
    }
    public void OnExecute(int state)
    {
        OnFSM(state);
    }
    public void OnExecute()
    {
        OnFSM(currentState);
    }
    public void OnSetState(int state)
    {
        currentState = state;
    }
}
