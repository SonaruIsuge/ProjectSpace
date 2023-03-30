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
    public virtual void TweenTo() { }

    /// <summary>
    /// Tween to -> from
    /// </summary>
    public virtual void TweenFrom() { }


    [ContextMenu("Debug Tween")]
    public void DebugTween()
    {
        ResetToBegin();
        TweenTo();
    }
}
