using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterMove : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 2f;
    public float moveRange = 10f;
    public float minIdleTime = 1f;   // Temps minimal en Idle
    public float maxIdleTime = 3f;   // Temps maximal en Idle

    private Rigidbody rb;
    private Animator animator;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool isIdle = false;
    private float idleTimer = 0f;
    private float idleDuration = 0f;

    private const float arrivalThreshold = 0.5f;  // Distance pour considérer qu'il a atteint la cible

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animator = GetComponentInChildren<Animator>();
        startPosition = transform.position;

        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (isIdle)
        {
            // Compter le temps d'Idle
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                PickNewTarget();
            }

            if (animator != null)
                animator.SetBool("isMoving", false);

            return;
        }

        // Déplacement vers la cible
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        bool isMoving = distance > 0.01f;

        // Animation
        if (animator != null)
            animator.SetBool("isMoving", isMoving);

        if (distance < arrivalThreshold)
        {
            EnterIdle();
        }
        else
        {
            // Déplacer le monstre
            Vector3 moveDir = direction.normalized;
            rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 5f);
        }
    }

    void PickNewTarget()
    {
        Vector3 newTarget;
        do
        {
            float randomX = Random.Range(-moveRange, moveRange);
            float randomZ = Random.Range(-moveRange, moveRange);
            newTarget = startPosition + new Vector3(randomX, 0, randomZ);
        } while (Vector3.Distance(transform.position, newTarget) < 1f);

        targetPosition = newTarget;
    }

    void EnterIdle()
    {
        isIdle = true;
        idleTimer = 0f;
        idleDuration = Random.Range(minIdleTime, maxIdleTime);
    }
}
