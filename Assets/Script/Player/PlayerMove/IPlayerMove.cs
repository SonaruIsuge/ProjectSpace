
using UnityEngine;


public interface IPlayerMove
{
    bool IsActive { get; }
    
    void SetActive(bool active);
    void Move(Vector3 direction, float speed, float jetPackAcceleration, float maxJetPackVelocity);
    void CalcInertia(bool needInertia);
}
