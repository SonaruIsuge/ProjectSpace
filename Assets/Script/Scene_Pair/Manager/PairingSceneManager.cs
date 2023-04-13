using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;


public class PairingSceneManager : MonoBehaviour
{
    [Header("Data")]
    public int minPlayerNum;
    
    [field: Header("Managers")]
    [field: SerializeField] public PlayerPairManager PairManager { get; private set; }
    [field: SerializeField] public PairingSceneUIManager PairUIManager{ get; private set; }
    [field: SerializeField] public MainMenuUIManager MainMenuUIManager { get; private set; }
    [field: SerializeField] public TutorialHintUIManager TutorialHintUIManager { get; private set; }
    //[field: SerializeField] public LevelMenuUIManager LevelMenuUIManager { get; private set; }
    [field: SerializeField] public PlayerPairActManager PlayerActManager { get; private set; }
    
    [Header("Transition")]
    [SerializeField] private Animator lobbyCutSceneAni;
    
    private EventSystem EventSystem => EventSystem.current;
    private InputActionAsset actionsAsset;
    
    private static readonly int Direction = Animator.StringToHash("Direction");
    private bool isPlayCutSceneAni;
    
    private bool underReadyProgress;
    //private bool finalCheck;

    private SceneIndex targetScene;

    public event Action UpdateEvent;
    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        underReadyProgress = false;
        //finalCheck = false;

