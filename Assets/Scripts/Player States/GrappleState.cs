using UnityEngine;

public class GrappleState : State
{
    public GrappleState(PlayerController _player, StateMachine _stateMachine): base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }
}
