using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;

    // Appelé quand on ajoute un item dans ce slot
    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.color = Color.white; // rendre visible
    }

    // Appelé si tu veux vider le slot plus tard
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0); // rendre invisible
    }
}
