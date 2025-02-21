using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Mouse Input")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private bool invertCameraXAxis;
    [SerializeField] private bool invertCameraYAxis;
    private float verticalRotation;
    [SerializeField] private float sensitivityStep = 0.5f;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 10f;


    [Header("Crouching")]
    [SerializeField] private float uncrouchedCameraHeight;
    [SerializeField] private float crouchedCameraHeight;
    [SerializeField] private float cameraHeightSwitchSpeed;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;

    private float mouseX, mouseY;

    public bool FreezeCameraRotation { get; set; } = false;

    private void Awake()
    {
        avatar = GetComponentInParent<Alteruna.Avatar>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        // if (!IsOwner) return;
        if (!avatar.IsMe)
        {
            return;
        }
        if(!FreezeCameraRotation)
        {
            ProcessCameraInput();
            ProcessCamera();
        }
    }

    private void LateUpdate()
    {
        //  if (!IsOwner) return;

        if (!avatar.IsMe)
        {
            return;
        }
        if (!FreezeCameraRotation) ProcessCameraCrouched();
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

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            ChangeSensitivity(scroll);
        }
    }

    private void ProcessCamera()
    {
        if (!playerController.MovementEnabled)
        {
            ResetCameraValues();
            return;
        }

        playerController.transform.Rotate(Vector3.up, mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        this.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void ProcessCameraCrouched()
    {
        Vector3 uncrouchedCamera = new(this.transform.localPosition.x, uncrouchedCameraHeight, this.transform.localPosition.z);
        Vector3 crouchedCamera = new(this.transform.localPosition.x, crouchedCameraHeight, this.transform.localPosition.z);

        this.transform.localPosition = playerController.IsCrouching
            ? Vector3.Lerp(this.transform.localPosition, crouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime)
            : Vector3.Lerp(this.transform.localPosition, uncrouchedCamera, cameraHeightSwitchSpeed * Time.fixedDeltaTime);
    }

    private void ResetCameraValues()
    {
        Vector3 resetToUncrouchedCamera = new(this.transform.localPosition.x, uncrouchedCameraHeight, this.transform.localPosition.z);
        this.transform.localPosition = resetToUncrouchedCamera;
    }


    private void ChangeSensitivity(float scrollAmount)
    {
        mouseSensitivity = Mathf.Clamp(mouseSensitivity + scrollAmount * sensitivityStep, minSensitivity, maxSensitivity);
    }


}


/*
using UnityEngine;
using Alteruna;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
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

    [Header("Separating Head and Body")]
    [SerializeField] private GameObject playerHead;
    [SerializeField] private GameObject playerTorso;
    [SerializeField] private MishSyncAnimations mishSync;
    [SerializeField] private float easingTorsoDuration;
    // part of player controller script gets the direction of the head each time the player moves, if they are moving foward it adjust their torso to the same direction

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private float horizontalRotation;

    private float mouseX, mouseY;

    private void Awake() {
        avatar = GetComponentInParent<Alteruna.Avatar>();
        playerController = GetComponentInParent<PlayerController>();
        mishSync = transform.root.GetComponent<MishSyncAnimations>();
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
        if (!avatar.IsMe){ return;}



        //playerHead.transform.Rotate(verticalRotation, horizontalRotation, 0);
        //playerHead.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        //playerHead.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        ProcessCameraCrouched();

        AnimateHeadAndTorso();

    }

    private void AnimateHeadAndTorso()
    {
        //transform.root.Rotate(verticalRotation, horizontalRotation, 0);

           playerTorso.transform.Rotate(new Vector3(0, horizontalRotation, 0));
           playerHead.transform.Rotate(new Vector3(verticalRotation, 0, 0));


        /*
        //Vector3 headEuler = playerHead.transform.rotation.eulerAngles;
        // Vector3 torsoEuler = playerTorso.transform.rotation.eulerAngles;
        //float yas = Mathf.Abs(headEuler.x - torsoEuler.x) + Mathf.Abs   (headEuler.y - torsoEuler.y) + Mathf.Abs(headEuler.z - torsoEuler.z);
        // yas = yas % 360;

        //    Debug.Log(yas);
        Debug.Log(Mathf.Abs(playerHead.transform.rotation.eulerAngles.y - playerTorso.transform.rotation.eulerAngles.y));


        if (mishSync.GetTargetAnimDot() != Vector2.zero)
        {
            //if moves

            //  playerTorso.transform.localRotation. = Quaternion.Euler(0, horizontalRotation, 0);
            // playerTorso.transform.DOLocalRotate(new Vector3(0, horizontalRotation, 0), easingTorsoDuration).SetEase(Ease.InOutSine);
            // playerTorso.transform.localRotation = Quaternion.Euler(0, horizontalRotation, 0);
            //playerHead.transform.localRotation = Quaternion.Euler(0, 0, 0);

            playerTorso.transform.parent.Rotate(0, horizontalRotation, 0);
        }
        else
        {
            //iff doesnt move
            //playerHead.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);

            playerHead.transform.Rotate(verticalRotation, horizontalRotation, 0);
        }


        if (Mathf.Abs(playerHead.transform.rotation.eulerAngles.y - playerTorso.transform.rotation.eulerAngles.y) > 80)
        {
            //playerTorso.transform.DORotate(new Vector3(0, horizontalRotation, 0), easingTorsoDuration).SetEase(Ease.InOutSine);
            //playerHead.transform.localRotation = Quaternion.Euler(verticalRotation, playerHead.transform.localRotation.y, playerHead.transform.localRotation.z);

            playerTorso.transform.parent.Rotate(0, horizontalRotation, 0);
            playerHead.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            // playerHead.transform.Rotate(verticalRotation, horizontalRotation, 0);
        }
        
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

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        //horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);


        transform.localRotation = Quaternion.Euler(new Vector3(verticalRotation, horizontalRotation, 0));
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
*/