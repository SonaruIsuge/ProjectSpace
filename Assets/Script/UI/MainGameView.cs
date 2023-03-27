using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainGameView : MonoBehaviour
{
    [SerializeField] private TMP_Text remainRubbishText;
    [SerializeField] private Image remainTimeBg;
    [SerializeField] private TMP_Text remainTimeText;
    [SerializeField] private RectTransform turnLeftTransform;
    [SerializeField] private RectTransform turnRightTransform;
    [SerializeField] private float randomIconRadius;


    public void SetRemainRubbish(int remainNum)
    {
        remainRubbishText.text = remainNum.ToString("00");
    }


    public void SetRemainTime(float remainTime, float remainTime01)
    {
        UnityTool.ChangeSecToHMS(remainTime, out var h, out var m, out var s);
        var mText = m.ToString("00");
        var sText = s.ToString("00");
        remainTimeText.text = $"{mText}:{sText}";
        remainTimeBg.fillAmount = remainTime01;
    }


    public Vector2 RandomFromRotateUI(float clockwise)
    {
        var ui = clockwise > 0 ? turnLeftTransform : turnRightTransform;
        var randomRad = (clockwise > 0 ? Random.Range(190, 440) : Random.Range(100, 350)) * Mathf.Deg2Rad;
        var vector = new Vector3(Mathf.Cos(randomRad), Mathf.Sin(randomRad), 0) * Random.Range(80, randomIconRadius);
        return ui.position + vector;
    }
}
