using UnityEngine;

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

    private void Awake() {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        ProcessCamera();
    }

    private void LateUpdate() {
        ProcessCameraCrouched();
    }

    private void ProcessCamera() {
        if(!playerController.MovementEnabled) {
            ResetCameraValues();
            return;
        }

        //Horizontal rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        if(invertCameraXAxis)
            mouseX = -mouseX;
        playerController.transform.Rotate(Vector3.up, mouseX);

        //Vertical rotation
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        if(invertCameraYAxis)
            mouseY = -mouseY;
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
