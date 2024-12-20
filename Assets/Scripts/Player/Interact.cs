using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;
using Alteruna;


public class Interact : MonoBehaviour, IObserver
{
    private Alteruna.Avatar avatar;
    GameObject heldObject = null;

    [Header("not important")]
    [SerializeField] GameObject hand;
    [SerializeField] Camera playerCamera;
    PlayerController playerController;


    [Header("Designer Values")]
    [SerializeField] float grabReach;
    [SerializeField] KeyCode primaryButton;
    [SerializeField] KeyCode interactButton;
    [SerializeField] Vector2 minMaxThrowStrength;
    [SerializeField] Vector2 minMaxThrowChargeUpTime;


    float currentChargeUpTime = 0;
    float currentThrowStrength = 0;
    LayerMask dynamicLayerMask;
    LayerMask stationaryLayerMask;
    LayerMask interactableLayerMask;
    bool finishedPickUp = true;
    bool isChargingUp = false;

    RigidbodySynchronizable rbToTrack;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        dynamicLayerMask = LayerMask.GetMask("DynamicInteractableObject");
        stationaryLayerMask = LayerMask.GetMask("StationaryInteractableObject");
        interactableLayerMask = dynamicLayerMask | stationaryLayerMask;
    }

    void Update()
    {
        if (!avatar.IsMe) { return; }

        ProcessInput();
        HighlightInteractable();

        if (isChargingUp) currentChargeUpTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        UpdateHeldObjectPhysics();
    }
    private void ProcessInput()
    {
        //release / place
        if (Input.GetMouseButtonUp(0) && heldObject != null)
        {
            if (finishedPickUp)
            {
                isChargingUp = false;
                heldObject.GetComponent<Rigidbody>().useGravity = true;

                if (currentChargeUpTime > minMaxThrowChargeUpTime.x)
                {
                    if (currentChargeUpTime > minMaxThrowChargeUpTime.y) currentChargeUpTime = minMaxThrowChargeUpTime.y;
                    AnimateReleaseChargebar();
                    currentThrowStrength = Mathf.Lerp(minMaxThrowStrength.x, minMaxThrowStrength.y, currentChargeUpTime);
                    currentChargeUpTime = 0;
                    Throw();
                }
                else
                {
                    Place();
                }
            }
            finishedPickUp = true;
        }

        //windup
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null && finishedPickUp)
            {
                isChargingUp = true;
                AnimateWindUpChanrgebar();
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity, dynamicLayerMask))
                {
                    if(Vector3.Distance(hit.transform.position, transform.position) < grabReach)
                    {
                        TryPickUp(hit.transform.gameObject);
                        finishedPickUp = false;
                    }
                    else
                    {
                        Debug.Log("Object too far");
                    }
                }
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            //if raycast with stationary, prioritize it
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity, stationaryLayerMask))
            {
                hit.transform.gameObject.GetComponent<StationaryInteractableObject>().Use();
            }
            else
            {
                if (heldObject != null)
                {
                    heldObject.GetComponent<DynamicInteractableObject>().Use();
                }
            }
        }
    }

    public void SpecialInteraction(InteractionEnum interaction)
    {
        if(interaction == InteractionEnum.ShotWithGun)
        {
            Die();
        }
    }
    private void Place()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity))
        {
            heldObject.transform.SetParent(GameObject.FindGameObjectWithTag("SceneParentForPlacedObjects").transform, true);
            heldObject.transform.position = hit.point + hit.normal.normalized;
            heldObject.transform.up = hit.normal;
            heldObject = null;
        }
    }
    private void Throw()
    {
        heldObject.GetComponent<Rigidbody>().freezeRotation = false;
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        rbToTrack.AddForce(playerCamera.transform.forward * currentThrowStrength, ForceMode.Impulse);

        currentThrowStrength = 0;
        heldObject.transform.parent = null;
        heldObject = null;
    }
    private void TryPickUp(GameObject pickedUp)
    {
        if(pickedUp.GetComponent<DynamicInteractableObject>() != null)
        {
            heldObject = pickedUp;
            heldObject.transform.parent = hand.transform;
            pickedUp.transform.rotation = Quaternion.Euler(0f, hand.transform.eulerAngles.y, 0f);
            heldObject.GetComponent<Rigidbody>().freezeRotation = true;
            heldObject.GetComponent<Rigidbody>().useGravity = false;
            rbToTrack = heldObject.GetComponent<RigidbodySynchronizable>();
        }
        else
        {
            Debug.Log("You can't pick up that");
        }
    }

    private void UpdateHeldObjectPhysics()
    {
        if (heldObject != null)
        {
            Vector3 actualPosition = hand.transform.position + (playerCamera.transform.forward * heldObject.transform.localScale.x);
            if (Vector3.Distance(rbToTrack.transform.position, actualPosition) > 0.01f)
            {
                Vector3 moveDirection = hand.transform.position - rbToTrack.transform.position;
                rbToTrack.AddForce(moveDirection * 10);
            }
        }
    }

    //art stuff
    private void AnimateWindUpChanrgebar()
    {

    }
    private void AnimateReleaseChargebar()
    {

    }
    private void HighlightInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity, interactableLayerMask))
        {
            // Highlight shader or whatever
        }
    }

    private void Die()
    {
        //whatever happens when you die (kicked out screen or whatever
    }
}



