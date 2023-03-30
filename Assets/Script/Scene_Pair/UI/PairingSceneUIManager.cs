
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PairingSceneUIManager : MonoBehaviour
{
    [SerializeField] private Transform canvasTrans;
    [SerializeField] private TMP_Text joinHint;
    [SerializeField] private List<RawImage> allPreparePair;
    [SerializeField] private List<RawImage> allPairedPlayerReady;
    [SerializeField] private Transform allReadyPanel;
    [SerializeField] private List<RawImage> allStartGameIcons;
    [SerializeField] private List<RawImage> allStartGameFocus;

    private IUITween joinHintTween;
    private IUITween allReadyPanelTween;
    
    private int pairedNum;
    private bool allReadyPanelShow;
    
    private Camera MainCam => Camera.main;


    public void Init()
    {
        foreach(var pairIcon in allPreparePair) pairIcon.gameObject.SetActive(true);
        foreach(var readyIcon in allPairedPlayerReady) readyIcon.gameObject.SetActive(false);
        foreach (var startIcon in allStartGameIcons) startIcon.gameObject.SetActive(false);
        foreach (var iconFocus in allStartGameFocus) iconFocus.gameObject.SetActive(false);
        
        pairedNum = 0;
        allReadyPanelShow = false;

        joinHintTween = joinHint.GetComponent<IUITween>();
        allReadyPanelTween = allReadyPanel.GetComponent<IUITween>();
    }
    

    public void PlayerChangeReady(DevicePairUnit unit, bool isReady)
    {
        allPairedPlayerReady[unit.CharacterIndex].gameObject.SetActive(isReady);
    }


    public void PlayerPair(DevicePairUnit unit)
    {
        if(pairedNum == 0) joinHintTween.TweenTo();
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(false);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(true);
        pairedNum++;
    }


    public void PlayerUnpair(DevicePairUnit unit)
    {
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(true);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(false);
        pairedNum--;
        if(pairedNum == 0) joinHintTween.TweenFrom();
    }


    public void ShowAllReadyPanel(bool isAllReady)
    {
        if(allReadyPanelShow == isAllReady) return;

        if (isAllReady) allReadyPanelTween.TweenTo();
        else allReadyPanelTween.TweenFrom();
        
        allReadyPanelShow = isAllReady;
    }


    public void SetStartGameIconFocus(DevicePairUnit unit, bool isReady)
    {
        allStartGameFocus[unit.CharacterIndex].gameObject.SetActive(isReady);
    }
}
