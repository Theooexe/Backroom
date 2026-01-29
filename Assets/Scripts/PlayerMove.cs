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

    [HideInInspector] public bool canLook = true;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private CharacterController characterController;
    private float stepTimer = 0f;

    // Endurance
    [HideInInspector] public bool IsTryingToRun = false;
    [HideInInspector] public bool IsRunning = false;
    private bool forceStopRunning = false;

    [HideInInspector] public PlayerStats playerStats;

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
        HandleMovement();
        HandleCameraRotation();
        HandleFootstepSounds();
    }

    // ------------------------
    // MOUVEMENT
    // ------------------------
    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        IsTryingToRun = Input.GetKey(KeyCode.LeftShift);
        if (forceStopRunning) IsTryingToRun = false;

        IsRunning = IsTryingToRun && !forceStopRunning &&
                    playerStats != null && playerStats.CurrentStamina > 0f;

        float currentSpeed =
            isCrouching ? crouchSpeed :
            (IsRunning ? runSpeed : walkSpeed);

        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        Vector3 move = transform.forward * inputZ + transform.right * inputX;
        move *= currentSpeed;

        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
                moveDirection.y = -2f;

            if (Input.GetButtonDown("Jump") && !isCrouching)
                moveDirection.y = jumpPower;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        moveDirection.x = move.x;
        moveDirection.z = move.z;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    // ------------------------
    // ROTATION CAMERA
    // ------------------------
    void HandleCameraRotation()
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ------------------------
    // FOOTSTEPS
    // ------------------------
    void HandleFootstepSounds()
    {
        bool isMoving = characterController.isGrounded &&
                        (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f);

        float interval = IsRunning ? runStepInterval : walkStepInterval;

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

    // ------------------------
    // BLOQUE LE SPRINT
    // ------------------------
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
}
