using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Paramètres de vie")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Référence à la barre de vie UI")]
    public HealthBar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }
}