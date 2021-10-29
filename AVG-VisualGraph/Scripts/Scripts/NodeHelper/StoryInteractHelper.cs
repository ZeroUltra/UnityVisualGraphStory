using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 互动
/// </summary>
public  class StoryInteractHelper : MonoBehaviour
{
    /// <summary>
    /// 互动结束
    /// </summary>
    public event System.Action OnInteractEndEvent;

    /// <summary>
    /// 设置动画结束
    /// </summary>
    public void SetInteractEnd()
    {
        OnInteractEndEvent?.Invoke();
    }
    public void SetInteractDelayEnd(float delay)
    {
        Timer.Register(delay, () => OnInteractEndEvent?.Invoke());
    }
}
