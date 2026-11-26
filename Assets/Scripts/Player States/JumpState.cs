using UnityEngine;

public class JumpState : State
{
    public override void Enter()
    {
        base.Enter();
        player.velocity.y += player.jumpHeight;
        player.PlayJumpAudio();
        stateMachine.ChangeState(player.fallState);
    }
}
