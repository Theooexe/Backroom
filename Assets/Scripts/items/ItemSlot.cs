using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;

    private void Awake()
    {
        Hide();
    }

    public void SetItem(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.color = Color.white;
    }

    public void Hide()
    {
        icon.color = new Color(1f, 1f, 1f, 0f);
    }
}
