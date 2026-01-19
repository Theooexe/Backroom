using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    // --- États de l'IA ---
    public enum MonsterState { Patrol, Idle, Chase };
    public MonsterState currentState = MonsterState.Patrol;

    // --- Composants ---
    private Animator animator;
    private NavMeshAgent agent;

    // --- Cibles et Détection ---
    [Header("Cibles et Détection")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    
    [Tooltip("La Transform du joueur.")]
    [SerializeField] private Transform playerTarget;
    
    [Tooltip("Angle maximum (en degrés) pour que le joueur soit dans le cône de vision.")]
    [SerializeField] private float viewAngle = 90f;
    
    [Tooltip("Portée maximale de la vision.")]
    [SerializeField] private float visionRange = 15f;
    
    [Tooltip("Portée minimale pour une détection garantie (même dans le dos).")]
    [SerializeField] private float guaranteedDetectionRange = 3f; 

    [Tooltip("Distance minimale pour déclencher l'attaque.")]
    [SerializeField] private float attackRange = 1.5f; // NOUVEAU PARAMÈTRE
    
    // --- Minuterie et Vitesse ---
    [Header("Timers et Vitesse")]
    [Tooltip("Temps de pause en secondes à chaque waypoint.")]
    [SerializeField] private float idleDuration = 5f;
    private float idleTimer;
    
    [Tooltip("Vitesse en mode Patrouille (Marche).")]
    [SerializeField] private float patrolSpeed = 1.5f;
    
    [Tooltip("Vitesse en mode Poursuite (Course).")]
    [SerializeField] private float chaseSpeed = 5f;

    // --- Paramètres d'Animation ---
    [Header("Animation Settings")]
    private const string IsWalkingParam = "isWalking";
    private const string IsRunningParam = "isRunning";
    private const string AttackTrigger = "Attack";

    [Tooltip("Vitesse minimale pour passer de 'idle' à 'walk'.")]
    [SerializeField] private float movementThreshold = 0.01f;
    
    private float runningThreshold = 2.0f; 

    // ===============================================
    // START & UPDATE
    // ===============================================

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (animator == null || agent == null)
        {
            Debug.LogError("Composant(s) manquant(s): Animator ou NavMeshAgent.");
            enabled = false;
            return;
        }

        if (waypoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
    }

    void Update()
    {
        // 1. Détection du joueur (Doit être vérifié en premier)
        CheckForPlayer();

        // 2. Gestion des états de l'IA
        switch (currentState)
        {
            case MonsterState.Patrol:
                HandlePatrol();
                break;
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Chase:
                HandleChase(); // LOGIQUE MODIFIÉE
                break;
        }

        // 3. Gestion des Animations (basée sur la vitesse de l'agent)
        HandleAnimationStates();
    }
    
    // ===============================================
    // LOGIQUE DE DÉTECTION
    // ===============================================

    private void CheckForPlayer()
    {
        if (playerTarget == null || currentState == MonsterState.Chase) 
        {
            return;
        }

        Vector3 directionToPlayer = playerTarget.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        Vector3 normalizedDirection = directionToPlayer.normalized;

        bool isPlayerDetected = false;

        // A. Détection Rapprochée (Portée garantie)
        if (distanceToPlayer <= guaranteedDetectionRange)
        {
            isPlayerDetected = true;
        }
        
        // B. Détection Visuelle (Distance et Angle)
        if (!isPlayerDetected && distanceToPlayer <= visionRange)
        {
            float dotProduct = Vector3.Dot(transform.forward, normalizedDirection);
            float cosViewAngle = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

            if (dotProduct >= cosViewAngle)
            {
                isPlayerDetected = true;
            }
        }

        // Changement d'état si détection
        if (isPlayerDetected)
        {
            Debug.Log("Joueur détecté ! Lancement de la course poursuite.");
            agent.isStopped = false; 
            currentState = MonsterState.Chase;
            agent.speed = chaseSpeed;
        }
    }

    // ===============================================
    // LOGIQUE D'ÉTAT DE L'IA
    // ===============================================

    private void HandlePatrol()
    {
        if (agent.speed != patrolSpeed)
        {
             agent.speed = patrolSpeed;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f && !agent.pathPending)
        {
            currentState = MonsterState.Idle;
            idleTimer = idleDuration;
            agent.isStopped = true;
        }
    }

    private void HandleIdle()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            currentState = MonsterState.Patrol;
            IncrementWaypointIndex();
            GoToNextWaypoint();
            agent.isStopped = false;
        }
    }

    private void HandleChase()
    {
        if (playerTarget == null) return;
        
        if (agent.speed != chaseSpeed)
        {
             agent.speed = chaseSpeed;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // LOGIQUE D'ATTAQUE : SI ASSEZ PROCHE
        if (distanceToPlayer <= attackRange)
        {
            // Arrête le mouvement pour attaquer
            agent.isStopped = true; 
            
            // On s'assure qu'on ne lance pas l'attaque à chaque frame si on est déjà en train d'attaquer
            // (Une vérification plus poussée est nécessaire en production, mais pour la base, on trigger)
            animator.SetTrigger(AttackTrigger);
        }
        else 
        {
            // Continuer la course-poursuite
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }
    }

    private void IncrementWaypointIndex()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // ===============================================
    // LOGIQUE D'ANIMATION
    // ===============================================
    
    private void HandleAnimationStates()
    {
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        bool isWalking = currentSpeed > movementThreshold && currentSpeed < runningThreshold;
        bool isRunning = currentSpeed >= runningThreshold; 

        if (isRunning)
        {
            animator.SetBool(IsRunningParam, true);
            animator.SetBool(IsWalkingParam, false);
        }
        else if (isWalking)
        {
            animator.SetBool(IsWalkingParam, true);
            animator.SetBool(IsRunningParam, false);
        }
        else // Vitesse très faible (Idle/Arrêté)
        {
            animator.SetBool(IsWalkingParam, false);
            animator.SetBool(IsRunningParam, false);
        }
    }
}