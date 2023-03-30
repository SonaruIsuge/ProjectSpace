

using DG.Tweening;
using UnityEngine;

public interface IUITween
{
    RectTransform TargetObj { get; }
    Tweener Tweener { get; }
    
    float Duration { get; }
    float Delay { get; }

    Ease EaseType { get; }


    void ResetToBegin();

    /// <summary>
    /// Tween from -> to
    /// </summary>
    void TweenTo();
    
    /// <summary>
    /// Tween to -> from
    /// </summary>
    void TweenFrom();
}
