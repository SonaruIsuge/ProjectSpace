using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class UIPosTween : UITweenBase
{
    private RectTransform targetUI;
    [field: SerializeField] public Vector3 From { get; private set; }
    [field: SerializeField] public Vector3 To { get; private set; }
    

    private void Awake()
    {
        targetUI = GetComponent<RectTransform>();
        InitTweener();
    }


    public override void ResetToBegin()
    {
        targetUI.anchoredPosition = From;
    }
    

    public override async void TweenTo()
    {
        InitTweener();
        await Task.Delay((int)(delay * 1000));
        tweener.Play();
    }
    
    
    public override async void TweenFrom()
    {
        InitTweener(reverse: true);
        await Task.Delay((int)(delay * 1000));
        tweener.Play();
    }


    private void InitTweener(bool reverse = false)
    {
        tweener = targetUI.DOAnchorPos(reverse ? From : To, duration).SetEase(easeType);
        tweener.Pause();
    }
}
