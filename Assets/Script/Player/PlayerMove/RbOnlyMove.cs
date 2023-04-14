using UnityEngine;


public class RbOnlyMove : IPlayerMove
{
    private readonly Player targetPlayer;
    private readonly Rigidbody rb;
    
    public bool IsActive { get; private set; }


    private Vector3 currentMoveVelocity;
    private float currentJetVelocity;
    private bool useInertia;
    private float currentRotate;

    private float lastX, lastY, lastZ;
    private bool needLandVFX;

    public RbOnlyMove(Player player)
    {
        targetPlayer = player;
        rb = targetPlayer.Rb;
        IsActive = true;

        currentMoveVelocity = Vector3.zero;
        currentJetVelocity = 0;

        needLandVFX = false;
    }


    public void SetActive(bool active) => IsActive = active;


    public void Move(Vector3 direction, float speed, float jetPackAcceleration, float maxJetPackVelocity)
    {
        if(!IsActive) return;
        
        var moveVelocity = new Vector2(direction.x, direction.z) * speed;
        
        if (direction.y == 0)
        {
            currentJetVelocity = 0;
        }
        else
        {
            var jetDir = direction.y > 0 ? 1 : -1;
            currentJetVelocity = Mathf.Min(Mathf.Abs(currentJetVelocity) + jetPackAcceleration * Time.deltaTime, maxJetPackVelocity) * jetDir;
        }
        
        if (useInertia) rb.AddForce(moveVelocity.x, currentJetVelocity, moveVelocity.y, ForceMode.Acceleration);
        else rb.velocity = new Vector3(moveVelocity.x, currentJetVelocity == 0 ? rb.velocity.y : currentJetVelocity, moveVelocity.y);
        
        targetPlayer.JetVFX.SetActive(rb.velocity.y >= -0.1f && !targetPlayer.IsGround);
        targetPlayer.MoveVFX.SetActive(targetPlayer.IsGround && moveVelocity.sqrMagnitude > 0);
        if(currentJetVelocity > 0 && !targetPlayer.IsGround) FXController.Instance.InitSFX(SFXType.PlayerFlying, true);
        
        if(!targetPlayer.IsGround) needLandVFX = true;
        if (targetPlayer.IsGround && needLandVFX)
        {
            FXController.Instance.InitVFX(VFXType.PlayerLanding, targetPlayer.transform.position);
            needLandVFX = false;
        }
        
        if (moveVelocity != Vector2.zero && targetPlayer.PlayerInteractController.CurrentInteract is not Item) 
            Rotate(moveVelocity);

        if (targetPlayer.PlayerInteractController.CurrentInteract is Item item && item.CarryPlayers.Count == 1)
        {
            Rotate(moveVelocity);
        }
    }

    public void CalcInertia(bool needInertia)
    {
        useInertia = needInertia;
    }


    private void Rotate(Vector2 direction)
    {
        rb.angularVelocity = Vector3.zero;
        var targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        var rotateY = Mathf.SmoothDampAngle(targetPlayer.transform.eulerAngles.y, targetAngle, ref currentRotate, 0.1f);
        
        //targetPlayer.transform.rotation = Quaternion.Euler(0, rotateY, 0);
        rb.MoveRotation(Quaternion.Euler(0, rotateY, 0));
    }
}
