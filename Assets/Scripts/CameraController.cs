using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public Camera mainCamera;
    public InputHandler inputHandler;
    public float mouseSensitivity = 1.0f;
    
    private float cameraVerticalRotation = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        // Get Mouse Input
        float inputX = inputHandler.GetMouseDirection().x * mouseSensitivity;
        float inputY = inputHandler.GetMouseDirection().y * mouseSensitivity;

        // Rotate Camera around Local X Axis
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90, 90);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation; 
        
        // Rotate Camera and Player around Local Y Axis
        
        playerTransform.Rotate(Vector3.up * inputX); // Is circumventing the character controller the right thing to do?
        
    }
}
