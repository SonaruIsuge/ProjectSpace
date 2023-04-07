using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private UITweenBase canvasTween;
    
    private EventSystem EventSystem => EventSystem.current;


    public void SetStartEvent(bool bind, UnityAction onClick)
    {
        if(!bind) startButton.onClick.RemoveAllListeners();
        else startButton.onClick.AddListener(onClick);
    }


    public void SetSettingEvent(bool bind, UnityAction onClick)
    {
        if(!bind) settingButton.onClick.RemoveAllListeners();
        else settingButton.onClick.AddListener(onClick);
    }


    public void SetQuitEvent(bool bind, UnityAction onClick)
    {
        if(!bind) quitButton.onClick.RemoveAllListeners();
        else quitButton.onClick.AddListener(onClick);
    }
    
    
    /// <summary>
    /// Set event System first select game object.
    /// </summary>
    public void InitMainMenuUI()
    {
        EventSystem.SetSelectedGameObject(startButton.gameObject);
    }


    public void ShowMainMenu(Action onShowComplete = null)
    {
        canvasTween.TweenFrom(onShowComplete);
        EventSystem.SetSelectedGameObject(startButton.gameObject);
    }


    public void HideMainMenu(Action onHideComplete = null)
    {
        EventSystem.SetSelectedGameObject(null);
        canvasTween.TweenTo(onHideComplete);
    }
}
