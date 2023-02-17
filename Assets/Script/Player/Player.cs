using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool underGravityInfluence;
    [SerializeField] private Transform testGravityControlMachine;
    
    
    private IPlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerJetPackController playerJetPackController;
    private PlayerGravityController playerGravityController;

    private InertiaState moveInertiaChecker = InertiaState.NoNeedInertia;
    private InertiaState jetInertiaChecker = InertiaState.NoNeedInertia;
    private Vector3 lastMove;
    private float lastJet;

    private void Awake()
    {
        playerInput = GetComponent<IPlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJetPackController = GetComponent<PlayerJetPackController>();
        playerGravityController = GetComponent<PlayerGravityController>();
        
        playerInput.Enable(true);
    }


    private void Update()
    {
        
        playerMovement.Move(playerInput.Movement);
        playerJetPackController.Jet(playerInput.JetDirection);
        
        underGravityInfluence = playerInput.JetDirection == 0 && Vector3.Distance(transform.position, testGravityControlMachine.position) <= 5;
        playerGravityController.CalcGravityFactor(underGravityInfluence);


        // Check if horizontal movement need add inertia. 
        if (Vector2.SqrMagnitude(playerInput.Movement) != 0)
        {
            moveInertiaChecker = InertiaState.Moving;
            lastMove = playerMovement.CurrentMoveDirection;
        }
        if (playerInput.Movement == Vector2.zero && moveInertiaChecker == InertiaState.Moving) moveInertiaChecker = InertiaState.GiveInertia;
        if (moveInertiaChecker == InertiaState.GiveInertia)
        {
            if(!underGravityInfluence) playerGravityController.AddInertia(lastMove);
            moveInertiaChecker = InertiaState.NoNeedInertia;
            lastMove = Vector3.zero;
        }
        
        // Check if vertical movement need add inertia.
        if (playerInput.JetDirection != 0)
        {
            jetInertiaChecker = InertiaState.Moving;
            lastJet = playerJetPackController.CurrentJetPackVelocity;
        }
        if (playerInput.JetDirection == 0 && jetInertiaChecker == InertiaState.Moving) jetInertiaChecker = InertiaState.GiveInertia;
        if (jetInertiaChecker == InertiaState.GiveInertia)
        {
            if(!underGravityInfluence) playerGravityController.AddInertia(Vector3.up * lastJet);
            jetInertiaChecker = InertiaState.NoNeedInertia;
            lastJet = 0;
        }
        
    }
    
}


public enum InertiaState
{
    Moving,
    GiveInertia,
    NoNeedInertia,
}