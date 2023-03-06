

using UnityEngine;


public class PlayerRbGravityController : IPlayerGravityController
{
    private Player targetPlayer;
    private Rigidbody rb;
    public bool IsGround { get; private set; }

    public PlayerRbGravityController(Player player)
    {
        targetPlayer = player;
        rb = player.Rb;
    }
    
    public void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity)
    {
        IsGround = Physics.Raycast(groundCheckPoint, Vector3.down, 0.1f);
        
        rb.useGravity = underGravity && !IsGround;
    }
}
