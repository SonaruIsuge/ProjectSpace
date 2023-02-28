using SonaruUtilities;
using UnityEngine;

public class NS_WorkingState : INormalSeparatorState
{
    public NormalSeparatorMachine Machine { get; }


    public NS_WorkingState(NormalSeparatorMachine machine)
    {
        Machine = machine;
    }
    
    
    public void Enter()
    {
        Machine.ProgressTimer.Reset(Machine.ProgressTime);
        Machine.debugTestMaterial.color = Color.yellow;
    }

    public void Stay()
    {
        
        if(Machine.ProgressTimer.IsFinish) OutputItem(Machine.CurrentProcessingItem);
        
        if(Machine.IsBroken) Machine.ChangeState(MachineStateType.Broken);
        if(!Machine.IsWorking) Machine.ChangeState(MachineStateType.Idle);
    }

    public void Exit()
    {
        
    }


    private void OutputItem(Item inputItem)
    {
        var data = inputItem.ItemData;
        var produceList = UnityTool.GetNoRepeatElement(data.ProductNumber, data.SeparateProducets);

        for (var i = 0; i < produceList.Count; i++)
        {
            var productObj = ItemManager.Instance.GetItem(produceList[i].type);
            Machine.Output(productObj, i);
        }
    }
}
