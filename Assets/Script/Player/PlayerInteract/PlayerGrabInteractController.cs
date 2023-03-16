

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
        targetPlayer.transform.position + Vector3.Scale(targetPlayer.transform.rotation * clawLocalOriginPos, targetPlayer.transform.localScale);

    
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
        // Item in hand -> cannot pick up other item
        if(CurrentInteract is Item) return;
        
        CurrentDetect = null;
        currentDetectCollider = null;
        
        var detectResults = new Collider[10];
        Physics.OverlapSphereNonAlloc(interactCenter , interactRange, detectResults);
        
        var minDistance = Mathf.Infinity;
        foreach (var result in detectResults)
        {
            if(!result || !result.transform.TryGetComponent<IInteractable>(out var interactable)) continue;
            
            if(interactable == CurrentInteract) continue;

            var clawPos = ClawTransform.position;
            var closePoint = result.ClosestPoint(clawPos);
            var distance = Vector3.Distance(closePoint, clawPos);

            if (distance >= minDistance) continue;
            
            // Physics.Raycast(ClawWorldOriginPos, closePoint - ClawWorldOriginPos, out var hitInfo, distance);
            // Debug.DrawRay(ClawWorldOriginPos, closePoint - ClawWorldOriginPos, Color.magenta);
            // if(hitInfo.transform != null) Debug.Log(hitInfo.transform.name);
            // if(hitInfo.transform != null && hitInfo.transform != result.transform) continue;

            minDistance = distance;
            CurrentDetect = interactable;
            currentDetectCollider = result;
        }
        if(currentDetectCollider)
            Debug.DrawLine(ClawWorldOriginPos, currentDetectCollider.ClosestPoint(ClawTransform.position), Color.red);
    }

    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        // First check if item in hand and drop down it
        if (CurrentInteract is Item)
        {
            CurrentInteract.Interact(interactPlayer, interactType);
            return;
        }
        CurrentInteract?.Interact(interactPlayer, interactType);
        
        // Nothing in hand -> can interact with other interactable
        if(CurrentDetect is Item item)
        {
            if(item.InteractType == interactType) CatchItem(item, interactPlayer, interactType);
        }
        else CurrentDetect?.Interact(interactPlayer, interactType);
    }


    private async void CatchItem(Item item, Player interactPlayer, InteractType interactType)
    {
        await AttachItem();

        if(item.ItemData.Size is ItemSize.Large or ItemSize.ExtraLarge) await PushPlayer();
        else await CatchItem(item);

        item.Interact(interactPlayer, interactType);
    }


    private async Task AttachItem()
    {
        var isAttach = false;
        while (!isAttach)
        {
            var clawPos = ClawTransform.position;
            var targetPoint = currentDetectCollider.ClosestPoint(clawPos);
            ClawTransform.LookAt(targetPoint);
            ClawTransform.position = Vector3.MoveTowards(clawPos, targetPoint, targetPlayer.ClawSpeed * Time.deltaTime);

            isAttach = Vector3.Distance(targetPoint, clawPos) < 0.01f;
            await Task.Yield();
        }
    }
    
    
    private async Task CatchItem(Item item)
    {
        var getItem = false;
        while (!getItem)
        {
            var clawPos = ClawTransform.position;
            var itemHitPos = currentDetectCollider.ClosestPoint(clawPos);
            var itemCenterHitDiff = itemHitPos - currentDetectCollider.transform.position;

            ClawTransform.LookAt(clawPos - ClawWorldOriginPos);
            ClawTransform.position =
                Vector3.MoveTowards(clawPos, ClawWorldOriginPos, targetPlayer.ClawSpeed * Time.deltaTime);
            item.transform.position = Vector3.MoveTowards(item.transform.position,
                ClawWorldOriginPos - itemCenterHitDiff, targetPlayer.ClawSpeed * Time.deltaTime);

            getItem = Vector3.Distance(ClawWorldOriginPos, itemHitPos) < 0.01f;
            await Task.Yield();
        }
        
        item.Rb.velocity = Vector3.zero;
    }


    private async Task PushPlayer()
    {
        var getItem = false;
        var currentItemPoint = currentDetectCollider.ClosestPoint(ClawTransform.position);

        if (targetPlayer.Cc) targetPlayer.Cc.enabled = false;

        while (!getItem)
        {
            var playerPos = targetPlayer.transform.position;
            var clawPos = ClawTransform.position;
            var itemHitPos = currentDetectCollider.ClosestPoint(clawPos);
            var clawPlayerWorldDiff = playerPos - ClawWorldOriginPos;

            ClawTransform.LookAt(clawPos - ClawWorldOriginPos);
            ClawTransform.position =
                Vector3.MoveTowards(clawPos, currentItemPoint, targetPlayer.ClawSpeed * Time.deltaTime);
            targetPlayer.transform.position = Vector3.MoveTowards(playerPos, currentItemPoint + clawPlayerWorldDiff,
                targetPlayer.ClawSpeed * Time.deltaTime);

            getItem = Vector3.Distance(playerPos, clawPos) <= clawPlayerWorldDiff.magnitude;
            await Task.Yield();
        }

        if (targetPlayer.Cc) targetPlayer.Cc.enabled = true;
    }
}
