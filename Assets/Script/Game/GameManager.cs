
using System;
using System.Threading.Tasks;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Game Data")] 
    [SerializeField] private float gameTimeLimit;
    
    [Header("Other Managers")]
    [SerializeField] private PlayerPairManager playerPairManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraController cameraController;

    private SimpleTimer gameTimer;
    
    public static event Action<bool> OnGameOver;
    
    private bool IsGameOver;

    private void Awake()
    {
        IsGameOver = false;
        playerPairManager.InitialSetUp();
        
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);
        
        gameTimer = new SimpleTimer(gameTimeLimit);
        gameTimer.Pause();
    }


    private void BindManager()
    {
        playerPairManager.OnPlayerPair += playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair += uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState += uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady += uiManager.AllPlayerReady;

        playerManager.OnRotateCameraCall += cameraController.RotateCam;

        machineManager.OnItemProducedByMachine += itemManager.RegisterItemEvent;

        uiManager.OnAllReadyUIFinish += GameStart;
        
        uiManager.OnPressReplay += ReStartGame;
        
        OnGameOver += GameOver;
    }


    private void UnbindManager()
    {
        playerPairManager.OnPlayerPair -= playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair -= uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState -= uiManager.PlayerReady;
        playerPairManager.OnAllPlayerReady -= uiManager.AllPlayerReady;
        
        playerManager.OnRotateCameraCall -= cameraController.RotateCam;
        
        machineManager.OnItemProducedByMachine -= itemManager.RegisterItemEvent;
        
        uiManager.OnAllReadyUIFinish -= GameStart;
        
        uiManager.OnPressReplay -= ReStartGame;
        
        OnGameOver -= GameOver;
    }


    private void Start()
    {
        BindManager();
    }
    

    private void Update()
    {
        playerManager.SetWorldRotate(cameraController.CurrentRotate - 180);
        uiManager.UpdateItemRemainText(itemManager.RemainItemNum);
        
        if(gameTimer.IsFinish) OnGameOver?.Invoke(false);
        else if(itemManager.RemainItemNum == 0 && !machineManager.SeparatorWorking && !IsGameOver) OnGameOver?.Invoke(true);
    }


    private void GameStart()
    {
        playerManager.SetStart(true);
        
        itemManager.SetStart(true);
        itemManager.InitialSetUp();
        
        machineManager.SetStart(true);
        machineManager.InitialSetUp();

        gameTimer.Resume();
        
        FXController.Instance.ChangeBGM(BGMType.MainGamePlay);
    }


    private async void GameOver(bool isWin)
    {
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);

        IsGameOver = true;
        gameTimer.Pause();
        await Task.Delay(1000);

        uiManager.SetGameOverUI(isWin, gameTimeLimit - gameTimer.Remain);
    }


    private void ReStartGame()
    {
        playerPairManager.UnpairAllDevice();
        UnbindManager();
        SceneManager.LoadScene(0);
    }
}
