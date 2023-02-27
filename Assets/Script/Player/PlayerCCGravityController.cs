using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerCCGravityController : IPlayerGravityController
{
    private Player targetPlayer;
    private CharacterController characterController;
    public bool IsGround { get; private set; }
    
    private float currentGravityVelocity;
    
    
    public PlayerCCGravityController(Player player)
    {
        targetPlayer = player;
        characterController = targetPlayer.Cc;
    }


    public void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity)
    {
        IsGround = Physics.Raycast(groundCheckPoint, Vector3.down, .01f);
        if (IsGround || !underGravity)
        {
            currentGravityVelocity = gravityInitialVelocity;
            return;
        }
        
        // If under gravity Effect
        currentGravityVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.down * (currentGravityVelocity * Time.deltaTime));
    }


    // public async void AddInertia(Vector3 movement)
    // {
    //     var inertiaVelocity = movement;
    //     // stop after one second
    //     var timer = 0f;
    //     while (timer <= 1f)
    //     {
    //         inertiaVelocity -= movement * Time.deltaTime;
    //         characterController.Move(inertiaVelocity * Time.deltaTime);
    //         timer += Time.deltaTime;
    //         await Task.Yield();
    //     }
    // }
}
