using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the three horror rules that the player must follow:
/// Rule 1: Don't look at sarcophagus for more than 1.5 seconds after 1 AM
/// Rule 2: Stop moving immediately when children laughing is heard
/// Rule 3: Turn off flashlight and crouch when mirror reflection stays still
/// </summary>
public class RuleManager : MonoBehaviour
{
    [SerializeField] private float sarcophagusLookLimitSeconds = 1.5f;
    [SerializeField] private float ruleViolationCooldown = 5f; // Prevent multiple violations from same trigger
    
    // Rule 1 data
    private float currentSarcophagusLookTime = 0f;
    private bool isLookingAtSarcophagus = false;
    private bool sarcophagusRuleActive = false;
    private float lastSarcophagusViolationTime = -999f;
    
    // Rule 2 data
    private bool isChildrenLaughingActive = false;
    private bool playerMustBeSilent = false;
    private float lastSilenceViolationTime = -999f;
    
    // Rule 3 data
    private bool isMirrorReflectionStill = false;
    private float mirrorStillStartTime = 0f;
    private float requiredCrouchTime = 5f; // 5 seconds as specified
    private float lastMirrorViolationTime = -999f;
    
    // Events
    public event Action<int> OnRuleActivated; // Rule number
    public event Action<int> OnRuleViolated; // Rule number
    public event Action<int> OnRuleResolved; // Rule number
    
    private bool isInitialized = false;
    
    public void Initialize()
    {
        ResetAllRules();
        isInitialized = true;
        Debug.Log("[RuleManager] Initialized - 3 rules activated");
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        
        UpdateRule1();
        UpdateRule2();
        UpdateRule3();
    }
    
    /// <summary>
    /// Updates Rule 1: Don't look at sarcophagus for more than 1.5 seconds after 1 AM.
    /// </summary>
    private void UpdateRule1()
    {
        // Activate rule after 1 AM
        if (GameManager.Instance.GetTimeManager().IsAfter1AM() && !sarcophagusRuleActive)
        {
            sarcophagusRuleActive = true;
            OnRuleActivated?.Invoke(1);
            Debug.Log("[RuleManager] Rule 1 ACTIVATED - Don't look at sarcophagus!");
        }
        
        if (!sarcophagusRuleActive) return;
        
        // If player is looking at sarcophagus
        if (isLookingAtSarcophagus)
        {
            currentSarcophagusLookTime += Time.deltaTime;
            
            // Check if exceeding limit
            if (currentSarcophagusLookTime > sarcophagusLookLimitSeconds)
            {
                // Check cooldown to avoid spam violations
                if (Time.time - lastSarcophagusViolationTime > ruleViolationCooldown)
                {
                    OnRuleViolated?.Invoke(1);
                    GameManager.Instance.GetSanityManager().OnRuleViolation(1);
                    lastSarcophagusViolationTime = Time.time;
                    
                    Debug.Log("[RuleManager] Rule 1 VIOLATED - Looked at sarcophagus too long!");
                }
            }
        }
        else
        {
            // Reset look time when not looking
            currentSarcophagusLookTime = 0f;
        }
    }
    
    /// <summary>
    /// Updates Rule 2: Stop moving immediately when children laughing is heard.
    /// </summary>
    private void UpdateRule2()
    {
        if (!playerMustBeSilent) return;
        
        // Check if player is moving
        if (GameManager.Instance.GetPlayerController().IsMoving())
        {
            // Check cooldown
            if (Time.time - lastSilenceViolationTime > ruleViolationCooldown)
            {
                OnRuleViolated?.Invoke(2);
                GameManager.Instance.GetSanityManager().OnMovementDuringSilence();
                lastSilenceViolationTime = Time.time;
                
                Debug.Log("[RuleManager] Rule 2 VIOLATED - Moved during silence!");
            }
        }
    }
    
