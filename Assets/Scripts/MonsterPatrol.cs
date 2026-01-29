using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public enum MonsterState { Patrol, Idle, Chase };
    public MonsterState currentState = MonsterState.Patrol;

    private Animator animator;
    private NavMeshAgent agent;

    [Header("Cibles et Détection")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [SerializeField] private Transform playerTarget;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private float loseSightDelay = 3f;
    private float loseSightTimer = 0f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float visionRange = 15f;
    [SerializeField] private float guaranteedDetectionRange = 3f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime = 0f;

    [Header("Timers et Vitesse")]
    [SerializeField] private float idleDuration = 5f;
    private float idleTimer;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 1.5f;

    [Header("Animation Settings")]
    private const string IsWalkingParam = "isWalking";
    private const string IsRunningParam = "isRunning";
    private const string AttackTrigger = "Attack";
    [SerializeField] private float movementThreshold = 0.01f;
    private float runningThreshold = 1.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource chaseAudioSource;
    [SerializeField] private AudioClip chaseClip;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private float footstepInterval = 0.5f; // intervalle entre deux sons de pas
    private float footstepTimer = 0f;


    [SerializeField] private float fadeOutSpeed = 1f; // volume par seconde
    private bool isFadingOut = false;

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
        CheckForPlayer();

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
        }
        
        if (isFadingOut && chaseAudioSource != null)
        {
            chaseAudioSource.volume -= fadeOutSpeed * Time.deltaTime;
            if (chaseAudioSource.volume <= 0f)
            {
                chaseAudioSource.Stop();
                chaseAudioSource.volume = 1f;
                isFadingOut = false;
            }
        }


        HandleAnimationStates();
    }

    private bool CanSeePlayer()
    {
        if (playerTarget == null) return false;

        Vector3 dir = playerTarget.position - transform.position;
        float distance = dir.magnitude;
        Vector3 dirNorm = dir.normalized;

        if (distance > visionRange && distance > guaranteedDetectionRange) return false;

        if (distance > guaranteedDetectionRange)
        {
            float dot = Vector3.Dot(transform.forward, dirNorm);
            if (dot < Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad)) return false;
        }

        RaycastHit hit;
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(eyePosition, dirNorm, out hit, visionRange))
        {
            if (hit.transform != playerTarget) return false;
        }

        return true;
    }

    private void CheckForPlayer()
    {
        if (playerTarget == null || currentState == MonsterState.Chase) return;

        if (CanSeePlayer())
        {
            if (currentState != MonsterState.Chase)
            {
                currentState = MonsterState.Chase;
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
                    chaseAudioSource.Play();
            }
        }

    }

    private void HandlePatrol()
    {
        if (agent.speed != patrolSpeed) agent.speed = patrolSpeed;

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

        if (CanSeePlayer()) loseSightTimer = 0f;
        else
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= loseSightDelay)
            {
                currentState = MonsterState.Patrol;
                agent.speed = patrolSpeed;
                GoToNextWaypoint();
                agent.isStopped = false;
                loseSightTimer = 0f;
                if (chaseAudioSource != null && chaseAudioSource.isPlaying)
                    isFadingOut = true;
                return;
            }
        }

        if (agent.speed != chaseSpeed) agent.speed = chaseSpeed;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
                animator.SetTrigger(AttackTrigger);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 position = transform.position + Vector3.up * 1.5f;
        Vector3 forward = transform.forward;

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);

        int segments = 20;
        float halfAngle = viewAngle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + (viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward * visionRange;
            RaycastHit hit;
            if (!Physics.Raycast(position, dir.normalized, out hit, visionRange))
                Gizmos.DrawLine(position, position + dir);
            else
                Gizmos.DrawLine(position, hit.point);
        }

        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, guaranteedDetectionRange);
    }

    private void AttackPlayer()
    {
        if (playerStats != null) playerStats.TakeDamage(damage);
    }

    private void IncrementWaypointIndex()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void HandleAnimationStates()
    {
        Vector3 horVel = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float speed = horVel.magnitude;

        bool isWalking = speed > movementThreshold && speed < runningThreshold;
        bool isRunning = speed >= runningThreshold;

        animator.SetBool(IsRunningParam, isRunning);
        animator.SetBool(IsWalkingParam, isWalking);

        // --- Gestion du son des pas ---
        if (footstepAudioSource != null && footstepClip != null)
        {
            if (speed > movementThreshold) // le monstre se déplace
            {
                footstepTimer += Time.deltaTime;
                float interval = footstepInterval;
                if (isRunning) interval /= 1.5f; // pas plus rapides quand il court

                if (footstepTimer >= interval)
                {
                    footstepAudioSource.PlayOneShot(footstepClip, footstepVolume);
                    footstepTimer = 0f;
                }
            }
            else
            {
                footstepTimer = footstepInterval; // reset timer si immobile
            }
        }
    }

}
