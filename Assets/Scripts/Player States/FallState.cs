using UnityEngine;

public class FallState : State
{
    // ReSharper disable Unity.PerformanceAnalysis
    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (isGrounded && inputHandler.GetMovement().magnitude > 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // if (player.yVelocity < player.maxFallSpeed)
        // {
            player.yVelocity += player.gravityValue;
            player.direction.y = player.yVelocity * Time.deltaTime;
        // }
        player.controller.Move(player.direction);
    }
    
}
