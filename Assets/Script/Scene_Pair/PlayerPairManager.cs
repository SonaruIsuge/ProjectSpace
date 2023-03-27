
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerPairManager : MonoBehaviour
{
    [SerializeField] private int maxPairNumber;
    private DevicePairUnit[] devicePairUnits;

    public List<DevicePairUnit> PairedUnit { get; private set; }
    public int PairedNum => PairedUnit.Count;
    public bool AllReady => PairedNum > 0 && PairedUnit.Aggregate(true, (current, unit) => current & unit.IsReady);

    public event Action<DevicePairUnit> OnDevicePair;
    public event Action<DevicePairUnit> OnDeviceUnpair;
    public event Action<DevicePairUnit, bool> OnDeviceChangeReady;

    
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
        for (var i = 0; i < PairedNum; i++)
        {
            PairedUnit[i].UpdateSelf();
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
        }
    }


    private void UnregisterEvent()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
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


    private void PairDeviceEvent(DevicePairUnit unit)
    {
        PairedUnit.Add(unit);
        OnDevicePair?.Invoke(unit);
    }


    private void UnpairDeviceEvent(DevicePairUnit unit)
    {
        PairedUnit.Remove(unit);
        OnDeviceUnpair?.Invoke(unit);
    }


    private void DeviceChangeReadyEvent(DevicePairUnit unit, bool isReady)
    {
        OnDeviceChangeReady?.Invoke(unit, isReady);
    }
}
