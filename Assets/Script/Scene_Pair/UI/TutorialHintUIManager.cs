using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class TutorialHintUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform hintPanel;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private Transform selectBox;
    private UITweenBase hintPanelTween;
    private EventSystem EventSystem => EventSystem.current;

    private void Awake()
    {
        hintPanelTween = hintPanel.GetComponent<UITweenBase>();
    }


    public void BindYseBtn(UnityAction onClick)
    {
        yesBtn.onClick.AddListener(onClick);
    }


    public void BindNoBtn(UnityAction onClick)
    {
        noBtn.onClick.AddListener(onClick);
    }


    public void ShowUI(Action onComplete = null)
    {
        selectBox.gameObject.SetActive(false);
        hintPanel.gameObject.SetActive(true);
        hintPanelTween.ResetToBegin();
        hintPanelTween.TweenTo(() =>
        {
            EventSystem.SetSelectedGameObject(yesBtn.gameObject);
            selectBox.gameObject.SetActive(true);
            onComplete?.Invoke();
        });
    }


    public void HindUI(Action onComplete = null)
    {
        selectBox.gameObject.SetActive(false);
        EventSystem.SetSelectedGameObject(null);
        hintPanelTween.TweenFrom(() =>
        {
            hintPanel.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}
