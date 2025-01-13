using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;
using Alteruna;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem.HID;
using Unity.Burst.CompilerServices;
using System;

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
    //AnimationSynchronizable animatorSync;


    private Transform currentOutlinedObject;

    private DisappearingObjs disappearingObjs;


    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();

        if (!avatar.IsMe) { return; }
        playerController = GetComponent<PlayerController>();
        disappearingObjs = GetComponent<DisappearingObjs>();
        //animator = transform.Find("Animation").GetComponent<Animator>();
      //  animatorSync = transform.Find("Animation").GetComponent<AnimationSynchronizable>();
       // animatorSync.Animator = transform.Find("Animation").GetComponent<Animator>();
    }
  //  private void OnEnable()
 //   {
   //     if (!avatar.IsMe) { return; }
  //      animatorSync.Animator = transform.Find("Animation").GetComponent<Animator>();
  //  }
    private void Start()
    {
        if (!avatar.IsMe) {
            int playerLayer = LayerMask.NameToLayer("PlayerLayer");
            gameObject.layer = playerLayer;
            SetLayerRecursively(gameObject, playerLayer);
            return;
        }
        else
        {
            //animatorSync.Animator = transform.Find("Animation").GetComponent<Animator>();

            dynamicLayerMask = LayerMask.GetMask("DynamicInteractableObject");
            stationaryLayerMask = LayerMask.GetMask("StationaryInteractableObject");
            interactableLayerMask = dynamicLayerMask | stationaryLayerMask;

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
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (heldObject != null)
            {
                heldObject.GetComponent<DynamicInteractableObject>().Use();
            }

        }

        if (heldObject)
        {
            HUDDisplay.Instance.SetState(new CarryDisplay(HUDDisplay.Instance));
        }
        else
        {
            HUDDisplay.Instance.SetState(new EmptyDisplay(HUDDisplay.Instance));
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity, interactableLayerMask))
        {
            ApplyOutline(hit.transform.gameObject);


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StationaryInteractableObject"))
            {
                HUDDisplay.Instance.SetState(new StationaryInteract(HUDDisplay.Instance));
                if (Input.GetMouseButtonDown(1))
                {
                    hit.transform.gameObject.GetComponent<StationaryInteractableObject>().Use();
                }

            }
 
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("DynamicInteractableObject"))
            {
                HUDDisplay.Instance.SetState(new DynamicInteract(HUDDisplay.Instance));
                if (Input.GetMouseButtonDown(0))
                {
                    TryPickUp(hit.transform.gameObject);
                    finishedPickUp = false;
                }
            }
        }
        else
        {
            ApplyOutline(null);
        }
    }


    private void ApplyOutline(GameObject objectToApply)
    {
        List<GameObject> tempChildList = new List<GameObject>();
        if (objectToApply == currentOutlinedObject)
        {
            return;
        }
        if (currentOutlinedObject != null && objectToApply != currentOutlinedObject)
        {
            ChangeChildrenLayers("Default", tempChildList);
        }
        
        if(objectToApply == null) return;

        currentOutlinedObject = objectToApply.transform;

        ChangeChildrenLayers("OutlineLayer", tempChildList);
    }
    
    private void ChangeChildrenLayers(string layerName, List<GameObject> tempChildList)
    {
        GetChildRecursive(currentOutlinedObject.gameObject, tempChildList);
        foreach (GameObject child in tempChildList)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        tempChildList.Clear();

    }

    private void GetChildRecursive(GameObject obj, List<GameObject> tempChildList)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            tempChildList.Add(child.gameObject);
            GetChildRecursive(child.gameObject, tempChildList);
        }
    }



    public bool GetHeldObjectDroppedOrThrown() {
        return heldObject == null;
    }

    private void Place()
    {
        SetLayerRecursively(heldObject, 11);
        LayerMask everythingButHeldObject = ~(1 << 11 | 10);

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, placeReach, everythingButHeldObject, QueryTriggerInteraction.Ignore))
        {
            SetLayerRecursively(heldObject, 7);

            //placing anim
            Spam1();

            //specific to placing
            Vector3 bounds = GetRenderersSize(heldObject);
            Vector3 alignsBestWith = GetClosestAxis(hit.normal);
            Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));


            float divider = 2;
            if (heldObject.gameObject.name.Contains("StickyNote")) divider = 20;
            heldObject.transform.position = hit.point + Vector3.Scale(hit.normal.normalized, temp) / divider;
            rbToTrack.SetPosition(heldObject.transform.position);

            heldObject.transform.forward = hit.normal;
            rbToTrack.SetRotation(heldObject.transform.rotation);


            if (heldObject.name.Contains("StickyNote"))
            {
                //heldObject.transform.parent = hit.collider.transform;
                heldObject.transform.SetParent(hit.collider.transform, true);

                heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PlacedStickyNote, this);
            }

            Spam2();
            //Debug.Break();
        }
        else
        {
            SetLayerRecursively(heldObject, 7);
        }
    }
    private void Throw()
    {
        //specifics to thtowing

        Spam1();

        //specifics t thowing
        // animatorSync.Animator.SetTrigger("Throwing");
        rbToTrack.AddForce(playerCamera.transform.forward * currentThrowStrength, ForceMode.Impulse);
        Debug.Log((playerCamera.transform.forward * currentThrowStrength).normalized);
        currentThrowStrength = 0;
        if (heldObject.name.Contains("StickyNote")) heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.ThrownStickyNote, this);
        if (heldObject.GetComponent<CoffeeCup>())
        {
            heldObject.GetComponent<CoffeeCup>().SpecialInteraction(InteractionEnum.CoffeeStain, this);
        }

        Debug.DrawRay(heldObject.transform.position, rbToTrack.velocity, Color.magenta);
        Debug.DrawRay(heldObject.transform.position, rb.angularVelocity, Color.green);

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

        heldObject.transform.SetParent(GameObject.FindGameObjectWithTag("SceneParentForPlacedObjects").transform, true);
        ResetMomentum();

        rb.freezeRotation = false;
        rb.useGravity = true;


    }
    private void Spam2()
    {
        //disappearingObjs.CheckIfPlayerHasDisappearingObjectsSymptom(heldObject);

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
            //get all necessary variales
            heldObject = pickedUp;
            rb = heldObject.GetComponent<Rigidbody>();
            rbToTrack = heldObject.GetComponent<RigidbodySynchronizable>();

            if (heldObject.name.Contains("StickyNote")) heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PickedUpStickyNote, this);

            //reset physics
            rb.freezeRotation = true;
            rb.useGravity = false;
            ResetMomentum();

            heldObject.transform.SetParent(clientHand.transform, true);

            //actually move
            UpdateHeldObjectPhysics();

            DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", avatar.Owner.Index);
            Debug.Log("owned by " + DIO.GetCurrentlyOwnedByAvatar());
            HandObjects.ToggleActive(heldObject.name.Replace("(Clone)", ""), true);
        }
        else
        {
            Debug.Log("You can't pick up that");
        }
    }
    private void ResetMomentum()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rbToTrack.velocity = Vector3.zero;
        rbToTrack.angularVelocity = Vector3.zero;
    }
    private void UpdateHeldObjectPhysics()
    {
        if (heldObject != null)
        {
            Vector3 targetPosition = clientHand.transform.position;
            Quaternion targetRotation = playerCamera.transform.rotation;

            heldObject.transform.position = targetPosition;
            heldObject.transform.rotation = targetRotation;

            rbToTrack.SetPosition(targetPosition);
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



