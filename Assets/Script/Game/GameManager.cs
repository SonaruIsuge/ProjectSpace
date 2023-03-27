
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Game Data")] 
    [SerializeField] private float gameTimeLimit;
    [SerializeField] private List<Player> players;
    
    [Header("Other Managers")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraController cameraController;

    private SimpleTimer gameTimer;
    private List<PlayerPairingUnit> pairedPlayerUnit;

    private event Action<Player> OnPlayerPaired;
    public static event Action OnGameStart;
    public static event Action<bool> OnGameOver;
    
    private bool IsGameOver;

    private void Awake()
    {
        IsGameOver = false;
        pairedPlayerUnit = new List<PlayerPairingUnit>();
        
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);
        
        gameTimer = new SimpleTimer(gameTimeLimit);
        gameTimer.Pause();
    }


    private void BindManager()
    {
        playerManager.OnRotateCameraCall += cameraController.RotateCam;
        machineManager.OnItemProducedByMachine += itemManager.RegisterItemEvent;

        OnPlayerPaired += uiManager.PlayerPair;
        OnGameOver += GameOver;

        uiManager.OnPressReplay += ReStartGame;
    }


    private void UnbindManager()
    {
        playerManager.OnRotateCameraCall -= cameraController.RotateCam;
        machineManager.OnItemProducedByMachine -= itemManager.RegisterItemEvent;

        OnPlayerPaired -= uiManager.PlayerPair;
        OnGameOver -= GameOver;
        
        uiManager.OnPressReplay -= ReStartGame;
    }


    private void Start()
    {
        BindManager();
        
        LoadPairedPlayer();

        GameStart();
    }
    

    private void Update()
    {
        playerManager.SetWorldRotate(cameraController.CurrentRotate - 180);
        
        uiManager.UpdateItemRemain(itemManager.RemainItemNum);
        uiManager.UpdateTimeRemain(gameTimer.Remain, gameTimer.Remain01);
        
        if(gameTimer.IsFinish) OnGameOver?.Invoke(false);
        else if(itemManager.RemainItemNum == 0 && !machineManager.SeparatorWorking && !IsGameOver) OnGameOver?.Invoke(true);
    }


    private void LoadPairedPlayer()
    {
        if (GameFlowManager.Instance.SceneData is not PairingData pairingData)
        {
            Debug.LogError("Can not get pairing data");
            return;
        }
        
        var pairedDevices = pairingData.PairingPlayers;
        for (var i = 0; i < pairedDevices.Length; i++)
        {
            var unit = new PlayerPairingUnit(i);
            unit.TryPairPlayerWithDevice(players[i], pairedDevices[i]);
            pairedPlayerUnit.Add(unit);
            
            unit.Player.SetActive(true);
            playerManager.AddActivePlayer(unit.Player);
            OnPlayerPaired?.Invoke(unit.Player);
        }
    }


    private void GameStart()
    {
        playerManager.SetStart(true);
        
        itemManager.SetStart(true);
        itemManager.InitialSetUp();
        
        machineManager.SetStart(true);
        machineManager.InitialSetUp();
        
        uiManager.SetGameStartUI();

        gameTimer.Resume();
        
        OnGameStart?.Invoke();
        
        FXController.Instance.ChangeBGM(BGMType.MainGamePlay);
    }


    private async void GameOver(bool isWin)
    {
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);
        foreach(var unit in pairedPlayerUnit) unit.UnpairDevice();
        
        IsGameOver = true;
        gameTimer.Pause();
        await Task.Delay(1000);

        uiManager.SetGameOverUI(isWin, gameTimeLimit - gameTimer.Remain);
    }


    private void ReStartGame()
    {
        UnbindManager();
        
        SceneManager.LoadScene(0);
    }
}
