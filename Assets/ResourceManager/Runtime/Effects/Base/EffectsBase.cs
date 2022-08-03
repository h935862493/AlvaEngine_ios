using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 动画特效基类
/// </summary>
public class EffectsBase : MonoBehaviour
{
    /// <summary>
    /// 执行时触发
    /// </summary>
    [HideInInspector]
    public UnityEvent ExecuteEvent;
    /// <summary>
    /// 恢复时触发
    /// </summary>
    [HideInInspector]
    public UnityEvent RecoverEvent;
    /// <summary>
    /// 结束时触发
    /// </summary>
    public UnityEvent EndEvent;

    /// <summary>
    /// 中断
    /// </summary>
    public virtual void Interrupt() { }
}
