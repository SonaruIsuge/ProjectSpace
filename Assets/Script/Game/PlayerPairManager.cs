
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


public class PlayerPairManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    private List<PlayerPairingUnit> allPlayerPairUnit;
    private int pairedPlayerNum;
    
    [SerializeField] private bool allReady;

    public event Action<Player> OnPlayerPair;
    public event Action<Player, int, bool> OnPlayerChangeReadyState;
    public event Action OnAllPlayerReady;
    

    private void Awake()
    {

        allPlayerPairUnit = new List<PlayerPairingUnit>();
        foreach (var player in players) allPlayerPairUnit.Add(new PlayerPairingUnit(player));
        pairedPlayerNum = 0;

        allReady = false;
    }


    private void OnEnable()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;

        foreach (var pairUnit in allPlayerPairUnit)
        {
            pairUnit.OnChangeReady += PairedPlayerChangeReady;
        }

        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        
        foreach (var pairUnit in allPlayerPairUnit)
        {
            pairUnit.OnChangeReady -= PairedPlayerChangeReady;
        }
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
        
        foreach (var pairUnit in allPlayerPairUnit)
        {
            var pairSuccess = pairUnit.TryPairPlayerWithDevice(c, e);
            
            if(!pairSuccess) continue;
            
            pairUnit.Player.SetActive(true);
            pairedPlayerNum++;
            OnPlayerPair?.Invoke(pairUnit.Player);
            return;
        }
    }


    public void UnpairAllDevice()
    {
        foreach (var pairUnit in allPlayerPairUnit.Where(pairUnit => pairUnit.IsPaired))
        {
            pairUnit.UnpairDevice();
        }
    }


    private void UpdateReady()
    {
        if(pairedPlayerNum == 0) return;

        foreach (var pairUnit in allPlayerPairUnit.Where(pairUnit => pairUnit.IsPaired))
        {
            pairUnit.UpdateReady();
        }

        if (allReady)
        {
            foreach (var pairUnit in allPlayerPairUnit.Where(pairUnit => !pairUnit.IsPaired))
            {
                pairUnit.Player.gameObject.SetActive(false);
            }

            StopListenUnpairDevice();
            OnAllPlayerReady?.Invoke();
        }
    }


    private void PairedPlayerChangeReady(PlayerPairingUnit unit, bool isReady)
    {
        OnPlayerChangeReadyState?.Invoke(unit.Player, allPlayerPairUnit.IndexOf(unit), isReady);
        
        allReady = true;
        foreach (var pairUnit in allPlayerPairUnit.Where(pairUnit => pairUnit.IsPaired)) allReady &= pairUnit.IsReady;
    }
}
