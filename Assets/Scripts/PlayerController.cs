using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Objects")]
    public CharacterController controller;
    public PlayerInput playerInput;
    public Animator animator;

    [Header("Controls")] 
    public float playerSpeed = 5.0f;

    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 1.0f;
    public float rotationSpeed = 5.0f;

    [Header("Smoothing")] 
    [Range(0, 1)] 
    public float speedDampTime = 0.1f;
    [Range(0, 1)] 
    public float velocityDampTime = 0.9f;
    [Range(0, 1)] 
    public float rotationDampTime = 0.2f;
    [Range(0, 1)] 
    public float airControl = 0.5f;
    
    [Header("State Machine")] 
    public StateMachine movementSM;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    public InteractState interactState;
    public GrappleState grappleState;

    [HideInInspector] public float gravityValue = -9.8f;
    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public Vector3 playerVelocity;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        gravityValue *= gravityMultiplier;
        
        movementSM.Initialize(idleState);
    }
}
