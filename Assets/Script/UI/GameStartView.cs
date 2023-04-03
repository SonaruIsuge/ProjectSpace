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

    private UITweenBase leftDoorTween;
    private UITweenBase rightDoorTween;

    private void Awake()
    {
        leftDoorTween = leftDoor.GetComponent<UITweenBase>();
        rightDoorTween = rightDoor.GetComponent<UITweenBase>();
    } 


    public void ResetTween()
    {
        leftDoorTween.ResetToBegin();
        rightDoorTween.ResetToBegin();
    }


    public void ShowAni(Action onAniComplete = null)
    {
        leftDoorTween.TweenTo();
        rightDoorTween.TweenTo(onAniComplete);
    }
}
