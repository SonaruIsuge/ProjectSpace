
using System;
using SonaruUtilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    [SerializeField] private UITweenBase[] allUITween;

    private EventSystem EventSystem => EventSystem.current;
    
    
    private void Awake()
    {
        allUITween = GetComponentsInChildren<UITweenBase>();
    }


    private void Start()
    {
        foreach (var uiTween in allUITween)
        {
            uiTween.ResetToBegin();
        }
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

        EventSystem.SetSelectedGameObject(isWin ? nextLevelBtn.gameObject : replayBtn.gameObject);
        
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
            uiTween.TweenTo();
        }
    }

    
    [ContextMenu("Debug Tween")]
    public void DebugTween()
    {
        gameObject.SetActive(true);
        SetGameOverData(true, 0);
    }
}
