
using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerCCSelfMovement : IPlayerMove
{
    private Player targetPlayer;
    private readonly CharacterController characterController;

    private Vector3 currentMoveDirection;   
    private Vector3 lastMoveVelocity;
    private float currentJetPackVelocity;
    private float lastJetVelocity;

    public bool IsActive { get; private set; }
    
    private bool needHorizontalInertia, needVerticalInertia;

    
    public PlayerCCSelfMovement(Player player)
    {
        targetPlayer = player;
        characterController = targetPlayer.Cc;
        IsActive = true;
        
        currentMoveDirection = Vector3.zero;
        currentJetPackVelocity = 0f;
        lastMoveVelocity = Vector3.zero;
        lastJetVelocity = 0f;
    }


    public void SetActive(bool isActive) => IsActive = isActive;

    
    public void Move(Vector3 direction, float moveSpeed, float jetPackAcceleration, float maxJetPackVelocity)
    {
        if(!IsActive) return;

        var transform = targetPlayer.transform;
        transform.Rotate(Vector3.up * (targetPlayer.RotateSpeed * direction.x * Time.deltaTime) );
        
        if (direction.y == 0) currentJetPackVelocity = 0;
        else currentJetPackVelocity = Mathf.Min(Mathf.Abs(currentJetPackVelocity) + jetPackAcceleration * Time.deltaTime, maxJetPackVelocity) * direction.y;

        currentMoveDirection = transform.forward * (direction.z * moveSpeed);
        characterController.Move(currentMoveDirection * Time.deltaTime + Vector3.up * (currentJetPackVelocity * Time.deltaTime));
        
        if (direction.z != 0) lastMoveVelocity = currentMoveDirection;
        if (direction.y != 0)lastJetVelocity = currentJetPackVelocity;
    }


    public void CalcInertia(bool needInertia)
    {
        if(currentMoveDirection != Vector3.zero)
            if (!needHorizontalInertia) needHorizontalInertia = true;
        if (currentMoveDirection == Vector3.zero && needHorizontalInertia)
        {
            if(needInertia) AddInertia(lastMoveVelocity);
            needHorizontalInertia = false;
        }
        if(currentJetPackVelocity != 0)
            if(!needVerticalInertia) needVerticalInertia = true;
        if (currentJetPackVelocity == 0 && needVerticalInertia)
        {
            if(needInertia) AddInertia(Vector3.up * lastJetVelocity);
            needVerticalInertia = false;
        }
    }

    private async void AddInertia(Vector3 direction)
    {
        var timer = 0f;
        while (timer <= 1f)
        {
            direction -= direction * Time.deltaTime;
            if (characterController.enabled) characterController.Move(direction * Time.deltaTime);
            timer += Time.deltaTime;
            await Task.Yield();
        }
    }
}