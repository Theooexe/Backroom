using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    private void Awake()
    {
        Hide();
    }

    public void SetItem(Sprite sprite)
    {
        if (icon == null) return;

        icon.sprite = sprite;
        icon.color = Color.white; // visible
    }

    public void Hide()
    {
        if (icon == null) return;

        // Invisible (garde la place)
        icon.color = new Color(1f, 1f, 1f, 0f);
        icon.sprite = null;
    }
}
