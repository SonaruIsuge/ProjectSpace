using System;
using System.Collections.Generic;
using SonaruUtilities;
using Unity.Mathematics;
using UnityEngine;


public class NormalSeparatorMachine : MonoBehaviour, IMachine, IInteractable
{
    public InteractType InteractType => InteractType.Tap; 
    public bool IsWorking { get; private set; }
    public bool IsBroken { get; private set; }
    public bool isInteract { get; private set; }
    public bool isSelect { get; private set; }

    [field: SerializeField] public Transform InputPoint { get; private set; }
    [field: SerializeField] public Transform[] OutputPoint { get; private set; }
    
    [field: SerializeField] public float ProgressTime { get; private set;}
    [field: SerializeField] public SimpleTimer ProgressTimer { get; private set; }

    private Dictionary<MachineStateType, INormalSeparatorState> machineStateDict;
    private MachineStateType currentState;

    [field: SerializeField] public Item CurrentProcessingItem { get; private set; }

    public event Action<Item> OnItemSeparated;
    public event Action<Item> OnNewItemOutput;

    public Material debugTestMaterial;

    private void Awake()
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


    private void Update()
    {
        machineStateDict[currentState]?.Stay();
    }
    
    
    public void Work()
    {
        
    }
    

    public void GetDamage()
    {
        IsBroken = true;

        if (CurrentProcessingItem != null)
        {
            CurrentProcessingItem.gameObject.SetActive(true);
            CurrentProcessingItem.Rb.AddForce(InputPoint.up * 0.5f, ForceMode.Impulse);
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
        CurrentProcessingItem.gameObject.SetActive(false);
        
        OnItemSeparated?.Invoke(inputItem);
    }


    public void Output(GameObject output, int outputLocation)
    {
        var newItemObj = Instantiate(output);
        var outputItem = newItemObj.GetComponent<Item>();
        var itemSize = outputItem.GetItemCollisionSize();
        
        newItemObj.transform.position = OutputPoint[outputLocation].position + Vector3.Scale(OutputPoint[outputLocation].forward, itemSize);
        newItemObj.transform.rotation = quaternion.identity;
        outputItem.Rb.AddForce(OutputPoint[outputLocation].forward * 0.5f, ForceMode.Impulse);
        
        CurrentProcessingItem = null;
        IsWorking = false;
        OnNewItemOutput?.Invoke(outputItem); 
    }


    private void Repair()
    {
        IsBroken = false;
    }
}
