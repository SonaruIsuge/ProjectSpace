using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameStartView : MonoBehaviour
{
    [SerializeField] private RawImage leftDoor;
    [SerializeField] private RawImage rightDoor;
    [SerializeField] private RawImage ready;
    [SerializeField] private RawImage go;

    private List<UITweenBase> allStartTween;

    private void Awake()
    {
        allStartTween = new List<UITweenBase>
        {
            leftDoor.GetComponent<UITweenBase>(),
            rightDoor.GetComponent<UITweenBase>(),
        };

        if (ready) allStartTween.Add(ready.GetComponent<UITweenBase>());
        if(go) allStartTween.Add(go.GetComponent<UITweenBase>());
    } 


    public void ResetTween()
    {
        foreach (var tween in allStartTween)
        {
            tween.ResetToBegin();
        }
    }


    public async Task ShowAni(Action onAniComplete = null)
    {
        var aniOver = false;
        for (var i = 0; i < allStartTween.Count; i++)
        {
            var tween = allStartTween[i];
            tween.TweenTo(i == allStartTween.Count - 1 ? () =>
                {
                    onAniComplete?.Invoke();
                    aniOver = true;
                }
                : () => tween.gameObject.SetActive(false));
        }

        while (!aniOver)
        {
            await Task.Yield();
        }
    }
}
