using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ������Ч����
/// </summary>
public class EffectsBase : MonoBehaviour
{
    /// <summary>
    /// ִ��ʱ����
    /// </summary>
    [HideInInspector]
    public UnityEvent ExecuteEvent;
    /// <summary>
    /// �ָ�ʱ����
    /// </summary>
    [HideInInspector]
    public UnityEvent RecoverEvent;
    /// <summary>
    /// ����ʱ����
    /// </summary>
    public UnityEvent EndEvent;

    /// <summary>
    /// �ж�
    /// </summary>
    public virtual void Interrupt() { }
}
