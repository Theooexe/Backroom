using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Vie")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Endurance")]
    [SerializeField] private float maxStamina = 100f;
    private float currentStamina;

    [Header("UI")]
    [SerializeField] private StatBar healthBar;
    [SerializeField] private StatBar staminaBar;

    [Header("Sprint / Stamina")]
    [SerializeField] private float staminaConsumptionRate = 20f; // par seconde
    [SerializeField] private float staminaRecoveryRate = 10f; // par seconde

    [Header("Son")]
    [SerializeField] private AudioClip damageClip;
    private AudioSource audioSource;

    private PlayerMove playerMove;

    // Propriété publique pour lecture par d'autres scripts
    public float CurrentStamina => currentStamina;
    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        UpdateUI();
    }

    private void Update()
    {
        HandleStamina();
    }

    // -------------------- Stamina --------------------
    private void HandleStamina()
    {
        if (playerMove == null) return;

        // Sprint activé et stamina > 0 ?
        if (playerMove.IsTryingToRun && currentStamina > 0f)
        {
            // Consomme la stamina
            currentStamina -= staminaConsumptionRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);

            // Si stamina vide, empêche le sprint
            if (currentStamina <= 0f)
            {
                playerMove.ForceStopRunning();
            }
        }
        else
        {
            // Régénération
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        // Met à jour la barre
        if (staminaBar != null)
            staminaBar.SetValue(currentStamina, maxStamina);
    }

    // -------------------- Vie --------------------
    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        UpdateUI();

        // Jouer le son de dégâts
        if (damageClip != null && audioSource != null)
            audioSource.PlayOneShot(damageClip);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    // -------------------- UI --------------------
    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.SetValue(currentHealth, maxHealth);

        if (staminaBar != null)
            staminaBar.SetValue(currentStamina, maxStamina);
    }

    // -------------------- Mort --------------------
    private void Die()
    {
        Debug.Log("Le joueur est mort");
        // TODO : animation, désactivation des contrôles, respawn...
    }
}
