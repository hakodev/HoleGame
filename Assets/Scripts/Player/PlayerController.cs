using UnityEngine;
using Alteruna;
using NUnit.Framework;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour {

    [field: SerializeField] public bool MovementEnabled { get; set; } = true;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchRunSpeed;

    [Header("Jumping & Physics")]
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float crouchedJumpHeight;

    private CharacterController characterController;
    private bool isMoving;
    private bool isRunning;
    private const float gravity = -9.81f;
    private const float startingVerticalVelocity = 2f;
    private Vector3 verticalVelocity;
    private float horizontalInput;
    private float verticalInput;

    private Alteruna.Avatar avatar;
    private GameObject animationTie;
    MishSyncAnimations mishSync;
    public bool IsCrouching { get; private set; }
    public Roles Role { get; set; }
    public int VotedCount { get; set; }
    public bool IsTaskManager { get; set; } = false;
    public SymptomsSO Symptom { get; set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();
    }
    private void Update()
    {

        if (!avatar.IsMe) { return; }


        if (!MovementEnabled)
        {
            ResetMovementValues();
            return;
        }

        ProcessInput();
        ProcessMovement();
    }




    private void ProcessInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isMoving = horizontalInput != 0 || verticalInput != 0;

        isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        IsCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
    private void ProcessMovement()
    {

        float currentSpeed = 0f;
        float currentjumpHeight;

        if (IsCrouching)
        {
            //  currentSpeed = isRunning ? crouchRunSpeed : crouchSpeed;
            if (isMoving)
            {
                //crouching walking animation
                currentSpeed = crouchSpeed;
            }
            //  currentjumpHeight = crouchedJumpHeight;
            currentjumpHeight = jumpHeight;
        }
        else
        {
            if (isMoving)
            {
                if (isRunning)
                {
                    currentSpeed = runSpeed;
                    mishSync.Running = true;
                }
                else
                {
                    currentSpeed = walkSpeed;
                    mishSync.Walking = true;
                }
            }

            currentjumpHeight = jumpHeight;
        }
        if (!isMoving)
        {
            mishSync.Walking = false;
            mishSync.Running = false;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 finalMovement = currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection);

        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -startingVerticalVelocity;
        }
        else
        {
            verticalVelocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        if (characterController.isGrounded)
        {
            mishSync.SetJumping(false);
        }

        // Handle jumping
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity.y = Mathf.Sqrt(currentjumpHeight * -2f * gravity);
            mishSync.SetJumping(true);
            mishSync.Walking = false;
            mishSync.Running = false;
        }
        finalMovement += verticalVelocity * Time.deltaTime;

        characterController.Move(finalMovement);
    }

    private void ResetMovementValues()
    {
        IsCrouching = false;
        horizontalInput = 0f;
        verticalInput = 0f;
    }
}


