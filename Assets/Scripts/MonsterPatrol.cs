using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] waypoints;        // Points de patrouille
    public float waitTime = 1f;          // Temps d'attente à chaque point
    public float waypointTolerance = 1f; // Distance pour considérer un point comme atteint

    private NavMeshAgent agent;
    private Animator anim;
    private int currentPoint = 0;
    private float waitCounter = 0f;
    private bool waiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (waypoints.Length > 0)
            GoToNextPoint();
    }

    void Update()
    {
        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                waiting = false;
                GoToNextPoint();
            }
            return;
        }

        // Arrivé au waypoint ?
        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            waiting = true;
            waitCounter = waitTime;

            if (anim) anim.SetBool("isWalking", false);
        }
        else
        {
            if (anim) anim.SetBool("isWalking", true);
        }
    }

    private void GoToNextPoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentPoint].position);

        currentPoint = (currentPoint + 1) % waypoints.Length;
    }
}