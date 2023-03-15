
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
        Machine.GravityRangeVisual.gameObject.SetActive(true);
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
        var size = Physics.BoxCastNonAlloc(Machine.GravityRangeVisual.position, Machine.GravityRadius, Vector3.up, results, quaternion.identity, 0);

        foreach (var result in results)
        {
            if(!result.transform) return;
            
            // check if inner oval
            if(!IsInnerOval(result.transform.position, Machine.GravityRangeVisual.position, Machine.GravityRadius)) continue;

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


    private bool IsInnerOval(Vector3 targetPoint, Vector3 ovalPoint, Vector3 ovalSize)
    {
        var x = Mathf.Pow(targetPoint.x - ovalPoint.x, 2) / Mathf.Pow(ovalSize.x, 2);
        var y = Mathf.Pow(targetPoint.y - ovalPoint.y, 2) / Mathf.Pow(ovalSize.y, 2);
        var z = Mathf.Pow(targetPoint.z - ovalPoint.z, 2) / Mathf.Pow(ovalSize.z, 2);

        return x + y + z <= 1;
    }
}
