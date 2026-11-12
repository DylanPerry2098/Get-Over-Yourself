using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // There may be a better way of doing this
    public InputActionReference moveActionRef;
    public InputActionReference lookActionRef;
    public InputActionReference jumpActionRef;
    public InputActionReference interactActionRef;
    public InputActionReference grappleActionRef; // Unused for now
    
    public Vector3 GetMovement()
    {
        Vector2 movement =  moveActionRef.action.ReadValue<Vector2>();
        return new Vector3(movement.x, 0, movement.y);
    }

    public Vector2 GetMouseDirection()
    {
        return lookActionRef.action.ReadValue<Vector2>();
    }

    public bool WantsToJump()
    { 
        return jumpActionRef.action.triggered;
    }

    public bool WantsToInteract()
    {
        return interactActionRef.action.triggered;
    }

    public bool WantsToGrapple()
    {
        return grappleActionRef.action.triggered;
    }
}
