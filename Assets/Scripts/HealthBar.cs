using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    /// <summary>
    /// Met à jour la barre de vie et le texte.
    /// </summary>
    public void SetHealth(float currentHealth, float maxHealth)
    {
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        fillImage.fillAmount = fillAmount;

        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
        }
    }   
}   