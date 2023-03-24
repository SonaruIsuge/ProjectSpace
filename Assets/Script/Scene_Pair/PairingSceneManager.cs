using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;


public class PairingSceneManager : MonoBehaviour
{
    [SerializeField] private int minPlayerNum;
    [SerializeField] private PlayerPairManager pairManager;
    
    private List<PlayerPairingUnit> allPairedUnits;
    [SerializeField] private bool isAllReady;

    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        allPairedUnits = new List<PlayerPairingUnit>();
        isAllReady = false;
    }
    
    
    private void Start()
    {
        pairManager.InitSetup();
        pairManager.StartListenUnpairDevice();
        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnEnable()
    {
        pairManager.OnPlayerPair += RecordPairedUnit;
    }


    private void OnDisable()
    {
        pairManager.OnPlayerPair -= RecordPairedUnit;
    }


    private void Update()
    {
        foreach (var unit in allPairedUnits)
        {
            unit.UpdateReady();
        }

        if (allPairedUnits.Count >= minPlayerNum && isAllReady) AllReady();
    }


    private void RecordPairedUnit(PlayerPairingUnit unit)
    {
        unit.OnChangeReady += UpdateReadyState;
        allPairedUnits.Add(unit);
        
        unit.Player.gameObject.SetActive(true);
    }


    private void UpdateReadyState(PlayerPairingUnit unit, bool isReady)
    {
        isAllReady = true;
        foreach (var pairUnit in allPairedUnits) isAllReady &= pairUnit.IsReady;
    }


    private void AllReady()
    {
        DataManager.Instance.SavePairedPlayer(allPairedUnits);
        pairManager.UnpairAllDevice();
        OnAllPlayerReady?.Invoke();
        
        GameFlowManager.Instance.LoadScene(1, null);
    }
}
