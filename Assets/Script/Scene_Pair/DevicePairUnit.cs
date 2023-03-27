
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class DevicePairUnit
{
    private PlayerInputAction playerInputAction;
    private InputUser inputUser;
    public InputDevice InputDevice { get; private set; }

    private bool PressReady => playerInputAction.Pair.Ready.WasPressedThisFrame();
    private bool Unpair => playerInputAction.Pair.Unpair.WasPressedThisFrame();

    public bool IsPaired { get; private set; }
    public bool IsReady { get; private set; }
    
    private bool startListenReadyInput;

    public event Action<DevicePairUnit> OnPairDevice;
    public event Action<DevicePairUnit> OnUnpairDevice;
    public event Action<DevicePairUnit, bool> OnChangeReady;
    

    public DevicePairUnit()
    {
        playerInputAction = new PlayerInputAction();

        IsPaired = false;
        startListenReadyInput = false;
    }
    

    public void Enable(bool enable)
    {
        if(enable) playerInputAction.Pair.Enable();
        else playerInputAction.Pair.Disable();
    }


    public bool TryPairDevice(InputDevice device)
    {
        if (inputUser.valid) return false;
        
        InputDevice = device;
           
        inputUser = InputUser.PerformPairingWithDevice(InputDevice);
        inputUser.AssociateActionsWithUser(playerInputAction);

        IsPaired = true;
        Enable(true);
        
        OnPairDevice?.Invoke(this);
        Debug.Log($"Success pair {InputDevice.name}");

        return true;
    }


    public void UnpairDevice()
    {
        if(!inputUser.valid) return;
        
        Enable(false);
        Debug.Log($"Unpair {InputDevice.name} with InputUser{inputUser.id}");
        
        inputUser.UnpairDevicesAndRemoveUser();
        
        IsPaired = false;
        IsReady = false;
        startListenReadyInput = false;
        
        OnUnpairDevice?.Invoke(this);
    }


    public void UpdateSelf()
    {
        if(!IsPaired) return;
        
        if (!startListenReadyInput)
        {
            if (PressReady) return;
            startListenReadyInput = true;
        }
        
        if (Unpair)
        {
            UnpairDevice();
            return;
        }
        
        if (PressReady)
        {
            IsReady = !IsReady;
            OnChangeReady?.Invoke(this, IsReady);
        }
    }
}
