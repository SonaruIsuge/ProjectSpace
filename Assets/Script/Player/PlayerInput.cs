

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : IPlayerInput
{
    public Vector2 Movement { get; private set; }
    public float JetDirection { get; private set; }
    public bool TapInteract => interactUnderPerform && playerInputAction.GamePlay.Interact.WasPressedThisFrame();
    public bool HoldInteract => interactUnderPerform;
    public bool ReleaseInteract => playerInputAction.GamePlay.Interact.WasReleasedThisFrame(); 
    public bool SwitchEquipment => switchEquipmentUnderPerform && playerInputAction.GamePlay.SwitchEquip.WasPressedThisFrame();

    
    private PlayerInputAction playerInputAction;
    private bool interactUnderPerform;
    private bool switchEquipmentUnderPerform;

    
    public PlayerInput()
    {
        playerInputAction = new PlayerInputAction();
    }


    public void RegisterInput()
    {
        playerInputAction.GamePlay.Movement.performed += OnMovePerform;
        playerInputAction.GamePlay.Movement.canceled += OnMoveCancel;
        playerInputAction.GamePlay.Jet.performed += OnJetDirectionPerform;
        playerInputAction.GamePlay.Jet.canceled += OnJetDirectionCancel;
        playerInputAction.GamePlay.Interact.performed += OnInteractPerform;
        playerInputAction.GamePlay.Interact.canceled += OnInteractCancel;
        playerInputAction.GamePlay.SwitchEquip.performed += OnSwitchEquipmentPerform;
        playerInputAction.GamePlay.SwitchEquip.canceled += OnSwitchEquipmentCancel;
    }


    public void UnregisterInput()
    {
        playerInputAction.GamePlay.Movement.performed -= OnMovePerform;
        playerInputAction.GamePlay.Movement.canceled -= OnMoveCancel;
        playerInputAction.GamePlay.Jet.performed -= OnJetDirectionPerform;
        playerInputAction.GamePlay.Jet.canceled -= OnJetDirectionCancel;
        playerInputAction.GamePlay.Interact.performed -= OnInteractPerform;
        playerInputAction.GamePlay.Interact.canceled -= OnInteractCancel;
        playerInputAction.GamePlay.SwitchEquip.performed -= OnSwitchEquipmentPerform;
        playerInputAction.GamePlay.SwitchEquip.canceled -= OnSwitchEquipmentCancel;
    }


    public void Enable(bool enable)
    {
        if (enable) playerInputAction.GamePlay.Enable();
        else playerInputAction.GamePlay.Disable();
    }
    
    
    private void OnMovePerform(InputAction.CallbackContext ctx) => Movement = ctx.ReadValue<Vector2>();
    private void OnMoveCancel(InputAction.CallbackContext ctx) => Movement = Vector2.zero;
    private void OnJetDirectionPerform(InputAction.CallbackContext ctx) => JetDirection = ctx.ReadValue<float>();
    private void OnJetDirectionCancel(InputAction.CallbackContext ctx) => JetDirection = 0;

    private void OnInteractPerform(InputAction.CallbackContext ctx) => interactUnderPerform = true;
    private void OnInteractCancel(InputAction.CallbackContext ctx) => interactUnderPerform = false;
    private void OnSwitchEquipmentPerform(InputAction.CallbackContext ctx) => switchEquipmentUnderPerform = true;
    private void OnSwitchEquipmentCancel(InputAction.CallbackContext ctx) => switchEquipmentUnderPerform = false;
}
