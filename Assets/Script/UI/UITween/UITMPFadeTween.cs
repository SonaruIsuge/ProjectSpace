using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class UITMPFadeTween : UITweenBase
{
    
    private TMP_Text targetText;

    [Range(0, 1)] [SerializeField] private float from;
    [Range(0, 1)] [SerializeField] private float to;


    private void Awake()
    {
        targetText = GetComponent<TMP_Text>();
        InitTweener();
    }


    public override void ResetToBegin()
    {
        var textColor = targetText.color;
        textColor.a = from;
        targetText.color = textColor;
    }


    protected override void InitTweener(bool reverse = false)
    {
        base.InitTweener(reverse);
        
        var textColor = targetText.color;
        textColor.a = reverse ? from : to;
        tweener = targetText.DOColor(textColor, duration);
        tweener.Pause();
    }
}
