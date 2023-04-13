using System;
using System.Threading.Tasks;
using UnityEngine;


public class NoTimeHint : MonoBehaviour
{
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private UICanvasGroupTween hintPanelTween;
    [SerializeField] private UICanvasGroupTween hintTween;
    [SerializeField] private float flashTimes;
    
    private bool isFlash;


    private void Start()
    {
        hintPanel.SetActive(false);
        hintPanelTween.ResetToBegin();
        hintTween.ResetToBegin();
    }


    // 0 to 1 and 1 to 0 as a cycle
    public async void Flash()
    {
        if(isFlash) return;
        isFlash = true;
        
        hintPanel.SetActive(true);
        
        await FadeIn(hintPanelTween);
        
        for (var i = 0; i < flashTimes; i++)
        {
            await FadeIn(hintTween);
            await FadeOut(hintTween);
        }
        
        await FadeOut(hintPanelTween);
        hintPanel.SetActive(false);
        
        isFlash = false;
    }


    private async Task FadeIn(UITweenBase tween)
    {
        var complete = false;
        tween.TweenTo(() => complete = true );
        while (!complete) await Task.Yield();
    }


    private async Task FadeOut(UITweenBase tween)
    {
        var complete = false;
        tween.TweenFrom(() => complete = true );
        while (!complete) await Task.Yield();
    }
}