        isPlayCutSceneAni = false;
        actionsAsset = EventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset;
    }
    
    
    private void Start()
    {
        MainMenuUIManager.InitMainMenuUI();
        PairManager.InitSetup();
        PairUIManager.InitPairUI();
        PlayerActManager.ResetPlayersPosition();

        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnEnable()
    {
        MainMenuUIManager.SetStartEvent(true, StartPairing);
        MainMenuUIManager.SetQuitEvent(true, QuitGame);
        
        TutorialHintUIManager.BindYseBtn(StartTutorial);
        TutorialHintUIManager.BindNoBtn(StartNormalGame);

        PairManager.OnBackToLastStage += CancelPairing;
        PairManager.OnDeviceChangeReady += ChangeReadyEvent;
        PairManager.OnDevicePair += PairEvent;
        PairManager.OnDeviceUnpair += UnpairEvent;
        PairManager.OnChangeAllReady += ChangeAllReadyEvent;

        PairManager.OnEnableFinalCheck += EnableFinalCheckEvent;
        PairManager.OnDeviceChangeFinalCheck += ChangeFinalCheckEvent;
    }


    private void OnDisable()
    {
        PairManager.OnBackToLastStage -= CancelPairing;
        PairManager.OnDeviceChangeReady -= ChangeReadyEvent;
        PairManager.OnDevicePair -= PairEvent;
        PairManager.OnDeviceUnpair -= UnpairEvent;
        PairManager.OnChangeAllReady -= ChangeAllReadyEvent;
        
        PairManager.OnEnableFinalCheck -= EnableFinalCheckEvent;
        PairManager.OnDeviceChangeFinalCheck -= ChangeFinalCheckEvent;
    }


    private void Update()
    {
        UpdateEvent?.Invoke();
    }


    private async void StartPairing()
    {
        FXController.Instance.InitSFX(SFXType.ButtonCheck);
        
        var hideUIEnd = false;
        MainMenuUIManager.HideMainMenu(() => hideUIEnd = true);
        while (!hideUIEnd) await Task.Yield();
        
        await Task.Delay(250);
        
        await PlayCutScene(null);
        await Task.Delay(500);
        
        PairManager.StartListenUnpairDevice();
        PairUIManager.EnableOriginPairUI(true);
        

        //UpdateEvent -= LevelChooseUpdate;
        UpdateEvent += PairingUpdate;
    }


    private void QuitGame()
    {
        FXController.Instance.InitSFX(SFXType.ButtonCancel);
        
        Application.Quit();
    }


    private async void CancelPairing()
    {
        PairUIManager.EnableOriginPairUI(false);
        
        UpdateEvent -= PairingUpdate;
        
        await PlayCutScene(null, true);
        
        MainMenuUIManager.ShowMainMenu();
    }


    private void PairingUpdate()
    {
        PairManager.UpdateSelf();
        PlayerActManager.UpdatePlayerAct();

        if (PairManager.PairedNum >= minPlayerNum && PairManager.AllCheck) StartGame();
    }


    private async Task PlayCutScene(Action onComplete, bool isReverse = false)
    {
        if(isPlayCutSceneAni) return;
        isPlayCutSceneAni = true;
        
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
        
        isPlayCutSceneAni = false;
    }

    
    // Disable input -> Clear UI -> Player animation -> camera zoom out & UI animation
    private async void StartGame()
    {
        // Prevent call start game twice
        if(underReadyProgress) return;
        underReadyProgress = true;
        
        // Stop listen all devices' input
        PairManager.StopListenUnpairDevice();
        foreach(var unit in PairManager.PairedUnit) unit.EnableInput(false);
        
        OnAllPlayerReady?.Invoke();
        
        // Arrange start game animation
        await AddTask(.2f, PairUIManager.ActiveStartGroup);

        await Task.Delay(800);
        for (var i = 0; i < PairManager.PairedUnit.Count; i++)
        {
            var player = PairManager.PairedUnit[i];
            await AddTask( i * 0.25f, player.CharacterIndex, PlayerActManager.SetPlayerReadyAni);
        }
        
        await AddTask(1.5f, PairUIManager.PlayStartAni);
        
        await AddTask(1.5f, () => TutorialHintUIManager.ShowUI() );
        //DelayDo(ChangeScene, 2f);
    }


    private void StartTutorial()
    {
        FXController.Instance.InitSFX(SFXType.ButtonCheck);
        targetScene = SceneIndex.Tutorial;
        TutorialHintUIManager.HindUI(ChangeScene);
    }


    private void StartNormalGame()
    {
        FXController.Instance.InitSFX(SFXType.ButtonCancel);
        targetScene = SceneIndex.FirstLevel1;
        TutorialHintUIManager.HindUI(ChangeScene);
    }
    

    private void ChangeScene()
    {
        var pairedDict = PairManager.PairedUnit.ToDictionary(unit => unit.CharacterIndex, unit => unit.InputDevice);
        PairManager.UnpairAllDevice();
        GameFlowManager.Instance.LoadScene((int)targetScene, new PairingData(pairedDict));
    }
    
    
    // private void StartLevelChoosing()
    // {
    //     MainMenuUIManager.EnableMainMenuPanel(false);
    //     LevelMenuUIManager.EnableLevelPanel(true);
    //     UpdateEvent += LevelChooseUpdate;
    // }
    //
    //
    // private void SetLevel(SceneIndex sceneIndex)
    // {
    //     targetScene = sceneIndex;
    //     if (sceneIndex == SceneIndex.None) targetScene = SceneIndex.MainMenu;
    //     else StartPairing();
    // }
    //
    //
    // private void LevelChooseUpdate()
    // {
    //     if (!actionsAsset.FindAction("Cancel").WasPressedThisFrame()) return;
    //     // To main menu
    //     MainMenuUIManager.EnableMainMenuPanel(true);
    //     LevelMenuUIManager.EnableLevelPanel(false);
    //     UpdateEvent -= LevelChooseUpdate;
    // }

    

    #region Bind Event Here
    
    private void ChangeReadyEvent(DevicePairUnit unit, bool isReady)
    {
        PairUIManager.PlayerChangeReady(unit, isReady);
        
    }
    
    
    private void PairEvent(DevicePairUnit unit)
    {
        PlayerActManager.MovePlayerIn(unit.CharacterIndex);
        PairUIManager.PlayerPair(unit);
    }
    
    
    private void UnpairEvent(DevicePairUnit unit)
    {
        PairUIManager.PlayerChangeReady(unit, false);
        PlayerActManager.MovePlayerOut(unit.CharacterIndex);
        PairUIManager.PlayerUnpair(unit);
    }


    private void ChangeAllReadyEvent(bool isAllReady)
    {
        PairUIManager.ToggleFinalCheckHint(isAllReady);
    }


    private void EnableFinalCheckEvent(bool enableFinalCheck)
    {
        PairUIManager.ShowAllReadyPanel(enableFinalCheck);
        PairUIManager.ToggleFinalCheckHint(!enableFinalCheck);
        
        if(enableFinalCheck) PairManager.StopListenUnpairDevice();
        else PairManager.StartListenUnpairDevice();
    }


    private void ChangeFinalCheckEvent(DevicePairUnit unit, bool check)
    {
        PairUIManager.SetStartGameIconFocus(unit, check);
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
    
    # region Task Function

    private async Task AddTask(float delay, Action onComplete)
    {
        await Task.Delay((int)(delay * 1000));
        onComplete?.Invoke();
    }
    
    private async Task AddTask<T>(float delay, T param, Action<T> onComplete)
    {
        await Task.Delay((int)(delay * 1000));
        onComplete?.Invoke(param);
    }
    
    #endregion
}
