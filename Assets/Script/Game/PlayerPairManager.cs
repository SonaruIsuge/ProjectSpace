
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerPairManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    private List<InputDevice> allPlayerDevices;
    private List<Player> pairPlayers;
    private List<bool> playerReady;

    [SerializeField] private bool allReady;

    public event Action<Player> OnPlayerPair;
    public event Action<Player, int, bool> OnPlayerChangeReadyState;
    public event Action OnAllPlayerReady;
    

    private void Awake()
    {
        allPlayerDevices = new List<InputDevice>();
        pairPlayers = new List<Player>();
        playerReady = new List<bool>();

        allReady = false;
    }


    private void OnEnable()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
    }


    private void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
    }


    private void Update()
    {
        if(!allReady) UpdateReady();
    }


    public void InitialSetUp()
    {
        InputUser.listenForUnpairedDeviceActivity = 4;
    }


    public void StopListenUnpairDevice()
    {
        InputUser.listenForUnpairedDeviceActivity = 0;
    }


    private void OnUnpairedDeviceUsed(InputControl c, InputEventPtr e)
    {
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
                pairPlayers.Add(player);
                playerReady.Add(false);
                player.SetActive(true);
                
                Debug.Log($"Pairing {player.name} with {c.device.name}");
                
                OnPlayerPair?.Invoke(player);
                
                return;
            }
        }
    }


    public void UnpairAllDevice()
    {
        for (var i = 0; i < pairPlayers.Count; i++)
        {
            if (pairPlayers[i].PlayerInput is not PlayerInput playerInput) continue;
            
            playerInput.inputUser.UnpairDevice(allPlayerDevices[i]);
        }
        
        pairPlayers.Clear();
        allPlayerDevices.Clear();
    }


    private void UpdateReady()
    {
        if(pairPlayers.Count  == 0) return;

        allReady = true;
        for (var i = 0; i < pairPlayers.Count; i++)
        {
            if (pairPlayers[i].PlayerInput.TapInteract)
            {
                playerReady[i] = !playerReady[i];
                OnPlayerChangeReadyState?.Invoke(pairPlayers[i], i, playerReady[i]);
            }
            allReady &= playerReady[i];
        }

        if (allReady) OnAllPlayerReady?.Invoke();
    }
}
