
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
    //[field: SerializeField] public List<Player> Players { get; private set; }
    //public List<PlayerPairingUnit> allPlayerPairUnit { get; private set; }

    private DevicePairUnit[] devicePairUnits;
    private int pairedPlayerNum;

    //public event Action<PlayerPairingUnit> OnPlayerPair;
    public event Action<DevicePairUnit> OnDevicePair;
    public event Action<DevicePairUnit> OnDeviceUnpair;
    

    private void OnDisable()
    {
        UnregisterEvent();
    }


    public void InitSetup()
    {
        //allPlayerPairUnit = new List<PlayerPairingUnit>();
        //for (var i = 0; i < maxPairNumber; i++) allPlayerPairUnit.Add(new PlayerPairingUnit(i));

        devicePairUnits = new DevicePairUnit[maxPairNumber];
        for (var i = 0; i < maxPairNumber; i++) devicePairUnits[i] = new DevicePairUnit();

        pairedPlayerNum = 0;

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


    private void RegisterEvent()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;

        foreach (var unit in devicePairUnits)
        {
            unit.OnPairDevice += PairDeviceEvent;
            unit.OnUnpairDevice += UnpairDeviceEvent;
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
            
            pairedPlayerNum++;
            return;
        }
    }


    public void UnpairDevice(DevicePairUnit unit)
    {
        unit.UnpairDevice();
    }


    public void UnpairAllDevice()
    {
        if(pairedPlayerNum == 0) return;
        
        foreach (var pairedDevice in devicePairUnits.Where(unit => unit.IsPaired))
        {
            pairedDevice.UnpairDevice();
        }
    }


    private void PairDeviceEvent(DevicePairUnit unit)
    {
        OnDevicePair?.Invoke(unit);
    }


    private void UnpairDeviceEvent(DevicePairUnit unit)
    {
        OnDeviceUnpair?.Invoke(unit);
    }
}
