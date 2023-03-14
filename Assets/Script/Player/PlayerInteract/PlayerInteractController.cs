using System;
using UnityEngine;


public class PlayerInteractController : IPlayerInteract
{
    private Player targetPlayer;
    
    public IInteractable CurrentDetect { get; private set; }
    public IInteractable CurrentInteract { get; private set; }

    private Transform interactPoint => targetPlayer.InteractPoint;
    private float interactRange => targetPlayer.InteractRange;


    public PlayerInteractController(Player player)
    {
        targetPlayer = player;
        CurrentDetect = null;
        CurrentInteract = null;
    }


    public void UpdateInteract()
    {
        CurrentDetect?.OnDeselect();
        DetectInteractable(interactPoint.position, interactRange);
        CurrentDetect?.OnSelect();
    }
    
    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        CurrentInteract?.Interact(interactPlayer, interactType);
        CurrentDetect?.Interact(interactPlayer, interactType);
    }


    public void SetCurrentInteract(IInteractable interactable)
    {
        CurrentInteract = interactable;
    }
    
    
    public void DetectInteractable(Vector3 interactCenter, float range)
    {
        CurrentDetect = null;
        var detectResults = new Collider[10];
        Physics.OverlapSphereNonAlloc(interactCenter, range, detectResults);
        
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
