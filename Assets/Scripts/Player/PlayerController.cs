using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask groundLayer;

    [Header("TESTING ONLY")]
    [SerializeField] private Material groundedColor;
    [SerializeField] private Material jumpingColor;
    [SerializeField] private float groundLayerMaxDistance;
    private MeshRenderer meshRenderer;

    [Header("Movement")]
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchRunSpeed;
    [SerializeField] private float jumpHeight;

    [Header("Crouching")]
    [SerializeField] private float uncrouchedCameraHeight;
    [SerializeField] private float crouchedCameraHeight;
    [SerializeField] private float cameraHeightSwitchSpeed;

    private Rigidbody rigid;
    private bool isMoving;
    private bool isGrounded;
    private bool isCrouching;
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

        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, groundLayerMaxDistance, groundLayer);

        float currentSpeed;

        if(!isCrouching && Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            currentSpeed = runSpeed;
        } else if(isCrouching && Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            currentSpeed = crouchRunSpeed;
        } else if(isCrouching && !Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.RightShift)) {
            currentSpeed = crouchSpeed;
        } else {
            currentSpeed = walkSpeed;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 targetVelocity = currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection);
        //targetVelocity.y = rigid.linearVelocity.y; //Preserve the existing vertical velocity (gravity/jump)

        // Apply movement force
        Vector3 currentVelocity = rigid.angularVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        rigid.AddForce(velocityChange, ForceMode.VelocityChange);

        // Handle jumping
        if(isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
