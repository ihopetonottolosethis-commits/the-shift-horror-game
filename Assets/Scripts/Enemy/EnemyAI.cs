using UnityEngine;
using System;

/// <summary>
/// Enemy AI with Idle, Patrol, Investigate, Chase states.
/// Becomes more aggressive as player sanity decreases.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 30f;
    [SerializeField] private float detectionRangeIncrement = 10f;
    [SerializeField] private float chaseDuration = 15f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float investigateSpeed = 3f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float gravity = -9.81f;
    
    private enum EnemyState { Idle, Patrol, Investigate, Chase }
    private EnemyState currentState = EnemyState.Idle;
    
    private Vector3 velocity = Vector3.zero;
    private Transform playerTransform;
    private float detectionTimer = 0f;
    private int currentPatrolIndex = 0;
    private float chaseTimer = 0f;
    private Vector3 investigatePosition = Vector3.zero;
    private float investigateTimer = 5f;
    
    public event Action<EnemyState> OnStateChanged;
    public event Action OnPlayerDetected;
    public event Action OnPlayerCaught;
    
    private bool isInitialized = false;
    
    public void Initialize()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (patrolPoints.Length == 0) Debug.LogWarning("[EnemyAI] No patrol points assigned!");
        
        playerTransform = GameManager.Instance.GetPlayerController().transform;
        currentState = EnemyState.Idle;
        isInitialized = true;
        
        Debug.Log("[EnemyAI] Enemy initialized with " + patrolPoints.Length + " patrol points");
    }
    
    private void Update()
    {
        if (!isInitialized || !GameManager.Instance.IsGameRunning()) return;
        UpdateDetectionRange();
        UpdateState();
        ExecuteCurrentState();
    }
    
    private void UpdateDetectionRange()
    {
        float sanityPercentage = GameManager.Instance.GetSanityManager().GetSanityPercentage();
        float sanityFactor = 1f - (sanityPercentage / 100f);
        detectionRange = 30f + (10f * sanityFactor);
    }
    
    private void UpdateState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool canSeePlayer = CanSeePlayer();
        bool isInDetectionRange = distanceToPlayer < detectionRange;
        
        EnemyState newState = currentState;
        
        if (canSeePlayer && isInDetectionRange)
        {
            newState = EnemyState.Chase;
            chaseTimer = chaseDuration;
            if (currentState != EnemyState.Chase)
            {
                OnPlayerDetected?.Invoke();
                Debug.Log("[EnemyAI] Player detected!");
            }
        }
        else if (chaseTimer > 0)
        {
            newState = EnemyState.Chase;
            chaseTimer -= Time.deltaTime;
        }
        else if (currentState == EnemyState.Investigate)
        {
            investigateTimer -= Time.deltaTime;
            if (investigateTimer <= 0) newState = EnemyState.Patrol;
        }
        else
        {
            newState = EnemyState.Patrol;
        }
        
        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
            Debug.Log($"[EnemyAI] State: {currentState}");
        }
    }
    
    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: ExecutePatrol(); break;
            case EnemyState.Investigate: ExecuteInvestigate(); break;
            case EnemyState.Chase: ExecuteChase(); break;
        }
        
        if (characterController.isGrounded && velocity.y < 0) velocity.y = -2f;
        else velocity.y += gravity * Time.deltaTime;
        
        characterController.Move(velocity * Time.deltaTime);
    }
    
    private void ExecutePatrol()
    {
        if (patrolPoints.Length == 0) return;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 directionToPatrol = (targetPoint.position - transform.position).normalized;
        velocity.x = directionToPatrol.x * patrolSpeed;
        velocity.z = directionToPatrol.z * patrolSpeed;
        if (Vector3.Distance(transform.position, targetPoint.position) < 2f)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
    
    private void ExecuteInvestigate()
    {
        Vector3 directionToInvestigate = (investigatePosition - transform.position).normalized;
        velocity.x = directionToInvestigate.x * investigateSpeed;
        velocity.z = directionToInvestigate.z * investigateSpeed;
    }
    
    private void ExecuteChase()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        velocity.x = directionToPlayer.x * chaseSpeed;
        velocity.z = directionToPlayer.z * chaseSpeed;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < 2f)
        {
            OnPlayerCaught?.Invoke();
            GameManager.Instance.EndGame(false);
            Debug.Log("[EnemyAI] Player caught!");
        }
    }
    
    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            return hit.transform == playerTransform;
        return false;
    }
    
    public void Investigate(Vector3 position)
    {
        investigatePosition = position;
        currentState = EnemyState.Investigate;
        investigateTimer = 5f;
    }
    
    public EnemyState GetCurrentState() => currentState;
}
