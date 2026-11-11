using UnityEngine;

public class JumpState : State
{
    public JumpState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }
}
