using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State
{
    
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
