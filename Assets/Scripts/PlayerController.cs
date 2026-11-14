using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Objects")]
    public CharacterController controller;
    public Animator animator;

    [Header("Controls")] 
    public float playerSpeed = 5.0f;
    public float jumpHeight = 10f;
    public float gravityMultiplier = 3.0f;
    public float maxFallSpeed = -25f;

    [Header("Smoothing")] 
    // [Range(0, 1)] 
    // public float speedDampTime = 0.1f;
    // [Range(0, 1)] 
    // public float velocityDampTime = 0.9f;
    // [Range(0, 1)] 
    // public float rotationDampTime = 0.2f;
    [Range(0, 1)] 
    public float airControl = 0.75f;
    
    [Header("State Machine")] 
    public StateMachine movementSM;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    public InteractState interactState;
    public GrappleState grappleState;

    [HideInInspector] public float gravityValue = -9.8f;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float yVelocity;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yVelocity = -2.0f;
        movementSM.Initialize(idleState);
    }
}
