using UnityEngine;

public class FallState : State
{
    // ReSharper disable Unity.PerformanceAnalysis
    
    public override void Enter()
    {
        base.Enter();
        
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (isGrounded && player.yVelocity <= 0)
        {
            player.yVelocity = groundingForce;
            if (moveInput.magnitude > 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
            
        }

        
        player.yVelocity += player.gravityValue * Time.deltaTime;
        player.yVelocity = Mathf.Max(player.yVelocity, player.maxFallSpeed);

        Vector3 velocity = player.direction;
        velocity.y = player.yVelocity;
        player.controller.Move(velocity * Time.deltaTime);
    }
    
}
