
using UnityEngine;

public interface IPlayerInput
{
    Vector2 Movement { get; }
    float JetDirection { get; }
    bool Interact { get; }
    bool SwitchEquipment { get; }

    


    void Enable(bool enable);
}
