using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerCCMovement : IPlayerMove
{
    private Player targetPlayer;
    private readonly CharacterController characterController;

    private Vector3 currentMoveDirection;   // Record it for no-gravity inertia use
    public Vector3 LastMoveVelocity { get; private set; }
    private float currentJetPackVelocity;
    public float LastJetVelocity { get; private set; }
    
    private float currentRotate;

    public bool IsActive { get; private set; }

    private bool needAddXInertia, needAddYInertia, needAddZInertia;
    

    public PlayerCCMovement(Player player)
    {
        targetPlayer = player;
        characterController = targetPlayer.Cc;
        IsActive = true;
        
        currentMoveDirection = Vector3.zero;
        LastMoveVelocity = Vector3.zero;
        LastJetVelocity = 0f;
    }


    public void SetActive(bool isActive) => IsActive = isActive;

    
    public void Move(Vector3 direction, float moveSpeed, float jetPackAcceleration, float maxJetPackVelocity)
    {
        if(!IsActive) return;

        var move = new Vector2(direction.x, direction.z);
        
        MoveOnGround(move, moveSpeed);
        JetInSpace(direction.y, jetPackAcceleration, maxJetPackVelocity);
        
    }


    public void CalcInertia(bool needInertia)
    {
        if(currentMoveDirection.x != 0)
            if (!needAddXInertia) needAddXInertia = true;
        if (currentMoveDirection.x == 0 && needAddXInertia)
        {
            if (needInertia) AddInertia(0, LastMoveVelocity.x);
            needAddXInertia = false;
        }
        if(currentJetPackVelocity != 0)
            if (!needAddYInertia) needAddYInertia = true;
        if (currentJetPackVelocity == 0 && needAddYInertia)
        {
            if (needInertia) AddInertia(1, LastJetVelocity);
            needAddYInertia = false;
        }
        if(currentMoveDirection.z != 0)
            if (!needAddZInertia) needAddZInertia = true;
        if (currentMoveDirection.z == 0 && needAddZInertia)
        {
            if (needInertia) AddInertia(2, LastMoveVelocity.z);
            needAddZInertia = false;
        }
    }


    private void MoveOnGround(Vector2 direction, float moveSpeed)
    {
        currentMoveDirection = new Vector3(direction.x * moveSpeed, 0, direction.y * moveSpeed);
        characterController.Move(currentMoveDirection * Time.deltaTime);
        if(direction != Vector2.zero) Rotate(direction);

        if (currentMoveDirection != Vector3.zero) LastMoveVelocity = currentMoveDirection;
    }


    private void JetInSpace(float jetDirection, float jetPackAcceleration, float maxJetPackVelocity)
    {
        if (jetDirection == 0)
        {
            currentJetPackVelocity = 0;
            return;
        }
        currentJetPackVelocity =
            Mathf.Min(Mathf.Abs(currentJetPackVelocity) + jetPackAcceleration * Time.deltaTime, maxJetPackVelocity) * jetDirection;
        characterController.Move(Vector3.up * (currentJetPackVelocity * Time.deltaTime));

        LastJetVelocity = currentJetPackVelocity;
    }


    private void Rotate(Vector2 direction)
    {
        var targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        var rotateY = Mathf.SmoothDampAngle(targetPlayer.transform.eulerAngles.y, targetAngle, ref currentRotate, 0.1f);
        
        targetPlayer.transform.rotation = Quaternion.Euler(0, rotateY, 0);
    }


    private async void AddInertia(int axis, float velocity)
    {
        var movement = axis switch
        {
            0 => Vector3.right * velocity,
            1 => Vector3.up * velocity,
            2 => Vector3.forward * velocity,
            _ => Vector3.zero
        };
        
        // stop after one second
        var timer = 0f;
        while (timer <= 1f)
        {
            movement -= movement * Time.deltaTime;
            if(characterController.enabled) characterController.Move(movement * Time.deltaTime);
            timer += Time.deltaTime;
            await Task.Yield();
        }
    }
}
