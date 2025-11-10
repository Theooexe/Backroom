using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterPatrol : MonoBehaviour
{
    // --- Références ---
    public Transform player;         // Ton joueur
    public Transform[] waypoints;    // Points de patrouille
    private NavMeshAgent agent;
    private Animator animator;

    // --- Paramètres de déplacement ---
    public float walkSpeed = 2f;
    public float runSpeed = 6f;

    // --- Paramètres de détection ---
    public float viewDistance =6f;
    public float viewAngle = 60f;
    public float detectionRadius = 3f; // Détection si le joueur est très proche
    public float attackRange = 2f;     // Distance à laquelle il attaque

    // --- États internes ---
    private int currentWaypoint = 0;
    private bool isWaiting = false;
    private bool isAttacking = false;
    private bool playerDetected = false;

    // --- Initialisation ---
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // TODO : initialiser le NavMeshAgent
        // TODO : lancer la première destination de patrouille
    }

    // --- Mise à jour à chaque frame ---
    void Update()
    {
        // TODO : calculer la distance au joueur

        // TODO : mettre à jour le paramètre Speed dans l'Animator

        // TODO : détection du joueur (vue ou proximité)

        // TODO : choisir l'état : Patrouille ou Poursuite/Attaque
    }

    // --- Méthodes de patrouille ---
    void Patrouiller()
    {
        // TODO : faire avancer vers le waypoint
        // TODO : gérer l'attente au waypoint
    }

    IEnumerator WaitAndGoNext()
    {
        // TODO : arrêter l'agent, attendre, passer au waypoint suivant
        yield return null;
    }

    // --- Méthodes de poursuite et attaque ---
    void ChasserOuAttaquer(float distanceToPlayer)
    {
        // TODO : si trop loin → courir vers le joueur
        // TODO : si assez proche → lancer l'attaque
    }

    IEnumerator AttackRoutine()
    {
        // TODO : arrêter l'agent, lancer l'animation d'attaque, attendre cooldown
        yield return null;
    }

    // --- Détection du joueur ---
    bool PeutVoirLeJoueur(float distanceToPlayer)
    {
        // TODO : vérifier l'angle de vision et les obstacles
        return false;
    }

    // --- Retour à la patrouille ---
    void RetourPatrouille()
    {
        // TODO : remettre le monstre sur son chemin de patrouille
    }

    // --- Optionnel : debug du cône de vision ---
    void OnDrawGizmosSelected()
    {
    if (!Application.isPlaying) return;

    // Position et orientation du monstre
    Vector3 origin = transform.position + Vector3.up; // légèrement au-dessus du sol

    // Couleur du cône
    Gizmos.color = Color.red;

    // Rayon de vision
    float radius = viewDistance;

    // Angle du cône (demi-angle)
    float halfAngle = viewAngle / 2f;

    // Directions gauche et droite
    Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * transform.forward;
    Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * transform.forward;

    // Ligne centrale (optionnelle)
    Gizmos.DrawLine(origin, origin + transform.forward * radius);

    // Côtés du cône
    Gizmos.DrawLine(origin, origin + leftDir * radius);
    Gizmos.DrawLine(origin, origin + rightDir * radius);

    // Pour visualiser le cône “rempli”, on peut tracer plusieurs lignes intermédiaires
    int segments = 10;
    for (int i = 1; i < segments; i++)
    {
        float angle = -halfAngle + (viewAngle / segments) * i;
        Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
        Gizmos.DrawLine(origin, origin + dir * radius);
    }
    }

}
