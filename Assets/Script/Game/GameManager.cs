
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Game Data")] 
    [SerializeField] private float gameTimeLimit;
    
    [Header("Other Managers")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraController cameraController;
    
    [Header("Hint Scripts")]
    [SerializeField] private RotateCamHintUI rotateCamHintUI;
    [SerializeField] private ItemDestinationHint curveHint;
    
    private SimpleTimer gameTimer;
    private bool startGameProgress;

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

        startGameProgress = false;

        //curveHint = GetComponent<ItemDestinationHint>();
    }


    private void BindManager()
    {
        playerManager.OnRotateCameraCall += cameraController.RotateCam;
        //playerManager.OnRotateCameraCall += uiManager.ShowPlayerIcon;
        playerManager.OnPlayerActive += uiManager.BindActivePlayerUI;
        
        if (rotateCamHintUI)
        {
            playerManager.OnPlayerActive += rotateCamHintUI.BindIconWithPlayer;
            playerManager.OnRotateCameraCall += rotateCamHintUI.ShowIcon;
        }

        machineManager.OnItemProducedByMachine += itemManager.RegisterItemEvent;
        itemManager.OnItemStartInteract += uiManager.ShowItemHint;
        itemManager.OnItemStartInteract += playerManager.NewItemInteractPlayer;
        itemManager.OnItemStartInteract += SetCurveHint;
        itemManager.OnItemEndInteract += playerManager.RemovePlayerInteractItem;

        uiManager.OnPressReplay += ReStartGame;
        uiManager.OnPressBackToPair += BackToPair;

        OnGameOver += GameOver;
    }


    private void UnbindManager()
    {
        playerManager.OnRotateCameraCall -= cameraController.RotateCam;
        //playerManager.OnRotateCameraCall -= uiManager.ShowPlayerIcon;
        playerManager.OnPlayerActive -= uiManager.BindActivePlayerUI;
        
        if (rotateCamHintUI)
        {
            playerManager.OnPlayerActive -= rotateCamHintUI.BindIconWithPlayer;
            playerManager.OnRotateCameraCall -= rotateCamHintUI.ShowIcon;
        }
        
        machineManager.OnItemProducedByMachine -= itemManager.RegisterItemEvent;
        itemManager.OnItemStartInteract -= uiManager.ShowItemHint;
        itemManager.OnItemStartInteract -= playerManager.NewItemInteractPlayer;
        itemManager.OnItemStartInteract -= SetCurveHint;
        itemManager.OnItemEndInteract -= playerManager.RemovePlayerInteractItem;
        
        uiManager.OnPressReplay -= ReStartGame;
        uiManager.OnPressBackToPair -= BackToPair;
        
        OnGameOver -= GameOver;
    }


    private async void Start()
    {
        BindManager();
        LoadPairedPlayer();

        await PlayStartAni(0.2f);

        GameStart();
    }
    

    private void Update()
    {
        foreach (var _ in playerManager.ActivePlayers.Where(p => p.PlayerInput.TapInteract))
            startGameProgress = true;
        
        if(!startGameProgress) return;   
        
        playerManager.SetWorldRotate(cameraController.CurrentRotate);
        
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
            playerManager.BindPlayerWithDevice(new Dictionary<int, InputDevice> { { 0, Keyboard.current.device } });
            return;
        }
        
        var pairedDevices = pairingData.PairingPlayers;
        playerManager.BindPlayerWithDevice(pairedDevices);
    }


    private async Task PlayStartAni(float delay)
    {
        await uiManager.ShowStartAni(delay, () => startGameProgress = true);
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
        var data = GameFlowManager.Instance.SceneData;
        GameFlowManager.Instance.LoadScene(1, data);
    }

    private void BackToPair()
    {
        UnbindManager();
        GameFlowManager.Instance.LoadScene(0, null);
    }
    
    
    // Side hint
    private async void SetCurveHint(Item item, Player player)
    {
        if(!curveHint) return;
        var machine = machineManager.GetMachineByType(DataManager.Instance.GetRecycleType(item.ItemData.type));
        await curveHint.SpawnCurve(player.HeadPoint.position, machine.position);
    }
}
