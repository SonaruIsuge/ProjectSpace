

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInput : IPlayerInput
{
    private Player targetPlayer;
    public InputUser inputUser;
    public PlayerInputAction playerInputAction;
    public Vector2 Movement { get; private set; }
    public float JetDirection { get; private set; }
    public bool Run { get; private set; }
    public bool TapInteract => interactUnderPerform && playerInputAction.GamePlay.Interact.WasPressedThisFrame();
    public bool HoldInteract => interactUnderPerform;
    public bool ReleaseInteract => playerInputAction.GamePlay.Interact.WasReleasedThisFrame(); 
    public bool SwitchEquipment => switchEquipmentUnderPerform && playerInputAction.GamePlay.SwitchEquip.WasPressedThisFrame();
    public float RotateCam => playerInputAction.GamePlay.RotateCam.WasPressedThisFrame() ? rotateCamValue : 0;

    
    
    private bool interactUnderPerform;
    private bool switchEquipmentUnderPerform;
    private int rotateCamValue;    
    
    
    public PlayerInput(Player player)
    {
        targetPlayer = player;
        playerInputAction = new PlayerInputAction();
    }


    public void RegisterInput()
    {
        playerInputAction.GamePlay.Movement.performed += OnMovePerform;
        playerInputAction.GamePlay.Movement.canceled += OnMoveCancel;
        playerInputAction.GamePlay.Jet.performed += OnJetDirectionPerform;
        playerInputAction.GamePlay.Jet.canceled += OnJetDirectionCancel;
        playerInputAction.GamePlay.Run.performed += OnRunPerform;
        playerInputAction.GamePlay.Run.canceled += OnRunCancel;
        playerInputAction.GamePlay.Interact.performed += OnInteractPerform;
        playerInputAction.GamePlay.Interact.canceled += OnInteractCancel;
        playerInputAction.GamePlay.SwitchEquip.performed += OnSwitchEquipmentPerform;
        playerInputAction.GamePlay.SwitchEquip.canceled += OnSwitchEquipmentCancel;
        playerInputAction.GamePlay.RotateCam.performed += OnRotateCamPerform;
        playerInputAction.GamePlay.RotateCam.canceled += OnRotateCamCancel;
    }


    public void UnregisterInput()
    {
        playerInputAction.GamePlay.Movement.performed -= OnMovePerform;
        playerInputAction.GamePlay.Movement.canceled -= OnMoveCancel;
        playerInputAction.GamePlay.Jet.performed -= OnJetDirectionPerform;
        playerInputAction.GamePlay.Jet.canceled -= OnJetDirectionCancel;
        playerInputAction.GamePlay.Run.performed -= OnRunPerform;
        playerInputAction.GamePlay.Run.canceled -= OnRunCancel;
        playerInputAction.GamePlay.Interact.performed -= OnInteractPerform;
        playerInputAction.GamePlay.Interact.canceled -= OnInteractCancel;
        playerInputAction.GamePlay.SwitchEquip.performed -= OnSwitchEquipmentPerform;
        playerInputAction.GamePlay.SwitchEquip.canceled -= OnSwitchEquipmentCancel;
        playerInputAction.GamePlay.RotateCam.performed -= OnRotateCamPerform;
        playerInputAction.GamePlay.RotateCam.canceled -= OnRotateCamCancel;
    }


    public void Enable(bool enable)
    {
        if (enable) playerInputAction.GamePlay.Enable();
        else playerInputAction.GamePlay.Disable();
    }


    public void PairWithDevice(InputDevice device)
    {
        inputUser = InputUser.PerformPairingWithDevice(device);
        inputUser.AssociateActionsWithUser(playerInputAction);
        Debug.Log($"Pair {targetPlayer.name} with {device.name}");
    }


    public void UnpairWithDevice()
    {
        if(!inputUser.valid) return;
        
        inputUser.UnpairDevicesAndRemoveUser();
    }
    
    
    private void OnMovePerform(InputAction.CallbackContext ctx) => Movement = ctx.ReadValue<Vector2>();
    private void OnMoveCancel(InputAction.CallbackContext ctx) => Movement = Vector2.zero;
    private void OnJetDirectionPerform(InputAction.CallbackContext ctx) => JetDirection = FixFloatValueToInt(ctx.ReadValue<float>());
    private void OnJetDirectionCancel(InputAction.CallbackContext ctx) => JetDirection = 0;
    private void OnRunPerform(InputAction.CallbackContext ctx) => Run = true;
    private void OnRunCancel(InputAction.CallbackContext ctx) => Run = false;
    private void OnInteractPerform(InputAction.CallbackContext ctx) => interactUnderPerform = true;
    private void OnInteractCancel(InputAction.CallbackContext ctx) => interactUnderPerform = false;
    private void OnSwitchEquipmentPerform(InputAction.CallbackContext ctx) => switchEquipmentUnderPerform = true;
    private void OnSwitchEquipmentCancel(InputAction.CallbackContext ctx) => switchEquipmentUnderPerform = false;
    private void OnRotateCamPerform(InputAction.CallbackContext ctx) => rotateCamValue = FixFloatValueToInt(ctx.ReadValue<float>());
    private void OnRotateCamCancel(InputAction.CallbackContext ctx) => rotateCamValue = 0;


    private int FixFloatValueToInt(float direction)
    {
        if (direction > 0) return 1;
        if (direction < 0) return -1;
        return 0;
    }
}
