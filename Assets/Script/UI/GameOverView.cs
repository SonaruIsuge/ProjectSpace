using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private string winText;
    [SerializeField] private string loseText;

    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private RectTransform timeBox;
    [SerializeField] private TMP_Text totalTimeText;
    [SerializeField] private RectTransform replayBtnBox;
    [SerializeField] private Button replayBtn;
    [SerializeField] private RectTransform nextLevelBtnBox;
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private RectTransform quitBtnBox;
    [SerializeField] private Button quitBtn;

    [SerializeField] private RectTransform selectBox;
    [SerializeField] private IUITween[] allUITween;


    private void Awake()
    {
        allUITween = GetComponentsInChildren<IUITween>();
    }
    

    public void BindReplayButton(UnityAction onClick)
    {
        replayBtn.onClick.AddListener(onClick);
    }
    
    
    public void BindNextLevelButton(UnityAction onClick)
    {
        nextLevelBtn.onClick.AddListener(onClick);
    }
    
    
    public void BindQuitButton(UnityAction onClick)
    {
        quitBtn.onClick.AddListener(onClick);
    }
    
    
    public void SetGameOverData(bool isWin, float useTime)
    {
        SetGameOverShowInfo(isWin);
        SetGameOverText(isWin);
        if(isWin) SetUseTime(useTime);

        UITween();
    }

    private void SetGameOverShowInfo(bool isWin)
    {
        timeBox.gameObject.SetActive(isWin);
        nextLevelBtnBox.gameObject.SetActive(isWin);
        replayBtnBox.gameObject.SetActive(!isWin);
    }


    private void SetGameOverText(bool isWin)
    {
        gameOverText.text = isWin ? winText : loseText;
    }


    private void SetUseTime(float useTime)
    {
        UnityTool.ChangeSecToHMS(useTime, out var h, out var m, out var s);
        var mText = m.ToString("00");
        var sText = s.ToString("00");
        totalTimeText.text = $"Time: {mText}:{sText}";
    }


    private void UITween()
    {
        foreach (var uiTween in allUITween)
        {
            uiTween.Tween();
        }
    }

    
    [ContextMenu("Debug Tween")]
    public void DebugTween()
    {
        gameObject.SetActive(true);
        SetGameOverData(true, 0);
    }

    [ContextMenu("Initial Tween")]
    public void ResetTween()
    {
        foreach (var uiTween in allUITween) uiTween.Initial();
    }
}
