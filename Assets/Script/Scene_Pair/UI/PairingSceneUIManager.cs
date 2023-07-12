
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PairingSceneUIManager : MonoBehaviour
{
    [SerializeField] private Transform pairUIRoot;
    [SerializeField] private string pairHintText;
    [SerializeField] private string readyHintText;
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

    /// From 1 -> To 0
    private UITweenBase finalCheckImageTween;
    /// From 1 -> To 0
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
        if (active) pairHintTween.TweenFrom();
        else pairHintTween.TweenTo();
    }
    

    public void PlayerChangeReady(DevicePairUnit unit, bool isReady)
    {
        allPairedPlayerReady[unit.CharacterIndex].gameObject.SetActive(isReady);
    }


    public void PlayerPair(DevicePairUnit unit)
    {
        if (pairedNum == 0)
            // pairHintTween.TweenTo(() => {
            //     pairHint.text = readyHintText;
            //     pairHintTween.TweenFrom();
            //     finalCheckImageTween.TweenFrom();
            // });
            UpdateHintPanel(readyHintText, false, true);
        
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(false);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(true);
        pairedNum++;
    }


    public void PlayerUnpair(DevicePairUnit unit)
    {
        allPreparePair[unit.CharacterIndex].gameObject.SetActive(true);
        allStartGameIcons[unit.CharacterIndex].gameObject.SetActive(false);
        pairedNum--;
        if (pairedNum == 0)
            // pairHintTween.TweenTo(() => {
            //     pairHint.text = pairHintText;
            //     pairHintTween.TweenFrom();
            //     finalCheckImageTween.TweenTo();
            // });
            UpdateHintPanel(pairHintText, true, false);
    }


    public void ToggleFinalCheckHint(bool activeFinalCheck)
    {
        if (activeFinalCheck)
        {
            UpdateHintPanel(finalCheckHintText, true, true);
            // finalCheckImageTween.TweenTo();
            // pairHintTween.TweenTo(() =>
            // {
            //     pairHint.text = finalCheckHintText;
            //     pairHintTween.TweenFrom();
            //     finalCheckImageTween.TweenFrom();
            // });
            // finalCheckImageTween.TweenFrom();
            // pairHint.text = finalCheckHintText;
        }
        else
        {
            // pairHintTween.TweenTo(() => pairHint.text = pairHintText);
            // finalCheckImageTween.TweenTo();
            UpdateHintPanel(readyHintText, true, true);
        }
    }


    public void ShowAllReadyPanel(bool isAllReady)
    {
        if(allReadyPanelShow == isAllReady) return;

        if (isAllReady) allReadyPanelTween.TweenTo(() => UpdateHintPanel("", true, false));
        else
        {
            allReadyPanelTween.TweenFrom();
            UpdateHintPanel(finalCheckHintText, false, true);
        }
        
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


    private void UpdateHintPanel(string newHint, bool nowHasImage, bool showImage)
    {
        if(nowHasImage) finalCheckImageTween.TweenTo(() =>
        {
            if(!showImage) finalCheckHintImage.gameObject.SetActive(false);
        });
        pairHintTween.TweenTo(() =>
        {
            pairHint.text = newHint;
            if(!nowHasImage && showImage) finalCheckHintImage.gameObject.SetActive(true);
            pairHintTween.TweenFrom();
            if(showImage) finalCheckImageTween.TweenFrom();
        });
    }
}
