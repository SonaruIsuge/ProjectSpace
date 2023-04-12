using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class RotateCamHintUI : MonoBehaviour
{
    [SerializeField] private PlayerRotateCamIcon playerRotateCamPrefab;
    [SerializeField] private List<Texture> allIconTextures;
    
    [SerializeField] private RectTransform canvasRectTrans;
    [SerializeField] private float iconFromCornerRad;
    [Range(0, 90)] [SerializeField] private float iconRangeAngle;

    private Rect CanvasRect => canvasRectTrans.rect;
    private Vector3 LT => canvasRectTrans.position + new Vector3(CanvasRect.xMin, CanvasRect.yMax, 0);
    private Vector3 RT => canvasRectTrans.position + new Vector3(CanvasRect.xMax, CanvasRect.yMax, 0);
    
    private Dictionary<Player, PlayerRotateCamIcon> playerIconDict;


    public void BindIconWithPlayer(Player player, int characterIndex)
    {
        var playerIcon = Instantiate(playerRotateCamPrefab, transform);
        playerIcon.BindPlayerWithIcon(player, allIconTextures[characterIndex]);
        
        playerIconDict ??= new Dictionary<Player, PlayerRotateCamIcon>(); 
        playerIconDict.Add(player, playerIcon);
    }
    
    
    public void ShowIcon(Player player, float clockwise)
    {
        var index = playerIconDict.Keys.ToList().IndexOf(player);
        playerIconDict[player].Show(CalcIconPos(index, clockwise), clockwise > 0);
    }
    
    
    private Vector2 CalcIconPos(int playerIndex, float clockwise)
    {
        var isClockwise = clockwise > 0;
        
        var referCornerPos = clockwise > 0 ? LT : RT;
        var fromAngle = isClockwise ? -45 + iconRangeAngle / 2 : 225 - iconRangeAngle / 2;
        var toAngle = isClockwise ? -45 - iconRangeAngle / 2 : 225 + iconRangeAngle / 2;
        var space = iconRangeAngle / 3;
        
        var iconRad = (fromAngle + (isClockwise ? -1 : 1) * space * playerIndex) * Mathf.Deg2Rad;
        return referCornerPos + new Vector3(Mathf.Cos(iconRad), Mathf.Sin(iconRad), 0) * iconFromCornerRad;
    }
}
