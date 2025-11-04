using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterPatrol : MonoBehaviour
{
    [Header("Patrouille")]
    public Transform[] waypoints; 
    public float waitTime = 2f;  

    [Header("Vision")]
    public Transform player;      
    public float viewDistance = 10f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    private int currentIndex = 0;
    private bool waiting = false;
    private bool chasing = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Coroutine chaseCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (waypoints.Length > 0)
            MoveToNextPoint();
    }

    void Update()
    {
        if (agent.pathPending)
            return;

        // Détection joueur
        if (!chasing && CanSeePlayer())
        {
            if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
            chaseCoroutine = StartCoroutine(ChasePlayer());
        }

        // Animation
        if (animator != null)
        {
            bool isMoving = !waiting && agent.remainingDistance > agent.stoppingDistance;
            animator.SetBool("isRunning", chasing);   // Course si poursuit le joueur
            animator.SetBool("isMoving", !chasing && isMoving); // Marche seulement en patrouille
        }

        // Patrouille normale
        if (!chasing && !waiting && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Vector3.Distance(transform.position, player.position) > viewDistance)
            return false;

        if (Vector3.Angle(transform.forward, directionToPlayer) > viewAngle / 2f)
            return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, viewDistance))
        {
            if (hit.transform != player)
                return false;
        }

        return true;
    }

    IEnumerator ChasePlayer()
    {
        chasing = true;
        float chaseTime = 5f;
        float elapsed = 0f;

        while (elapsed < chaseTime)
        {
            agent.SetDestination(player.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        chasing = false;
        MoveToNextPoint();
    }

    IEnumerator WaitAndMove()
    {
        waiting = true;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(waitTime);
        MoveToNextPoint();
        waiting = false;
    }

    void MoveToNextPoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentIndex].position);
        currentIndex = (currentIndex + 1) % waypoints.Length;
    }
}
