using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// In-game HUD displaying time, sanity, battery, tasks, and interaction prompts.
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField] private Text timeDisplay;
    [SerializeField] private Image sanityBar;
    [SerializeField] private Text sanityText;
    [SerializeField] private Image batteryBar;
    [SerializeField] private Text batteryText;
    [SerializeField] private Text taskDisplay;
    [SerializeField] private Text interactionPrompt;
    [SerializeField] private Image lookAtSarcophagusWarning;
    [SerializeField] private Image silenceWarning;
    [SerializeField] private Image mirrorWarning;
    
    private TimeManager timeManager;
    private SanityManager sanityManager;
    private InventoryManager inventoryManager;
    
    private void Start()
    {
        timeManager = GameManager.Instance.GetTimeManager();
        sanityManager = GameManager.Instance.GetSanityManager();
        inventoryManager = GameManager.Instance.GetInventoryManager();
        
        // Subscribe to events
        timeManager.OnTimeUpdated += UpdateTimeDisplay;
        sanityManager.OnSanityChanged += UpdateSanityDisplay;
        GameManager.Instance.GetPlayerController().GetFlashlight().OnBatteryChanged += UpdateBatteryDisplay;
        inventoryManager.OnTaskCompleted += UpdateTaskDisplay;
        
        Debug.Log("[HUD] In-game HUD initialized");
    }
    
    private void Update()
    {
        UpdateWarnings();
    }
    
    private void UpdateTimeDisplay(int hour, int minute, float second)
    {
        int displayHour = hour;
        string period = "AM";
        if (displayHour >= 12) { period = "PM"; if (displayHour > 12) displayHour -= 12; }
        if (displayHour == 0) displayHour = 12;
        
        timeDisplay.text = $"{displayHour}:{minute:D2} {period}";
    }
    
    private void UpdateSanityDisplay(float sanity)
    {
        sanityBar.fillAmount = sanityManager.GetSanityNormalized();
        sanityText.text = $"SANITY: {sanityManager.GetSanityPercentage():F0}%";
        
        // Change color based on sanity level
        if (sanityManager.IsSanityCritical())
            sanityBar.color = Color.red;
        else if (sanityManager.IsSanityLow())
            sanityBar.color = new Color(1f, 0.5f, 0f); // Orange
        else
            sanityBar.color = Color.green;
    }
    
    private void UpdateBatteryDisplay(float batteryPercentage)
    {
        batteryBar.fillAmount = batteryPercentage / 100f;
        batteryText.text = $"BATTERY: {batteryPercentage:F0}%";
        
        if (batteryPercentage < 20f)
            batteryBar.color = Color.red;
        else if (batteryPercentage < 50f)
            batteryBar.color = new Color(1f, 0.5f, 0f);
        else
            batteryBar.color = Color.green;
    }
    
    private void UpdateTaskDisplay(InventoryManager.Task completedTask)
    {
        int completed = inventoryManager.GetCompletedTaskCount();
        int total = inventoryManager.GetTotalTaskCount();
        taskDisplay.text = $"Tasks: {completed}/{total}";
    }
    
    private void UpdateWarnings()
    {
        // Rule 1 warning - looking at sarcophagus
        if (GameManager.Instance.GetRuleManager().IsRule1Active())
        {
            float lookTime = GameManager.Instance.GetRuleManager().GetSarcophagusLookTime();
            lookAtSarcophagusWarning.fillAmount = Mathf.Clamp01(lookTime / 1.5f);
            lookAtSarcophagusWarning.gameObject.SetActive(lookTime > 0);
        }
        
        // Rule 2 warning - silence
        silenceWarning.gameObject.SetActive(GameManager.Instance.GetRuleManager().MustPlayerBeSilent());
        
        // Rule 3 warning - mirror
        mirrorWarning.gameObject.SetActive(GameManager.Instance.GetRuleManager().IsRule3Active());
    }
    
    private void OnDestroy()
    {
        if (timeManager != null) timeManager.OnTimeUpdated -= UpdateTimeDisplay;
        if (sanityManager != null) sanityManager.OnSanityChanged -= UpdateSanityDisplay;
    }
}
