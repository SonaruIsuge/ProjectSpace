
using System;
using System.Threading.Tasks;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerPairManager playerPairManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraController cameraController;

    public static event Action OnGameOver;
    
    private bool IsGameOver;
    public int RemainRubbish => itemManager.GetItemInStageNum();
    
    private void Awake()
    {
        IsGameOver = false;
        playerPairManager.InitialSetUp();
        
        playerManager.SetStart(false);
        itemManager.SetStart(false);
        machineManager.SetStart(false);
    }


    private void BindManager()
    {
        playerPairManager.OnPlayerPair += playerManager.AddActivePlayer;
        playerPairManager.OnPlayerPair += uiManager.PlayerPair;
        
        playerPairManager.OnPlayerChangeReadyState += uiManager.PlayerReady;
        
        playerPairManager.OnAllPlayerReady += uiManager.AllPlayerReady;

        playerManager.OnRotateCameraCall += cameraController.RotateCam;

        machineManager.OnItemProducedByMachine += itemManager.ItemAppear;

        uiManager.OnAllReadyUIFinish += GameStart;
        uiManager.OnAllReadyUIFinish += playerPairManager.StopListenUnpairDevice;
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
        
        machineManager.OnItemProducedByMachine -= itemManager.ItemAppear;
        
        uiManager.OnAllReadyUIFinish -= GameStart;
        uiManager.OnAllReadyUIFinish -= playerPairManager.StopListenUnpairDevice;
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
        uiManager.UpdateItemRemainText(RemainRubbish);
        
        if(RemainRubbish == 0 && !machineManager.SeparatorWorking && !IsGameOver) OnGameOver?.Invoke();
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

        IsGameOver = true;
        await Task.Delay(1000);

        uiManager.SetGameOverUI();
    }


    private void ReStartGame()
    {
        playerPairManager.UnpairAllDevice();
        UnbindManager();
        SceneManager.LoadScene(0);
    }
}
