using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class UITMPFadeTween : MonoBehaviour, IUITween
{
    public RectTransform TargetObj { get; private set; }
    public Tweener Tweener { get; private set; }
    private TMP_Text targetText;
    
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Delay { get; private set; }

    [Range(0, 1)] [SerializeField] private float from;
    [Range(0, 1)] [SerializeField] private float to;
    
    [field: SerializeField] public Ease EaseType { get; private set; }


    private void Awake()
    {
        TargetObj = GetComponent<RectTransform>();
        targetText = GetComponent<TMP_Text>();
        
        InitTweener();
    }


    public void ResetToBegin()
    {
        var textColor = targetText.color;
        textColor.a = from;
        targetText.color = textColor;
    }

    
    public async void TweenTo()
    {
        InitTweener();
        await Task.Delay((int)(Delay * 1000));
        Tweener.Play();
    }


    public async void TweenFrom()
    {
        InitTweener(reverse: true);
        await Task.Delay((int)(Delay * 1000));
        Tweener.Play();
    }
    
    
    private void InitTweener(bool reverse = false)
         {
             var textColor = targetText.color;
     
             textColor.a = reverse ? from : to;
             Tweener = targetText.DOColor(textColor, Duration);
             Tweener.Pause();
         }
}
