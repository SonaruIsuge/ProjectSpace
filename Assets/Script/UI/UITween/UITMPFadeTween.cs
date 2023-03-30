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
        var textColor = targetText.color;

        textColor.a = reverse ? from : to;
        tweener = targetText.DOColor(textColor, duration);
        tweener.Pause();
    }
}
