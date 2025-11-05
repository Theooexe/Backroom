using System.Collections;
using System.Collections.Generic;
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
    public AudioClip stepClip;       // Même son pour marche et course
    public AudioSource audioSource;  // AudioSource pour les pas
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private CharacterController characterController;
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D
        }
    }

    void Update()
    {
        // Inputs
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // Vitesse
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        // Mouvement horizontal
        Vector3 move = (transform.forward * inputZ + transform.right * inputX) * currentSpeed;

        // Gravité et saut
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

        // Sons de pas
        HandleFootstepSounds(currentSpeed);

        // Rotation caméra
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
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
            stepTimer = interval; // reset timer quand on s'arrête
        }
    }
}
