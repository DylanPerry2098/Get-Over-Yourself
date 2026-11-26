using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State
{

    
    public override void Enter()
    {
        base.Enter();
        player.velocity = Vector3.zero;
        InvokeRepeating("Footstep", 0f, 0.45f);
    }

    public override void Exit()
    {
        CancelInvoke("Footstep");
        base.Exit();
    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (moveInput.magnitude <= 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (inputHandler.WantsToJump() && isGrounded)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (!isGrounded)
        {
            State s = player.TetherIsMaxLength() ? player.swingState : player.fallState;
            stateMachine.ChangeState(s);
            return;
        }


        Transform trans = player.transform;
        Vector3 horizontal = (trans.right * moveInput.x + trans.forward * moveInput.y) * player.playerSpeed;
        player.direction = horizontal;

        Vector3 velocity = horizontal;
        velocity.y = groundingForce;
        player.controller.Move(velocity * Time.deltaTime);
    }
    
    private void Footstep()
    {
        player.PlayFootstepAudio();
    }
}
