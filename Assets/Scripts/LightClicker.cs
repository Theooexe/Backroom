using UnityEngine;
using System.Collections;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] private Light lightSource;

    [Header("Intervalle entre les clignotements")]
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 5f;

    [Header("Clignotement rapide")]
    [SerializeField] private int flickerCountMin = 3;
    [SerializeField] private int flickerCountMax = 6;
    [SerializeField] private float flickerSpeed = 0.05f;

    void Start()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light>();

        // Forcer allumée au départ
        lightSource.enabled = true;

        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Attente entre deux séquences de clignotement
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            int flickerCount = Random.Range(flickerCountMin, flickerCountMax);

            for (int i = 0; i < flickerCount; i++)
            {
                lightSource.enabled = false;
                yield return new WaitForSeconds(flickerSpeed);

                lightSource.enabled = true;
                yield return new WaitForSeconds(flickerSpeed);
            }
        }
    }
}