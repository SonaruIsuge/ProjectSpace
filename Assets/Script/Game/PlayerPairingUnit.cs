
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


[Serializable]
public class PlayerPairingUnit
{
    public int PlayerIndex { get; }
    public Player Player { get; private set; }
    public PlayerInput PlayerInput => Player ? Player.PlayerInput as PlayerInput : null;
    public InputUser InputUser { get; private set; }

    public InputDevice InputDevice { get; private set; }
    public int DeviceId => InputDevice?.deviceId ?? -1;

    public bool IsPaired { get; private set; }
    public bool IsReady { get; private set; }

    private bool startListenReadyInput;

    public event Action<PlayerPairingUnit, bool> OnChangeReady;


    public PlayerPairingUnit(int playerIndex)
    {
           PlayerIndex = playerIndex;
           Player = null;
           InputDevice = null;

           IsPaired = false;
           IsReady = false;
           startListenReadyInput = false;
    }


    public bool TryPairPlayerWithDevice(Player player, InputDevice device)
    {
           if (player.PlayerInput == null) return false;
           if (InputUser.valid) return false;

           Player = player;
           InputDevice = device;
           
           InputUser = InputUser.PerformPairingWithDevice(InputDevice);
           InputUser.AssociateActionsWithUser(PlayerInput.playerInputAction);

           IsPaired = true;
           Debug.Log($"Pairing {Player.name} with {InputDevice.name}");

           return true;
    }


    public void UnpairDevice()
    {
           if (PlayerInput == null) return;
           
           InputUser.UnpairDevicesAndRemoveUser();
           
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
    
