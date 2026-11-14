using UnityEngine;

public class IdleState : State
{
    
    public override void Enter()
    {
        base.Enter();
        player.direction = Vector3.zero;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (inputHandler.WantsToJump() && isGrounded)
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (!isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
        }
        
        if (inputHandler.WantsToGrapple())
        {
            stateMachine.ChangeState(player.grappleState);
        }

        if (inputHandler.GetMovement().magnitude > 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        
        player.controller.Move(Vector3.up * (groundingForce * Time.deltaTime));

    
    }
    public override void Exit()
    {
        base.Exit();
    }
}
