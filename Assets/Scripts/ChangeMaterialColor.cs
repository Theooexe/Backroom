using UnityEngine;
using UnityEngine.Rendering;

public class DarkMode : MonoBehaviour
{
    [Header("Paramètres du mode sombre")]
    public Material materialToChange;
    public Color newEmissionColor = Color.red;
    public string emissionPropertyName = "_Emission_Color";

    [Range(0f, 1f)]
    public float ambientIntensity = 0.05f;

    void Start()
    {
        // --- Changer la couleur d'émission ---
        if (materialToChange != null)
        {
            materialToChange.SetColor(emissionPropertyName, newEmissionColor);
            materialToChange.EnableKeyword("_EMISSION");
        }

        // --- Retirer la Skybox ---
        RenderSettings.skybox = null;

        // --- Réduire la lumière ambiante ---
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black * ambientIntensity;

        // --- Réduire toutes les lumières directionnelles ---
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in lights)
        {
            if (l.type == LightType.Directional)
                l.intensity *= 0.05f;
        }

        // --- Désactiver les reflets sur tous les Renderers ---
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer r in renderers)
        {
            // Désactiver les probes de réflexion
            r.reflectionProbeUsage = ReflectionProbeUsage.Off;

            // Ajuster les matériaux PBR (optionnel)
            foreach (Material mat in r.sharedMaterials)
            {
                if (!mat) continue;

                if (mat.HasProperty("_Metallic"))
                    mat.SetFloat("_Metallic", 0f);

                if (mat.HasProperty("_Smoothness"))
                    mat.SetFloat("_Smoothness", 0f);
            }
        }
    }
}
