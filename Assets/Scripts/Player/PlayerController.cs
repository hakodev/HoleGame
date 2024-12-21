using UnityEngine;
using UnityEngine.InputSystem.XR;
using Alteruna;
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
    public bool IsCrouching { get; private set; }

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Update() {
        if (!avatar.IsMe) { return; }
        ProcessMovement();
    }

    private void ProcessMovement() {
        if(!MovementEnabled) {
            ResetMovementValues();
            return;
        }

        isMoving = horizontalInput > 0 || verticalInput > 0;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        IsCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        float currentSpeed;
        float currentjumpHeight;

        if(IsCrouching) {
            currentSpeed = isRunning ? crouchRunSpeed : crouchSpeed;
            currentjumpHeight = crouchedJumpHeight;
        } else {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            currentjumpHeight = jumpHeight;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 finalMovement = currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection);

        if(characterController.isGrounded && verticalVelocity.y < 0) {
            verticalVelocity.y = -startingVerticalVelocity;
        } else {
            verticalVelocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        // Handle jumping
        if(characterController.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            verticalVelocity.y = Mathf.Sqrt(currentjumpHeight * -2f * gravity);
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
