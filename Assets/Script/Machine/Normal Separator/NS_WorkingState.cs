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
        
        Machine.progressBg.gameObject.SetActive(true);
    }

    public void Stay()
    {
        SetProgressUI();
        if(Machine.ProgressTimer.IsFinish) OutputItem(Machine.CurrentProcessingItem);
        
        if(Machine.IsBroken) Machine.ChangeState(MachineStateType.Broken);
        else if(!Machine.IsWorking) Machine.ChangeState(MachineStateType.Idle);
    }

    public void Exit()
    {
        Machine.progressBg.gameObject.SetActive(false);
    }


    private void OutputItem(Item inputItem)
    {
        var data = inputItem.ItemData;
        var produceList = UnityTool.GetNoRepeatElement(data.ProductNumber, data.SeparateProducets);

        for (var i = 0; i < produceList.Count; i++)
        {
            var productObj = ItemManager.Instance.GetItem(produceList[i].type);
            Machine.Output(productObj);
        }
    }


    private void SetProgressUI()
    {
        var progress = 1f - Machine.ProgressTimer.Remain01;
        var width = Machine.progressBg.rect.width * progress;
        Machine.progressContent.sizeDelta = new Vector2(width, Machine.progressBg.rect.height);
    }
}
