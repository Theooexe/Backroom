using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterMovement : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 2f;              // Vitesse du monstre
    public float changeTargetInterval = 3f;   // Temps avant nouvelle destination
    public float moveRange = 10f;             // Rayon de patrouille autour du point de départ

    [Header("Animations")]
    public Animator animator;                  // Animator du monstre
    private string animMoveParam = "isMoving"; // Nom du bool dans l'Animator

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Si Animator non assigné dans l'inspecteur, on le récupère automatiquement
        if (animator == null)
            animator = GetComponent<Animator>();

        startPosition = transform.position;
        PickNewTarget();
    }

    void FixedUpdate()
    {
        // Calcul de la direction vers la cible
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Déplacement via Rigidbody pour conserver la physique
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Garde le monstre vertical
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        // Animation : idle si immobile, walk si en mouvement
        if (animator != null)
            animator.SetBool(animMoveParam, direction.magnitude > 0.01f);

        // Timer pour changement de destination
        timer += Time.fixedDeltaTime;
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f || timer >= changeTargetInterval)
        {
            PickNewTarget();
            timer = 0f;
        }
    }

    void PickNewTarget()
    {
        // Choisir un point aléatoire dans le rayon de patrouille
        float randomX = Random.Range(-moveRange, moveRange);
        float randomZ = Random.Range(-moveRange, moveRange);

        targetPosition = startPosition + new Vector3(randomX, 0, randomZ);
    }
}
