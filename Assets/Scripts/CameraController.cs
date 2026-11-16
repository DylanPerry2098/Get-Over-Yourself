using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public Camera mainCamera;
    public InputHandler inputHandler;
    public float mouseSensitivity = 0.5f;
    
    private float cameraVerticalRotation = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        // Get Mouse Input
        Vector2 mouseDirection = inputHandler.GetMouseDirection();

        float inputX = mouseDirection.x * mouseSensitivity;
        float inputY = mouseDirection.y * mouseSensitivity;
        
        // Rotate Camera around Local X Axis
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90, 90);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation; 
        
        // Rotate Camera and Player around Local Y Axis
        
        playerTransform.Rotate(Vector3.up * inputX); // Is circumventing the character controller the right thing to do?
        
    }
}
