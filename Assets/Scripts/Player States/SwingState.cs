using UnityEngine;

public class SwingState : State

// TODO: If rope is fully taut but still on ground, avoid switching to swing.
// TODO: add some control while swinging 
// TODO: don't allow for infinite jump chaining. 

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
        isGrounded = player.controller.isGrounded;
        if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (inputHandler.WantsToJump())
        {
            player.TryGrapple();
            stateMachine.ChangeState(player.jumpState);
        }
        
        if (!player.IsTethered())
        {
            stateMachine.ChangeState(player.fallState);
        }
        
        
        Vector3 directionToGrapplePoint = (player.transform.position - player.GetGrapplePoint()).normalized;
        
        player.velocity.y += gravityValue * (gravityMultiplier + fallGravityAddition) * Time.deltaTime;
        player.velocity.y = Mathf.Max(player.velocity.y, player.maxFallSpeed);

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
