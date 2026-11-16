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

        if (player.TetherIsMaxLength())
        {
            stateMachine.ChangeState(player.swingState);
        }
        
        if (isGrounded && player.velocity.y <= 0)
        {
            player.velocity.y = groundingForce;
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
        if (player.velocity.y < 0) // If player is going towards ground
        {
            player.velocity.y += gravityValue * (gravityMultiplier + fallGravityAddition) * Time.deltaTime;
        }
        else // If player is jumping or going upwards
        {
            player.velocity.y += gravityValue * gravityMultiplier * Time.deltaTime;
        }
        player.velocity.y = Mathf.Max(player.velocity.y, player.maxFallSpeed);
        
        // Horizontal Movement
        Transform playerTransform = player.transform;
        Vector3 horizontal = (playerTransform.right * moveInput.x + playerTransform.forward * moveInput.y) * (player.playerSpeed * player.airControl);
        
        // Updating player position
        player.velocity.x = horizontal.x;
        player.velocity.z = horizontal.z;
        player.controller.Move(player.velocity * Time.deltaTime);
    }

}
