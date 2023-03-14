

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
    private Vector3 playerClawDiff;


    public PlayerGrabInteractController(Player player)
    {
        targetPlayer = player;
        CurrentDetect = null;
        CurrentInteract = null;
        
        clawLocalPos = clawTransform.localPosition;
        playerClawDiff = clawTransform.transform.position - player.transform.position;
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
        var itemCenterHitDiff = itemHitPos - item.transform.position;
        var clawFireOrigin = clawTransform.position;
        var pointCenterDiff = clawFireOrigin - itemHitPos;
        var centerTargetPos = item.transform.position + pointCenterDiff;
        
        // clawTransform.DOMove(itemHitPos, .2f);
        // await Task.Delay(200);

        var attachItem = false;
        var detect = currentDetectCollider;
        while (!attachItem)
        {
            var targetPoint = detect.ClosestPoint(clawTransform.position);
            clawTransform.LookAt(targetPoint);
            clawTransform.position = Vector3.MoveTowards(clawTransform.position, targetPoint, targetPlayer.ClawSpeed * Time.deltaTime);

            if (Vector3.Distance(targetPoint, clawTransform.position) < 0.01f) attachItem = true;
            await Task.Yield();
        }

        var clawReturn = false;
        var returnPos = targetPlayer.transform.position + Vector3.Scale(targetPlayer.transform.forward, clawLocalPos);
        var clawLook = clawTransform.position - returnPos;
        while (!clawReturn)
        {
            returnPos = targetPlayer.transform.position + targetPlayer.transform.rotation * clawLocalPos;
            clawTransform.LookAt(clawLook);
            clawTransform.position =
                Vector3.MoveTowards(clawTransform.position, returnPos, targetPlayer.ClawSpeed * Time.deltaTime);
            item.transform.position = Vector3.MoveTowards(item.transform.position, returnPos - itemCenterHitDiff, targetPlayer.ClawSpeed * Time.deltaTime);
            
            if (Vector3.Distance(returnPos, clawTransform.position) < 0.01f) clawReturn = true;
            await Task.Yield();
        }
        // item.transform.DOMove(centerTargetPos, .2f);
        // clawTransform.DOLocalMove(clawLocalPos, .2f);
        // await Task.Delay(200);
        
        item.Interact(interactPlayer, interactType);
    }
}
