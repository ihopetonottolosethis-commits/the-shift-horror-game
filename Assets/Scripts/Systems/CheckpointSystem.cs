using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the checkpoint/save system for hourly progression.
/// Saves player state at the end of each hour.
/// </summary>
public class CheckpointSystem : MonoBehaviour
{
    [System.Serializable]
    private class CheckpointData
    {
        public string checkpointName;
        public float gameTime;
        public float playerSanity;
        public Vector3 playerPosition;
        public int tasksCompleted;
    }
    
    private Dictionary<string, CheckpointData> checkpoints = new Dictionary<string, CheckpointData>();
    private string lastCheckpointName = "";
    
    private bool isInitialized = false;
    
    public void Initialize()
    {
        checkpoints.Clear();
        isInitialized = true;
        Debug.Log("[CheckpointSystem] Checkpoint system initialized");
    }
    
    /// <summary>
    /// Creates a checkpoint at the current game state.
    /// </summary>
    public void CreateCheckpoint(string name)
    {
        if (!isInitialized) return;
        
        CheckpointData checkpoint = new CheckpointData
        {
            checkpointName = name,
            gameTime = GameManager.Instance.GetTimeManager().GetElapsedTime(),
            playerSanity = GameManager.Instance.GetSanityManager().CurrentSanity,
            playerPosition = GameManager.Instance.GetPlayerController().transform.position,
            tasksCompleted = GameManager.Instance.GetInventoryManager().GetCompletedTaskCount()
        };
        
        checkpoints[name] = checkpoint;
        lastCheckpointName = name;
        
        Debug.Log($"[CheckpointSystem] Checkpoint created: {name}");
    }
    
    /// <summary>
    /// Loads a checkpoint.
    /// </summary>
    public void LoadCheckpoint(string name)
    {
        if (!checkpoints.ContainsKey(name))
        {
            Debug.LogWarning($"[CheckpointSystem] Checkpoint not found: {name}");
            return;
        }
        
        CheckpointData checkpoint = checkpoints[name];
        
        // Restore player position
        GameManager.Instance.GetPlayerController().transform.position = checkpoint.playerPosition;
        
        // Note: Time and sanity restoration would require more complex state management
        Debug.Log($"[CheckpointSystem] Checkpoint loaded: {name}");
    }
    
    /// <summary>
    /// Gets the last created checkpoint.
    /// </summary>
    public string GetLastCheckpoint() => lastCheckpointName;
    
    /// <summary>
    /// Gets all checkpoint names.
    /// </summary>
    public string[] GetAllCheckpoints() => new List<string>(checkpoints.Keys).ToArray();
}
