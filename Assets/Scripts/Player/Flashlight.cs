using UnityEngine;
using System;

/// <summary>
/// Manages the player flashlight with battery system.
/// Battery drains over time and when flashlight is on.
/// </summary>
public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;
    [SerializeField] private float maxBattery = 300f; // 5 minutes of flashlight use
    [SerializeField] private float currentBattery = 300f;
    [SerializeField] private float drainRate = 0.5f; // Battery % per second
    [SerializeField] private float rechargeRate = 0.2f; // Recharge speed (when not using)
    
    private bool isEnabled = false;
    private bool isBatteryDepleted = false;
    
    // Events
    public event Action<float> OnBatteryChanged; // Passes battery percentage (0-100)
    public event Action OnFlashlightToggled;
    public event Action OnBatteryDepleted;
    public event Action OnBatteryRestored;
    
    private void Start()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponent<Light>();
        }
        
        currentBattery = maxBattery;
        DisableFlashlight();
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGameRunning()) return;
        
        UpdateBattery();
    }
    
    /// <summary>
    /// Updates battery drain/recharge.
    /// </summary>
    private void UpdateBattery()
    {
        if (isEnabled)
        {
            // Drain battery
            currentBattery -= maxBattery * drainRate * Time.deltaTime / 100f;
            
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isBatteryDepleted = true;
                DisableFlashlight();
                OnBatteryDepleted?.Invoke();
                Debug.Log("[Flashlight] Battery depleted!");
            }
        }
        else if (!isBatteryDepleted)
        {
            // Recharge battery when off
            currentBattery = Mathf.Min(currentBattery + maxBattery * rechargeRate * Time.deltaTime / 100f, maxBattery);
        }
        
        OnBatteryChanged?.Invoke(GetBatteryPercentage());
    }
    
    /// <summary>
    /// Toggles the flashlight on/off.
    /// </summary>
    public void Toggle()
    {
        if (isEnabled)
        {
            DisableFlashlight();
        }
        else
        {
            EnableFlashlight();
        }
        
        OnFlashlightToggled?.Invoke();
    }
    
    /// <summary>
    /// Enables the flashlight.
    /// </summary>
    public void EnableFlashlight()
    {
        if (isBatteryDepleted) return;
        
        isEnabled = true;
        flashlightLight.enabled = true;
        Debug.Log("[Flashlight] Enabled");
    }
    
    /// <summary>
    /// Disables the flashlight.
    /// </summary>
    public void DisableFlashlight()
    {
        isEnabled = false;
        flashlightLight.enabled = false;
        Debug.Log("[Flashlight] Disabled");
    }
    
    /// <summary>
    /// Gets battery as percentage (0-100).
    /// </summary>
    public float GetBatteryPercentage() => (currentBattery / maxBattery) * 100f;
    
    /// <summary>
    /// Gets battery as normalized value (0-1).
    /// </summary>
    public float GetBatteryNormalized() => currentBattery / maxBattery;
    
    /// <summary>
    /// Checks if flashlight is enabled.
    /// </summary>
    public bool IsEnabled => isEnabled;
    
    /// <summary>
    /// Checks if battery is depleted.
    /// </summary>
    public bool IsBatteryDepleted => isBatteryDepleted;
    
    /// <summary>
    /// Restores battery to full (for debugging or special events).
    /// </summary>
    public void RestoreBattery()
    {
        currentBattery = maxBattery;
        isBatteryDepleted = false;
        OnBatteryRestored?.Invoke();
        Debug.Log("[Flashlight] Battery restored");
    }
    
    /// <summary>
    /// Gets the flashlight light component.
    /// </summary>
    public Light GetLight() => flashlightLight;
}
