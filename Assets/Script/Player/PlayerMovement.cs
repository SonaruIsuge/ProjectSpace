using System;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    public Vector3 CurrentMoveDirection;   // Record it for no-gravity status use
    
    private float currentVelocity;

    
    private CharacterController characterController;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        CurrentMoveDirection = Vector3.zero;
    }


    public void Move(Vector2 direction)
    {
        CurrentMoveDirection = new Vector3(direction.x * moveSpeed, 0, direction.y * moveSpeed);
        
        characterController.Move(CurrentMoveDirection * Time.deltaTime);
        
        if(direction != Vector2.zero) Rotate(direction);
    }


    private void Rotate(Vector2 direction)
    {
        var targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        var rotateY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, 0.1f);
        
        transform.rotation = Quaternion.Euler(0, rotateY, 0);
    }
}
