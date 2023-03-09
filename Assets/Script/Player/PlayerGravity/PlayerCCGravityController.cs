using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerCCGravityController : IPlayerGravityController
{
    private Player targetPlayer;
    private CharacterController characterController;

    private float currentGravityVelocity;
    
    
    public PlayerCCGravityController(Player player)
    {
        targetPlayer = player;
        characterController = targetPlayer.Cc;
    }


    public void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity)
    {
        if (targetPlayer.IsGround || !underGravity)
        {
            currentGravityVelocity = gravityInitialVelocity;
            return;
        }
        
        // If under gravity Effect
        currentGravityVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.down * (currentGravityVelocity * Time.deltaTime));
    }
}
