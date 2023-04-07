
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
            { RecycleType.Combustible, combustibleRecycleMachine.transform },
            { RecycleType.NonCombustible, nonCombustibleRecycleMachine.transform },
            { RecycleType.Recyclable, recyclableRecycleMachine.transform },
            { RecycleType.ToxicSubstances, toxicRecycleMachine.transform },
            { RecycleType.Energy , gravityControlMachine.transform },
            { RecycleType.CannotRecycle, normalSeparatorMachine.transform }
        };
    }


    private void OnEnable()
    {
        normalSeparatorMachine.OnNewItemOutput += ItemProduced;
    }
    

    public void InitialSetUp()
    {
        foreach (var machine in allMachine)
        {
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
            if(!machine.IsActive) continue;
            
            machine.Work();
        }
    }


    private void ItemProduced(Item item)
    {
        OnItemProducedByMachine?.Invoke(item);
    }
}
