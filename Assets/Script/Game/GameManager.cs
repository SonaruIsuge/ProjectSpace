
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

    private event Action<Player> OnPlayerPaired;
    public static event Action OnGameStart;
    public static event Action<bool> OnGameOver;
    
    private bool IsGameOver;

    private void Awake()
    {
        IsGameOver = false;
        
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
            if (players[i].PlayerInput is not PlayerInput playerInput)
            {
                Debug.LogError($"Player{i} cannot pair with device");
                continue;
            }
            
            playerInput.PairWithDevice(pairedDevices[i]);
            playerManager.ActivePlayer(players[i]);
            OnPlayerPaired?.Invoke(players[i]);
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

        playerManager.RemoveAllActivePlayer();
        
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
