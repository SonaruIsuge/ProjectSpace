using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class UIPosTween : MonoBehaviour, IUITween
{
    public RectTransform TargetObj { get; private set; }
    public Tweener Tweener { get; private set; }
    
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Delay { get; private set; }
    [field: SerializeField] public Vector3 From { get; private set; }
    [field: SerializeField] public Vector3 To { get; private set; }
    [field: SerializeField] public Ease EaseType { get; private set; }

    private void Awake()
    {
        TargetObj = GetComponent<RectTransform>();

        Initial();
    }


    public void Initial()
    {
        TargetObj.anchoredPosition = From;
        Tweener = TargetObj.DOAnchorPos(To, Duration).SetEase(EaseType);
        Tweener.Pause();
    }
    

    public async void Tween()
    {
        await Task.Delay((int)(Delay * 1000));
        Tweener.Play();
    }
}
