
using UnityEngine;

public class GC_NoPowerState : IGravityControlMachineState
{
    public GravityControlMachine Machine { get; private set; }


    public GC_NoPowerState(GravityControlMachine machine)
    {
        Machine = machine;
    }
    

    public void Enter()
    {
        Machine.debugTestMaterial.color = new Color(1, 0.57f, 0);
        Machine.GravityRangeVisual.gameObject.SetActive(false);
    }

    public void Stay()
    {
        
    }

    public void Exit()
    {
        
    }
}
