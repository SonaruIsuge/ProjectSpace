

using UnityEngine;

public interface IPlayerGravityController
{
    bool IsGround { get; }
    void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity);
}
