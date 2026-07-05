using UnityEngine;
using System;

/// <summary>
/// Manages the player's sanity meter. Sanity decreases with horror events and rule violations.
/// When sanity reaches 0, the entity catches the player and game ends.
/// </summary>
public class SanityManager : MonoBehaviour
{
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;
    [SerializeField] private float naturalDecayRate = 0.1f; // Sanity lost per second over time
    [SerializeField] private float ruleViolationPenalty = 15f; // Sanity lost per rule violation
    [SerializeField] private float lookingSarcophagusPenalty = 5f; // Per 0.1 seconds looking at sarcophagus
    [SerializeField] private float movementDuringSilencePenalty = 20f; // For moving during Rule 2
    [SerializeField] private float sanityRecoveryRate = 0.5f; // Sanity regain per second
    [SerializeField] private float maxRecoveryPerSecond = 1f; // Cap on recovery
    
    private float sanityRecoveryTimer = 0f;
    private float recoveryDelay = 3f; // Delay before sanity starts recovering after disturbance
    private bool isRecovering = false;
    
    // Events
    public event Action<float> OnSanityChanged; // Passes current sanity (0-100)
    public event Action OnSanityDepleted; // Fired when sanity reaches 0
    public event Action OnSanityLow; // Fired when sanity drops below 20%
    public event Action OnSanityRecovery; // Fired when sanity starts recovering
    
    private bool isInitialized = false;
    private bool hasTriggeredLowSanity = false;
    
    public void Initialize()
    {
        currentSanity = maxSanity;
        sanityRecoveryTimer = 0f;
        isRecovering = false;
        hasTriggeredLowSanity = false;
        isInitialized = true;
        
        Debug.Log("[SanityManager] Sanity initialized to: " + maxSanity);
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        
        // Natural sanity decay
        ReduceSanity(naturalDecayRate * Time.deltaTime);
        
        // Sanity recovery logic
        UpdateSanityRecovery();
        
        // Check for critical thresholds
        CheckSanityCritical();
    }
    
    /// <summary>
    /// Reduces sanity by a specified amount.
    /// </summary>
    public void ReduceSanity(float amount)
    {
        if (amount <= 0) return;
        
        currentSanity = Mathf.Max(0f, currentSanity - amount);
        isRecovering = false;
        sanityRecoveryTimer = 0f;
        
        OnSanityChanged?.Invoke(currentSanity);
        
        // Check for depletion
        if (currentSanity <= 0)
        {
            OnSanityDepleted?.Invoke();
            GameManager.Instance.EndGame(false); // Game over - entity catches player
            Debug.Log("[SanityManager] SANITY DEPLETED - Entity claims player!");
        }
    }
    
    /// <summary>
    /// Increases sanity by a specified amount.
    /// </summary>
    public void RestoreSanity(float amount)
    {
        if (amount <= 0) return;
        
        float previousSanity = currentSanity;
        currentSanity = Mathf.Min(maxSanity, currentSanity + amount);
        
        OnSanityChanged?.Invoke(currentSanity);
        
        if (previousSanity < 20f && currentSanity >= 20f)
        {
            hasTriggeredLowSanity = false; // Reset low sanity flag
        }
    }
    
    /// <summary>
    /// Handles sanity loss from rule violations.
    /// </summary>
    public void OnRuleViolation(int ruleNumber)
    {
        float penalty = ruleViolationPenalty;
        
        // Increase penalty severity based on which rule and how late in the game
        if (GameManager.Instance.GetTimeManager().IsAfter3AM())
        {
            penalty *= 1.5f; // 50% more severe after 3 AM
        }
        
        if (GameManager.Instance.GetTimeManager().IsAfter5AM())
        {
            penalty *= 2f; // 100% more severe after 5 AM
        }
        
        ReduceSanity(penalty);
        Debug.Log($"[SanityManager] Rule {ruleNumber} violation - Sanity reduced by {penalty:F1}");
    }
    
    /// <summary>
    /// Handles continuous sanity loss when looking at sarcophagus past the time limit.
    /// </summary>
    public void OnLookingSarcophagusExceedLimit(float excessLookTime)
    {
        float penalty = lookingSarcophagusPenalty * excessLookTime;
        ReduceSanity(penalty);
    }
    
    /// <summary>
    /// Handles sanity loss from moving during the Silence rule.
    /// </summary>
    public void OnMovementDuringSilence()
    {
        ReduceSanity(movementDuringSilencePenalty);
        Debug.Log("[SanityManager] Moved during silence - Sanity reduced");
    }
    
    /// <summary>
    /// Triggers sanity recovery (when looking away).
    /// </summary>
    public void StartSanityRecovery()
    {
        if (!isRecovering)
        {
            isRecovering = true;
            sanityRecoveryTimer = recoveryDelay;
            OnSanityRecovery?.Invoke();
            Debug.Log("[SanityManager] Sanity recovery started");
        }
    }
    
    /// <summary>
    /// Updates sanity recovery over time.
    /// </summary>
    private void UpdateSanityRecovery()
    {
        if (!isRecovering) return;
        
        if (sanityRecoveryTimer > 0)
        {
            sanityRecoveryTimer -= Time.deltaTime;
            return; // Still in recovery delay
        }
        
        // Apply recovery
        float recovery = Mathf.Min(sanityRecoveryRate * Time.deltaTime, maxRecoveryPerSecond * Time.deltaTime);
        RestoreSanity(recovery);
    }
    
    /// <summary>
    /// Checks for critical sanity thresholds.
    /// </summary>
    private void CheckSanityCritical()
    {
        if (currentSanity <= 0)
        {
            OnSanityDepleted?.Invoke();
            return;
        }
        
        // Trigger low sanity warning at 20% health
        if (currentSanity < maxSanity * 0.2f && !hasTriggeredLowSanity)
        {
            hasTriggeredLowSanity = true;
            OnSanityLow?.Invoke();
            Debug.Log("[SanityManager] LOW SANITY WARNING");
        }
    }
    
    /// <summary>
    /// Gets the current sanity value (0-100).
    /// </summary>
    public float CurrentSanity => currentSanity;
    
    /// <summary>
    /// Gets sanity as a normalized value (0-1).
    /// </summary>
    public float GetSanityNormalized() => currentSanity / maxSanity;
    
    /// <summary>
    /// Gets the sanity percentage (0-100%).
    /// </summary>
    public float GetSanityPercentage() => (currentSanity / maxSanity) * 100f;
    
    /// <summary>
    /// Checks if sanity is critically low (below 20%).
    /// </summary>
    public bool IsSanityLow() => currentSanity < maxSanity * 0.2f;
    
    /// <summary>
    /// Checks if sanity is critically depleted (below 10%).
    /// </summary>
    public bool IsSanityCritical() => currentSanity < maxSanity * 0.1f;
    
    /// <summary>
    /// Gets max sanity value.
    /// </summary>
    public float GetMaxSanity() => maxSanity;
    
    /// <summary>
    /// Temporarily multiplies sanity decay (for intense horror moments).
    /// </summary>
    public void SetSanityDecayMultiplier(float multiplier)
    {
        // This would be called during intense scenes
        // Implementation depends on how you want to handle temporary effects
    }
}
