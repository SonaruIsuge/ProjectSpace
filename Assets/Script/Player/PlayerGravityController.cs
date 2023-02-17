using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerGravityController : MonoBehaviour
{
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float gravity;
    [SerializeField] private float gravityInitialVelocity;

    [SerializeField] private float currentGravityVelocity;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }


    public void CalcGravityFactor(bool underGravity)
    {
        var checkGround = Physics.CheckSphere(groundCheckPoint.position, .01f);
        if (checkGround || !underGravity)
        {
            currentGravityVelocity = gravityInitialVelocity;
            return;
        }
        
        // If under gravity Effect
        currentGravityVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.down * (currentGravityVelocity * Time.deltaTime));
    }


    public async void AddInertia(Vector3 movement)
    {
        var inertiaVelocity = movement;
        // stop after one second
        var timer = 0f;
        while (timer <= 1f)
        {
            inertiaVelocity -= movement * Time.deltaTime;
            characterController.Move(inertiaVelocity * Time.deltaTime);
            timer += Time.deltaTime;
            await Task.Yield();
        }
    }
}
