using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SnapAxis
{
    x,
    y,
    z
}

public enum AxisUI
{
    x,
    y
}

/// <summary>
/// 运动特效基类
/// </summary>
public class SportEffectsBase : EffectsBase
{
    /// <summary>
    /// 执行
    /// </summary>
    public virtual void Execute()
    {
        this.enabled = true;
        ExecuteEvent?.Invoke();
    }

    /// <summary>
    /// 停止
    /// </summary>
    public virtual void StopExecute()
    {
        this.enabled = true;
        EndEvent?.Invoke();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public virtual void Recover()
    {
        RecoverEvent?.Invoke();
    }
}
