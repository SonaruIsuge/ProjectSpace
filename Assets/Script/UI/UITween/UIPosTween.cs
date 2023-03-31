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


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);
        
        tweener = targetUI.DOAnchorPos(reverse ? From : To, duration).SetEase(easeType);
        tweener.Pause();
    }
}
