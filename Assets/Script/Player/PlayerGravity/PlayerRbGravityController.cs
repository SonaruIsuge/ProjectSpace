

using UnityEngine;


public class PlayerRbGravityController : IPlayerGravityController
{
    private Player targetPlayer;
    private Rigidbody rb;
   

    public PlayerRbGravityController(Player player)
    {
        targetPlayer = player;
        rb = player.Rb;
    }
    
    public void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity)
    {
        rb.useGravity = underGravity && !targetPlayer.IsGround;
    }
}
