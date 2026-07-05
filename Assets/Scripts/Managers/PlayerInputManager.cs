using UnityEngine;

/// <summary>
/// Manages player input for all platforms (PC, Mobile, Gamepad).
/// Centralizes input handling for the entire game.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCamera playerCamera;
    
    private void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        
        if (playerCamera == null)
            playerCamera = FindObjectOfType<PlayerCamera>();
        
        SetupInputAxes();
        Debug.Log("[PlayerInputManager] Input manager initialized");
    }
    
    /// <summary>
    /// Sets up custom input axes (called at runtime for cross-platform support).
    /// </summary>
    private void SetupInputAxes()
    {
        // Ensure camera sensitivities are set for all platforms
        float mouseSensitivity = 2f;
        float touchSensitivity = 0.5f;
        float gamepadSensitivity = 2f;
        
        playerCamera.SetCameraSensitivity(mouseSensitivity, touchSensitivity, gamepadSensitivity);
    }
    
    /// <summary>
    /// Gets horizontal movement input (WASD / Joystick).
    /// </summary>
    public float GetHorizontalInput() => Input.GetAxis("Horizontal");
    
    /// <summary>
    /// Gets vertical movement input (WASD / Joystick).
    /// </summary>
    public float GetVerticalInput() => Input.GetAxis("Vertical");
    
    /// <summary>
    /// Gets horizontal camera input.
    /// </summary>
    public float GetLookHorizontal()
    {
        #if UNITY_ANDROID || UNITY_IOS
        return 0f; // Touch handled directly in PlayerCamera
        #else
        return Input.GetAxis("Mouse X");
        #endif
    }
    
    /// <summary>
    /// Gets vertical camera input.
    /// </summary>
    public float GetLookVertical()
    {
        #if UNITY_ANDROID || UNITY_IOS
        return 0f; // Touch handled directly in PlayerCamera
        #else
        return Input.GetAxis("Mouse Y");
        #endif
    }
    
    /// <summary>
    /// Checks if sprint button pressed.
    /// </summary>
    public bool IsSprintPressed() => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Fire2");
    
    /// <summary>
    /// Checks if crouch button pressed.
    /// </summary>
    public bool IsCrouchPressed() => Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Fire1");
    
    /// <summary>
    /// Checks if interact button pressed.
    /// </summary>
    public bool IsInteractPressed() => Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire3");
    
    /// <summary>
    /// Checks if flashlight toggle pressed.
    /// </summary>
    public bool IsFlashlightPressed() => Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Jump");
}
