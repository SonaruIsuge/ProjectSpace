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
    
    private EventSystem EventSystem => EventSystem.current;


    public void BindStartEvent(UnityAction onClick)
    {
        startButton.onClick.AddListener(onClick);
    }


    public void BindSettingEvent(UnityAction onClick)
    {
        settingButton.onClick.AddListener(onClick);
    }


    public void BindQuitEvent(UnityAction onClick)
    {
        quitButton.onClick.AddListener(onClick);
    }
    
    
    /// <summary>
    /// Set event System first select game object.
    /// </summary>
    public void InitMainMenuUI()
    {
        EventSystem.SetSelectedGameObject(startButton.gameObject);
    }


    public void ShowMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        EventSystem.SetSelectedGameObject(startButton.gameObject);
    }


    public void HideMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
    }
}
