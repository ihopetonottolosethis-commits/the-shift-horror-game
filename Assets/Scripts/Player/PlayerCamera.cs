using UnityEngine;
using System;

/// <summary>
/// Manages player camera look mechanics with full cross-platform support.
/// Supports: Mouse (Desktop), Touch (Mobile), Gamepad.
/// Features smooth camera control with configurable sensitivity.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float touchSensitivity = 0.5f;
    [SerializeField] private float gamepadSensitivity = 2f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private float smoothing = 0.1f;
    
    [SerializeField] private Transform sarcophagusTransform;
    [SerializeField] private float sarcophagusViewAngleTolerance = 30f;
    
    private bool isInitialized = false;
    private Vector3 lastMousePosition = Vector3.zero;
    private float smoothedXRotation = 0f;
    private float smoothedYRotation = 0f;
    
    public event Action<bool> OnLookingSarcophagus;
    
    private void Start()
    {
        isInitialized = true;
        lastMousePosition = Input.mousePosition;
        LockCursor();
        Debug.Log("[PlayerCamera] Cross-platform camera initialized");
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        HandleCameraInput();
        CheckSarcophagusLooking();
    }
    
    private void HandleCameraInput()
    {
        float deltaX = 0f;
        float deltaY = 0f;
        
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.mousePresent)
        {
            deltaX = Input.GetAxis("Mouse X") * mouseSensitivity;
            deltaY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        #endif
        
        float gamepadX = Input.GetAxis("Horizontal_LookAround");
        float gamepadY = Input.GetAxis("Vertical_LookAround");
        
        if (Mathf.Abs(gamepadX) > 0.1f || Mathf.Abs(gamepadY) > 0.1f)
        {
            deltaX = gamepadX * gamepadSensitivity;
            deltaY = gamepadY * gamepadSensitivity;
        }
        
        #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                deltaX = touch.deltaPosition.x * touchSensitivity * Time.deltaTime;
                deltaY = -touch.deltaPosition.y * touchSensitivity * Time.deltaTime;
            }
        }
        #endif
        
        smoothedXRotation = Mathf.Lerp(smoothedXRotation, deltaX, smoothing);
        smoothedYRotation = Mathf.Lerp(smoothedYRotation, -deltaY, smoothing);
        
        xRotation += smoothedYRotation;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * smoothedXRotation);
    }
    
    private void CheckSarcophagusLooking()
    {
        if (sarcophagusTransform == null) return;
        if (!GameManager.Instance.GetRuleManager().IsRule1Active()) return;
        
        Vector3 directionToSarcophagus = (sarcophagusTransform.position - transform.position).normalized;
        Vector3 cameraForward = transform.forward;
        float angle = Vector3.Angle(cameraForward, directionToSarcophagus);
        bool isLooking = angle < sarcophagusViewAngleTolerance;
        
        GameManager.Instance.GetRuleManager().SetLookingAtSarcophagus(isLooking);
        OnLookingSarcophagus?.Invoke(isLooking);
    }
    
    private void LockCursor()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        #endif
    }
    
    public void UnlockCursor()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        #endif
    }
    
    public void SetSarcophagusReference(Transform sarcophagus)
    {
        sarcophagusTransform = sarcophagus;
        Debug.Log("[PlayerCamera] Sarcophagus reference set");
    }
    
    public void SetCameraSensitivity(float mouse, float touch, float gamepad)
    {
        mouseSensitivity = mouse;
        touchSensitivity = touch;
        gamepadSensitivity = gamepad;
    }
}
