using UnityEngine;

public class JumpState : State
{
    public override void Enter()
    {
        base.Enter();
        player.yVelocity = player.jumpHeight;
        stateMachine.ChangeState(player.fallState);

    }
}
