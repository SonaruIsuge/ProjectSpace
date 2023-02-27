

public interface IInteractable
{
    InteractType InteractType { get; }
    bool isInteract { get; }
    bool isSelect { get; }

    void OnSelect();
    void Interact(Player interactPlayer, InteractType interactType);
    void OnDeselect();
    
}
