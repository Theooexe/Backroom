using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public GameObject flashlightPrefab;   // ton prefab de lampe
    public Transform attachPoint;         // FlashlightHolder
    public Vector3 localPositionOffset = Vector3.zero;
    public Vector3 localRotationOffset = Vector3.zero;

    private GameObject flashlightInstance;

    void Start()
    {
        if (flashlightPrefab != null && attachPoint != null)
        {
            // Instancie la lampe comme enfant de attachPoint
            flashlightInstance = Instantiate(flashlightPrefab, attachPoint);
            flashlightInstance.transform.localPosition = localPositionOffset;
            flashlightInstance.transform.localRotation = Quaternion.Euler(localRotationOffset);
        }
    }
}
