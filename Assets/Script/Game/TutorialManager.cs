using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private TutorialView tutorialView;

    [SerializeField] private List<TutorialFragment> tutorialList;

    private int currentRemain;
    private Dictionary<TutorialCondition, Func<bool>> conditionFuncDict;

    private void OnEnable()
    {
        GameManager.OnGameStart += StartTutorial;
    }


    private void Start()
    {
        conditionFuncDict = new Dictionary<TutorialCondition, Func<bool>>
        {
            { TutorialCondition.None, null },
            { TutorialCondition.PlayerJet, PlayerFly },
            { TutorialCondition.ItemRecycled , ItemRecycled }
        };
    }


    private async void StartTutorial()
    {
        // disable all player and machine feature at start
        foreach (var player in playerManager.ActivePlayers)
        {
            player.EnableMove(false);
            player.EnableJet(false);
            player.EnableInteract(false);
        }
        machineManager.EnableRecycle(false);
        machineManager.EnableSeparator(false);

        // start tutorial
        foreach (var tutorial in tutorialList)
        {
            tutorial.onStart.AddListener(() => tutorialView.SetTextArea(tutorial.hintText) );
            tutorial.onStart.AddListener(tutorialView.ShowTextFrame);
            tutorial.onComplete.AddListener(tutorialView.HideTextFrame);
            
            tutorial.SetConditionFunc(conditionFuncDict[tutorial.triggerCondition]);
            await tutorial.StartTutorial();
            
            tutorial.ResetEvents();
        }
    }


    public void EnableMove()
    {
        foreach (var player in playerManager.ActivePlayers)
        {
            player.EnableMove(true);
        }
    }


    public void EnableJet()
    {
        foreach (var player in playerManager.ActivePlayers)
        {
            player.EnableJet(true);
        }
    }


    public void EnableInteract()
    {
        foreach (var player in playerManager.ActivePlayers) player.EnableInteract(true);
    }


    public void EnableRecycle() => machineManager.EnableRecycle(true);
    
    
    public void EnableSeparator() => machineManager.EnableSeparator(true);


    public void RecordRemainRubbish() => currentRemain = itemManager.RemainItemNum;

    private bool PlayerFly() => playerManager.ActivePlayers.Any(player => player.PlayerInput.JetDirection > 0);


    private bool ItemRecycled() => itemManager.RemainItemNum < currentRemain;
}


[System.Serializable]
public class TutorialFragment
{
    public float waitTime;
    public TutorialCondition triggerCondition;
    public float duringTime;
    [TextArea(2, 5)] public string hintText;
    public UnityEvent onStart;
    public UnityEvent onUpdate;
    public UnityEvent onComplete;
    public Func<bool> endCondition;


    public void SetConditionFunc(Func<bool> func)
    {
        endCondition = func;
    }
    
    
    public async Task StartTutorial()
    {
        if (endCondition != null)
        {
            var conditionComplete = (endCondition == null);
            while (!conditionComplete)
            {
                conditionComplete = endCondition.Invoke();
                await Task.Yield();
            }
        }
        
        else await Task.Delay((int)(waitTime * 1000));
        onStart?.Invoke();
        
        float timer = 0;
        while (timer < duringTime)
        {
            onUpdate?.Invoke();
            timer += Time.deltaTime;
            await Task.Yield();
        }

        onComplete?.Invoke();
    }


    public void ResetEvents()
    {
        onStart.RemoveAllListeners();
        onUpdate.RemoveAllListeners();
        onComplete.RemoveAllListeners();
    }
}