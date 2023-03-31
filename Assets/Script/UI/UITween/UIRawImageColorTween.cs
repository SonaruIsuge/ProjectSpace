
using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRawImageColorTween : UITweenBase
{
    private RawImage targetImg;

    [SerializeField] private Color from;
    [SerializeField] private Color to;


    private void Awake()
    {
        targetImg = GetComponent<RawImage>();
        InitTweener();
    }


    public override void ResetToBegin()
    {
        targetImg.color = from;
    }


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);
        
        tweener = targetImg.DOColor(reverse ? from : to, duration);
        tweener.Pause();
    }
}
