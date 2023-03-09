

using UnityEngine;

public interface IPlayerGravityController
{
    void AddGravity(bool underGravity, Vector3 groundCheckPoint, float gravity, float gravityInitialVelocity);
}
