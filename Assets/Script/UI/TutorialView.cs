using TMPro;
using UnityEngine;


public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject textFrame;
    [SerializeField] private TMP_Text textArea;
    [SerializeField] private UITweenBase textTween;


    public void SetTextArea(string text)
    {
        textArea.text = text;
    }


    public void ShowTextFrame()
    {
        textFrame.SetActive(true);
        textTween.ResetToBegin();
        textTween.TweenTo();
    }


    public void HideTextFrame()
    {
        textTween.TweenFrom(() =>
        {
            textFrame.SetActive(false);
        });
        
    }
}
