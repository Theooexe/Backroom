using UnityEngine;

public class MonsterPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints; // Public pour assignation facile
    
    [Header("Paramètres de mouvement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float waypointReachDistance = 0.5f;
    
    [Header("Temps d'attente")]
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    
    [Header("Détection du joueur")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;
    
    private Animator animator;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("Aucun waypoint assigné au monstre!");
        }
        
        // Si le player n'est pas assigné, on essaie de le trouver
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }
    
    void Update()
    {
        // Vérifier la détection du joueur
        CheckPlayerDetection();
        
        if (isAttacking)
        {
            // En mode attaque, on ne bouge pas
            UpdateAnimator(0f, true);
        }
        else if (isChasing && player != null)
        {
            // Poursuite du joueur
            ChasePlayer();
        }
        else if (waypoints.Length > 0)
        {
            // Patrouille normale
            Patrol();
        }
    }
    
    void Patrol()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            UpdateAnimator(0f, false);
            
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        direction.y = 0; // Garder le mouvement horizontal
        
        // Déplacement vers le waypoint
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Rotation vers le waypoint
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Mettre à jour l'animation
        UpdateAnimator(moveSpeed, false);
        
        // Vérifier si le waypoint est atteint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance <= waypointReachDistance)
        {
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }
    
    void CheckPlayerDetection()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Vérifier la portée d'attaque
        if (distanceToPlayer <= attackRange)
        {
            isAttacking = true;
            isChasing = false;
            
            // Regarder le joueur
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        // Vérifier la portée de détection
        else if (distanceToPlayer <= detectionRange)
        {
            isAttacking = false;
            isChasing = true;
            isWaiting = false;
        }
        // Retour à la patrouille
        else
        {
            isAttacking = false;
            isChasing = false;
        }
    }
    
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        // Déplacement vers le joueur
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Rotation vers le joueur
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        UpdateAnimator(moveSpeed, false);
    }
    
    void UpdateAnimator(float speed, bool attack)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
            animator.SetBool("Attack", attack);
        }
    }
    
    // Visualiser les waypoints et les portées dans l'éditeur
    void OnDrawGizmosSelected()
    {
        // Dessiner les waypoints
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawWireSphere(waypoints[i].position, waypointReachDistance);
                    
                    // Dessiner les lignes entre les waypoints
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                    else if (i == waypoints.Length - 1 && waypoints[0] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                    }
                }
            }
        }
        
        // Dessiner la portée de détection
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Dessiner la portée d'attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}