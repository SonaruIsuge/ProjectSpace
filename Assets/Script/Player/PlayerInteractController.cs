using System;
using UnityEngine;


public class PlayerInteractController
{
    private Player targetPlayer;
    
    public IInteractable CurrentDetect { get; private set; }
    public IInteractable CurrentInteract { get; private set; }


    public PlayerInteractController(Player player)
    {
        targetPlayer = player;
        CurrentDetect = null;
        CurrentInteract = null;
    }


    public void UpdateInteract(Vector3 interactCenter, float interactRange)
    {
        CurrentDetect?.OnDeselect();
        DetectInteractable(interactCenter, interactRange);
        CurrentDetect?.OnSelect();
    }
    
    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        //if(CurrentInteract is Item item && interactType == item.InteractType) item.DropDown(interactPlayer);
        //if (CurrentInteract is Item item) item.Interact(interactPlayer, interactType);
        CurrentInteract?.Interact(interactPlayer, interactType);
        CurrentDetect?.Interact(interactPlayer, interactType);
        
    }


    public void SetCurrentInteract(IInteractable interactable)
    {
        CurrentInteract = interactable;
    }
    
    
    private void DetectInteractable(Vector3 interactCenter, float interactRange)
    {
        CurrentDetect = null;
        var detectResults = new Collider[10];
        Physics.OverlapSphereNonAlloc(interactCenter, interactRange, detectResults);
        
        var minDistance = Mathf.Infinity;
        foreach (var item in detectResults)
        {
            if(!item || !item.TryGetComponent<IInteractable>(out var interactable)) continue;
            
            if(interactable == CurrentInteract) continue;
            
            var distance = Vector3.Distance(targetPlayer.transform.position, item.transform.position);
            if (distance >= minDistance) continue;
            
            minDistance = distance;
            CurrentDetect = interactable;
        }
    }

    
}
