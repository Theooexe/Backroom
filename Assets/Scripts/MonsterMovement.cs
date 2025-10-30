using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;            // Vitesse du monstre
    public float changeTargetInterval = 3f; // Temps entre chaque changement de direction
    public float moveRange = 10f;           // Rayon autour du point de départ

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        // Déplacement vers la cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Regarde vers la direction du déplacement
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 2f);

        // Si arrivé ou temps écoulé → nouvelle cible
        timer += Time.deltaTime;
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f || timer >= changeTargetInterval)
        {
            PickNewTarget();
            timer = 0f;
        }
    }

    void PickNewTarget()
    {
        float randomX = Random.Range(-moveRange, moveRange);
        float randomZ = Random.Range(-moveRange, moveRange);

        targetPosition = startPosition + new Vector3(randomX, 0, randomZ);
    }
}
