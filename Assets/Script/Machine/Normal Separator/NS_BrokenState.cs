﻿

using UnityEngine;

public class NS_BrokenState : INormalSeparatorState
{
    public NormalSeparatorMachine Machine { get; }


    public NS_BrokenState(NormalSeparatorMachine machine)
    {
        Machine = machine;
    }
    
    
    public void Enter()
    {
        
    }

    
    public void Stay()
    {
        if(!Machine.IsBroken) Machine.ChangeState(MachineStateType.Idle);
    }

    
    public void Exit()
    {
        
    }
}
