
using System;
using System.Threading.Tasks;
using SonaruUtilities;
using UnityEngine;


public class GameManager : TSingletonMonoBehaviour<GameManager>
{
    [SerializeField] private PlayerPairManager playerPairManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;

    public static event Action OnGameOver;
    
    private bool IsGameOver;
    public int RemainRubbish => itemManager.GetItemInStageNum();
    
    protected override void Awake()
    {
        base.Awake();
        IsGameOver = false;
    }


    private void OnEnable()
    {
        playerPairManager.OnPlayerPair += playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair += uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState += uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady += uiManager.AllPlayerReady;

        uiManager.OnAllReadyUIFinish += GameStart;
        uiManager.OnAllReadyUIFinish += playerPairManager.StopListenUnpairDevice;

        OnGameOver += GameOver;
    }


    private void OnDisable()
    {
        playerPairManager.OnPlayerPair -= playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair -= uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState -= uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady -= uiManager.AllPlayerReady;
        
        uiManager.OnAllReadyUIFinish -= GameStart;
        uiManager.OnAllReadyUIFinish -= playerPairManager.StopListenUnpairDevice;
        
        OnGameOver -= GameOver;
    }
    

    private void Update()
    {
        uiManager.UpdateItemRemainText(RemainRubbish);
        
        if(RemainRubbish == 0 & !IsGameOver) OnGameOver?.Invoke();
    }


    private void GameStart()
    {
        playerManager.SetStart(true);
        
        itemManager.SetStart(true);
        itemManager.InitialSetUp();
        
        machineManager.SetStart(true);
        machineManager.InitialSetUp();
    }


    private async void GameOver()
    {
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);
        
        await Task.Delay(1000);

        uiManager.SetGameOverUI();
    }
}
