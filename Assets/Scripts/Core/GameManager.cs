using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Central game manager that controls overall game state, initialization, and coordination.
/// Implements singleton pattern for easy access throughout the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private SanityManager sanityManager;
    [SerializeField] private RuleManager ruleManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private CheckpointSystem checkpointSystem;
    
    public static GameManager Instance { get; private set; }
    
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    
    private GameState currentState = GameState.MainMenu;
    private Dictionary<GameState, Action> stateEnterCallbacks = new Dictionary<GameState, Action>();
    private Dictionary<GameState, Action> stateExitCallbacks = new Dictionary<GameState, Action>();
    
    // Game session data
    public class SessionData
    {
        public float startTime;
        public float endTime;
        public float finalSanity;
        public int tasksCompleted;
        public bool playerSurvived;
        public float playtime;
    }
    
    private SessionData currentSession;
    
    // Events
    public event Action<GameState> OnGameStateChanged;
    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeGame();
    }
    
    private void Start()
    {
        RegisterStateCallbacks();
    }
    
    private void InitializeGame()
    {
        // Initialize all managers
        if (timeManager == null) timeManager = GetComponent<TimeManager>();
        if (sanityManager == null) sanityManager = GetComponent<SanityManager>();
        if (ruleManager == null) ruleManager = GetComponent<RuleManager>();
        if (inventoryManager == null) inventoryManager = GetComponent<InventoryManager>();
        if (audioManager == null) audioManager = GetComponent<AudioManager>();
        if (checkpointSystem == null) checkpointSystem = GetComponent<CheckpointSystem>();
        
        Debug.Log("[GameManager] Initialization complete");
    }
    
    /// <summary>
    /// Starts a new game session.
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[GameManager] Starting new game session");
        
        currentSession = new SessionData
        {
            startTime = Time.realtimeSinceStartup,
            tasksCompleted = 0,
            playerSurvived = false,
            playtime = 0
        };
        
        // Reset all systems
        timeManager.Initialize();
        sanityManager.Initialize();
        ruleManager.Initialize();
        inventoryManager.Initialize();
        playerController.Initialize();
        enemyAI.Initialize();
        
        // Start audio systems
        audioManager.PlayAmbience();
        audioManager.StartDynamicMusic(0);
        
        SetGameState(GameState.Playing);
        OnGameStarted?.Invoke();
    }
    
    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        
        SetGameState(GameState.Paused);
        Time.timeScale = 0f;
        audioManager.PauseAll();
        OnGamePaused?.Invoke();
        
        Debug.Log("[GameManager] Game paused");
    }
    
    /// <summary>
    /// Resumes the game from paused state.
    /// </summary>
    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        
        Time.timeScale = 1f;
        audioManager.ResumeAll();
        SetGameState(GameState.Playing);
        OnGameResumed?.Invoke();
        
        Debug.Log("[GameManager] Game resumed");
    }
    
    /// <summary>
    /// Ends the game with specified outcome.
    /// </summary>
    public void EndGame(bool victory)
    {
        if (currentState != GameState.Playing) return;
        
        currentSession.endTime = Time.realtimeSinceStartup;
        currentSession.playtime = currentSession.endTime - currentSession.startTime;
        currentSession.finalSanity = sanityManager.CurrentSanity;
        currentSession.playerSurvived = victory;
        
        audioManager.StopAll();
        
        SetGameState(victory ? GameState.Victory : GameState.GameOver);
        OnGameEnded?.Invoke();
        
        Debug.Log($"[GameManager] Game ended - Victory: {victory}, Playtime: {currentSession.playtime:F2}s");
    }
    
    /// <summary>
    /// Sets the current game state and triggers callbacks.
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (newState == currentState) return;
        
        // Call exit callback for old state
        if (stateExitCallbacks.ContainsKey(currentState))
        {
            stateExitCallbacks[currentState]?.Invoke();
        }
        
        currentState = newState;
        
        // Call enter callback for new state
        if (stateEnterCallbacks.ContainsKey(newState))
        {
            stateEnterCallbacks[newState]?.Invoke();
        }
        
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] Game state changed to: {newState}");
    }
    
    /// <summary>
    /// Registers callbacks for state transitions.
    /// </summary>
    private void RegisterStateCallbacks()
    {
        stateEnterCallbacks[GameState.Playing] = () =>
        {
            Time.timeScale = 1f;
        };
        
        stateExitCallbacks[GameState.Playing] = () =>
        {
            // Cleanup when exiting play state
        };
    }
    
    /// <summary>
    /// Registers a callback for when entering a specific game state.
    /// </summary>
    public void RegisterStateEnterCallback(GameState state, Action callback)
    {
        if (stateEnterCallbacks.ContainsKey(state))
        {
            stateEnterCallbacks[state] += callback;
        }
        else
        {
            stateEnterCallbacks[state] = callback;
        }
    }
    
    /// <summary>
    /// Registers a callback for when exiting a specific game state.
    /// </summary>
    public void RegisterStateExitCallback(GameState state, Action callback)
    {
        if (stateExitCallbacks.ContainsKey(state))
        {
            stateExitCallbacks[state] += callback;
        }
        else
        {
            stateExitCallbacks[state] = callback;
        }
    }
    
    /// <summary>
    /// Creates a checkpoint at the current game state.
    /// </summary>
    public void CreateCheckpoint(string name)
    {
        if (checkpointSystem != null)
        {
            checkpointSystem.CreateCheckpoint(name);
            Debug.Log($"[GameManager] Checkpoint created: {name}");
        }
    }
    
    /// <summary>
    /// Loads a checkpoint.
    /// </summary>
    public void LoadCheckpoint(string name)
    {
        if (checkpointSystem != null)
        {
            checkpointSystem.LoadCheckpoint(name);
            Debug.Log($"[GameManager] Checkpoint loaded: {name}");
        }
    }
    
    /// <summary>
    /// Gets the current game state.
    /// </summary>
    public GameState GetCurrentState() => currentState;
    
    /// <summary>
    /// Gets the current session data.
    /// </summary>
    public SessionData GetSessionData() => currentSession;
    
    /// <summary>
    /// Checks if the game is currently running.
    /// </summary>
    public bool IsGameRunning() => currentState == GameState.Playing;
    
    /// <summary>
    /// Checks if the game is paused.
    /// </summary>
    public bool IsGamePaused() => currentState == GameState.Paused;
    
    /// <summary>
    /// Returns the time manager.
    /// </summary>
    public TimeManager GetTimeManager() => timeManager;
    
    /// <summary>
    /// Returns the sanity manager.
    /// </summary>
    public SanityManager GetSanityManager() => sanityManager;
    
    /// <summary>
    /// Returns the rule manager.
    /// </summary>
    public RuleManager GetRuleManager() => ruleManager;
    
    /// <summary>
    /// Returns the inventory manager.
    /// </summary>
    public InventoryManager GetInventoryManager() => inventoryManager;
    
    /// <summary>
    /// Returns the audio manager.
    /// </summary>
    public AudioManager GetAudioManager() => audioManager;
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
