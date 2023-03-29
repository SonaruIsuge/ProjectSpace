using UnityEngine;

namespace Script.Other
{
    public class RbOnlyGravity : IPlayerGravityController
    {
        private Player targetPlayer;
        private Rigidbody rb;

        public RbOnlyGravity(Player player)
        {
            targetPlayer = player;
            rb = player.Rb;
        }
    
        public void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity)
        {
            rb.useGravity = underGravity && !targetPlayer.IsGround;
        }
    }
}