

using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;


public class PlayerGrabInteractController : IPlayerInteract
{
    private Player targetPlayer;
    public IInteractable CurrentDetect { get; private set; }
    private Collider currentDetectCollider;
    public IInteractable CurrentInteract { get; private set; }
    private Transform interactPoint => targetPlayer.InteractPoint;
    private float interactRange => targetPlayer.InteractRange;

    private Transform clawTransform => targetPlayer.ClawTransform;
    private Vector3 clawLocalPos;


    public PlayerGrabInteractController(Player player)
    {
        targetPlayer = player;
        CurrentDetect = null;
        CurrentInteract = null;
        clawLocalPos = clawTransform.localPosition;
    }
    
    
    public void UpdateInteract()
    {
        CurrentDetect?.OnDeselect();
        DetectInteractable(interactPoint.position, interactRange);
        CurrentDetect?.OnSelect();
    }
    
    
    public void SetCurrentInteract(IInteractable interactable)
    {
        CurrentInteract = interactable;
    }
    

    public void DetectInteractable(Vector3 interactCenter, float range)
    {
        CurrentDetect = null;
        currentDetectCollider = null;
        
        var detectResults = new Collider[10];
        var playerForward = targetPlayer.transform.forward;
        Physics.OverlapSphereNonAlloc(interactCenter , interactRange, detectResults);
        
        var minDistance = Mathf.Infinity;
        foreach (var result in detectResults)
        {
            if(!result || !result.transform.TryGetComponent<IInteractable>(out var interactable)) continue;
            
            if(interactable == CurrentInteract) continue;

            var distance = Vector3.Distance(result.transform.position, clawTransform.position);
            if (distance >= minDistance || distance == 0) continue;
            
            minDistance = distance;
            CurrentDetect = interactable;
            currentDetectCollider = result;
        }
        if(currentDetectCollider)
            Debug.DrawLine(interactCenter - playerForward, currentDetectCollider.ClosestPoint(clawTransform.position), Color.red);
    }

    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        CurrentInteract?.Interact(interactPlayer, interactType);
        
        if (CurrentDetect is Item item)
        {
            if(item.InteractType == interactType) CatchItem(item, interactPlayer, interactType);
        }
        else CurrentDetect?.Interact(interactPlayer, interactType);
    }


    private async void CatchItem(Item item, Player interactPlayer, InteractType interactType)
    {
        var itemHitPos = currentDetectCollider.ClosestPoint(clawTransform.position);
        var clawFireOrigin = clawTransform.position;
        var pointCenterDiff = clawFireOrigin - itemHitPos;
        var centerTargetPos = item.transform.position + pointCenterDiff;

        // set claw speed is range/per sec.
        clawTransform.DOMove(itemHitPos, .2f);
        await Task.Delay(200);
        
        item.transform.DOMove(centerTargetPos, .2f);
        clawTransform.DOLocalMove(clawLocalPos, .2f);
        await Task.Delay(200);
        
        item.Interact(interactPlayer, interactType);
    }
}
