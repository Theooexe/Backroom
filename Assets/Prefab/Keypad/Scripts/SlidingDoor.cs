using UnityEngine;

namespace NavKeypad
{
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        public bool IsOpen => isOpen;
        private bool isOpen = false;

        private void Awake()
        {
            // Si pas assigné, on essaie de le trouver sur l'objet ou ses enfants
            if (anim == null)
                anim = GetComponent<Animator>();

            if (anim == null)
                anim = GetComponentInChildren<Animator>();

            if (anim == null)
                Debug.LogError($"❌ SlidingDoor: aucun Animator trouvé sur '{gameObject.name}' ou ses enfants.");
        }

        public void ToggleDoor()
        {
            if (anim == null) return;

            isOpen = !isOpen;
            anim.SetBool("isOpen", isOpen);
        }

        public void OpenDoor()
        {
            if (anim == null) return;

            isOpen = true;
            anim.SetBool("isOpen", isOpen);
        }

        public void CloseDoor()
        {
            if (anim == null) return;

            isOpen = false;
            anim.SetBool("isOpen", isOpen);
        }
    }
}
