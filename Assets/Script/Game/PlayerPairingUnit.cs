

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerPairingUnit
{
    public Player Player { get; }
    public PlayerInput PlayerInput { get; }
    public InputUser InputUser { get; private set; }

    private InputDevice inputDevice;

    public bool IsPaired { get; private set; }
    public bool IsReady { get; private set; }

    private bool startListenReadyInput;

    public event Action<PlayerPairingUnit, bool> OnChangeReady;


    public PlayerPairingUnit(Player player)
    {
           Player = player;
           PlayerInput = player.PlayerInput as PlayerInput;
           InputUser = PlayerInput?.inputUser ?? default;
           inputDevice = null;

           IsPaired = false;
           IsReady = false;
           startListenReadyInput = false;
    }


    public bool TryPairPlayerWithDevice(InputControl c, InputEventPtr e)
    {
           if (PlayerInput == null) return false;
           if (InputUser.valid) return false;

           inputDevice = c.device;
           InputUser = InputUser.PerformPairingWithDevice(inputDevice);
           InputUser.AssociateActionsWithUser(PlayerInput.playerInputAction);

           IsPaired = true;
           Debug.Log($"Pairing {Player.name} with {inputDevice.name}");

           return true;
    }


    public void UnpairDevice()
    {
           if (PlayerInput == null) return;

           InputUser.UnpairDevice(inputDevice);
           IsPaired = false;
    }


    public void UpdateReady()
    {
           if (!startListenReadyInput)
           {
                  if (PlayerInput.TapInteract) return;
                  startListenReadyInput = true;
           }

           if (PlayerInput.TapInteract) IsReady = !IsReady;
           OnChangeReady?.Invoke(this, IsReady);
    }
}
    
