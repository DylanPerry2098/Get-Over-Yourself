using UnityEngine;
using UnityEngine.InputSystem;

public class State : MonoBehaviour
{
    public PlayerController player;
    public StateMachine stateMachine;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    // There may be a better way of doing this
    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference grapple; // Unused for now

    public State(PlayerController _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;

        move = player.moveReference;
        jump = player.jumpReference;
        grapple = player.grappleReference;

    }
    
    public virtual void Enter()
    {
        Debug.Log("enter state: " + ToString());
    }

    public virtual void HandleInput()
    {
        
    }

    private void CheckTransition()
    {
        
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
