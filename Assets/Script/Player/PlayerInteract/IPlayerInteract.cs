

using UnityEngine;


public interface IPlayerInteract
{
    IInteractable CurrentDetect { get; }
    IInteractable CurrentInteract { get; }


    void SetCurrentInteract(IInteractable interactable);
    void DetectInteractable(Vector3 interactCenter, float range);
    void Interact(Player interactPlayer, InteractType interactType);
    void UpdateInteract();
}
