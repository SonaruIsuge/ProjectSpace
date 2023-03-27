using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PairingSceneManager : MonoBehaviour
{
    [SerializeField] private int minPlayerNum;
    
    [SerializeField] private PlayerPairManager pairManager;
    [SerializeField] private PairingSceneUIManager uiManager;
    [SerializeField] private PlayerPairActManager playerActManager;
    
    [SerializeField] private bool isAllReady;
    //private List<PlayerPairingUnit> allPairedUnits;
    private List<DevicePairUnit> allPairedUnits;
    private bool underReadyProgress;
    
    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        //allPairedUnits = new List<PlayerPairingUnit>();
        allPairedUnits = new List<DevicePairUnit>();
        isAllReady = false;
        underReadyProgress = false;
    }
    
    
    private void Start()
    {
        pairManager.InitSetup();
        pairManager.StartListenUnpairDevice();
        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnEnable()
    {
        //pairManager.OnPlayerPair += RecordPairedUnit;
        //pairManager.OnPlayerPair += uiManager.AddReadyText;
        pairManager.OnDevicePair += RecordPairedUnit;
        pairManager.OnDeviceUnpair += RemoveUnpairUnit;
    }


    private void OnDisable()
    {
        //pairManager.OnPlayerPair -= RecordPairedUnit;
        //pairManager.OnPlayerPair -= uiManager.AddReadyText;
        pairManager.OnDevicePair -= RecordPairedUnit;
        pairManager.OnDeviceUnpair -= RemoveUnpairUnit;
    }


    private void Update()
    {
        for (var i = 0; i < allPairedUnits.Count; i++)
        {
            allPairedUnits[i].UpdateSelf();
        }

        if (allPairedUnits.Count >= minPlayerNum && isAllReady) AllReady();
    }


    private void RecordPairedUnit(DevicePairUnit unit)
    {
        unit.OnChangeReady += UpdateReadyState;
        allPairedUnits.Add(unit);

        //unit.Player.gameObject.SetActive(true);
    }


    private void RemoveUnpairUnit(DevicePairUnit unit)
    {
        if(!allPairedUnits.Contains(unit)) return;
        
        unit.OnChangeReady -= UpdateReadyState;
        allPairedUnits.Remove(unit);
    }


    private void UpdateReadyState(DevicePairUnit unit, bool isReady)
    {
        var playerIndex = allPairedUnits.IndexOf(unit);
        uiManager.PlayerChangeReady(playerIndex, isReady);
        
        isAllReady = true;
        foreach (var pairUnit in allPairedUnits) isAllReady &= pairUnit.IsReady;
    }


    private async void AllReady()
    {
        if(underReadyProgress) return;
        
        underReadyProgress = true;
        pairManager.StopListenUnpairDevice();
        OnAllPlayerReady?.Invoke();

        await Task.Delay(2000);
        
        var allPairedDevice = new InputDevice[allPairedUnits.Count];
        for (var i = 0; i < allPairedUnits.Count; i++) allPairedDevice[i] = allPairedUnits[i].InputDevice;
        pairManager.UnpairAllDevice();
        GameFlowManager.Instance.LoadScene(1, new PairingData(allPairedDevice));
    }
}
