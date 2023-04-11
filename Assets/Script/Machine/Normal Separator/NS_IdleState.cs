
using UnityEngine;

public class NS_IdleState : INormalSeparatorState
{
    public NormalSeparatorMachine Machine { get; }


    public NS_IdleState(NormalSeparatorMachine machine)
    {
        Machine = machine;
    }
    
    
    public void Enter()
    {
        
    }

    public void Stay()
    {
        DetectItemInput();
        
        if(Machine.IsBroken) Machine.ChangeState(MachineStateType.Broken);
        else if(Machine.IsWorking) Machine.ChangeState(MachineStateType.Working);
    }

    public void Exit()
    {
        
    }


    private void DetectItemInput()
    {
        var objects = new Collider[5];
        Physics.OverlapSphereNonAlloc(Machine.InputPoint.position, Machine.InputDetectRange, objects);
        
        var minDistance = Mathf.Infinity;
        Item separateItem = null;
        foreach (var obj in objects)
        {
            if(!obj || !obj.TryGetComponent<Item>(out var item)) continue;

            var itemRecycle = DataManager.Instance.GetRecycleType(item.ItemData.type);
            if(itemRecycle != RecycleType.CannotRecycle) continue;
            
            var distance = Vector3.Distance(Machine.transform.position, item.transform.position);
            if (distance >= minDistance) continue;
            
            minDistance = distance;
            separateItem = item;
        }

        if (separateItem != null)
        {
            if(separateItem.isInteract) separateItem.ForceDisconnect();
            Machine.Input(separateItem);
        }
        
    }
}