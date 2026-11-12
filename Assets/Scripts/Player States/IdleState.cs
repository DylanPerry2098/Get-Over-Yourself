using UnityEngine;

public class IdleState : State
{
    
    // TODO: Separate Move and Idle States
    public override void Enter()
    {
        base.Enter();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void StateUpdate()
    {
        base.StateUpdate();
        if (inputHandler.WantsToJump() && player.controller.isGrounded)
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (inputHandler.WantsToGrapple())
        {
            stateMachine.ChangeState(player.grappleState);
        }

        if (inputHandler.GetMovement().magnitude > 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        
        // input = inputHandler.GetMovement();
        // velocity = new Vector3(input.x, 0, input.y);
        //
        // velocity = velocity.x * player.cameraTransform.right.normalized +
        //            velocity.z * player.cameraTransform.forward.normalized;
        // velocity.y = 0f;
        
        
    }

    public override void StateFixedUpdate()
    {
        // TODO: Put these in their respective classes
        // base.StateFixedUpdate();
        // gravityVelocity.y += gravityValue * Time.deltaTime;
        // grounded = player.controller.isGrounded;
        //
        // if (grounded && gravityVelocity.y < 0)
        // {
        //     gravityVelocity.y = 0f;
        // }
        //
        // currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        // player.controller.Move(currentVelocity * (Time.deltaTime * playerSpeed) + gravityVelocity * Time.deltaTime)
    }

    public override void Exit()
    {
        base.Exit();

        // gravityVelocity.y = 0f;
        // player.playerVelocity = new Vector3(input.x, 0, input.y);
        //
        // if (velocity.sqrMagnitude > 0)
        // {
        //     player.transform.rotation = Quaternion.LookRotation(velocity);
        // }
    }
}
