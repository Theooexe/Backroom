using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    // --- États de l'IA ---
    // ATTENTION : J'ai réintégré l'état Attacking pour gérer l'immobilité lors de l'attaque
    public enum MonsterState { Patrol, Idle, Chase, Attacking };
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
    
    [Tooltip("Angle maximum (en degrés) pour le cône de vision.")]
    [SerializeField] private float viewAngle = 90f;
    
    [Tooltip("Portée maximale de la vision.")]
    [SerializeField] private float visionRange = 15f;
    
    [Tooltip("Portée minimale pour une détection garantie.")]
    [SerializeField] private float guaranteedDetectionRange = 3f; 

    [Tooltip("Distance minimale pour déclencher l'attaque.")]
    [SerializeField] private float attackRange = 1.5f;
    
    // --- Minuterie et Vitesse ---
    [Header("Timers et Vitesse")]
    [Tooltip("Temps de pause en secondes à chaque waypoint.")]
    [SerializeField] private float idleDuration = 5f;
    private float idleTimer;
    
    [Tooltip("Durée de l'animation d'attaque (pour l'immobilisation).")]
    [SerializeField] private float attackDuration = 1.0f; 
    private float attackTimer;

    [Tooltip("Délai après lequel le monstre abandonne la poursuite si le joueur est hors de vue.")]
    [SerializeField] private float loseSightDelay = 3f; // NOUVEAU PARAMÈTRE
    private float loseSightTimer; // NOUVEAU TIMER
    
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

        // ... (Vérifications inchangées) ...

        if (waypoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
        
        // Initialise la minuterie de perte de cible au maximum
        loseSightTimer = loseSightDelay;
    }

    void Update()
    {
        // 1. Détection du joueur
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
                HandleChase();
                break;
            case MonsterState.Attacking:
                HandleAttacking();
                break;
        }

        // 3. Gestion des Animations
        HandleAnimationStates();
    }
    
    // ===============================================
    // LOGIQUE DE DÉTECTION
    // ===============================================

    /// <summary>Vérifie si le joueur est dans le champ de vision ou à portée garantie.</summary>
    private bool IsPlayerInSight()
    {
        if (playerTarget == null) return false;
        
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        Vector3 normalizedDirection = directionToPlayer.normalized;

        // A. Détection Rapprochée (Portée garantie)
        if (distanceToPlayer <= guaranteedDetectionRange)
        {
            return true;
        }
        
        // B. Détection Visuelle (Distance et Angle)
        if (distanceToPlayer <= visionRange)
        {
            float dotProduct = Vector3.Dot(transform.forward, normalizedDirection);
            float cosViewAngle = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

            if (dotProduct >= cosViewAngle)
            {
                // NOTE: Idéalement, ajouter un Raycast ici pour vérifier les obstacles (murs)
                return true;
            }
        }
        return false;
    }

    private void CheckForPlayer()
    {
        // Si déjà en chasse ou en attaque, on ne change pas l'état ici, on laisse HandleChase/Attacking gérer.
        if (currentState == MonsterState.Chase || currentState == MonsterState.Attacking) 
        {
            return;
        }
        
        // Si le joueur est détecté, on passe en mode Chase immédiatement
        if (IsPlayerInSight())
        {
            Debug.Log("Joueur détecté ! Lancement de la course poursuite.");
            agent.isStopped = false; 
            currentState = MonsterState.Chase;
            agent.speed = chaseSpeed;
            // Réinitialise le timer de perte de cible à chaque fois qu'on le voit
            loseSightTimer = loseSightDelay; 
        }
    }

    // ===============================================
    // LOGIQUE D'ÉTAT DE L'IA
    // ===============================================

    private void HandlePatrol()
    {
        // ... (Logique Patrouille inchangée) ...
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
        // ... (Logique Idle inchangée) ...
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

        // --- GESTION DE LA PERTE DE CIBLE ---
        if (IsPlayerInSight())
        {
            // Le joueur est VU : on réinitialise le timer d'abandon
            loseSightTimer = loseSightDelay;
        }
        else
        {
            // Le joueur n'est plus VU : on décrémente le timer
            loseSightTimer -= Time.deltaTime;
        }

        // Si le timer atteint zéro, on abandonne la poursuite
        if (loseSightTimer <= 0)
        {
            Debug.Log("Joueur perdu de vue. Retour à la patrouille.");
            currentState = MonsterState.Idle; // On passe en Idle avant la patrouille
            agent.isStopped = true;
            return; // Sortir immédiatement pour éviter le reste du code de Chase
        }
        // ------------------------------------

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // LOGIQUE D'ATTAQUE : SI ASSEZ PROCHE
        if (distanceToPlayer <= attackRange)
        {
            // Changement d'état -> Attaque (gère l'immobilisation et le trigger)
            currentState = MonsterState.Attacking;
        }
        else 
        {
            // Continuer la course-poursuite
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
            // OPTIONNEL: Tourner vers le joueur pendant la course
            LookAtPlayer(); 
        }
    }

    private void HandleAttacking()
    {
        // 1. Immobilisation et Lancement (seulement à la première frame de l'état Attacking)
        if (agent.isStopped == false)
        {
            agent.isStopped = true; 
            animator.SetTrigger(AttackTrigger);
            attackTimer = attackDuration; 
            LookAtPlayer();
        }

        // 2. Gestion de la Minuterie (Attendre la fin de l'animation)
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            // L'animation est terminée : retour à la poursuite (on revient en Chase pour la vérif de la portée)
            currentState = MonsterState.Chase;
            agent.isStopped = false;
        }
    }


    private void LookAtPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
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
            // On empêche le passage en Idle si l'attaque est en cours
            if (currentState != MonsterState.Attacking)
            {
                animator.SetBool(IsWalkingParam, false);
                animator.SetBool(IsRunningParam, false);
            }
        }
    }
}