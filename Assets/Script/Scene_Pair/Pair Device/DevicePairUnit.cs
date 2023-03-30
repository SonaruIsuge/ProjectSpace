
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[Serializable]
public class DevicePairUnit
{
    private PlayerInputAction playerInputAction;
    private InputUser inputUser;
    public InputDevice InputDevice { get; private set; }
    public int CharacterIndex { get; }
    public bool PressReady => playerInputAction.Pair.Ready.WasPressedThisFrame();
    public bool PressCancel => playerInputAction.Pair.Unpair.WasPressedThisFrame();
    

    [field: SerializeField] public bool IsPaired { get; private set; }
    [field: SerializeField] public bool IsReady { get; private set; }
    [field: SerializeField] public bool IsFinalCheck { get; private set; }

    [field: SerializeField] public PairUnitState CurrentState { get; private set; }
    
    private bool listenBtnClickFlag;
    private bool enableFinalCheck;

    public event Action<DevicePairUnit> OnPairDevice;
    public event Action<DevicePairUnit> OnUnpairDevice;
    public event Action<DevicePairUnit, bool> OnChangeReady;
    public event Action<DevicePairUnit, bool> OnChangeFinalCheck;


    public DevicePairUnit(int characterIndex)
    {
        playerInputAction = new PlayerInputAction();

        CharacterIndex = characterIndex;
        IsPaired = false;
        IsReady = false;
        IsFinalCheck = false;

        CurrentState = PairUnitState.Unpair;
        listenBtnClickFlag = false;
    }
    

    public void Enable(bool enable)
    {
        if(enable) playerInputAction.Pair.Enable();
        else playerInputAction.Pair.Disable();
    }


    public void EnableFinalCheck(bool enable)
    {
        enableFinalCheck = enable;
    }


    public bool TryPairDevice(InputDevice device)
    {
        if (inputUser.valid) return false;
        
        InputDevice = device;
           
        inputUser = InputUser.PerformPairingWithDevice(InputDevice);
        inputUser.AssociateActionsWithUser(playerInputAction);

        IsPaired = true;
        CurrentState = PairUnitState.Paired;
        Enable(true);
        
        OnPairDevice?.Invoke(this);
        Debug.Log($"Success pair Character{CharacterIndex} with {InputDevice.name}");

        return true;
    }


    public void UnpairDevice()
    {
        if(!inputUser.valid) return;
        
        Enable(false);
        //Debug.Log($"Unpair {InputDevice.name} with Character{CharacterIndex}");
        
        inputUser.UnpairDevicesAndRemoveUser();
        
        IsPaired = false;
        IsReady = false;
        listenBtnClickFlag = false;
        CurrentState = PairUnitState.Unpair;
        
        OnUnpairDevice?.Invoke(this);
    }


    public void UpdateSelf()
    {
        if(!IsPaired) return;
        
        if (!listenBtnClickFlag)
        {
            // when state change, wasPressedThisFrame = true will impact next determine in one frame.
            // Therefore, need to check if button pressed this frame, and stop other determine until next frame.
            if (PressReady) return;
            if(PressCancel) return;
            
            listenBtnClickFlag = true;
        }

        if (PressCancel)
        {
            ChangeCurrentState(false);
        }
        
        else if (PressReady)
        {
            ChangeCurrentState(true);
        }
    }


    // states: Unpair <-> Paired <-> Ready <-> FinalCheck
    private void ChangeCurrentState(bool nextState)
    {
        var addState = nextState ? 1 : -1;
        
        var maxState = Enum.GetValues(typeof(PairUnitState)).Length - 1;
        if (!enableFinalCheck) maxState--;
        var minState = enableFinalCheck ? (int)PairUnitState.Ready : 0;

        var nowState = (int)CurrentState;
        var newState = Mathf.Clamp(nowState + addState, minState, maxState);
        CurrentState = (PairUnitState)newState;
        
        if(nowState != newState) DealCurrentState();
    }


    private void DealCurrentState()
    {
        switch (CurrentState)
        {
            case PairUnitState.Unpair:
                UnpairDevice();
                break;
            
            case PairUnitState.Paired:
                IsReady = false;
                OnChangeReady?.Invoke(this, false);
                break;
            
            case PairUnitState.Ready:
                IsReady = true;
                IsFinalCheck = false;
                OnChangeReady?.Invoke(this, true);
                OnChangeFinalCheck?.Invoke(this, false);
                break;
            
            case PairUnitState.FinalCheck:
                IsFinalCheck = true;
                OnChangeFinalCheck?.Invoke(this, true);
                break;
        }
    }
}
