using UnityEngine;

public class FallState : State
{
    // ReSharper disable Unity.PerformanceAnalysis
    private float gravityValue;
    private float gravityMultiplier;
    private float fallGravityAddition = 1.0f;
    
    public override void Enter()
    {
        base.Enter();
        gravityValue = player.gravityValue;
        gravityMultiplier = player.gravityMultiplier;

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (isGrounded && player.yVelocity <= 0)
        {
            player.yVelocity = groundingForce;
            if (moveInput.magnitude > 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
            
        }

        // Vertical Calculations
        if (player.yVelocity < 0) // If player is going towards ground
        {
            player.yVelocity += gravityValue * (gravityMultiplier + fallGravityAddition) * Time.deltaTime;
        }
        else // If player is jumping or going upwards
        {
            player.yVelocity += gravityValue * gravityMultiplier * Time.deltaTime;
        }
        player.yVelocity = Mathf.Max(player.yVelocity, player.maxFallSpeed);
        
        // Horizontal Movement
        Transform transform = player.transform;
        Vector3 horizontal = (transform.right * moveInput.x + transform.forward * moveInput.y) * (player.playerSpeed * player.airControl);
        
        // Updating player position
        Vector3 velocity = horizontal;
        velocity.y = player.yVelocity;
        player.controller.Move(velocity * Time.deltaTime);
    }

}
