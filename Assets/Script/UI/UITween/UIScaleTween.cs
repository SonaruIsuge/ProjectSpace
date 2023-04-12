using DG.Tweening;
using UnityEngine;


public class UIScaleTween : UITweenBase
{
    private RectTransform targetUI;
    [SerializeField] private float from;
    [SerializeField] private float to;
    

    private void Awake()
    {
        targetUI = GetComponent<RectTransform>();
        InitTweener();
    }


    public override void ResetToBegin()
    {
        targetUI.localScale = Vector3.one * from;
    }


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);
        
        tweener = targetUI.DOScale(reverse ? from : to, duration).SetEase(easeType);
        tweener.Pause();
    }
}
