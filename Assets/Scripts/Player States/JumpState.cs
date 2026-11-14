using UnityEngine;

public class JumpState : State
{
    
    public override void Enter()
    {
        base.Enter();
        player.yVelocity = player.jumpHeight;
        player.direction.y = player.yVelocity;
        player.controller.Move(player.direction);
        
        stateMachine.ChangeState(player.fallState);
    }
}
