using System;
using UnityEngine;

public class FallState : State
{
    // ReSharper disable Unity.PerformanceAnalysis
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
            player.velocity.y += _gravityValue * (_gravityMultiplier + FallGravityAddition) * Time.deltaTime;
        }
        else // If player is jumping or going upwards
        {
            player.velocity.y += _gravityValue * _gravityMultiplier * Time.deltaTime;
        }
        player.velocity.y = Mathf.Max(player.velocity.y, player.maxFallSpeed);
        
        // Horizontal Movement
        Transform playerTransform = player.transform;
        Vector3 horizontal = (playerTransform.right * moveInput.x + playerTransform.forward * moveInput.y) * (player.playerSpeed * player.airControl);

        if (horizontal.x != 0f && Math.Sign(player.velocity.x) != Math.Sign(horizontal.x))
        {
            player.velocity.x = player.velocity.x *= (1 - Time.deltaTime);
        }

        if (horizontal.z != 0f && Math.Sign(player.velocity.z) != Math.Sign(horizontal.z))
        {
            player.velocity.z = player.velocity.z *= (1 - Time.deltaTime);
        }
        
        // Updating player position
        player.controller.Move((player.velocity + horizontal) * Time.deltaTime);
    }

}
