using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PairingSceneManager : MonoBehaviour
{
    [SerializeField] private int minPlayerNum;
    [SerializeField] private PlayerPairManager pairManager;
    [SerializeField] private PairingSceneUIManager pairUIManager;
    [SerializeField] private MainMenuUIManager mainMenuUIManager;
    [SerializeField] private PlayerPairActManager playerActManager;
    
    [SerializeField] private Animator lobbyCutSceneAni;
    private static readonly int Direction = Animator.StringToHash("Direction");
    
    private bool underReadyProgress;
    private bool finalCheck;
    

    public event Action UpdateEvent;
    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        underReadyProgress = false;
        finalCheck = false;
    }
    
    
    private void Start()
    {
        mainMenuUIManager.InitMainMenuUI();
        pairManager.InitSetup();
        pairUIManager.InitPairUI();
        playerActManager.ResetPlayersPosition();
    }


    private void OnEnable()
    {
        mainMenuUIManager.BindStartEvent(StartPairing);
        mainMenuUIManager.BindQuitEvent(QuitGame);

        pairManager.OnBackToLastStage += CancelPairing;
        pairManager.OnDeviceChangeReady += ChangeReadyEvent;
        pairManager.OnDevicePair += PairEvent;
        pairManager.OnDeviceUnpair += UnpairEvent;
        pairManager.OnChangeAllReady += ChangeAllReadyEvent;

        pairManager.OnEnableFinalCheck += EnableFinalCheckEvent;
        pairManager.OnDeviceChangeFinalCheck += ChangeFinalCheckEvent;
    }


    private void OnDisable()
    {
        pairManager.OnBackToLastStage -= CancelPairing;
        pairManager.OnDeviceChangeReady -= ChangeReadyEvent;
        pairManager.OnDevicePair -= PairEvent;
        pairManager.OnDeviceUnpair -= UnpairEvent;
        pairManager.OnChangeAllReady -= ChangeAllReadyEvent;
        
        pairManager.OnEnableFinalCheck -= EnableFinalCheckEvent;
        pairManager.OnDeviceChangeFinalCheck -= ChangeFinalCheckEvent;
    }


    private void Update()
    {
        UpdateEvent?.Invoke();
    }


    private void StartPairing()
    {
        PlayCutScene(() =>
        {
            mainMenuUIManager.HideMainMenu();
            
            pairManager.StartListenUnpairDevice();
            pairUIManager.EnableOriginPairUI(true);

            FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
        
            UpdateEvent += PairingUpdate;
        });
        
       
    }


    private void QuitGame()
    {
        Application.Quit();
    }


    private void CancelPairing()
    {
        pairUIManager.EnableOriginPairUI(false);
        mainMenuUIManager.ShowMainMenu();
        UpdateEvent -= PairingUpdate;
        
        PlayCutScene(null, true);
    }
    
    
    private void PairingUpdate()
    {
        pairManager.UpdateSelf();
        playerActManager.UpdatePlayerAct();

        if (pairManager.PairedNum >= minPlayerNum && pairManager.AllCheck) StartGame();
    }


    private async void PlayCutScene(Action onComplete, bool isReverse = false)
    {
        lobbyCutSceneAni.enabled = true;
        
        lobbyCutSceneAni.SetFloat(Direction, isReverse ? -1 : 1);
        lobbyCutSceneAni.Play("A_DollyCamLobby");
        
        var timer = 0f;
        
        while (timer < lobbyCutSceneAni.GetCurrentAnimatorStateInfo(0).length)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }

        lobbyCutSceneAni.enabled = false;
        onComplete?.Invoke();
    }

    
    // Disable input -> Clear UI -> Player animation -> camera zoom out & UI animation
    private void StartGame()
    {
        // Prevent call start game twice
        if(underReadyProgress) return;
        underReadyProgress = true;
        
        // Stop listen all devices' input
        pairManager.StopListenUnpairDevice();
        foreach(var unit in pairManager.PairedUnit) unit.EnableInput(false);
        
        OnAllPlayerReady?.Invoke();
        
        // Arrange start game animation
        DelayDo(pairUIManager.ActiveStartGroup, 0.2f);

        for (var i = 0; i < pairManager.PairedUnit.Count; i++)
        {
            var player = pairManager.PairedUnit[i];
            DelayDo(playerActManager.SetPlayerReadyAni, player.CharacterIndex, 1 + i * 0.25f);
        }
        
        DelayDo(pairUIManager.PlayStartAni, 3f);

        DelayDo(ChangeScene, 5f);
    }


    private void ChangeScene()
    {
        var pairedDict = pairManager.PairedUnit.ToDictionary(unit => unit.CharacterIndex, unit => unit.InputDevice);
        pairManager.UnpairAllDevice();
        GameFlowManager.Instance.LoadScene(1, new PairingData(pairedDict));
    }

    

    #region Bind Event Here
    
    private void ChangeReadyEvent(DevicePairUnit unit, bool isReady)
    {
        pairUIManager.PlayerChangeReady(unit, isReady);
        
    }
    
    
    private void PairEvent(DevicePairUnit unit)
    {
        playerActManager.MovePlayerIn(unit.CharacterIndex);
        pairUIManager.PlayerPair(unit);
    }
    
    
    private void UnpairEvent(DevicePairUnit unit)
    {
        pairUIManager.PlayerChangeReady(unit, false);
        playerActManager.MovePlayerOut(unit.CharacterIndex);
        pairUIManager.PlayerUnpair(unit);
    }


    private void ChangeAllReadyEvent(bool isAllReady)
    {
        pairUIManager.ToggleFinalCheckHint(isAllReady);
    }


    private void EnableFinalCheckEvent(bool enableFinalCheck)
    {
        pairUIManager.ShowAllReadyPanel(enableFinalCheck);
        pairUIManager.ToggleFinalCheckHint(!enableFinalCheck);
        
        if(enableFinalCheck) pairManager.StopListenUnpairDevice();
        else pairManager.StartListenUnpairDevice();
    }


    private void ChangeFinalCheckEvent(DevicePairUnit unit, bool check)
    {
        pairUIManager.SetStartGameIconFocus(unit, check);
    }
    
    #endregion
    
    
    
    #region Delay Do Function
    
    public void DelayDo(Action onComplete, float delay)
    {
        StartCoroutine(DelayDoInner(delay, onComplete));
    }

    public void DelayDo<T>(Action<T> onComplete, T param1, float delay)
    {
        StartCoroutine(DelayDoInner<T>(delay, onComplete, param1));
    }

    private IEnumerator DelayDoInner(float delay, Action onComplete = null)
    {
        yield return new WaitForSeconds(delay);
        
        onComplete?.Invoke();
    }

    private IEnumerator DelayDoInner<T>(float delay, Action<T> onComplete, T param1)
    {
        yield return new WaitForSeconds(delay);
        
        onComplete?.Invoke(param1);
    }
    
    #endregion
}
