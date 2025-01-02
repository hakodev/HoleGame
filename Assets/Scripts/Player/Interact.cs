using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;
using Alteruna;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem.HID;

public class Interact : AttributesSync, IObserver
{
    private Alteruna.Avatar avatar;
    GameObject heldObject = null;

    [Header("not important")]
    [SerializeField] GameObject clientHand;
    [SerializeField] GameObject serverHand;

    [SerializeField] Camera playerCamera;
    PlayerController playerController;


    [Header("Designer Values")]
    [SerializeField] float grabReach;
    [SerializeField] float placeReach;
    [SerializeField] KeyCode primaryButton;
    [SerializeField] KeyCode interactButton;
    [SerializeField] Vector2 minMaxThrowStrength;
    [SerializeField] Vector2 minMaxThrowChargeUpTime;
    [SerializeField] float smoothingHeldObjectMovement;

    float currentChargeUpTime = 0;
    float currentThrowStrength = 0;
    LayerMask dynamicLayerMask;
    LayerMask stationaryLayerMask;
    LayerMask interactableLayerMask;
    bool finishedPickUp = true;
    bool isChargingUp = false;

    RigidbodySynchronizable rbToTrack;
    Rigidbody rb;
    AnimationSynchronizable animatorSync;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        playerController = GetComponent<PlayerController>();
        //animator = transform.Find("Animation").GetComponent<Animator>();
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
                //isChargingUp = false;
//                heldObject.GetComponent<Rigidbody>().useGravity = true;

                if (currentChargeUpTime > minMaxThrowChargeUpTime.x)
                {
                    if (currentChargeUpTime > minMaxThrowChargeUpTime.y) currentChargeUpTime = minMaxThrowChargeUpTime.y;
                    AnimateReleaseChargebar();
                    currentThrowStrength = Mathf.Lerp(minMaxThrowStrength.x, minMaxThrowStrength.y, currentChargeUpTime);
                    //currentChargeUpTime = 0;
                    Throw();
                }
                else
                {
                    Place();
                }
            }
            finishedPickUp = true;
            currentChargeUpTime = 0;
            isChargingUp = false;
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
            //placing anim
            Spam1();

            //specific to placing
            Vector3 bounds = GetRenderersSize(heldObject);
            Vector3 alignsBestWith = GetClosestAxis(hit.normal);

            Vector3 temp = Vector3.zero;
            temp.x = Mathf.Abs(bounds.x * alignsBestWith.normalized.x);
            temp.y = Mathf.Abs(bounds.y * alignsBestWith.normalized.y);
            temp.z = Mathf.Abs(bounds.z * alignsBestWith.normalized.z);

            Debug.Log("temp " + temp);

            heldObject.transform.position = hit.point + Vector3.Scale(hit.normal.normalized, temp)/8;
            rbToTrack.MovePosition(hit.point + Vector3.Scale(hit.normal.normalized, temp)/8);

            heldObject.transform.forward = -hit.normal;
            rbToTrack.SetRotation(heldObject.transform.rotation);


            if (heldObject.name.Contains("StickyNote"))
            {
                heldObject.transform.parent = hit.collider.transform;
                heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PlacedStickyNote, this);
            }

            Spam2();
            //Debug.Break();
        }
    }
    private void Throw()
    {
        //specifics to thtowing
        animatorSync.Animator.SetTrigger("Throwing");

        Spam1();

        //specifics t thowing
        rbToTrack.AddForce(playerCamera.transform.forward * currentThrowStrength, ForceMode.Impulse);
        currentThrowStrength = 0;
        if (heldObject.name.Contains("StickyNote")) heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.ThrownStickyNote, this);

        Spam2();
    }

    private Vector3 GetRenderersSize(GameObject obj)
    {
        Renderer[] temp = obj.GetComponentsInChildren<Renderer>();
        List<Renderer> renderers = new List<Renderer>();

        for(int i=0; i<temp.Length; i++)
        {
            if (temp[i]!=null)
            {
                renderers.Add(temp[i]);
            }
        }

        if (renderers.Count > 0)
        {
            Bounds combinedBounds = renderers[0].bounds;

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null) 
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }
            }
            return combinedBounds.size;
        }
        else
        {
            return Vector3.zero;
        }
    }
    Vector3 GetClosestAxis(Vector3 vector)
    {
        vector.Normalize();
        Vector3[] axes = { Vector3.right, Vector3.up, Vector3.forward,
                           -Vector3.right, -Vector3.up, -Vector3.forward };

        Vector3 closest = axes[0];
        float maxDot = Vector3.Dot(vector, axes[0]);

        for (int i = 1; i < axes.Length; i++)
        {
            float dot = Vector3.Dot(vector, axes[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = axes[i];
            }
        }

        return closest;
    }
    private void Spam1()
    {
        HandObjects.ToggleActive(heldObject.name.Replace("(Clone)", ""), false);
        //heldObject.transform.SetParent(GameObject.FindGameObjectWithTag("SceneParentForPlacedObjects").transform, true);
       // rb.linearVelocity = Vector3.zero;
       // rb.angularVelocity = Vector3.zero;
        rbToTrack.velocity = Vector3.zero;
        rb.freezeRotation = false;
        rb.useGravity = true;
        heldObject.transform.parent = GameObject.FindGameObjectWithTag("SceneParentForPlacedObjects").transform;
    }
    private void Spam2()
    {
        DynamicInteractableObject DIO = heldObject.GetComponent<DynamicInteractableObject>();
        DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", -1);

       // rbToTrack.enabled = true;
        heldObject = null;
        rbToTrack = null;
        rb = null;
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
            rbToTrack = heldObject.GetComponent<RigidbodySynchronizable>();

            if (heldObject.name.Contains("StickyNote")) heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PickedUpStickyNote, this);
            heldObject.transform.parent = clientHand.transform;
            heldObject.transform.rotation = Quaternion.Euler(0f, clientHand.transform.eulerAngles.y, 0f);

            rb.freezeRotation = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
           // rbToTrack.enabled = false;
            DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", avatar.Owner.Index);
            Debug.Log("owned by " + DIO.GetCurrentlyOwnedByAvatar());


            HandObjects.ToggleActive(heldObject.name.Replace("(Clone)", ""), true);
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
            Vector3 targetPosition = clientHand.transform.position;
            Quaternion targetRotation = playerCamera.transform.rotation;

           // rb.DOMove(targetPosition, smoothingHeldObjectMovement);
         //   heldObject.transform.DORotateQuaternion(targetRotation, smoothingHeldObjectMovement);

            rbToTrack.MovePosition(targetPosition);
            rbToTrack.SetRotation(targetRotation);
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



