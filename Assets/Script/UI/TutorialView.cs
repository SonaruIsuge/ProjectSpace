using TMPro;
using UnityEngine;


public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject textFrame;
    [SerializeField] private TMP_Text textArea;


    public void SetTextArea(string text)
    {
        textArea.text = text;
    }


    public void ShowTextFrame()
    {
        textFrame.SetActive(true);
    }


    public void HideTextFrame()
    {
        textFrame.SetActive(false);
    }
}
