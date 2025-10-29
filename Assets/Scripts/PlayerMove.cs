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
    public AudioClip[] walkClips;  // plusieurs sons de marche
    public AudioClip[] runClips;   // plusieurs sons de course
    public AudioSource audioSource;
    public float stepInterval = 0.5f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canMove) return;

        // Lecture des inputs
        float inputX = Input.GetAxis("Horizontal"); // A/D
        float inputZ = Input.GetAxis("Vertical");   // W/S
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Crouch sur Ctrl
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        float currentSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            characterController.height = crouchHeight;
        }
        else
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            characterController.height = defaultHeight;
        }

        // Mouvement horizontal
        Vector3 move = (transform.forward * inputZ + transform.right * inputX) * currentSpeed;

        // Gestion du saut et gravité
        if (characterController.isGrounded)
        {
            moveDirection.y = 0f;
            if (Input.GetButton("Jump") && !isCrouching)
                moveDirection.y = jumpPower;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        // Combine horizontal et vertical
        moveDirection.x = move.x;
        moveDirection.z = move.z;

        // Déplacement
        characterController.Move(moveDirection * Time.deltaTime);

        // Jouer sons de pas
        PlayFootstepSound(currentSpeed, inputX, inputZ);

        // Rotation caméra
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
    }

    void PlayFootstepSound(float speed, float inputX, float inputZ)
    {
        Vector3 horizontalMove = new Vector3(moveDirection.x, 0, moveDirection.z);
        bool isMoving = characterController.isGrounded && horizontalMove.magnitude > 0.1f;

        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval / (speed / walkSpeed)) // plus rapide si course
            {
                AudioClip[] clips = (speed > walkSpeed) ? runClips : walkClips;
                if (clips.Length > 0 && audioSource != null)
                {
                    audioSource.clip = clips[Random.Range(0, clips.Length)];
                    audioSource.pitch = Random.Range(0.9f, 1.1f); // variation de pitch
                    audioSource.Play();
                }
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval; // reset timer si arrêt
        }
    }
}
