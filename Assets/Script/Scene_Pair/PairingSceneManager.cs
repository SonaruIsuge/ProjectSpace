using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PairingSceneManager : MonoBehaviour
{
    [SerializeField] private int minPlayerNum;
    [SerializeField] private List<Player> pairPlayers;
    [SerializeField] private PlayerPairManager pairManager;
    [SerializeField] private PairingSceneUIManager uiManager;
    [SerializeField] private PlayerPairActManager playerActManager;
    
    private bool underReadyProgress;
    
    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        underReadyProgress = false;
    }
    
    
    private void Start()
    {
        pairManager.InitSetup();
        pairManager.StartListenUnpairDevice();
        uiManager.Init();
        
        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnEnable()
    {
        pairManager.OnDeviceChangeReady += uiManager.PlayerChangeReady;
        pairManager.OnDeviceUnpair += ResetReadyUI;
    }


    private void OnDisable()
    {
        pairManager.OnDeviceChangeReady -= uiManager.PlayerChangeReady;
        pairManager.OnDeviceUnpair -= ResetReadyUI;
    }


    private void Update()
    {
        pairManager.UpdateSelf();

        if(pairManager.PairedNum >= minPlayerNum && pairManager.AllReady) StartGame();
    }

    
    private void ResetReadyUI(DevicePairUnit unit)
    {
        uiManager.PlayerChangeReady(unit, false);
    }
    

    private async void StartGame()
    {
        if(underReadyProgress) return;
        
        underReadyProgress = true;
        pairManager.StopListenUnpairDevice();
        OnAllPlayerReady?.Invoke();

        await Task.Delay(500);

        var pairedDict = pairManager.PairedUnit.ToDictionary(unit => unit.CharacterIndex, unit => unit.InputDevice);
        pairManager.UnpairAllDevice();
        GameFlowManager.Instance.LoadScene(1, new PairingData(pairedDict));
    }
}
