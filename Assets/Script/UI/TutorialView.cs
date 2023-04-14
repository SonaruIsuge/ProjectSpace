using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject textFrame;
    [SerializeField] private TMP_Text textArea;
    [SerializeField] private UITweenBase textTween;
    [SerializeField] private RectTransform hintArrow;
    private Animator HintArrowAni => hintArrow.GetComponentInChildren<Animator>();
    

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


    public void ShowHintArrow(Vector3 pos)
    {
        hintArrow.gameObject.SetActive(true);
        hintArrow.position = pos;
        HintArrowAni.enabled = true;
    }
    
    
    public void ShowHintArrow(Vector2 anchorPos)
    {
        hintArrow.gameObject.SetActive(true);
        hintArrow.anchoredPosition = anchorPos;
        HintArrowAni.enabled = true;
    }


    public void HindHintArrow()
    {
        HintArrowAni.enabled = false;
        hintArrow.gameObject.SetActive(false);
    }
}
