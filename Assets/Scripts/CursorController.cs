using UnityEngine;

public class CursorController : MonoBehaviour
{
    public RectTransform cursorImage;

    void Start()
    {
        Cursor.visible = false; 
    }

    void Update()
    {
        if(cursorImage != null)
            cursorImage.position = Input.mousePosition; 
    }
}
