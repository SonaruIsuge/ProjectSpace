

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

    private Vector3 ClawWorldOriginPos => targetPlayer.ClawBaseTransform.position;
        //targetPlayer.transform.position + Vector3.Scale(targetPlayer.transform.rotation * clawLocalOriginPos, targetPlayer.transform.localScale);
    private Vector3 ClawPos => ClawTransform.position;
    private Vector3 ClawHeadPos => targetPlayer.ClawHeadTransform.position;
    private Vector3 ClawHeadDiff => ClawHeadPos - ClawPos;
    
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
        clawLineRenderer.SetPosition(1, ClawPos);
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
            
            var closePoint = result.ClosestPoint(ClawPos);
            var distance = Vector3.Distance(closePoint, ClawPos);

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
            Debug.DrawLine(ClawWorldOriginPos, currentDetectCollider.ClosestPoint(ClawPos), Color.red);
    }

    
    public void Interact(Player interactPlayer, InteractType interactType)
    {
        // First check if item in hand and drop down it
        if (CurrentInteract is Item)
        {
            CurrentInteract.Interact(interactPlayer, interactType);
            return;
        }
        
        // Nothing in hand -> can interact with other interactable
        if(CurrentDetect is Item item)
        {
            if(item.InteractType == interactType) CatchItem(item, interactPlayer, interactType);
        }
        else CurrentDetect?.Interact(interactPlayer, interactType);
    }


    private async void CatchItem(Item item, Player interactPlayer, InteractType interactType)
    {
        var successAttach = await AttachItem();
        
        if (!successAttach)
        {
            await ForceClawReturn();
            return;
        }

        bool successCatchItem;
        if(item.ItemData.Size is ItemSize.Large or ItemSize.ExtraLarge) successCatchItem = await PushPlayer();
        else successCatchItem = await CatchItem(item);
        
        if (!successCatchItem)
        {
            await ForceClawReturn();
            return;
        }

        item.Interact(interactPlayer, interactType);
    }


    private async Task<bool> AttachItem()
    {
        var isAttach = false;
        while (!isAttach)
        {
            if(!currentDetectCollider) return false;
            
            var targetPoint = currentDetectCollider.ClosestPoint(ClawHeadPos);
            ClawTransform.LookAt(targetPoint);
            ClawTransform.position = Vector3.MoveTowards(ClawPos, targetPoint - ClawHeadDiff, targetPlayer.ClawSpeed * Time.deltaTime);

            isAttach = Vector3.Distance(targetPoint, ClawHeadPos) < 0.01f;
            await Task.Yield();
        }

        return true;
    }
    
    
    private async Task<bool> CatchItem(Item item)
    {
        var getItem = false;
        while (!getItem)
        {
            if(!currentDetectCollider) return false;
            
            var itemHitPos = currentDetectCollider.ClosestPoint(ClawPos);
            var itemCenterHitDiff = itemHitPos - currentDetectCollider.transform.position;

            ClawTransform.LookAt(itemHitPos);
            ClawTransform.position =
                Vector3.MoveTowards(ClawPos, ClawWorldOriginPos, targetPlayer.ClawSpeed * Time.deltaTime);
            
            item.transform.position = Vector3.MoveTowards(item.transform.position,
                ClawWorldOriginPos + ClawHeadDiff - itemCenterHitDiff, targetPlayer.ClawSpeed * Time.deltaTime);

            getItem = Vector3.Distance(ClawWorldOriginPos, ClawPos) < 0.01f;
            await Task.Yield();
        }
        
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;
        targetPlayer.Rb.velocity = Vector3.zero;
        targetPlayer.Rb.angularVelocity = Vector3.zero;
        return true;
    }


    private async Task<bool> PushPlayer()
    {
        var getItem = false;
        
        if(!currentDetectCollider) return false;
        
        var currentItemPoint = currentDetectCollider.ClosestPoint(ClawPos);

        if (targetPlayer.Cc) targetPlayer.Cc.enabled = false;

        while (!getItem)
        {
            var playerPos = targetPlayer.transform.position;
            var clawPlayerWorldDiff = playerPos - ClawWorldOriginPos;

            //ClawTransform.LookAt(currentItemPoint);
            ClawTransform.position =
                Vector3.MoveTowards(ClawPos, currentItemPoint, targetPlayer.ClawSpeed * Time.deltaTime);
            targetPlayer.transform.position = Vector3.MoveTowards(playerPos, currentItemPoint + clawPlayerWorldDiff,
                targetPlayer.ClawSpeed * Time.deltaTime);

            getItem = Vector3.Distance(playerPos, ClawPos) <= clawPlayerWorldDiff.magnitude;
            await Task.Yield();
        }

        if (targetPlayer.Cc) targetPlayer.Cc.enabled = true;
        return true;
    }


    private async Task ForceClawReturn()
    {
        var clawReturn = false;
        while (!clawReturn)
        {
            var clawOriginLookVector = (ClawPos - ClawWorldOriginPos).normalized;
            ClawTransform.LookAt(ClawTransform.position + clawOriginLookVector);
            ClawTransform.position = Vector3.MoveTowards(ClawPos, ClawWorldOriginPos, targetPlayer.ClawSpeed * Time.deltaTime);
            
            clawReturn = Vector3.Distance(ClawWorldOriginPos, ClawPos) < 0.01f;
            await Task.Yield();
        }
    }
}
