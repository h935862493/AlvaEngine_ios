using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlvaEventIterator : MonoBehaviour
{
    [SerializeField]
    public List<Objects> list = new List<Objects>();
    [System.Serializable]
    public class Objects
    {
        [SerializeField]
        public float time = 0.0f;
        [SerializeField]
        public List<UnityEngine.Events.UnityEvent> list = new List<UnityEngine.Events.UnityEvent>();
    }

    private IEnumerator OnIterator()
    {
        foreach (var item in list)
        {
            yield return new WaitForSeconds(item.time);
            foreach (UnityEngine.Events.UnityEvent m_event in item.list)
            {
                m_event?.Invoke();
            }
        }
    }
    public void OnExecute()
    {
        StartCoroutine(OnIterator());
    }
    public void OnStop()
    {
        StopCoroutine(OnIterator());
    }
}
