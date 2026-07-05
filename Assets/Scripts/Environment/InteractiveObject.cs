using UnityEngine;

/// <summary>
/// Interactive objects in museum: sarcophagus, doors, cameras, etc.
/// </summary>
public class InteractiveObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private string taskId = "";
    [SerializeField] private float interactionDuration = 2f;
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private InteractionType type = InteractionType.Task;
    
    public enum InteractionType
    {
        Task, Sarcophagus, Mirror, Door, Camera, Generator, Fuse, Document
    }
    
    private bool isInteracting = false;
    private float interactionProgress = 0f;
    private PlayerController interactingPlayer = null;
    
    public void Interact(PlayerController player)
    {
        if (isInteracting) return;
        interactingPlayer = player;
        isInteracting = true;
        interactionProgress = 0f;
        HandleInteractionType();
        Debug.Log($"[InteractiveObject] Interaction started: {gameObject.name}");
    }
    
    private void HandleInteractionType()
    {
        switch (type)
        {
            case InteractionType.Task: HandleTaskInteraction(); break;
            case InteractionType.Sarcophagus: HandleSarcophagusInteraction(); break;
            case InteractionType.Mirror: HandleMirrorInteraction(); break;
            case InteractionType.Door: HandleDoorInteraction(); break;
            case InteractionType.Camera: HandleCameraInteraction(); break;
            case InteractionType.Generator: HandleGeneratorInteraction(); break;
            case InteractionType.Fuse: HandleFuseInteraction(); break;
            case InteractionType.Document: HandleDocumentInteraction(); break;
        }
    }
    
    private void HandleTaskInteraction()
    {
        if (!string.IsNullOrEmpty(taskId))
        {
            GameManager.Instance.GetInventoryManager().CompleteTask(taskId);
            PlayInteractionSound();
            isInteracting = false;
        }
    }
    
    private void HandleSarcophagusInteraction()
    {
        GameManager.Instance.GetAudioManager().PlaySarcophagusRattle(transform.position);
        GameManager.Instance.GetRuleManager().OnSarcophagusRattle();
        isInteracting = false;
    }
    
    private void HandleMirrorInteraction()
    {
        GameManager.Instance.GetRuleManager().TriggerMirrorReflectionStill();
        isInteracting = false;
    }
    
    private void HandleDoorInteraction()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("IsOpen", !animator.GetBool("IsOpen"));
        PlayInteractionSound();
        isInteracting = false;
    }
    
    private void HandleCameraInteraction()
    {
        Debug.Log("[InteractiveObject] Camera system activated");
        PlayInteractionSound();
        isInteracting = false;
    }
    
    private void HandleGeneratorInteraction()
    {
        Debug.Log("[InteractiveObject] Generator restarted");
        PlayInteractionSound();
        GameManager.Instance.GetInventoryManager().CompleteTask("restart_generator");
        isInteracting = false;
    }
    
    private void HandleFuseInteraction()
    {
        Debug.Log("[InteractiveObject] Fuse replaced");
        PlayInteractionSound();
        GameManager.Instance.GetInventoryManager().CompleteTask("replace_fuses");
        isInteracting = false;
    }
    
    private void HandleDocumentInteraction()
    {
        Debug.Log("[InteractiveObject] File sorted");
        PlayInteractionSound();
        isInteracting = false;
    }
    
    private void PlayInteractionSound()
    {
        if (interactionSound != null)
            GameManager.Instance.GetAudioManager().PlaySFX(interactionSound.name, transform.position);
    }
    
    public string GetInteractionPrompt() => interactionPrompt;
}
