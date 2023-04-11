
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class MachineManager : MonoBehaviour
{
    [SerializeField] private NormalSeparatorMachine normalSeparatorMachine;
    [SerializeField] private GravityControlMachine gravityControlMachine;
    [SerializeField] private RecycleMachine combustibleRecycleMachine;
    [SerializeField] private RecycleMachine nonCombustibleRecycleMachine;
    [SerializeField] private RecycleMachine recyclableRecycleMachine;
    [SerializeField] private RecycleMachine toxicRecycleMachine;

    private List<IMachine> allMachine;
    private Dictionary<RecycleType, Transform> machineTypeDict;

    public bool SeparatorWorking => normalSeparatorMachine.IsWorking;

    public event Action<Item> OnItemProducedByMachine;
    
    private bool isStart;
    public void SetStart(bool start) => isStart = start;

    private void Awake()
    {
        allMachine = new List<IMachine>
        {
            normalSeparatorMachine,
            gravityControlMachine,
            combustibleRecycleMachine,
            nonCombustibleRecycleMachine,
            recyclableRecycleMachine,
            toxicRecycleMachine
        };

        machineTypeDict = new Dictionary<RecycleType, Transform>
        {
            { RecycleType.Combustible, combustibleRecycleMachine ? combustibleRecycleMachine.transform : null},
            { RecycleType.NonCombustible, nonCombustibleRecycleMachine ? nonCombustibleRecycleMachine.transform : null},
            { RecycleType.Recyclable, recyclableRecycleMachine ? recyclableRecycleMachine.transform : null },
            { RecycleType.ToxicSubstances, toxicRecycleMachine ? toxicRecycleMachine.transform : null },
            { RecycleType.Energy , gravityControlMachine ? gravityControlMachine.transform : null },
            { RecycleType.CannotRecycle, normalSeparatorMachine ? normalSeparatorMachine.transform : null }
        };
    }


    private void OnEnable()
    {
        if(normalSeparatorMachine) normalSeparatorMachine.OnNewItemOutput += ItemProduced;
    }
    

    public void InitialSetUp()
    {
        foreach (var machine in allMachine)
        {
            if(machine == null) continue;
            
            machine.SetUp();
            machine.SetActive(true);
        }
    }


    public Transform GetMachineByType(RecycleType type)
    {
        return machineTypeDict.ContainsKey(type) ? machineTypeDict[type] : null;
    }


    private void Update()
    {
        if(!isStart) return;

        foreach (var machine in allMachine)
        {
            if(machine is not { IsActive: true }) continue;

            machine.Work();
        }
    }


    private void ItemProduced(Item item)
    {
        OnItemProducedByMachine?.Invoke(item);
    }
}
