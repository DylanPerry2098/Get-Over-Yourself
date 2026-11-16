using UnityEngine;

public class SwingState : State

{
    private float _gravityValue;
    private float _gravityMultiplier;
    private const float FallGravityAddition = 1.0f;
    
    public override void Enter()
    {
        base.Enter();
        _gravityValue = player.gravityValue;
        _gravityMultiplier = player.gravityMultiplier;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (inputHandler.WantsToJump()) // Only jump if the grapple point is above the player to avoid shooting the tether downwards and chaining together infinite jumps.
        {
            State s = player.GetGrapplePoint().y > player.transform.position.y ? player.jumpState : player.fallState;
            player.ToggleGrapple();
            
            stateMachine.ChangeState(s);
            return;
        }
        
        if (!player.IsTethered())
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        
        
        
        // Vertical Calculations
        if (player.velocity.y < 0) // If player is going towards ground
        {
            player.velocity.y += _gravityValue * (_gravityMultiplier + FallGravityAddition) * Time.deltaTime;
        }
        else // If player is jumping or going upwards
        {
            player.velocity.y += _gravityValue * _gravityMultiplier * Time.deltaTime;
        }
        player.velocity.y = Mathf.Max(player.velocity.y, player.maxFallSpeed);
        
        // Add horizontal movement
        Transform playerTransform = player.transform;
        Vector3 horizontal = (playerTransform.right * moveInput.x + playerTransform.forward * moveInput.y) * (player.playerSpeed * player.airControl);
        
        player.velocity += (horizontal * Time.deltaTime);

        if (player.TetherIsMaxLength()) // Apply pendulum physics.
        {
            Vector3 directionToGrapplePoint = (player.transform.position - player.GetGrapplePoint()).normalized;

            float radial = Vector3.Dot(player.velocity, directionToGrapplePoint);

            if (radial > 0f)
            {
                player.velocity -= directionToGrapplePoint * radial;
            }
        }

        player.controller.Move(player.velocity * Time.deltaTime);

    }
}
