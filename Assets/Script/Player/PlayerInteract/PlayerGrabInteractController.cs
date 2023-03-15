

using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;


public class PlayerGrabInteractController : IPlayerInteract
{
    private Player targetPlayer;
    private LineRenderer clawLineRenderer;
    public IInteractable CurrentDetect { get; private set; }
    private Collider currentDetectCollider;
    public IInteractable CurrentInteract { get; private set; }
    private Transform interactPoint => targetPlayer.InteractPoint;
    private float interactRange => targetPlayer.InteractRange;

    
    private Transform ClawTransform => targetPlayer.ClawTransform;
    private Vector3 clawLocalOriginPos;
    private Vector3 ClawWorldOriginPos =>
        targetPlayer.transform.position + targetPlayer.transform.rotation * clawLocalOriginPos;

    public PlayerGrabInteractController(Player player)
    {
        targetPlayer = player;
        CurrentDetect = null;
        CurrentInteract = null;
        
        clawLocalOriginPos = ClawTransform.localPosition;
        
        clawLineRenderer = ClawTransform.GetComponent<LineRenderer>();
    }
    
    
    public void UpdateInteract()
    {
        CurrentDetect?.OnDeselect();
        DetectInteractable(interactPoint.position, interactRange);
        CurrentDetect?.OnSelect();
        
        clawLineRenderer.SetPosition(0, ClawWorldOriginPos);
        clawLineRenderer.SetPosition(1, ClawTransform.position);
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

            var distance = Vector3.Distance(result.transform.position, ClawTransform.position);
            if (distance >= minDistance || distance == 0) continue;
            
            minDistance = distance;
            CurrentDetect = interactable;
            currentDetectCollider = result;
        }
        if(currentDetectCollider)
            Debug.DrawLine(interactCenter - playerForward, currentDetectCollider.ClosestPoint(ClawTransform.position), Color.red);
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
        var itemHitPos = currentDetectCollider.ClosestPoint(ClawTransform.position);
        var itemCenterHitDiff = itemHitPos - item.transform.position;
        var clawFireOrigin = ClawTransform.position;
        var pointCenterDiff = clawFireOrigin - itemHitPos;
        var centerTargetPos = item.transform.position + pointCenterDiff;

        var attachItem = false;
        var detect = currentDetectCollider;
        while (!attachItem)
        {
            var targetPoint = detect.ClosestPoint(ClawTransform.position);
            ClawTransform.LookAt(targetPoint);
            ClawTransform.position = Vector3.MoveTowards(ClawTransform.position, targetPoint, targetPlayer.ClawSpeed * Time.deltaTime);

            if (Vector3.Distance(targetPoint, ClawTransform.position) < 0.01f) attachItem = true;
            await Task.Yield();
        }

        var clawReturn = false;
        
        var clawLook = ClawTransform.position - ClawWorldOriginPos;
        while (!clawReturn)
        {
            ClawTransform.LookAt(clawLook);
            ClawTransform.position =
                Vector3.MoveTowards(ClawTransform.position, ClawWorldOriginPos, targetPlayer.ClawSpeed * Time.deltaTime);
            item.transform.position = Vector3.MoveTowards(item.transform.position, ClawWorldOriginPos - itemCenterHitDiff, targetPlayer.ClawSpeed * Time.deltaTime);
            
            if (Vector3.Distance(ClawWorldOriginPos, ClawTransform.position) < 0.01f) clawReturn = true;
            await Task.Yield();
        }
        
        item.Interact(interactPlayer, interactType);
    }
}
