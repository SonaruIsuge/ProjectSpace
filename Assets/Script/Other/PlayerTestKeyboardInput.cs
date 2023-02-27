using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerTestKeyboardInput : IPlayerInput
{
    public Vector2 Movement => MovementInput();
    public float JetDirection => Jet();
    public bool TapInteract => Keyboard.current.rightShiftKey.wasPressedThisFrame;
    public bool HoldInteract => Keyboard.current.rightShiftKey.isPressed;
    public bool ReleaseInteract => Keyboard.current.rightShiftKey.wasReleasedThisFrame;
    public bool SwitchEquipment => Keyboard.current.slashKey.wasPressedThisFrame;
    
    
    public void RegisterInput()
    {
        
    }

    
    public void UnregisterInput()
    {
        
    }

    
    public void Enable(bool enable)
    {
        
    }


    private Vector2 MovementInput()
    {
        var horizontal = 0;
        if (Keyboard.current.leftArrowKey.isPressed) horizontal = -1;
        else if (Keyboard.current.rightArrowKey.isPressed) horizontal = 1;
        else horizontal = 0;
        var vertical = 0;
        if (Keyboard.current.downArrowKey.isPressed) vertical = -1;
        else if (Keyboard.current.upArrowKey.isPressed) vertical = 1;
        else vertical = 0;
        return new Vector2(horizontal, vertical);
    }


    private float Jet()
    {
        var jetDirection = 0;
        if (Keyboard.current.oKey.isPressed) jetDirection = 1;
        else if (Keyboard.current.lKey.isPressed) jetDirection = -1;
        else jetDirection = 0;
        return jetDirection;
    }
}
