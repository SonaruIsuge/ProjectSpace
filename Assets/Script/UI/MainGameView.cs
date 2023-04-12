using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainGameView : MonoBehaviour
{
    [SerializeField] private TMP_Text remainRubbishText;
    [SerializeField] private Image remainTimeBg;
    [SerializeField] private TMP_Text remainTimeText;
    

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
}
