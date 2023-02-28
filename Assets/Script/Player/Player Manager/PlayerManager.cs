using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;

    private SinglePlayerMoveCalculator singlePlayerMoveCalculator;
    private MultiPlayerCarryItemMoveCalculator multiPlayerCarryItemMoveCalculator;
    private ItemSizeMoveCalculator itemSizeMoveCalculator;
    
    private Dictionary<Item, List<Player>> playerInteractItemDict;


    private void Awake()
    {
        singlePlayerMoveCalculator = new SinglePlayerMoveCalculator();
        multiPlayerCarryItemMoveCalculator = new MultiPlayerCarryItemMoveCalculator();
        itemSizeMoveCalculator = new ItemSizeMoveCalculator();
        
        playerInteractItemDict = new Dictionary<Item, List<Player>>();
    }


    private void OnEnable()
    {
        ItemManager.OnItemStartInteract += NewItemInteractPlayer;
        ItemManager.OnItemEndInteract += RemovePlayerInteractItem;
    }


    private void LateUpdate()
    {
        foreach (var player in players)
        {
            var itemSizeBuff = 1f;
            // check if player is interact with item
            if (player.PlayerInteractController.CurrentInteract is Item item)
            {
                if (!playerInteractItemDict.ContainsKey(item) || !playerInteractItemDict[item].Contains(player))
                {
                    Debug.LogError($"{player} interact with {item} but not recorded!");
                    continue;
                }

                var playerInteractGroup = playerInteractItemDict[item];
                itemSizeBuff = itemSizeMoveCalculator.GetMovementBuff(playerInteractGroup, item);
            }
            
            var playerMoveVelocity = singlePlayerMoveCalculator.GetMovement(new List<Player> { player }, null);
            playerMoveVelocity *= itemSizeBuff;
            
            // player update
            player.Move(playerMoveVelocity);
            
            player.DetectInteract();
        }
    }
    

    private void NewItemInteractPlayer(Item item, Player interactor)
    {
        if (playerInteractItemDict.ContainsKey(item)) playerInteractItemDict[item].Add(interactor);
        else playerInteractItemDict.Add(item, new List<Player> { interactor });
    }


    private void RemovePlayerInteractItem(Item item, Player interactor)
    {
        if (playerInteractItemDict[item].Count == 1) playerInteractItemDict.Remove(item);
        else playerInteractItemDict[item].Remove(interactor);
    }
}
