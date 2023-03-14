using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> activePlayers;
    
    private SinglePlayerMoveCalculator singlePlayerMoveCalculator;
    private MultiPlayerCarryItemMoveCalculator multiPlayerCarryItemMoveCalculator;
    private ItemSizeMoveCalculator itemSizeMoveCalculator;
    
    private Dictionary<Item, List<Player>> playerInteractItemDict;

    public event Action<float> OnRotateCameraCall;

    
    private float worldRotateAngle;
    public void SetWorldRotate(float angle) => worldRotateAngle = angle;
    
    private bool isStart;
    public void SetStart(bool start) => isStart = start;


    private void Awake()
    {
        singlePlayerMoveCalculator = new SinglePlayerMoveCalculator();
        multiPlayerCarryItemMoveCalculator = new MultiPlayerCarryItemMoveCalculator();
        itemSizeMoveCalculator = new ItemSizeMoveCalculator();

        activePlayers = new List<Player>();
        playerInteractItemDict = new Dictionary<Item, List<Player>>();

        worldRotateAngle = 0;
    }


    private void OnEnable()
    {
        ItemManager.OnItemStartInteract += NewItemInteractPlayer;
        ItemManager.OnItemEndInteract += RemovePlayerInteractItem;
    }


    private void OnDisable()
    {
        ItemManager.OnItemStartInteract -= NewItemInteractPlayer;
        ItemManager.OnItemEndInteract -= RemovePlayerInteractItem;
    }


    private void LateUpdate()
    {
        if(!isStart) return;
        
        foreach (var player in activePlayers)
        {
            if(!player.IsActive) continue;
            
            var itemSizeBuff = 1f;
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
            playerMoveVelocity = Quaternion.Euler(0, worldRotateAngle, 0) * playerMoveVelocity;
            
            // player update
            player.Move(playerMoveVelocity);
            
            player.DetectInteract();
            
            // World turns in the opposite direction of the camera
            if(player.PlayerInput.RotateCam != 0) OnRotateCameraCall?.Invoke(-player.PlayerInput.RotateCam);    
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


    public void AddActivePlayer(Player player)
    {
        if(!activePlayers.Contains(player)) activePlayers.Add(player);
    }

    
}
