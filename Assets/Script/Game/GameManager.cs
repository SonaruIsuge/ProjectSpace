
using System;
using SonaruUtilities;
using UnityEngine;


public class GameManager : TSingletonMonoBehaviour<GameManager>
{
    [SerializeField] private PlayerPairManager playerPairManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;

    public int RemainRubbish => itemManager.GetItemInStageNum();
    
    protected override void Awake()
    {
        base.Awake();
        
    }


    private void OnEnable()
    {
        playerPairManager.OnPlayerPair += playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair += uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState += uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady += uiManager.AllPlayerReady;

        uiManager.OnAllReadyUIFinish += GameStart;
        uiManager.OnAllReadyUIFinish += playerPairManager.StopListenUnpairDevice;
    }


    private void OnDisable()
    {
        playerPairManager.OnPlayerPair -= playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair -= uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState -= uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady -= uiManager.AllPlayerReady;
        
        uiManager.OnAllReadyUIFinish -= GameStart;
        uiManager.OnAllReadyUIFinish -= playerPairManager.StopListenUnpairDevice;
    }
    

    private void Update()
    {
        uiManager.UpdateItemRemainText(itemManager.GetItemInStageNum());
    }


    private void GameStart()
    {
        playerManager.SetStart(true);
        
        machineManager.SetStart(true);
        
        itemManager.SetStart(true);
        itemManager.InitialSetUp();
    }
}
