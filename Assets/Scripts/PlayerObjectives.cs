using UnityEngine;

public class PlayerObjectives : MonoBehaviour
{
    public bool hasKey;
    public bool hasHammer;

    public bool CanOpenExitDoor() => hasKey && hasHammer;

    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("🔑 Clé récupérée !");
    }

    public void CollectHammer()
    {
        hasHammer = true;
        Debug.Log("🔨 Marteau récupéré !");
    }
}
