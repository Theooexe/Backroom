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

    [Header("Game Over")]
    [SerializeField] private GameOverMenu gameOverMenu;  // référence au script GameOverManager

    [Header("Sprint / Stamina")]
    [SerializeField] private float staminaConsumptionRate = 20f;
    [SerializeField] private float staminaRecoveryRate = 10f;

    [Header("Son")]
    [SerializeField] private AudioClip damageClip;
    private AudioSource audioSource;

    private PlayerMove playerMove;
    private bool isDead = false;         // ⭐

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
        if (isDead) return; // ⭐
        HandleStamina();
    }

    // -------------------- Stamina --------------------
    private void HandleStamina()
    {
        if (playerMove == null) return;

        if (playerMove.IsTryingToRun && currentStamina > 0f)
        {
            currentStamina -= staminaConsumptionRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);

            if (currentStamina <= 0f)
                playerMove.ForceStopRunning();
        }
        else
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        if (staminaBar != null)
            staminaBar.SetValue(currentStamina, maxStamina);
    }

    // -------------------- Vie --------------------
    public void TakeDamage(float amount)
    {
        if (amount <= 0f || isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        UpdateUI();

        if (damageClip != null && audioSource != null)
            audioSource.PlayOneShot(damageClip);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || isDead) return;

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
        Debug.Log("💀 Le joueur est mort");

        if (gameOverMenu != null)
        {
            gameOverMenu.ShowGameOver();  // ✅ appelle ton menu
        }
        else
        {
            Debug.LogError("❌ GameOverMenu non assigné dans PlayerStats");
        }
    }

}
