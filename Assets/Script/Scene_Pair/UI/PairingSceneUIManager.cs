
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PairingSceneUIManager : MonoBehaviour
{
    [SerializeField] private Transform pairUIRoot;
    [SerializeField] private string pairHintText;
    [SerializeField] private string finalCheckHintText;
    [SerializeField] private TMP_Text pairHint;
    [SerializeField] private RawImage finalCheckHintImage;

    [SerializeField] private List<RawImage> allPreparePair;
    [SerializeField] private List<RawImage> allPairedPlayerReady;
    [SerializeField] private Transform allReadyPanel;
    [SerializeField] private List<RawImage> allStartGameIcons;
    [SerializeField] private List<RawImage> allStartGameFocus;
    [SerializeField] private Transform startUIRoot;
    [SerializeField] private List<UITweenBase> startAniTween;

    private UITweenBase finalCheckImageTween;
    private UITweenBase pairHintTween;
    private UITweenBase allReadyPanelTween;
    
    private int pairedNum;
    private bool allReadyPanelShow;


    /// <summary>
    /// <para>Set UI gameObject inactive.</para>
    /// <para>Get all ui tween</para>
    /// </summary>
    public void InitPairUI()
    {
        foreach(var pairIcon in allPreparePair) pairIcon.gameObject.SetActive(false);
        foreach(var readyIcon in allPairedPlayerReady) readyIcon.gameObject.SetActive(false);
        foreach (var startIcon in allStartGameIcons) startIcon.gameObject.SetActive(false);
        foreach (var iconFocus in allStartGameFocus) iconFocus.gameObject.SetActive(false);
        pairUIRoot.gameObject.SetActive(false);
        
        finalCheckImageTween = finalCheckHintImage.GetComponent<UITweenBase>();
        pairHintTween = pairHint.GetComponent<UITweenBase>();
        allReadyPanelTween = allReadyPanel.GetComponent<UITweenBase>();
    }


    /// <summary>
    /// <para>Active/Inactive origin pair UI</para>
    /// <para>Reset script value.</para>
    /// </summary>
    public void EnableOriginPairUI(bool active)
    {
        pairUIRoot.gameObject.SetActive(active);
        foreach(var pairIcon in allPreparePair) pairIcon.gameObject.SetActive(active);
        pairedNum = 0;
        allReadyPanelShow = false;
    }
    

    public void PlayerChangeReady(DevicePairUnit unit, bool isReady)
    {
        allPairedPlayerReady[unit.CharacterIndex].gameObject.SetActive(isReady);
    }


    public void PlayerPair(DevicePairUnit unit)
    {
        if(pairedNum == 0) pairHintTween.TweenTo();
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(false);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(true);
        pairedNum++;
    }


    public void PlayerUnpair(DevicePairUnit unit)
    {
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(true);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(false);
        pairedNum--;
        if(pairedNum == 0) pairHintTween.TweenFrom();
    }


    public void ToggleFinalCheckHint(bool activeFinalCheck)
    {
        if (activeFinalCheck)
        {
            pairHintTween.TweenFrom();
            finalCheckImageTween.TweenFrom();
            pairHint.text = finalCheckHintText;
            finalCheckHintImage.gameObject.SetActive(true);
        }
        else
        {
            pairHintTween.TweenTo(() => pairHint.text = pairHintText);
            finalCheckImageTween.TweenTo( () => finalCheckHintImage.gameObject.SetActive(false));
        }
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


    /// <summary>
    /// Hide all ready panel and enable start UI.
    /// </summary>
    public void ActiveStartGroup()
    {
        allReadyPanelTween.TweenFrom();
        startUIRoot.gameObject.SetActive(true);
    }


    public void PlayStartAni()
    {
        foreach (var tween in startAniTween)
        {
            tween.TweenTo();
        }
    }
}
