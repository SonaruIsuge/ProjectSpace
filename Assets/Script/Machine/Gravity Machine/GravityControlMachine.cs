using System;
using System.Collections.Generic;
using UnityEngine;


public class GravityControlMachine : MonoBehaviour, IMachine, IInteractable
{
    public InteractType InteractType { get; private set;}
    public bool IsActive { get; private set; }
    public bool isInteract { get; private set;}
    public bool isSelect { get; private set;}
    public bool IsWorking { get; private set; }
    public bool IsBroken { get; private set; }

    [field: SerializeField] public Vector3 GravityRadius { get; private set; }
    [field: SerializeField] public float GravityStrength { get; private set; }
    [field: SerializeField] public Transform GravityRangeVisual { get; private set; }

    private Dictionary<MachineStateType, IGravityControlMachineState> machineStateDict;
    private MachineStateType currentState;

    public IPowerConsumable PowerConsumable { get; private set; }


    public void SetActive(bool active) => IsActive = active;
    
    
    public void SetUp()
    {
        IsBroken = false;
        IsWorking = true;
        
        PowerConsumable = GetComponent<IPowerConsumable>();
        PowerConsumable?.InitialSetup();

        if (PowerConsumable != null)
        {
            PowerConsumable.OnPowerRunOut += () => ChangeState(MachineStateType.NoPower);
            PowerConsumable.OnPowerCharged += () => ChangeState(MachineStateType.Working);
        }

        machineStateDict = new Dictionary<MachineStateType, IGravityControlMachineState>
        {
            { MachineStateType.Working, new GC_WorkingState(this) },
            { MachineStateType.NoPower , new GC_NoPowerState(this) },
            { MachineStateType.Broken, new GC_BrokenState(this) },
        };
        ChangeState(MachineStateType.Working);
    }


    public void Work()
    {
        machineStateDict[currentState].Stay();
    }

    public void GetDamage()
    {
        IsBroken = true;
    }

    
    public void OnSelect()
    {
        isSelect = true;
    }

    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        if (IsBroken) Repair();
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
    
    
    private void Repair()
    {
        IsBroken = false;
    }


}
