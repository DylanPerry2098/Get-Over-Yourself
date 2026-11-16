using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // There may be a better way of doing this
    public InputActionReference moveActionRef;
    public InputActionReference lookActionRef;
    public InputActionReference jumpActionRef;
    public InputActionReference interactActionRef;
    public InputActionReference grappleActionRef;
    public InputActionReference retractActionRef;
    public InputActionReference extendActionRef;
    
    public Vector3 GetMovement()
    {
        Vector2 movementInput =  moveActionRef.action.ReadValue<Vector2>();
        return movementInput;
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

    public bool WantsToRetract()
    {
        return retractActionRef.action.inProgress;
    }

    public bool WantsToExtend()
    {
        return extendActionRef.action.inProgress;
    }
}
