using UnityEngine;
using Unity.Netcode;
using Alteruna;
public class CameraMovement : MonoBehaviour {
    [Header("Mouse Input")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private bool invertCameraXAxis;
    [SerializeField] private bool invertCameraYAxis;
    private float verticalRotation;

    [Header("Crouching")]
    [SerializeField] private float uncrouchedCameraHeight;
    [SerializeField] private float crouchedCameraHeight;
    [SerializeField] private float cameraHeightSwitchSpeed;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;

    private float mouseX, mouseY;

    private void Awake() {
        avatar = GetComponentInParent<Alteruna.Avatar>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Start() {

        if (!avatar.IsMe)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        // if (!IsOwner) return;
        if (!avatar.IsMe)
        {
            return;
        }
        ProcessCameraInput();
        ProcessCamera();
    }

    private void LateUpdate() {
        //  if (!IsOwner) return;

        if (!avatar.IsMe)
        {
            return;
        }
        ProcessCameraCrouched();
    }

    private void ProcessCameraInput()
    {
        //Horizontal rotation
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        if (invertCameraXAxis)
            mouseX = -mouseX;

        //Vertical rotation
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        if (invertCameraYAxis)
            mouseY = -mouseY;
    }

    private void ProcessCamera() {
        if(!playerController.MovementEnabled) {
            ResetCameraValues();
            return;
        }

        playerController.transform.Rotate(Vector3.up, mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        this.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void ProcessCameraCrouched() {
        Vector3 uncrouchedCamera = new(this.transform.localPosition.x, uncrouchedCameraHeight, this.transform.localPosition.z);
        Vector3 crouchedCamera = new(this.transform.localPosition.x, crouchedCameraHeight, this.transform.localPosition.z);

        this.transform.localPosition = playerController.IsCrouching
            ? Vector3.Lerp(this.transform.localPosition, crouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime)
            : Vector3.Lerp(this.transform.localPosition, uncrouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime);
    }

    private void ResetCameraValues() {
        Vector3 resetToUncrouchedCamera = new(this.transform.localPosition.x, uncrouchedCameraHeight, this.transform.localPosition.z);
        this.transform.localPosition = resetToUncrouchedCamera;
    }
}
