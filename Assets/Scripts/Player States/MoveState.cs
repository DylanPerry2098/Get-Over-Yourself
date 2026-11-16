using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State
{
    private Transform playerTransform;
    public override void Enter()
    {
        base.Enter();
        player.velocity = Vector3.zero;
        
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
            State s = player.TetherIsMaxLength() ? player.swingState : player.fallState;
            stateMachine.ChangeState(player.fallState);
        }


        Transform transform = player.transform;
        Vector3 horizontal = (transform.right * moveInput.x + transform.forward * moveInput.y) * player.playerSpeed;
        player.direction = horizontal;

        Vector3 velocity = horizontal;
        velocity.y = groundingForce;
        player.controller.Move(velocity * Time.deltaTime);
    }
}
