using UnityEngine;
using System;

/// <summary>
/// Manages player movement, sprinting, crouching, and interaction.
/// Implements first-person controller with mobile-optimized input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Flashlight flashlight;
    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 5f;
    
    [SerializeField] private float crouchHeight = 0.6f;
    [SerializeField] private float normalHeight = 1.8f;
    [SerializeField] private float crouchYOffset = 0.6f;
    
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;
    
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = true;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isMoving = false;
    private float currentSpeed = 0f;
    
    // Input
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    
    // Events
    public event Action<bool> OnSprintChanged;
    public event Action<bool> OnCrouchChanged;
    public event Action<bool> OnMovingChanged;
    public event Action<RaycastHit> OnInteractionDetected;
    
    private bool isInitialized = false;
    private Vector3 normalCameraPos;
    private Vector3 crouchCameraPos;
    
    public void Initialize()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        if (flashlight == null)
            flashlight = GetComponentInChildren<Flashlight>();
        
        normalCameraPos = playerCamera.transform.localPosition;
        crouchCameraPos = playerCamera.transform.localPosition;
        crouchCameraPos.y -= crouchYOffset;
        
        currentSpeed = walkSpeed;
        isInitialized = true;
        
        Debug.Log("[PlayerController] Initialized");
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        
        HandleInput();
        HandleMovement();
        HandleInteraction();
        CheckFootstepTrigger();
    }
    
    /// <summary>
    /// Handles player input from mobile controls.
    /// </summary>
    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        // Sprint (R1)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Fire2"))
        {
            SetSprinting(!isSprinting);
        }
        
        // Crouch (L1)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Fire1"))
        {
            SetCrouching(!isCrouching);
        }
        
        // Flashlight toggle (Y)
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Jump"))
        {
            flashlight.Toggle();
        }
        
        // Interact (E)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire3"))
        {
            HandleInteractionPressed();
        }
    }
    
    /// <summary>
    /// Handles player movement and physics.
    /// </summary>
    private void HandleMovement()
    {
        // Determine if moving
        bool wasMoving = isMoving;
        isMoving = horizontalInput != 0 || verticalInput != 0;
        
        if (isMoving != wasMoving)
        {
            OnMovingChanged?.Invoke(isMoving);
        }
        
        // Check Rule 2: Stop if children laughing
        if (GameManager.Instance.GetRuleManager().MustPlayerBeSilent() && isMoving)
        {
            isMoving = false;
            horizontalInput = 0f;
            verticalInput = 0f;
        }
        
        // Determine current speed
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting && isMoving)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        
        // Calculate movement direction
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection = moveDirection.normalized;
        
        // Apply velocity
        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
        
        // Apply gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative to keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Apply drag
        velocity *= (1 - Time.deltaTime * groundDrag);
        
        // Move character
        characterController.Move(velocity * Time.deltaTime);
        isGrounded = characterController.isGrounded;
    }
    
    /// <summary>
    /// Sets sprint state.
    /// </summary>
    private void SetSprinting(bool sprinting)
    {
        if (isCrouching) return; // Can't sprint while crouching
        
        isSprinting = sprinting;
        OnSprintChanged?.Invoke(sprinting);
        
        if (sprinting)
        {
            GameManager.Instance.GetAudioManager().PlaySFX("sprint_start", transform.position);
        }
        
        Debug.Log($"[PlayerController] Sprint: {sprinting}");
    }
    
    /// <summary>
    /// Sets crouch state and adjusts camera height.
    /// </summary>
    private void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
        isSprinting = false; // Can't sprint while crouching
        
        // Adjust character controller height
        characterController.height = crouching ? crouchHeight : normalHeight;
        
        // Adjust camera position
        Vector3 targetCameraPos = crouching ? crouchCameraPos : normalCameraPos;
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetCameraPos,
            Time.deltaTime * 8f
        );
        
        OnCrouchChanged?.Invoke(crouching);
        Debug.Log($"[PlayerController] Crouch: {crouching}");
    }
    
    /// <summary>
    /// Handles interaction detection and triggering.
    /// </summary>
    private void HandleInteraction()
    {
        // Raycast from camera center
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                OnInteractionDetected?.Invoke(hit);
            }
        }
    }
    
    /// <summary>
    /// Handles interaction button press.
    /// </summary>
    private void HandleInteractionPressed()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }
    
    /// <summary>
    /// Triggers footstep sound based on movement.
    /// </summary>
    private void CheckFootstepTrigger()
    {
        if (!isMoving) return;
        
        // Footstep audio is handled by PlayerFootsteps component
        // This is just for reference
    }
    
    /// <summary>
    /// Gets if player is moving.
    /// </summary>
    public bool IsMoving() => isMoving;
    
    /// <summary>
    /// Gets if player is sprinting.
    /// </summary>
    public bool IsSprinting() => isSprinting;
    
    /// <summary>
    /// Gets if player is crouching.
    /// </summary>
    public bool IsCrouching() => isCrouching;
    
    /// <summary>
    /// Gets the flashlight component.
    /// </summary>
    public Flashlight GetFlashlight() => flashlight;
    
    /// <summary>
    /// Gets the camera component.
    /// </summary>
    public Camera GetCamera() => playerCamera;
    
    /// <summary>
    /// Gets current movement speed.
    /// </summary>
    public float GetCurrentSpeed() => currentSpeed;
}

/// <summary>
/// Interface for interactive objects in the world.
/// </summary>
public interface IInteractable
{
    void Interact(PlayerController player);
    string GetInteractionPrompt();
}
