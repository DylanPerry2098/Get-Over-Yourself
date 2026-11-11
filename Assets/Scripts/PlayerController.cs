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

    [Header("State Machine")] 
    public StateMachine movementSM;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public GrappleState grappleState;
    
    [Header("Input Action References")] 
    public InputActionReference moveReference;
    public InputActionReference jumpReference;
    public InputActionReference grappleReference; // Unused for now

    [HideInInspector] public float gravityValue = -9.8f;
    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public Vector3 playerVelocity;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = GetComponent<Camera>().transform;
        gravityValue *= gravityMultiplier;

        movementSM = new StateMachine();
        idleState = new IdleState(this, movementSM);
        moveState = new MoveState(this, movementSM);
        jumpState = new JumpState(this, movementSM);
        grappleState = new GrappleState(this, movementSM);
    }

    // eeeehhhhhhh
    private void Update()
    {
        movementSM.currentState.HandleInput();
        movementSM.currentState.StateUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.currentState.StateFixedUpdate();
    }
}
