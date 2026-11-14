using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State
{
    private Transform playerTransform;
    public override void Enter()
    {
        base.Enter();
        
    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (moveInput.magnitude <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (inputHandler.WantsToJump() && isGrounded)
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (!isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
        }
        

        playerTransform = player.transform;
        player.direction = (playerTransform.right * moveInput.x) + (playerTransform.forward * moveInput.y);
        player.direction.y = player.yVelocity;
        player.controller.Move(player.direction * (player.playerSpeed * Time.deltaTime));
    }
}
