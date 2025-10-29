using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 90f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

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

    // Calcul de la vitesse
    float speed = isRunning ? runSpeed : walkSpeed;

    // Mouvement horizontal
    Vector3 move = (transform.forward * inputZ + transform.right * inputX) * speed;

    // Gestion du saut et gravité
        if (characterController.isGrounded)
        {
            moveDirection.y = 0f; // reset vertical velocity
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpPower;
        }
    moveDirection.y -= gravity * Time.deltaTime;

    // Combine horizontal et vertical
    moveDirection.x = move.x;
    moveDirection.z = move.z;

    // Déplacement
    characterController.Move(moveDirection * Time.deltaTime);

    // Rotation caméra
    rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
    rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
    }

}