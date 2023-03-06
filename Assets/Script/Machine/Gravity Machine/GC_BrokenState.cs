
using UnityEngine;

public class GC_BrokenState : IGravityControlMachineState
{
    public GravityControlMachine Machine { get; private set; }


    public GC_BrokenState(GravityControlMachine machine)
    {
        Machine = machine;
    }
    

    public void Enter()
    {
        Machine.debugTestMaterial.color = Color.red;
    }

    public void Stay()
    {
        if(!Machine.IsBroken) Machine.ChangeState(MachineStateType.Working);
    }

    public void Exit()
    {
        
    }
}