    /// <summary>
    /// Updates Rule 3: Turn off flashlight and crouch when mirror reflection stays still.
    /// </summary>
    private void UpdateRule3()
    {
        if (!isMirrorReflectionStill) return;
        
        // Check if player has turned off flashlight and crouched
        PlayerController player = GameManager.Instance.GetPlayerController();
        
        if (!player.GetFlashlight().IsEnabled && player.IsCrouching())
        {
            // Increment time in safe state
            mirrorStillStartTime += Time.deltaTime;
            
            if (mirrorStillStartTime >= requiredCrouchTime)
            {
                // Rule 3 resolved
                isMirrorReflectionStill = false;
                OnRuleResolved?.Invoke(3);
                Debug.Log("[RuleManager] Rule 3 RESOLVED - Survived mirror reflection!");
            }
        }
        else
        {
            // Player failed to follow rule 3
            if (Time.time - lastMirrorViolationTime > ruleViolationCooldown)
            {
                OnRuleViolated?.Invoke(3);
                GameManager.Instance.GetSanityManager().OnRuleViolation(3);
                lastMirrorViolationTime = Time.time;
                
                // Reset mirror state
                isMirrorReflectionStill = false;
                Debug.Log("[RuleManager] Rule 3 VIOLATED - Failed to crouch and disable flashlight!");
            }
        }
    }
    
    /// <summary>
    /// Called when the sarcophagus starts rattling.
    /// </summary>
    public void OnSarcophagusRattle()
    {
        isLookingAtSarcophagus = false;
        currentSarcophagusLookTime = 0f;
        Debug.Log("[RuleManager] Sarcophagus rattled!");
    }
    
    /// <summary>
    /// Called from the camera when player is looking at sarcophagus.
    /// </summary>
    public void SetLookingAtSarcophagus(bool looking)
    {
        if (sarcophagusRuleActive)
        {
            isLookingAtSarcophagus = looking;
        }
    }
    
    /// <summary>
    /// Gets current look time at sarcophagus.
    /// </summary>
    public float GetSarcophagusLookTime() => currentSarcophagusLookTime;
    
    /// <summary>
    /// Called when children laughing audio plays.
    /// </summary>
    public void TriggerChildrenLaughing()
    {
        if (isChildrenLaughingActive) return; // Already active
        
        isChildrenLaughingActive = true;
        playerMustBeSilent = true;
        lastSilenceViolationTime = Time.time;
        
        OnRuleActivated?.Invoke(2);
        Debug.Log("[RuleManager] Rule 2 ACTIVATED - Children laughing! Stop moving!");
    }
    
    /// <summary>
    /// Deactivates Rule 2 after the laughing stops.
    /// </summary>
    public void EndChildrenLaughing()
    {
        isChildrenLaughingActive = false;
        playerMustBeSilent = false;
        OnRuleResolved?.Invoke(2);
        Debug.Log("[RuleManager] Rule 2 RESOLVED - Laughing stopped");
    }
    
    /// <summary>
    /// Called when mirror reflection becomes static.
    /// </summary>
    public void TriggerMirrorReflectionStill()
    {
        if (isMirrorReflectionStill) return; // Already active
        
        isMirrorReflectionStill = true;
        mirrorStillStartTime = 0f;
        lastMirrorViolationTime = Time.time;
        
        OnRuleActivated?.Invoke(3);
        Debug.Log("[RuleManager] Rule 3 ACTIVATED - Mirror reflection is still! Turn off flashlight and crouch!");
    }
    
    /// <summary>
    /// Checks if player must be silent (Rule 2).
    /// </summary>
    public bool MustPlayerBeSilent() => playerMustBeSilent;
    
    /// <summary>
    /// Checks if Rule 1 is currently active.
    /// </summary>
    public bool IsRule1Active() => sarcophagusRuleActive;
    
    /// <summary>
    /// Checks if Rule 3 is currently active (mirror reflection still).
    /// </summary>
    public bool IsRule3Active() => isMirrorReflectionStill;
    
    /// <summary>
    /// Resets all rules to initial state.
    /// </summary>
    private void ResetAllRules()
    {
        isLookingAtSarcophagus = false;
        currentSarcophagusLookTime = 0f;
        sarcophagusRuleActive = false;
        
        isChildrenLaughingActive = false;
        playerMustBeSilent = false;
        
        isMirrorReflectionStill = false;
        mirrorStillStartTime = 0f;
        
        lastSarcophagusViolationTime = -999f;
        lastSilenceViolationTime = -999f;
        lastMirrorViolationTime = -999f;
    }
    
    public PlayerController GetPlayerController() => GameManager.Instance.GetPlayerController();
}
