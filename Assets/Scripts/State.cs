using UnityEngine;
using UnityEngine.InputSystem;

public class State: MonoBehaviour
{
    public PlayerController player;
    public StateMachine stateMachine;
    public InputHandler inputHandler;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;
    
    public virtual void Enter()
    {
        Debug.Log("enter state: " + ToString());
    }

    public virtual void StateUpdate()
    {
        
    }

    public virtual void StateFixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }
}
