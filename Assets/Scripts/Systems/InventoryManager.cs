using UnityEngine;
using System;

/// <summary>
/// Manages the player's inventory and task tracking.
/// Tracks which tasks are completed and manages interactive items.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class Task
    {
        public string taskId;
        public string taskName;
        public string description;
        public bool isCompleted;
        public int hourAssigned; // Which hour this task should be done
        public float completionReward; // Sanity recovery when completed
        public AudioClip completionSound;
    }
    
    private Task[] tasks = new Task[7];
    private int completedTaskCount = 0;
    
    // Events
    public event Action<Task> OnTaskStarted;
    public event Action<Task> OnTaskCompleted;
    public event Action<int> OnProgressUpdated; // Total tasks completed
    
    private bool isInitialized = false;
    
    public void Initialize()
    {
        InitializeTasks();
        completedTaskCount = 0;
        isInitialized = true;
        
        Debug.Log("[InventoryManager] Inventory initialized with 7 tasks");
    }
    
    /// <summary>
    /// Initializes all available tasks.
    /// </summary>
    private void InitializeTasks()
    {
        tasks[0] = new Task
        {
            taskId = "mop_floors",
            taskName = "Mop Floors",
            description = "Clean the main hallway floors",
            isCompleted = false,
            hourAssigned = 1,
            completionReward = 5f
        };
        
        tasks[1] = new Task
        {
            taskId = "lock_doors",
            taskName = "Lock Doors",
            description = "Secure all entry points",
            isCompleted = false,
            hourAssigned = 2,
            completionReward = 5f
        };
        
        tasks[2] = new Task
        {
            taskId = "check_cameras",
            taskName = "Check Cameras",
            description = "Monitor security feeds in office",
            isCompleted = false,
            hourAssigned = 2,
            completionReward = 8f
        };
        
        tasks[3] = new Task
        {
            taskId = "replace_fuses",
            taskName = "Replace Fuses",
            description = "Restore power to generator room",
            isCompleted = false,
            hourAssigned = 3,
            completionReward = 10f
        };
        
        tasks[4] = new Task
        {
            taskId = "sort_files",
            taskName = "Sort Files",
            description = "Organize documents in archive",
            isCompleted = false,
            hourAssigned = 4,
            completionReward = 5f
        };
        
        tasks[5] = new Task
        {
            taskId = "empty_trash",
            taskName = "Empty Trash",
            description = "Remove waste from all rooms",
            isCompleted = false,
            hourAssigned = 4,
            completionReward = 3f
        };
        
        tasks[6] = new Task
        {
            taskId = "restart_generator",
            taskName = "Restart Generator",
            description = "Restart the backup generator",
            isCompleted = false,
            hourAssigned = 5,
            completionReward = 15f // Biggest reward for final task
        };
    }
    
    /// <summary>
    /// Completes a task and applies rewards.
    /// </summary>
    public void CompleteTask(string taskId)
    {
        Task task = FindTask(taskId);
        if (task == null || task.isCompleted) return;
        
        task.isCompleted = true;
        completedTaskCount++;
        
        // Apply sanity reward
        GameManager.Instance.GetSanityManager().RestoreSanity(task.completionReward);
        
        OnTaskCompleted?.Invoke(task);
        OnProgressUpdated?.Invoke(completedTaskCount);
        
        Debug.Log($"[InventoryManager] Task completed: {task.taskName} - Sanity +{task.completionReward}");
    }
    
    /// <summary>
    /// Starts a task (player interaction).
    /// </summary>
    public void StartTask(string taskId)
    {
        Task task = FindTask(taskId);
        if (task == null || task.isCompleted) return;
        
        OnTaskStarted?.Invoke(task);
        Debug.Log($"[InventoryManager] Task started: {task.taskName}");
    }
    
    /// <summary>
    /// Finds a task by ID.
    /// </summary>
    private Task FindTask(string taskId)
    {
        foreach (Task task in tasks)
        {
            if (task != null && task.taskId == taskId)
                return task;
        }
        return null;
    }
    
    /// <summary>
    /// Gets all tasks.
    /// </summary>
    public Task[] GetAllTasks() => tasks;
    
    /// <summary>
    /// Gets completed task count.
    /// </summary>
    public int GetCompletedTaskCount() => completedTaskCount;
    
    /// <summary>
    /// Gets total task count.
    /// </summary>
    public int GetTotalTaskCount() => tasks.Length;
    
    /// <summary>
    /// Gets progress percentage.
    /// </summary>
    public float GetProgressPercentage() => (completedTaskCount / (float)tasks.Length) * 100f;
}
