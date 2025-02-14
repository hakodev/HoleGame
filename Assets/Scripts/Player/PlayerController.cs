using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField] public bool MovementEnabled { get; set; } = true;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchRunSpeed;
    [SerializeField] private float walkSpeedBack;
    [SerializeField] private float runSpeedBack;

    [Header("Jumping & Physics")]
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float crouchedJumpHeight;

    private CharacterController characterController;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isRunning;
    private const float gravity = -9.81f;
    private const float startingVerticalVelocity = 2f;
    private Vector3 verticalVelocity;
    private float horizontalInput;
    private float verticalInput;
    private float currentjumpHeight;
    [SerializeField] float timeSavingJumpInput;
    float maxTimeSavingJumpInput;

    private Alteruna.Avatar avatar;
    private GameObject animationTie;
    MishSyncAnimations mishSync;


    [Header("Nerd SHIT - Programming")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform moveTransform;


    PlayerRole role;

    public bool IsCrouching { get; private set; }

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        characterController = GetComponent<CharacterController>();
        mishSync = GetComponent<MishSyncAnimations>();
        role = GetComponent<PlayerRole>();
    }
    private void Start()
    {
        if (!avatar.IsMe) { return; }
        maxTimeSavingJumpInput = timeSavingJumpInput;
    }

    public void AddVerticalVelocity(float bonus)
    {
       verticalVelocity.y += Mathf.Sqrt(bonus * -2f * gravity); //finish later
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
        mishSync.SetInputDirection(new Vector2(horizontalInput, verticalInput));

        isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        IsCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
    private void ProcessMovement()
    {

        float currentSpeed = 0f;
        

        if (IsCrouching)
        {
            if(mishSync.GetCurrentStance() != StanceEnum.Crouching)
            {
                mishSync.SetStance(StanceEnum.Crouching);
            }
                
            //  currentSpeed = isRunning ? crouchRunSpeed : crouchSpeed;
            if (isMoving)
            {
                //crouching walking animation
                currentSpeed = crouchSpeed;
            }
            currentjumpHeight = crouchedJumpHeight;
        }
        else
        {
            if (isMoving)
            {
                if (isRunning)
                {
                    currentSpeed = runSpeed;
                    if (verticalInput < 0) currentSpeed = runSpeedBack;
                    if (mishSync.GetCurrentStance() != StanceEnum.Running)
                    {
                        mishSync.SetStance(StanceEnum.Running);
                    }
                }
                else
                {
                    currentSpeed = walkSpeed;
                    if (verticalInput < 0) currentSpeed = walkSpeedBack;
                    if (mishSync.GetCurrentStance() != StanceEnum.Walking)
                    {
                        mishSync.SetStance(StanceEnum.Walking);
                    }
                }
            }
            else
            {
                if (mishSync.GetCurrentStance() != StanceEnum.Walking)
                {
                    mishSync.SetStance(StanceEnum.Walking);
                }
            }

            currentjumpHeight = jumpHeight;
        }

        if(SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[0] && role.GetRole() == Roles.Machine) {
            //Inverted controls
            horizontalInput = -horizontalInput;
            verticalInput = -verticalInput;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        moveTransform.localRotation = Quaternion.Euler(new Vector3(0, cameraTransform.localEulerAngles.y, 0));
        Vector3 finalMovement = currentSpeed * Time.deltaTime * moveTransform.TransformDirection(moveDirection);

        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -startingVerticalVelocity;
        }
        else
        {
            verticalVelocity.y += gravity * gravityMultiplier * Time.deltaTime;
        }

        if (characterController.isGrounded && verticalVelocity.y <= 0)
        {
            mishSync.SetJumping(false);
        }

        // Handle jumping + coyotte time
        if (Input.GetKey(KeyCode.Space) && mishSync.GetCurrentStance()!=StanceEnum.Dead)
        {
            timeSavingJumpInput = maxTimeSavingJumpInput;
        }
        if(timeSavingJumpInput>0)
        {
            timeSavingJumpInput-= Time.deltaTime;
            Jump();
        }

        finalMovement += verticalVelocity * Time.deltaTime;

        characterController.Move(finalMovement);
    }

    public void Jump() {
        if(characterController.isGrounded) {
            verticalVelocity.y = Mathf.Sqrt(currentjumpHeight * -2f * gravity);
            mishSync.SetJumping(true);
        }
    }



    private void ResetMovementValues()
    {
        IsCrouching = false;
        horizontalInput = 0f;
        verticalInput = 0f;
    }
}
