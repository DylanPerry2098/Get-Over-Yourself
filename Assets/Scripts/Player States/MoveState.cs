using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State
{

    public MoveState(PlayerController player, StateMachine _stateMachine): base(player, _stateMachine)
    {
        base.player = player;
        stateMachine = _stateMachine;

        // There may be a better way of doing this

    }
    
    public virtual void Enter()
    {
        Debug.Log("enter state: " + ToString());
    }

    public void HandleInput()
    {
        
    }

    public void StateUpdate()
    {
        
    }

    public void StateFixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }
}
