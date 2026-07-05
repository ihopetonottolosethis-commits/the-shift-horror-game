using UnityEngine;
using System;

/// <summary>
/// Manages player camera look mechanics and checks for sarcophagus viewing.
/// Implements first-person camera control with mobile touch input support.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float touchSensitivity = 1.5f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private float maxLookAngle = 90f;
    
    [SerializeField] private Transform sarcophagusTransform; // Reference to sarcophagus for Rule 1
    [SerializeField] private float sarcophagusViewAngleTolerance = 30f;
    
    private bool isInitialized = false;
    
    // Events
    public event Action<bool> OnLookingSarcophagus;
    
    private void Start()
    {
        isInitialized = true;
        LockCursor();
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        
        HandleCameraInput();
        CheckSarcophagusLooking();
    }
    
    /// <summary>
    /// Handles camera input (mouse or touch).
    /// </summary>
    private void HandleCameraInput()
    {
        // Desktop input (mouse)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Mobile input (touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                mouseX = touch.deltaPosition.x * touchSensitivity * Time.deltaTime;
                mouseY = touch.deltaPosition.y * touchSensitivity * Time.deltaTime;
            }
        }
        
        // Rotate around X axis (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate player body around Y axis (look left/right)
        transform.parent.Rotate(Vector3.up * mouseX);
    }
    
    /// <summary>
    /// Checks if player is looking at the sarcophagus (for Rule 1).
    /// </summary>
    private void CheckSarcophagusLooking()
    {
        if (sarcophagusTransform == null) return;
        if (!GameManager.Instance.GetRuleManager().IsRule1Active()) return;
        
        // Get direction from camera to sarcophagus
        Vector3 directionToSarcophagus = (sarcophagusTransform.position - transform.position).normalized;
        Vector3 cameraForward = transform.forward;
        
        // Calculate angle between camera forward and sarcophagus direction
        float angle = Vector3.Angle(cameraForward, directionToSarcophagus);
        
        bool isLooking = angle < sarcophagusViewAngleTolerance;
        
        // Update rule manager
        GameManager.Instance.GetRuleManager().SetLookingAtSarcophagus(isLooking);
        OnLookingSarcophagus?.Invoke(isLooking);
    }
    
    /// <summary>
    /// Locks cursor to center of screen (desktop).
    /// </summary>
    private void LockCursor()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        #endif
    }
    
    /// <summary>
    /// Unlocks cursor (for menus).
    /// </summary>
    public void UnlockCursor()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.None;
        #endif
    }
    
    /// <summary>
    /// Sets sarcophagus reference for Rule 1 checking.
    /// </summary>
    public void SetSarcophagusReference(Transform sarcophagus)
    {
        sarcophagusTransform = sarcophagus;
    }
}
