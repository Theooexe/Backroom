using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlightLight;
    private bool isOn = true;
    public bool IsOn => isOn;

    void Start()
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }

    public void ToggleFlashlight()
    {
        if (flashlightLight != null)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
        }
    }
}
