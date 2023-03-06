using System.Collections.Generic;
using UnityEngine;


public class SinglePlayerMoveCalculator : IPlayerMoveCalculator
{
    private Vector2 movement;
    private float jet;
    
    
    public Vector3 GetMovement(List<Player> allPlayer, IInteractable interactItem)
    {
        if (allPlayer.Count == 0) return Vector3.zero;

        movement = allPlayer[0].PlayerInput.Movement;
        jet = allPlayer[0].PlayerInput.JetDirection;
        
        var runValue = allPlayer[0].PlayerInput.Run && allPlayer[0].IsGround ? 2 : 1;
        
        return new Vector3(movement.x * runValue, jet, movement.y * runValue);
    }
}
