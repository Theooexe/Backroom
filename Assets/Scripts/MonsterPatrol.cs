using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] waypoints;     // Points à suivre
    public float waitTime = 2f;       // Temps d’attente entre chaque point

    private int currentIndex = 0;
    private bool waiting = false;
    private NavMeshAgent agent;
    private Animator animator;

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

        // Animation
        if (animator != null)
        {
            // Si on attend sur un point → idle
            bool isMoving = !waiting && agent.velocity.magnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);
        }

        // Si arrivé au point et pas déjà en attente
        if (!waiting && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }
    }


    System.Collections.IEnumerator WaitAndMove()
    {
        waiting = true;
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(waitTime);
        MoveToNextPoint();
        waiting = false;
    }

    void MoveToNextPoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentIndex].position);
        currentIndex = (currentIndex + 1) % waypoints.Length; // boucle
    }
}
