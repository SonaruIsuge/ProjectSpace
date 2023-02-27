

using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMoveCalculator
{
    Vector3 GetMovement(List<Player> allPlayer, IInteractable interactItem);
}
