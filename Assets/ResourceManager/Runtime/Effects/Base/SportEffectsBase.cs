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
/// �˶���Ч����
/// </summary>
public class SportEffectsBase : EffectsBase
{
    /// <summary>
    /// ִ��
    /// </summary>
    public virtual void Execute()
    {
        this.enabled = true;
        ExecuteEvent?.Invoke();
    }

    /// <summary>
    /// ֹͣ
    /// </summary>
    public virtual void StopExecute()
    {
        this.enabled = true;
        EndEvent?.Invoke();
    }

    /// <summary>
    /// �ָ�
    /// </summary>
    public virtual void Recover()
    {
        RecoverEvent?.Invoke();
    }
}
