
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelMenuUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform levelPanel;
    [SerializeField] private Button levelBtnPrefab;
    [SerializeField] private List<SceneNamePair> allLevel;
    [SerializeField] private UITweenBase levelPanelTween;
    private List<Button> allButton;
    
    private EventSystem EventSystem => EventSystem.current;

    public event Action<SceneIndex> OnLevelChosed; 


    public void InitLevelButton()
    {
        allButton = new List<Button>();
        foreach (var level in allLevel)
        {
            var btn = Instantiate(levelBtnPrefab, levelPanel);
            btn.gameObject.name = $"{level.name} Btn";
            allButton.Add(btn);
            
            var btnText = btn.GetComponentInChildren<TMP_Text>();
            if(btnText) btnText.text = level.name;
            
            if (!RectTransformUtility.RectangleContainsScreenPoint(levelPanel, btn.transform.position))
                btn.gameObject.SetActive(false);
            
            btn.onClick.AddListener(() => OnLevelChosed?.Invoke(level.sceneIndex));
        }
    }
    


    public void EnableLevelPanel(bool enable)
    {
        if (enable)
        {
            levelPanelTween.gameObject.SetActive(true);
            levelPanelTween.TweenFrom(() => EventSystem.SetSelectedGameObject(allButton[0].gameObject));
        }
        else levelPanelTween.TweenTo(() => levelPanelTween.gameObject.SetActive(false));
    }
}



[System.Serializable]
public class SceneNamePair
{
    public string name;
    public SceneIndex sceneIndex;
}
