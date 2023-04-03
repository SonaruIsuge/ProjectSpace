using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


public class UIPosTween : UITweenBase
{
    private RectTransform targetUI;
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to;
    

    private void Awake()
    {
        targetUI = GetComponent<RectTransform>();
        InitTweener();
    }


    public override void ResetToBegin()
    {
        targetUI.anchoredPosition = from;
    }


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);
        
        tweener = targetUI.DOAnchorPos(reverse ? from : to, duration).SetEase(easeType);
        tweener.Pause();
    }
}
