
using UnityEngine;

public interface IPlayerInput
{
    Vector2 Movement { get; }
    float JetDirection { get; }
    
    bool Run { get; }
    bool TapInteract { get; }
    bool HoldInteract { get; }
    bool ReleaseInteract { get; }
    
    bool SwitchEquipment { get; }
    float RotateCam { get; }



    void RegisterInput();
    void UnregisterInput();
    void Enable(bool enable);
}
