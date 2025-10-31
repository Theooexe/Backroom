using UnityEngine;

public class MonsterPatrol : MonoBehaviour
{
    public Transform[] waypoints;  // Points de patrouille à assigner dans l'inspecteur
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    private int currentIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                currentIndex = (currentIndex + 1) % waypoints.Length;
            }
            else
            {
                if (animator != null)
                    animator.SetBool("isMoving", false);
                return;
            }
        }

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position);
        float distance = direction.magnitude;

        if (distance < 0.5f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
        else
        {
            Vector3 moveDir = direction.normalized;
            rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 5f);

            if (animator != null)
                animator.SetBool("isMoving", true);
        }
    }
}
