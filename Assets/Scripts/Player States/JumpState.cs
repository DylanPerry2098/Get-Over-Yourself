using UnityEngine;

public class JumpState : State
{
    
    public override void Enter()
    {
        base.Enter();
        player.direction.y = player.jumpHeight;
        player.controller.Move(player.direction);
        
        stateMachine.ChangeState(player.fallState);
    }
}
