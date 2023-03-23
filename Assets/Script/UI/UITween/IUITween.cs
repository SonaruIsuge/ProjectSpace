

using DG.Tweening;
using UnityEngine;

public interface IUITween
{
    RectTransform TargetObj { get; }
    Tweener Tweener { get; }
    
    float Duration { get; }
    float Delay { get; }

    Ease EaseType { get; }

    void Initial();
    void Tween();
}
