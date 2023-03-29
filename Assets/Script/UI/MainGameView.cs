using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainGameView : MonoBehaviour
{
    [SerializeField] private TMP_Text remainRubbishText;
    [SerializeField] private Image remainTimeBg;
    [SerializeField] private TMP_Text remainTimeText;
    
    // For player rotate camera icon
    [SerializeField] private RectTransform canvasRectTrans;
    [SerializeField] private float iconFromCornerRad;
    [Range(0, 90)] [SerializeField] private float iconRangeAngle;

    private Rect CanvasRect => canvasRectTrans.rect;
    private Vector3 LT => canvasRectTrans.position + new Vector3(CanvasRect.xMin, CanvasRect.yMax, 0);
    private Vector3 RT => canvasRectTrans.position + new Vector3(CanvasRect.xMax, CanvasRect.yMax, 0);
    

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


    public Vector2 RandomFromRotateUI(int playerIndex, float clockwise)
    {
        var isClockwise = clockwise > 0;
        
        var referCornerPos = clockwise > 0 ? LT : RT;
        var fromAngle = isClockwise ? -45 + iconRangeAngle / 2 : 225 - iconRangeAngle / 2;
        var toAngle = isClockwise ? -45 - iconRangeAngle / 2 : 225 + iconRangeAngle / 2;
        var space = iconRangeAngle / 3;
        Debug.Log(referCornerPos);
        var iconRad = (fromAngle + (isClockwise ? -1 : 1) * space * playerIndex) * Mathf.Deg2Rad;
        return referCornerPos + new Vector3(Mathf.Cos(iconRad), Mathf.Sin(iconRad), 0) * iconFromCornerRad;
    }


    
}
