
using System;
using DG.Tweening;
using UnityEngine;

public class UICanvasGroupTween : UITweenBase
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float from;
    [SerializeField] private float to;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    
    public override void ResetToBegin()
    {
        canvasGroup.alpha = from;
    }


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);

        tweener = canvasGroup.DOFade(reverse ? from : to, duration);
        tweener.Pause();
    }
}
