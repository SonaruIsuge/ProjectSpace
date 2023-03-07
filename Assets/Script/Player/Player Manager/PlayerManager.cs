using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    private List<InputDevice> allPlayerDevices;
    
    private SinglePlayerMoveCalculator singlePlayerMoveCalculator;
    private MultiPlayerCarryItemMoveCalculator multiPlayerCarryItemMoveCalculator;
    private ItemSizeMoveCalculator itemSizeMoveCalculator;
    
    private Dictionary<Item, List<Player>> playerInteractItemDict;


    private void Awake()
    {
        ++InputUser.listenForUnpairedDeviceActivity;
        allPlayerDevices = new List<InputDevice>();
        
        singlePlayerMoveCalculator = new SinglePlayerMoveCalculator();
        multiPlayerCarryItemMoveCalculator = new MultiPlayerCarryItemMoveCalculator();
        itemSizeMoveCalculator = new ItemSizeMoveCalculator();
        
        playerInteractItemDict = new Dictionary<Item, List<Player>>();
    }


    private void OnEnable()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
        
        ItemManager.OnItemStartInteract += NewItemInteractPlayer;
        ItemManager.OnItemEndInteract += RemovePlayerInteractItem;
    }


    private void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        
        ItemManager.OnItemStartInteract -= NewItemInteractPlayer;
        ItemManager.OnItemEndInteract -= RemovePlayerInteractItem;
    }


    private void LateUpdate()
    {
        foreach (var player in players)
        {
            if(!player.IsActive) continue;
            
            var itemSizeBuff = 1f;
            // check if player is interact with item
            //Debug.Log(player.PlayerInput.JetDirection);
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


    private void OnUnpairedDeviceUsed(InputControl c, InputEventPtr e)
    {
        // device not keyboard / gamepad
        if(c.device.GetType() == Mouse.current.GetType()) return;
        if (!(c.device.GetType() == Keyboard.current.GetType() || c.device.GetType() == Gamepad.current.GetType()))
            return;
        

        foreach (var player in players)
        {
            if(player.PlayerInput is not PlayerInput playerInput) continue;

            if (!playerInput.inputUser.valid)
            {
                playerInput.inputUser = InputUser.PerformPairingWithDevice(c.device);
                playerInput.inputUser.AssociateActionsWithUser(playerInput.playerInputAction);
                allPlayerDevices.Add(c.device);
                player.SetActive(true);
                Debug.Log($"Pairing {player.name} with {c.device.name}");
                return;
            }
        }
    }
    
    
    private void UnPairAllDevices()
    {
        for (var i = 0; i < players.Count; i++)
        {
            if (players[i].PlayerInput is not PlayerInput playerInput) continue;
            playerInput.inputUser.UnpairDevice(allPlayerDevices[i]);
        }
    }
}
