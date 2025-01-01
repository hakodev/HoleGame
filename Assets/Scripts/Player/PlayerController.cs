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
    //private Animator animator;
    private AnimationSynchronizable animatorSync;
    private GameObject animationTie;
    public bool IsCrouching { get; private set; }

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        //animator = transform.Find("Animation").GetComponent<Animator>(); // Automatically assign Animator if not set
        animatorSync = transform.Find("Animation").GetComponent<AnimationSynchronizable>();
    }

    private void Update() {

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
    private void ProcessMovement() {

        float currentSpeed = 0f;
        float currentjumpHeight;

        if(IsCrouching) {
            //  currentSpeed = isRunning ? crouchRunSpeed : crouchSpeed;
            if (isMoving) 
            {
                //crouching walking animation
                currentSpeed = crouchSpeed;
            }
            //  currentjumpHeight = crouchedJumpHeight;
            currentjumpHeight = jumpHeight;
        } else {
            if(isMoving)
            {
                if (isRunning)
                {
                    currentSpeed = runSpeed;
                    // animator.SetBool("Running", true);
                    //  animator.SetBool("Walking", false);

                   // animatorSync.Animator.SetBool("Running", true);
                  //  animatorSync.Animator.SetBool("Walking", false);

                    animatorSync.SetBool("Running", true);
                    animatorSync.SetBool("Walking", false);
                }
                else
                {
                    currentSpeed = walkSpeed;
                    // animator.SetBool("Walking", true);
                    // animator.SetBool("Running", false);

                  //  animatorSync.Animator.SetBool("Walking", true);
                   // animatorSync.Animator.SetBool("Running", false);

                    animatorSync.SetBool("Walking", true);
                    animatorSync.SetBool("Running", false);
                }
            }
           
            currentjumpHeight = jumpHeight;
        }
        if(!isMoving)
        {
            // animator.SetBool("Running", false);
            //  animator.SetBool("Walking", false);

         //   animatorSync.Animator.SetBool("Running", false);
         //   animatorSync.Animator.SetBool("Walking", false);

            animatorSync.SetBool("Running", false);
            animatorSync.SetBool("Walking", false);
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 finalMovement = currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection);

        if(characterController.isGrounded && verticalVelocity.y < 0) {
            verticalVelocity.y = -startingVerticalVelocity;
        } else {
            verticalVelocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        if (characterController.isGrounded)
        {
            //  animator.SetBool("Jumping", false);

            //animatorSync.Animator.SetBool("Jumping", false);

            animatorSync.SetBool("Jumping", false);
        }

        // Handle jumping
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            verticalVelocity.y = Mathf.Sqrt(currentjumpHeight * -2f * gravity);
            // animator.SetBool("Jumping", true);
            //  animator.SetBool("Running", false);
            //  animator.SetBool("Walking", false);

        //    animatorSync.Animator.SetBool("Jumping", true);
        //    animatorSync.Animator.SetBool("Running", false);
         //   animatorSync.Animator.SetBool("Walking", false);


            animatorSync.SetBool("Jumping", true);
            animatorSync.SetBool("Running", false);
            animatorSync.SetBool("Walking", false);
        }
        finalMovement += verticalVelocity * Time.deltaTime;

        characterController.Move(finalMovement);
    }

    private void ResetMovementValues() {
        IsCrouching = false;
        horizontalInput = 0f;
        verticalInput = 0f;
    }
}
