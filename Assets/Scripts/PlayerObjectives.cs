using UnityEngine;

public class PlayerObjectives : MonoBehaviour
{
    public bool hasKey;
    public bool hasHammer;
    public bool codeOk;

    public bool CanExit => hasKey && hasHammer && codeOk;

    public void SetCodeOkTrue()
    {
        codeOk = true;
        Debug.Log("✅ codeOk = true (SetCodeOkTrue)");
    }

}
