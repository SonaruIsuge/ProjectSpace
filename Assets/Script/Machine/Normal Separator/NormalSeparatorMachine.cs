using System;
using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;
using Random = UnityEngine.Random;


public class NormalSeparatorMachine : MonoBehaviour, IMachine, IInteractable
{
    public InteractType InteractType => InteractType.Tap;
    public bool IsActive { get; private set; }
    public bool IsWorking { get; private set; }
    public bool IsBroken { get; private set; }
    public bool isInteract { get; private set; }
    public bool isSelect { get; private set; }

    [field: SerializeField] public Transform InputPoint { get; private set; }
    [field: SerializeField] public float InputDetectRange { get; private set; }
    [field: SerializeField] public Transform OutputPoint { get; private set; }
    [field: SerializeField] public float ProgressTime { get; private set;}
    [field: SerializeField] public SimpleTimer ProgressTimer { get; private set; }

    private Dictionary<MachineStateType, INormalSeparatorState> machineStateDict;
    private MachineStateType currentState;

    [field: SerializeField] public Item CurrentProcessingItem { get; private set; }

    [field: Header("UI for Debug")]
    [field: SerializeField] public RectTransform progressBg { get; private set; }
    [field: SerializeField] public RectTransform progressContent { get; private set; }
    
    public event Action<Item> OnItemSeparated;
    public event Action<Item> OnNewItemOutput;


    public void SetActive(bool active) => IsActive = active;
    
    
    public void SetUp()
    {
        ProgressTimer = new SimpleTimer(ProgressTime);
        
        IsWorking = false;
        IsBroken = false;
        isInteract = false;
        isSelect = false;

        machineStateDict = new Dictionary<MachineStateType, INormalSeparatorState>
        {
            { MachineStateType.Idle, new NS_IdleState(this) },
            { MachineStateType.Working , new NS_WorkingState(this) },
            { MachineStateType.Broken, new NS_BrokenState(this) },
        };
        ChangeState(MachineStateType.Idle);
    }


    public void Work()
    {
        machineStateDict[currentState]?.Stay();
    }
    

    public void GetDamage()
    {
        IsBroken = true;

        if (IsWorking)
        {
            CurrentProcessingItem.AddItem();
            CurrentProcessingItem.Rb.AddForce(InputPoint.up, ForceMode.Impulse);
            CurrentProcessingItem = null;
            IsWorking = false;
        }
    }

    
    public void OnSelect()
    {
        isSelect = true;
    }

    public void Interact(Player interactPlayer, InteractType interactType)
    {
        if (interactType != InteractType) return;
        
        // check if broken -> repair it
        if(IsBroken) Repair();
    }

    public void OnDeselect()
    {
        isSelect = false;
    }


    public void ChangeState(MachineStateType newState)
    {
        if(!machineStateDict.ContainsKey(newState)) return;
        
        if(machineStateDict.ContainsKey(currentState)) machineStateDict[currentState].Exit();
        currentState = newState;
        machineStateDict[currentState].Enter();
    }


    public void Input(Item inputItem)
    {
        CurrentProcessingItem = inputItem;
        IsWorking = true;
        CurrentProcessingItem.RemoveItem();
        
        OnItemSeparated?.Invoke(inputItem);
    }


    public void Output(GameObject outputPrefab)
    {
        var newItemObj = Instantiate(outputPrefab);
        var outputItem = newItemObj.GetComponent<Item>();
        var itemSize = outputItem.GetItemCollisionSize();
        
        
        newItemObj.transform.position = OutputPoint.position + Vector3.Scale(OutputPoint.forward, itemSize);
        newItemObj.transform.rotation = Random.rotation;
        var outputVector =
            (OutputPoint.forward * Random.Range(1, 10) + OutputPoint.right * Random.Range(0, 5) +
             OutputPoint.up * Random.Range(0, 5));
        outputVector.y = Mathf.Abs(outputVector.y);
        outputItem.Rb.AddForce(outputVector, ForceMode.VelocityChange);
        
        CurrentProcessingItem = null;
        IsWorking = false;
        OnNewItemOutput?.Invoke(outputItem); 
        outputItem.AddItem();
    }


    private void Repair()
    {
        IsBroken = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(InputPoint.position, InputDetectRange);
    }
}
