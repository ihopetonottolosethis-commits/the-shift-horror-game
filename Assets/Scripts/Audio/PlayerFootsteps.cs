using UnityEngine;

/// <summary>
/// Footsteps system - generates appropriate sounds based on movement and surface.
/// </summary>
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float footstepInterval = 0.5f; // Time between footsteps
    
    private float footstepTimer = 0f;
    private PlayerController playerController;
    
    private void Start()
    {
        if (audioManager == null)
            audioManager = GameManager.Instance.GetAudioManager();
        
        playerController = GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGameRunning()) return;
        if (!playerController.IsMoving()) return;
        
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            PlayFootstep();
            footstepTimer = footstepInterval;
        }
    }
    
    private void PlayFootstep()
    {
        string surfaceType = GetCurrentSurfaceType();
        audioManager.PlayFootstep(transform.position, surfaceType);
    }
    
    private string GetCurrentSurfaceType()
    {
        // Cast ray downward to detect surface
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("Stone")) return "stone";
            if (hit.collider.CompareTag("Tile")) return "tile";
            if (hit.collider.CompareTag("Carpet")) return "carpet";
            if (hit.collider.CompareTag("Metal")) return "metal";
        }
        
        return "stone"; // Default
    }
}
