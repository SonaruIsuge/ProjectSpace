

using UnityEngine;


public interface IPlayerInteract
{
    IInteractable CurrentDetect { get; }
    IInteractable CurrentInteract { get; }
    bool Enable { get; }


    void EnableInteract(bool enable);
    void SetCurrentInteract(IInteractable interactable);
    void DetectInteractable(Vector3 interactCenter, float range);
    void Interact(Player interactPlayer, InteractType interactType);
    void UpdateInteract();
}
