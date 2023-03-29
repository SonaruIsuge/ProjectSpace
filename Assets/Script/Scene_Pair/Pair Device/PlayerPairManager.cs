
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.tvOS;


public class PlayerPairManager : MonoBehaviour
{
    [SerializeField] private int maxPairNumber;
    private DevicePairUnit[] devicePairUnits;

    [field: SerializeField] public List<DevicePairUnit> PairedUnit { get; private set; }
    public int PairedNum => PairedUnit.Count;

    private bool allReady;
    public bool AllReady
    {
        get => allReady;
        private set
        {
            if (allReady == value) return;
            allReady = value;
            OnChangeAllReady?.Invoke(allReady);
        }
    }

    [SerializeField] private bool enableFinalCheck;
    public bool EnableFinalCheck
    {
        get => enableFinalCheck;
        private set
        {
            if (enableFinalCheck == value) return;
            enableFinalCheck = value;
            OnEnableFinalCheck?.Invoke(enableFinalCheck);
        }
    }

    public bool AllCheck;
    public bool AllNotCheck;
    
    
    public event Action<DevicePairUnit> OnDevicePair;
    public event Action<DevicePairUnit> OnDeviceUnpair;
    public event Action<DevicePairUnit, bool> OnDeviceChangeReady;
    public event Action<bool> OnChangeAllReady;
    public event Action<bool> OnEnableFinalCheck;
    public event Action<DevicePairUnit, bool> OnDeviceChangeFinalCheck;
    
    private void OnDisable()
    {
        UnregisterEvent();
    }


    public void InitSetup()
    {
        devicePairUnits = new DevicePairUnit[maxPairNumber];
        for (var i = 0; i < maxPairNumber; i++) devicePairUnits[i] = new DevicePairUnit(i);

        PairedUnit = new List<DevicePairUnit>();

        RegisterEvent();
    }


    public void StartListenUnpairDevice()
    {
        InputUser.listenForUnpairedDeviceActivity = maxPairNumber;
    }


    public void StopListenUnpairDevice()
    {
        InputUser.listenForUnpairedDeviceActivity = 0;
    }


    public void UpdateSelf()
    {
        if (AllReady)
        {
            foreach (var _ in PairedUnit.Where(unit => unit.PressReady)) EnableFinalCheck = true;
        }

        if (AllNotCheck)
        {
            foreach (var _ in PairedUnit.Where(unit => unit.PressCancel)) EnableFinalCheck = false;
        }


        foreach (var unit in devicePairUnits)
        {
            unit.UpdateSelf();
        }
    }


    private void RegisterEvent()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;

        foreach (var unit in devicePairUnits)
        {
            unit.OnPairDevice += PairDeviceEvent;
            unit.OnUnpairDevice += UnpairDeviceEvent;
            unit.OnChangeReady += DeviceChangeReadyEvent;
            unit.OnChangeFinalCheck += DeviceChangeFinalCheckEvent;
        }

        OnEnableFinalCheck += UpdateAllEnableFinalCheck;
    }


    private void UnregisterEvent()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        OnEnableFinalCheck -= UpdateAllEnableFinalCheck;
    }


    private void OnUnpairedDeviceUsed(InputControl c, InputEventPtr e)
    {
        if(c.device.GetType() == Mouse.current.GetType()) return;
        if (!(c.device.GetType() == Keyboard.current.GetType() || c.device.GetType() == Gamepad.current.GetType()))
            return;
        
        for (var i = 0; i < maxPairNumber; i++)
        {
            var pairUnit = devicePairUnits[i];
            var pairSuccess = pairUnit.TryPairDevice(c.device);

            if (!pairSuccess) continue;
            return;
        }
    }


    public void UnpairDevice(DevicePairUnit unit)
    {
        unit.UnpairDevice();
    }


    public void UnpairAllDevice()
    {
        if(PairedNum == 0) return;
        
        foreach (var unit in PairedUnit)
        {
            // Remove unit from PairedUnit will cause error
            unit.OnUnpairDevice -= UnpairDeviceEvent;
            
            unit.UnpairDevice();
        }
        PairedUnit.Clear();
    }


    private void UpdateAllReady()
    {
        AllReady = PairedNum > 0 && PairedUnit.Aggregate(true, (current, unit) => current & unit.IsReady);
    }


    private void UpdateAllCheck(out bool allCheck, out bool allNotCheck)
    {
        allCheck = PairedUnit.Aggregate(true, (current, unit) => current & unit.IsFinalCheck);
        allNotCheck = PairedUnit.Aggregate(true, (current, unit) => current & !unit.IsFinalCheck);
    }


    public void UpdateAllEnableFinalCheck(bool enable)
    {
        foreach(var unit in devicePairUnits) unit.EnableFinalCheck(enable);
    }


    private void PairDeviceEvent(DevicePairUnit unit)
    {
        PairedUnit.Add(unit);
        OnDevicePair?.Invoke(unit);
        UpdateAllReady();
    }


    private void UnpairDeviceEvent(DevicePairUnit unit)
    {
        PairedUnit.Remove(unit);
        OnDeviceUnpair?.Invoke(unit);
        UpdateAllReady();
    }


    private void DeviceChangeReadyEvent(DevicePairUnit unit, bool isReady)
    {
        OnDeviceChangeReady?.Invoke(unit, isReady);
        UpdateAllReady();
    }


    private void DeviceChangeFinalCheckEvent(DevicePairUnit unit, bool isCheck)
    {
        OnDeviceChangeFinalCheck?.Invoke(unit, isCheck);
        UpdateAllCheck(out AllCheck, out AllNotCheck);        
    }
}
