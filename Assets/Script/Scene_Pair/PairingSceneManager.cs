using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PairingSceneManager : MonoBehaviour
{
    [SerializeField] private int minPlayerNum;
    [SerializeField] private PlayerPairManager pairManager;
    [SerializeField] private PairingSceneUIManager uiManager;
    [SerializeField] private PlayerPairActManager playerActManager;

    private bool underReadyProgress;
    private bool finalCheck;
    
    public event Action OnAllPlayerReady;

    
    private void Awake()
    {
        underReadyProgress = false;
        finalCheck = false;
    }
    
    
    private void Start()
    {
        pairManager.InitSetup();
        pairManager.StartListenUnpairDevice();
        uiManager.Init();
        playerActManager.ResetPlayersPosition();

        FXController.Instance.ChangeBGM(BGMType.ChooseCharacter);
    }


    private void OnEnable()
    {
        pairManager.OnDeviceChangeReady += ChangeReadyEvent;
        pairManager.OnDevicePair += PairEvent;
        pairManager.OnDeviceUnpair += UnpairEvent;
        pairManager.OnChangeAllReady += ChangeAllReadyEvent;

        pairManager.OnEnableFinalCheck += EnableFinalCheckEvent;
        pairManager.OnDeviceChangeFinalCheck += ChangeFinalCheckEvent;
    }


    private void OnDisable()
    {
        pairManager.OnDeviceChangeReady -= ChangeReadyEvent;
        pairManager.OnDevicePair -= PairEvent;
        pairManager.OnDeviceUnpair -= UnpairEvent;
        pairManager.OnChangeAllReady -= ChangeAllReadyEvent;
        
        pairManager.OnEnableFinalCheck -= EnableFinalCheckEvent;
        pairManager.OnDeviceChangeFinalCheck -= ChangeFinalCheckEvent;
    }


    private void Update()
    {
        pairManager.UpdateSelf();
        playerActManager.UpdatePlayerAct();
        
        if(pairManager.PairedNum >= minPlayerNum && pairManager.AllCheck) StartGame();
    }

    
    private void ResetReadyUI(DevicePairUnit unit)
    {
        uiManager.PlayerChangeReady(unit, false);
    }

    
    // Disable input -> Clear UI -> Player animation -> camera zoom out & UI animation
    private void StartGame()
    {
        if(underReadyProgress) return;
        underReadyProgress = true;

        StopInput();
        OnAllPlayerReady?.Invoke();
        
        DelayDo(uiManager.SwitchStartGroup, 0.2f);

        for (var i = 0; i < pairManager.PairedUnit.Count; i++)
        {
            var player = pairManager.PairedUnit[i];
            DelayDo(playerActManager.SetPlayerReadyAni, player.CharacterIndex, 1 + i * 0.25f);
        }
        
        DelayDo(uiManager.PlayStartAni, 3f);

        DelayDo(ChangeScene, 5f);
    }


    private void StopInput()
    {
        pairManager.StopListenUnpairDevice();
        
        foreach(var unit in pairManager.PairedUnit) unit.EnableInput(false);
    }


    private void ChangeScene()
    {
        var pairedDict = pairManager.PairedUnit.ToDictionary(unit => unit.CharacterIndex, unit => unit.InputDevice);
        pairManager.UnpairAllDevice();
        GameFlowManager.Instance.LoadScene(1, new PairingData(pairedDict));
    }


    
    
    
    private void ChangeReadyEvent(DevicePairUnit unit, bool isReady)
    {
        uiManager.PlayerChangeReady(unit, isReady);
        
    }
    
    
    private void PairEvent(DevicePairUnit unit)
    {
        playerActManager.MovePlayerIn(unit.CharacterIndex);
        uiManager.PlayerPair(unit);
    }
    
    
    private void UnpairEvent(DevicePairUnit unit)
    {
        ResetReadyUI(unit);
        playerActManager.MovePlayerOut(unit.CharacterIndex);
        uiManager.PlayerUnpair(unit);
    }


    private void ChangeAllReadyEvent(bool isAllReady)
    {
        
    }


    private void EnableFinalCheckEvent(bool enableFinalCheck)
    {
        uiManager.ShowAllReadyPanel(enableFinalCheck);
        
        if(enableFinalCheck) pairManager.StopListenUnpairDevice();
        else pairManager.StartListenUnpairDevice();
    }


    private void ChangeFinalCheckEvent(DevicePairUnit unit, bool check)
    {
        uiManager.SetStartGameIconFocus(unit, check);   
    }
    
    
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
