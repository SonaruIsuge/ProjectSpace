using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class UITweenBase : MonoBehaviour
{
    protected Tweener tweener;

    [SerializeField] protected float duration;
    [SerializeField] protected float delay;
    [SerializeField] protected Ease easeType;

    /// <summary>
    /// Force target ui to the from state
    /// </summary>
    public virtual void ResetToBegin() { }

    
    /// <summary>
    /// Tween from -> to
    /// </summary>
    public virtual async void TweenTo(Action onComplete = null)
    {
        InitTweener();
        await Task.Delay((int)(delay * 1000));
        tweener.Play().OnComplete(() => onComplete?.Invoke());
    }

    
    /// <summary>
    /// Tween to -> from
    /// </summary>
    public virtual async void TweenFrom(Action onComplete = null)
    {
        InitTweener(reverse: true);
        await Task.Delay((int)(delay * 1000));
        tweener.Play().OnComplete(() => onComplete?.Invoke());
    }


    protected virtual void InitTweener(bool reverse = false)
    {
        tweener?.Kill();
    }


    [ContextMenu("Debug Tween")]
    public void DebugTween()
    {
        ResetToBegin();
        TweenTo();
    }
}
