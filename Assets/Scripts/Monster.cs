using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 2f;    // vitesse du monstre
    public Transform target;        // le joueur à suivre

    void Update()
    {
        if (target != null)
        {
            // se dirige vers la cible
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        }
        else
        {
            // avance tout droit
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
