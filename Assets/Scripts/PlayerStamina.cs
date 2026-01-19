using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRunCost = 20f;    // perte par seconde en courant
    public float staminaRegenRate = 10f;  // régénération par seconde
    public float currentStamina;

    public StaminaBar staminaBar;

    private PlayerMove moveScript;

    private void Start()
    {
        moveScript = GetComponent<PlayerMove>();
        currentStamina = maxStamina;

        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina, maxStamina);
    }

    private void Update()
    {
        HandleStaminaUsage();
        RegenerateStamina();

        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina, maxStamina);
    }

    private void HandleStaminaUsage()
    {
        if (moveScript == null) return;

        if (moveScript.IsTryingToRun && currentStamina > 0)
        {
            currentStamina -= staminaRunCost * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                moveScript.ForceStopRunning();
            }
        }
    }

    private void RegenerateStamina()
    {
        if (moveScript == null) return;

        if (!moveScript.IsTryingToRun)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
        }
    }
}