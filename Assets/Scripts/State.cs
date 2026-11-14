using UnityEngine;
using UnityEngine.InputSystem;

public class State: MonoBehaviour
{
    public PlayerController player;
    public StateMachine stateMachine;
    public InputHandler inputHandler;
    
    protected Vector2 moveInput;
    protected bool isGrounded;
    protected float groundingForce = 2.0f;
    
    public virtual void Enter()
    {
        Debug.Log("enter state: " + ToString());
    }

    public virtual void StateUpdate()
    {
        isGrounded = player.controller.isGrounded;
        moveInput = inputHandler.GetMovement();
    }

    public virtual void StateFixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }
}
