using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask groundLayer;

    [Header("TESTING ONLY")]
    [SerializeField] private Material groundedColor;
    [SerializeField] private Material jumpingColor;
    private MeshRenderer meshRenderer;

    [Header("Movement")]
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchRunSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float crouchedJumpHeight;
    [SerializeField] private float groundLayerDistance;

    [Header("Crouching")]
    [SerializeField] private float uncrouchedCameraHeight;
    [SerializeField] private float crouchedCameraHeight;
    [SerializeField] private float cameraHeightSwitchSpeed;

    private Rigidbody rigid;
    private bool isMoving;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private const float gravity = -9.81f;
    private float horizontalInput;
    private float verticalInput;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        if(playerCamera == null) {
            Debug.LogError("Player camera is not set up in the inspector!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
    }

    private void Update() {
        if(isGrounded) {
            meshRenderer.material = groundedColor;
        } else {
            meshRenderer.material = jumpingColor;
        }

        ProcessMovement();
    }

    private void LateUpdate() {
        ProcessCameraCrouched();
    }

    private void ProcessMovement() {
        if(!movementEnabled) {
            ResetMovementValues();
            return;
        }

        isMoving = horizontalInput > 0 || verticalInput > 0;
        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, groundLayerDistance, groundLayer);
        isRunning = isMoving && Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);

        float currentSpeed;
        float jumpForce;

        if(isCrouching) {
            currentSpeed = isRunning ? crouchRunSpeed : crouchSpeed;
            jumpForce = Mathf.Sqrt(crouchedJumpHeight * -2f * gravity);
        } else {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        this.transform.position += currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection);

        // Handle jumping
        if(isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ProcessCameraCrouched() {
        Vector3 uncrouchedCamera = new(playerCamera.transform.localPosition.x, uncrouchedCameraHeight, playerCamera.transform.localPosition.z);
        Vector3 crouchedCamera = new(playerCamera.transform.localPosition.x, crouchedCameraHeight, playerCamera.transform.localPosition.z);

        playerCamera.transform.localPosition = isCrouching
            ? Vector3.Lerp(playerCamera.transform.localPosition, crouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime)
            : Vector3.Lerp(playerCamera.transform.localPosition, uncrouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime);
    }

    private void ResetMovementValues() {
        Vector3 resetToUncrouchedCamera = new(playerCamera.transform.localPosition.x, uncrouchedCameraHeight, playerCamera.transform.localPosition.z);
        playerCamera.transform.localPosition = resetToUncrouchedCamera;

        isCrouching = false;
        horizontalInput = 0f;
        verticalInput = 0f;
    }
}
