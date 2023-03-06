
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GC_WorkingState : IGravityControlMachineState
{
    public GravityControlMachine Machine { get; private set; }
    private List<IGravityAffectable> gravityAffectableRecordList;

    public GC_WorkingState(GravityControlMachine machine)
    {
        Machine = machine;
        
        gravityAffectableRecordList = new List<IGravityAffectable>();
    }
    

    public void Enter()
    {
        Machine.debugTestMaterial.color = Color.cyan;
    }

    public void Stay()
    {
        if (Machine.IsBroken)
        {
            Machine.ChangeState(MachineStateType.Broken);
            return;
        }
        
        ClearAllGravity();
        DetectGravity();
        ApplyGravityToAll();
        Machine.PowerConsumable?.ConsumePower();
    }

    public void Exit()
    {
        ClearAllGravity();
        ApplyGravityToAll();
    }


    private void DetectGravity()
    {
        var results = new RaycastHit[50];
        var size = Physics.SphereCastNonAlloc(Machine.transform.position - Vector3.up, Machine.GravityRange, Vector3.up, results, 0);

        foreach (var result in results)
        {
            if(!result.transform) return;
            
            var isAffectable = result.transform.TryGetComponent<IGravityAffectable>(out var affectable);
            if (result.transform.GetComponentInParent<IGravityAffectable>() != null) affectable = result.transform.GetComponentInParent<IGravityAffectable>();
            if(!isAffectable && affectable == null) continue;
            
            if(!gravityAffectableRecordList.Contains(affectable)) gravityAffectableRecordList.Add(affectable);
            affectable.UnderGravity = true;
        }
    }
    
    
    private void ClearAllGravity()
    {
        foreach (var affectable in gravityAffectableRecordList) affectable.UnderGravity = false;
    }
    
    
    private void ApplyGravityToAll()
    {
        foreach(var affectable in gravityAffectableRecordList) affectable.ApplyGravity(Machine.GravityStrength);
    }
}
