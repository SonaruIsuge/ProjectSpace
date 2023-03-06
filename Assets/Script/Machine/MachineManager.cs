
using System;
using System.Collections.Generic;
using UnityEngine;


public class MachineManager : MonoBehaviour
{
    [SerializeField] private NormalSeparatorMachine normalSeparatorMachine;
    [SerializeField] private GravityControlMachine gravityControlMachine;
    [SerializeField] private RecycleMachine combustibleRecycleMachine;
    [SerializeField] private RecycleMachine nonCombustibleRecycleMachine;
    [SerializeField] private RecycleMachine energyRecycleMachine;
    [SerializeField] private RecycleMachine toxicRecycleMachine;

    private List<IMachine> allMachine;


    private void Awake()
    {
        allMachine = new List<IMachine>
        {
            normalSeparatorMachine,
            gravityControlMachine,
            combustibleRecycleMachine,
            nonCombustibleRecycleMachine,
            energyRecycleMachine,
            toxicRecycleMachine
        };


        foreach (var machine in allMachine)
        {
            machine.SetUp();
            machine.SetActive(true);
        }
    }


    private void Update()
    {
        foreach (var machine in allMachine)
        {
            if(!machine.IsActive) continue;
            
            machine.Work();
        }
    }
}
