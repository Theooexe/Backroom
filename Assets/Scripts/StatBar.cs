using UnityEngine;

public class StatBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;

    private float maxWidth;

    private void Awake()
    {
        if (fillRect == null)
        {
            Debug.LogError("Fill RectTransform manquant.");
            enabled = false;
            return;
        }

        maxWidth = fillRect.sizeDelta.x;
    }

    public void SetValue(float current, float max)
    {
        if (max <= 0f) return;

        float ratio = Mathf.Clamp01(current / max);

        fillRect.sizeDelta = new Vector2(
            maxWidth * ratio,
            fillRect.sizeDelta.y
        );
    }
}

