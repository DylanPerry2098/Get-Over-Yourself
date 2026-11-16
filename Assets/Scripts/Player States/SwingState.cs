using UnityEngine;

public class SwingState : State

{
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
        if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (inputHandler.WantsToJump())
        {
            State s = player.GetGrapplePoint().y > player.transform.position.y ? player.jumpState : player.fallState;
            player.TryGrapple();
            
            stateMachine.ChangeState(s);
            return;
        }
        
        if (!player.IsTethered())
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        
        
        Vector3 directionToGrapplePoint = (player.transform.position - player.GetGrapplePoint()).normalized;
        
        player.velocity.y += gravityValue * (gravityMultiplier + fallGravityAddition) * Time.deltaTime;
        player.velocity.y = Mathf.Max(player.velocity.y, player.maxFallSpeed);
        
        Transform playerTransform = player.transform;
        Vector3 horizontal = (playerTransform.right * moveInput.x + playerTransform.forward * moveInput.y) * (player.playerSpeed * player.airControl);

        player.velocity += (horizontal * Time.deltaTime);

        if (player.TetherIsMaxLength())
        {

            float radial = Vector3.Dot(player.velocity, directionToGrapplePoint);

            if (radial > 0f)
            {
                player.velocity -= directionToGrapplePoint * radial;
            }
        }

        player.controller.Move(player.velocity * Time.deltaTime);

    }
}
