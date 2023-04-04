using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> AllPlayers;
    [SerializeField] private List<Player> activePlayers;
    
    private SinglePlayerMoveCalculator singlePlayerMoveCalculator;
    private MultiPlayerCarryItemMoveCalculator multiPlayerCarryItemMoveCalculator;
    private ItemSizeMoveCalculator itemSizeMoveCalculator;
    
    private Dictionary<Item, List<Player>> playerInteractItemDict;
    
    private float worldRotateAngle;
    private bool isStart;

    public event Action<Player, int> OnPlayerActive;
    public event Action<Player, float> OnRotateCameraCall;
    
    
    public void SetWorldRotate(float angle)
    {
        worldRotateAngle = angle;
    }
    
    
    public void SetStart(bool start)
    {
        isStart = start;
    }


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
            if(player.PlayerInput.RotateCam != 0) OnRotateCameraCall?.Invoke(player, -player.PlayerInput.RotateCam);
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


    public void BindPlayerWithDevice(Dictionary<int, InputDevice> bindPlayerDeviceDict)
    {
        for (var i = 0; i < AllPlayers.Count; i++)
        {
            var player = AllPlayers[i];
            if (!bindPlayerDeviceDict.Keys.Contains(i))
            {
                player.SetActive(false);
                player.gameObject.SetActive(false);
                continue;
            }
            
            if (player.PlayerInput is not PlayerInput playerInput)
            {
                Debug.LogError($"Player{i} cannot pair with device");
                continue;
            }
            
            playerInput.PairWithDevice(bindPlayerDeviceDict[i]);
            ActivePlayer(player);
            OnPlayerActive?.Invoke(player, i);
        }
    }
    

    public void ActivePlayer(Player player)
    {
        player.SetActive(true);
        if(!activePlayers.Contains(player)) activePlayers.Add(player);
    }

    
    public void RemoveAllActivePlayer()
    {
        foreach (var player in activePlayers)
        {
            if(player.PlayerInput is not PlayerInput input) continue;
            input.UnpairWithDevice();
            player.SetActive(false);
        }
        activePlayers.Clear();
    }
}
