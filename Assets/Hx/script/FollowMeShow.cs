using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FollowMeShow : MonoBehaviour
{
    public UnityEvent enableEvent, disableEvent;
    private void OnEnable()
    {
        if (enableEvent != null)
        {
            enableEvent.Invoke();
        }
    }

    private void OnDisable()
    {
        if (disableEvent != null)
        {
            disableEvent.Invoke();
        }
    }
}
