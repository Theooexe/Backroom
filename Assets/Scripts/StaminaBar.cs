using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetStamina(float current, float max)
    {
        float amount = Mathf.Clamp01(current / max);
        fillImage.fillAmount = amount;
    }
}