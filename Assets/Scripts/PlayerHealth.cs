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
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
    }
}