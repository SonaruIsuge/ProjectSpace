

using System.Collections.Generic;
using UnityEngine;

public class ItemSizeMoveCalculator
{
    
    
    public float GetMovementBuff(List<Player> allPlayer, IInteractable interactItem)
    {
        if (interactItem is not Item item) return 1;

        var itemSize = item.ItemData.Size;

        switch (itemSize)
        {
            case ItemSize.Small:
                return 1;
            
            case ItemSize.Medium:
                return allPlayer.Count * 0.5f;
            
            case ItemSize.Large:
                if (allPlayer.Count == 1) return 0;
                return allPlayer.Count * 0.5f;
            
            case ItemSize.ExtraLarge:
                return 0;
            
            default:
                return 1;
        }
    }
}
