
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;


public class PlayerPairManager : MonoBehaviour
{
    [field: SerializeField] public List<Player> Players { get; private set; }
    public List<PlayerPairingUnit> allPlayerPairUnit { get; private set; }
    private int pairedPlayerNum;

    public event Action<PlayerPairingUnit> OnPlayerPair;


    private void OnDisable()
    {
        UnregisterEvent();
    }


    public void InitSetup()
    {
        allPlayerPairUnit = new List<PlayerPairingUnit>();
        for (var i = 0; i < Players.Count; i++) allPlayerPairUnit.Add(new PlayerPairingUnit(i));

        pairedPlayerNum = 0;

        RegisterEvent();
    }


    public void StartListenUnpairDevice()
    {
        InputUser.listenForUnpairedDeviceActivity = 4;
    }


    public void StopListenUnpairDevice()
    {
        InputUser.listenForUnpairedDeviceActivity = 0;
    }


    private void RegisterEvent()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
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
        Debug.Log("pair");
        for (var i = 0; i < allPlayerPairUnit.Count; i++)
        {
            var pairUnit = allPlayerPairUnit[i];
            var pairSuccess = pairUnit.TryPairPlayerWithDevice(Players[i], c.device);

            if (!pairSuccess) continue;

            pairUnit.Player.SetActive(true);
            pairedPlayerNum++;
            OnPlayerPair?.Invoke(pairUnit);
            return;
        }
    }


    public void UnpairAllDevice()
    {
        if(allPlayerPairUnit.Count == 0) return;
        
        foreach (var pairUnit in allPlayerPairUnit.Where(pairUnit => pairUnit.IsPaired))
        {
            pairUnit.UnpairDevice();
        }
    }
}
