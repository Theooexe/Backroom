using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Color baseColor = Color.red; // couleur de base
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogError("Pas de Renderer trouvé !");
            return;
        }

        // Si tu veux mettre la couleur de base pour tous les objets
        rend.sharedMaterial.color = baseColor;

        // Active le keyword _EMISSION
        rend.sharedMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float f = getIntensity(); // ta fonction perso
        Color emissionColor = baseColor * f;

        // Applique la couleur d'émission
        rend.sharedMaterial.SetColor("Emission Color", emissionColor);
        rend.sharedMaterial.EnableKeyword("_EMISSION"); // s'assure que l'émission est active
    }

    float getIntensity()
    {
        // Exemple : animation pulsante
        return Mathf.PingPong(Time.time, 1f);
    }
}
