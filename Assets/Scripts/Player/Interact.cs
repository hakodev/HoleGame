using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;
using Alteruna;
using DG.Tweening;
using System.Collections.Generic;

public class Interact : AttributesSync, IObserver
{
    private Alteruna.Avatar avatar;
    GameObject heldObject = null;

    [Header("not important")]
    [SerializeField] GameObject clientHand;
    [SerializeField] GameObject serverHand;
    GameObject serverHeldObject;
    List<GameObject> allServerHandObjects = new List<GameObject>();
    [SerializeField] Camera playerCamera;
    PlayerController playerController;


    [Header("Designer Values")]
    [SerializeField] float grabReach;
    [SerializeField] float placeReach;
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
    Rigidbody rb;
    Animator animator;
    AnimationSynchronizable animatorSync;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        playerController = GetComponent<PlayerController>();
        animator = transform.Find("Animation").GetComponent<Animator>();
        animatorSync = transform.Find("Animation").GetComponent<AnimationSynchronizable>();
    }
    private void Start()
    {
        dynamicLayerMask = LayerMask.GetMask("DynamicInteractableObject");
        stationaryLayerMask = LayerMask.GetMask("StationaryInteractableObject");
        interactableLayerMask = dynamicLayerMask | stationaryLayerMask;

        if (!avatar.IsMe) {
            gameObject.layer = LayerMask.NameToLayer("PlayerLayer");
            return;
        }
        else
        {
            int selfLayer = LayerMask.NameToLayer("SelfPlayerLayer");
            gameObject.layer = selfLayer;
            SetLayerRecursively(gameObject, selfLayer);

            foreach (GameObject child in serverHand.GetComponentInChildren<Transform>())
            {
                allServerHandObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }
    }
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
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
                if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, grabReach, dynamicLayerMask))
                {
                    TryPickUp(hit.transform.gameObject);
                    finishedPickUp = false;
                }
            }
        }


        if (Input.GetMouseButtonDown(1))
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
    private void Place()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, placeReach))
        {
            heldObject.transform.SetParent(GameObject.FindGameObjectWithTag("SceneParentForPlacedObjects").transform, true);
            heldObject.transform.position = hit.point + hit.normal.normalized;
            heldObject.transform.up = hit.normal;
            DynamicInteractableObject DIO = heldObject.GetComponent<DynamicInteractableObject>();
            DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", -1);
            heldObject = null;
            rbToTrack = null;
            rb = null;

            ToggleHandServerObject(serverHeldObject.name.Replace("(Clone)", ""), false);
        }
    }
    private void Throw()
    {
        animator.SetTrigger("Throwing");
        animatorSync.SetTrigger("Throwing");

        rb.freezeRotation = false;
        rb.useGravity = true;
        rbToTrack.AddForce(playerCamera.transform.forward * currentThrowStrength, ForceMode.Impulse);

        currentThrowStrength = 0;
        DynamicInteractableObject DIO = heldObject.GetComponent<DynamicInteractableObject>();
        DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", -1);
        heldObject.transform.parent = null;
        heldObject = null;
        rbToTrack = null;
        rb= null;

        ToggleHandServerObject(serverHeldObject.name.Replace("(Clone)", ""), false);
    }
    private void TryPickUp(GameObject pickedUp)
    {
     //   animator.SetTrigger("PickingUp");
     //   animatorSync.SetTrigger("PickingUp");

        DynamicInteractableObject DIO = pickedUp.GetComponent<DynamicInteractableObject>();

        Debug.Log("owned by " + DIO.GetCurrentlyOwnedByAvatar());
        if (DIO != null && DIO.GetCurrentlyOwnedByAvatar() == null)
        {
            heldObject = pickedUp;

            rb = heldObject.GetComponent<Rigidbody>();
            heldObject.transform.parent = clientHand.transform;
            heldObject.transform.rotation = Quaternion.Euler(0f, clientHand.transform.eulerAngles.y, 0f);

            rb.freezeRotation = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rbToTrack = heldObject.GetComponent<RigidbodySynchronizable>();
            DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", avatar.Owner.Index);
            Debug.Log("owned by " + DIO.GetCurrentlyOwnedByAvatar());

            ToggleHandServerObject(pickedUp.name.Replace("(Clone)", ""), true);

            //Instantiate(pickedUp, serverHand.transform);
            serverHeldObject.GetComponent<DynamicInteractableObject>().enabled = false;
            SetLayerRecursively(serverHeldObject, LayerMask.NameToLayer("SelfPlayerLayer"));
        }
        else
        {
            Debug.Log("You can't pick up that");
        }
    }
    private GameObject ToggleHandServerObject(string name, bool state)
    {
        GameObject found = null;
        foreach (GameObject child in allServerHandObjects)
        {
            if (child.name.Contains(name))
            {
                serverHeldObject = child;
                serverHeldObject.SetActive(state);
                break;
            }
        }
        return found;
    }
    private void UpdateHeldObjectPhysics()
    {
        if (heldObject != null)
        {
            Vector3 targetPosition = clientHand.transform.position + (playerCamera.transform.forward * heldObject.transform.localScale.x);

            heldObject.transform.DOMove(targetPosition, 0.1f);
            Quaternion targetRotation = clientHand.transform.rotation;
            heldObject.transform.DORotateQuaternion(targetRotation, 0.1f);

            rbToTrack.MovePosition(targetPosition);
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


    public void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if (interaction == InteractionEnum.ShotWithGun)
        {
            Gun gun = (Gun)caller;
            Debug.Log("Special Interaction Gun Player");
            Health health = gameObject.GetComponent<Health>();
            health.DamagePlayer(gun.Damage());
            Debug.Log(gun.Damage());
        }
    }
}



