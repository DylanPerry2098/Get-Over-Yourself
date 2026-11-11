using UnityEngine;

public class IdleState : State
{
    public IdleState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
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
