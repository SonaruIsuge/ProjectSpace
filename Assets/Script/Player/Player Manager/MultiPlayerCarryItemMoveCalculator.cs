
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerCarryItemMoveCalculator : IPlayerMoveCalculator
{
    
    public Vector3 GetMovement(List<Player> allPlayer, IInteractable interactItem)
    {
        var totalDirection = Vector3.zero;
        foreach (var player in allPlayer)
        {
            totalDirection += new Vector3(player.PlayerInput.Movement.x, player.PlayerInput.JetDirection, player.PlayerInput.Movement.y);
        }

        totalDirection /= allPlayer.Count;
        
        return totalDirection;
    }
}
