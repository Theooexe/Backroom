using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Player Settings")]
    public Camera playerCamera;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 90f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 1f;

    [Header("Footsteps")]
    public AudioClip stepClip;
    public AudioSource audioSource;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private CharacterController characterController;
    private float stepTimer = 0f;

    // 🔥 Endurance
    [HideInInspector] public bool IsTryingToRun = false; // appuie sur shift
    [HideInInspector] public bool IsRunning = false;    // sprint réel
    private bool forceStopRunning = false;

    [HideInInspector] public PlayerStats playerStats; // référence pour stamina

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // 🔥 Détection de "je veux courir"
        IsTryingToRun = Input.GetKey(KeyCode.LeftShift);

        // Empêché par la stamina ?
        if (forceStopRunning)
            IsTryingToRun = false;

        // 🔥 Détermine si le joueur court réellement
        IsRunning = IsTryingToRun && !forceStopRunning && playerStats != null && playerStats.CurrentStamina > 0f;

        // Choix de la vitesse
        float currentSpeed =
            isCrouching ? crouchSpeed :
            (IsRunning ? runSpeed : walkSpeed);

        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        // Mouvement horizontal
        Vector3 move = (transform.forward * inputZ + transform.right * inputX) * currentSpeed;

        // Gravité + saut
        if (characterController.isGrounded)
        {
            moveDirection.y = 0f;

            if (Input.GetButton("Jump") && !isCrouching)
                moveDirection.y = jumpPower;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        moveDirection.x = move.x;
        moveDirection.z = move.z;

        characterController.Move(moveDirection * Time.deltaTime);

        HandleFootstepSounds(currentSpeed);

        // Rotation caméra
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
    }

    public void ForceStopRunning()
    {
        forceStopRunning = true;
        StartCoroutine(ResetRunBlock());
    }

    private IEnumerator ResetRunBlock()
    {
        yield return new WaitForSeconds(0.1f);
        forceStopRunning = false;
    }

    void HandleFootstepSounds(float speed)
    {
        bool isMoving = characterController.isGrounded && (moveDirection.x != 0 || moveDirection.z != 0);
        float interval = speed > walkSpeed ? runStepInterval : walkStepInterval;

        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                if (stepClip != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(stepClip);
                }
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = interval;
        }
    }
}
