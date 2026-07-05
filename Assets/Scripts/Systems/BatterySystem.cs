using UnityEngine;
using System;

/// <summary>
/// Manages battery system for the flashlight.
/// Battery depletes when flashlight is on and recharges when off.
/// </summary>
public class BatterySystem : MonoBehaviour
{
    [SerializeField] private Flashlight flashlight;
    [SerializeField] private float warningThreshold = 0.2f; // 20% battery warning
    
    private bool hasWarned = false;
    
    // Events
    public event Action OnBatteryLow;
    public event Action OnBatteryDepleted;
    
    private void Start()
    {
        if (flashlight == null)
            flashlight = GetComponent<Flashlight>();
        
        flashlight.OnBatteryChanged += HandleBatteryChanged;
        flashlight.OnBatteryDepleted += HandleBatteryDepleted;
    }
    
    /// <summary>
    /// Handles battery change events.
    /// </summary>
    private void HandleBatteryChanged(float batteryPercentage)
    {
        if (batteryPercentage < warningThreshold * 100f && !hasWarned)
        {
            hasWarned = true;
            OnBatteryLow?.Invoke();
            Debug.Log("[BatterySystem] Low battery warning!");
        }
        
        if (batteryPercentage > warningThreshold * 100f)
        {
            hasWarned = false;
        }
    }
    
    /// <summary>
    /// Handles battery depletion.
    /// </summary>
    private void HandleBatteryDepleted()
    {
        OnBatteryDepleted?.Invoke();
        Debug.Log("[BatterySystem] Battery completely depleted!");
    }
    
    private void OnDestroy()
    {
        if (flashlight != null)
        {
            flashlight.OnBatteryChanged -= HandleBatteryChanged;
            flashlight.OnBatteryDepleted -= HandleBatteryDepleted;
        }
    }
}
