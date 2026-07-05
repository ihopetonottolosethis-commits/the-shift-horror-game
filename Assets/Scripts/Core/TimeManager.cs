using UnityEngine;
using System;

/// <summary>
/// Manages game time progression from 12 AM to 6 AM (12-minute session).
/// Each game hour equals 2 real minutes (120 seconds).
/// </summary>
public class TimeManager : MonoBehaviour
{
    [SerializeField] private float gameHourDurationSeconds = 120f; // 2 real minutes per game hour
    [SerializeField] private int startHour = 0; // 12 AM (0:00)
    [SerializeField] private int endHour = 6; // 6 AM (6:00)
    
    private float elapsedGameTime = 0f;
    private int currentHour = 0;
    private int currentMinute = 0;
    private float currentSecond = 0f;
    private bool isInitialized = false;
    
    // Events
    public event Action<int> OnHourChanged; // Fired when game hour changes (1 AM, 2 AM, etc.)
    public event Action<int, int, float> OnTimeUpdated; // Fired every frame with current time
    public event Action OnGameOver; // Fired when 6 AM is reached
    public event Action<int> OnHourReached; // Fired when specific hour is reached
    
    private const int TOTAL_GAME_HOURS = 6; // 12 AM to 6 AM
    private const float TOTAL_GAME_TIME_SECONDS = 720f; // 6 hours * 120 seconds
    
    public void Initialize()
    {
        elapsedGameTime = 0f;
        currentHour = startHour;
        currentMinute = 0;
        currentSecond = 0f;
        isInitialized = true;
        
        Debug.Log("[TimeManager] Time initialized - Starting at 12 AM");
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        
        UpdateGameTime();
        CheckGameEnd();
    }
    
    /// <summary>
    /// Updates the game time and checks for hour changes.
    /// </summary>
    private void UpdateGameTime()
    {
        int previousHour = currentHour;
        
        elapsedGameTime += Time.deltaTime;
        
        // Calculate hours, minutes, seconds from elapsed time
        float totalMinutes = elapsedGameTime / 60f;
        currentHour = startHour + Mathf.FloorToInt(totalMinutes / 60f);
        currentMinute = Mathf.FloorToInt(totalMinutes % 60f);
        currentSecond = elapsedGameTime % 60f;
        
        // Clamp to end time
        if (currentHour > endHour)
        {
            currentHour = endHour;
            currentMinute = 0;
            currentSecond = 0f;
        }
        
        // Fire time updated event
        OnTimeUpdated?.Invoke(currentHour, currentMinute, currentSecond);
        
        // Check if hour changed
        if (currentHour != previousHour && currentHour != startHour)
        {
            OnHourChanged?.Invoke(currentHour);
            OnHourReached?.Invoke(currentHour);
            
            Debug.Log($"[TimeManager] Hour changed to: {currentHour}:00 ({GetTimeString()})");
            
            // Create checkpoint at hour boundaries
            GameManager.Instance.CreateCheckpoint($"Hour_{currentHour}");
        }
    }
    
    /// <summary>
    /// Checks if the game time has reached 6 AM (game end).
    /// </summary>
    private void CheckGameEnd()
    {
        if (currentHour >= endHour && currentMinute >= 0 && elapsedGameTime >= TOTAL_GAME_TIME_SECONDS)
        {
            OnGameOver?.Invoke();
            GameManager.Instance.EndGame(true); // Victory if survived until 6 AM
            Debug.Log("[TimeManager] Game time reached 6 AM - Game Over!");
        }
    }
    
    /// <summary>
    /// Gets the current game time as a formatted string (e.g., "1:30 AM").
    /// </summary>
    public string GetTimeString()
    {
        int displayHour = currentHour;
        string period = "AM";
        
        // Convert 24-hour to 12-hour format
        if (displayHour >= 12)
        {
            period = "PM";
            if (displayHour > 12) displayHour -= 12;
        }
        
        if (displayHour == 0) displayHour = 12; // Midnight = 12 AM
        
        return $"{displayHour}:{currentMinute:D2} {period}";
    }
    
    /// <summary>
    /// Gets the progress through the entire game (0-1).
    /// </summary>
    public float GetGameProgress()
    {
        return Mathf.Clamp01(elapsedGameTime / TOTAL_GAME_TIME_SECONDS);
    }
    
    /// <summary>
    /// Gets the time remaining until 6 AM in seconds.
    /// </summary>
    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, TOTAL_GAME_TIME_SECONDS - elapsedGameTime);
    }
    
    /// <summary>
    /// Gets the progress through the current hour (0-1).
    /// </summary>
    public float GetHourProgress()
    {
        float minutesIntoHour = (elapsedGameTime % (gameHourDurationSeconds)) / gameHourDurationSeconds;
        return Mathf.Clamp01(minutesIntoHour);
    }
    
    /// <summary>
    /// Gets the current hour (0-6, where 0 = 12 AM).
    /// </summary>
    public int GetCurrentHour() => currentHour;
    
    /// <summary>
    /// Gets the current minute (0-59).
    /// </summary>
    public int GetCurrentMinute() => currentMinute;
    
    /// <summary>
    /// Gets the current second (0-59).
    /// </summary>
    public float GetCurrentSecond() => currentSecond;
    
    /// <summary>
    /// Gets total elapsed game time in seconds.
    /// </summary>
    public float GetElapsedTime() => elapsedGameTime;
    
    /// <summary>
    /// Checks if a specific hour has been reached.
    /// </summary>
    public bool IsHourReached(int hour)
    {
        return currentHour >= hour;
    }
    
    /// <summary>
    /// Gets the duration of one game hour in real seconds.
    /// </summary>
    public float GetGameHourDuration() => gameHourDurationSeconds;
    
    /// <summary>
    /// Checks if it's after 1 AM (for Rule 1 triggering).
    /// </summary>
    public bool IsAfter1AM() => currentHour >= 1;
    
    /// <summary>
    /// Checks if it's after 3 AM (for increased difficulty).
    /// </summary>
    public bool IsAfter3AM() => currentHour >= 3;
    
    /// <summary>
    /// Checks if it's after 5 AM (for final phase).
    /// </summary>
    public bool IsAfter5AM() => currentHour >= 5;
}
