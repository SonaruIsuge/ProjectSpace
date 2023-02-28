
using UnityEngine;

public class PlayerRbMovement : IPlayerMove
{
    private readonly Player targetPlayer;
    private readonly Rigidbody rb;
    
    public bool IsActive { get; private set; }


    private Vector3 currentMoveVelocity;
    private float currentJetVelocity;
    private bool useInertia;
    private float currentRotate;

    public PlayerRbMovement(Player player)
    {
        targetPlayer = player;
        rb = targetPlayer.Rb;
        IsActive = true;

        currentMoveVelocity = Vector3.zero;
        currentJetVelocity = 0;
    }


    public void SetActive(bool active) => IsActive = active;


    public void Move(Vector3 direction, float speed, float jetPackAcceleration, float maxJetPackVelocity)
    {
        if(!IsActive) return;
        
        var moveVelocity = new Vector2(direction.x, direction.z) * speed;
        currentJetVelocity = direction.y;
        if (currentJetVelocity != 0) currentJetVelocity = Mathf.Min(currentJetVelocity + jetPackAcceleration * Time.deltaTime, maxJetPackVelocity) * speed;
        //rb.AddForce(moveVelocity.x, currentJetVelocity, moveVelocity.y, ForceMode.Acceleration);
        rb.velocity = new Vector3(moveVelocity.x, currentJetVelocity, moveVelocity.y);

        if (!useInertia)
        {
            var velocity = rb.velocity;
            if (direction.x == 0) rb.velocity = new Vector3(0, velocity.y, velocity.z);
            if (direction.y == 0) rb.velocity = new Vector3(velocity.x, 0, velocity.z);
            if (direction.z == 0) rb.velocity = new Vector3(velocity.x, velocity.y, 0);
        }

        if (moveVelocity != Vector2.zero)
        {
            if (targetPlayer.PlayerInteractController.CurrentInteract is Item item)
            {
                targetPlayer.transform.rotation = rb.transform.rotation;
                rb.transform.localRotation = Quaternion.Euler(Vector3.zero);
                // Rotate(item.transform.position - targetPlayer.transform.position);
            }
            else Rotate(moveVelocity);
        }
        
        targetPlayer.transform.position = rb.transform.position;
        rb.transform.localPosition = Vector3.zero;
    }

    public void CalcInertia(bool needInertia)
    {
        useInertia = needInertia;
    }


    public void Reset()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    private void Rotate(Vector2 direction)
    {
        var targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        var rotateY = Mathf.SmoothDampAngle(targetPlayer.transform.eulerAngles.y, targetAngle, ref currentRotate, 0.1f);
        
        targetPlayer.transform.rotation = Quaternion.Euler(0, rotateY, 0);
    }
}
