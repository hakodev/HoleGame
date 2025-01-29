using Alteruna;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class Interact : AttributesSync, IObserver
{
    private Alteruna.Avatar avatar;
    GameObject heldObject = null;

    [Header("not important")]
    [SerializeField] GameObject clientHand;

    [SerializeField] Camera playerCamera;
    PlayerController playerController;
    private HUDDisplay hudDisplay;


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
    private Alteruna.Spawner spawner;

    //AnimationSynchronizable animatorSync;


    private Transform currentOutlinedObject;
    GameObject pickedUp;
    DynamicInteractableObject DIO;


    private void Awake()
    {
        hudDisplay = GetComponentInChildren<HUDDisplay>();
        avatar = GetComponent<Alteruna.Avatar>();
        spawner = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Alteruna.Spawner>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (!avatar.IsMe)
        {
            int playerLayer = LayerMask.NameToLayer("PlayerLayer");
            gameObject.layer = playerLayer;
            SetLayerRecursively(gameObject, playerLayer);
            return;
        }
        else
        {
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
        if (isChargingUp) currentChargeUpTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        UpdateHeldObjectPhysics();
    }

    private void ProcessInput()
    {    
        if (Input.GetMouseButtonUp(0) && heldObject != null)
        {
            if (finishedPickUp && !StickyNote.currentlyDrawing)
            {
                if (currentChargeUpTime > minMaxThrowChargeUpTime.x)
                {
                    if (currentChargeUpTime > minMaxThrowChargeUpTime.y) currentChargeUpTime = minMaxThrowChargeUpTime.y;
                    currentThrowStrength = Mathf.Lerp(minMaxThrowStrength.x, minMaxThrowStrength.y, currentChargeUpTime);
                   // BroadcastRemoteMethod(nameof(Throw));
                    Throw();
                }
                else
                {
                    //BroadcastRemoteMethod(nameof(Place));
                    Place();
                }
            }
            finishedPickUp = true;
            currentChargeUpTime = 0;
            isChargingUp = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null && finishedPickUp && !StickyNote.currentlyDrawing)
            {
                isChargingUp = true;
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
            if (DIO is StickyNote)
            {
                hudDisplay.SetState(new StickyNoteDisplay(hudDisplay));
            }
            else if (DIO is Marker)
            {
                hudDisplay.SetState(new MarkerDisplay(hudDisplay));
            }
            else
            {
                hudDisplay.SetState(new CarryDisplay(hudDisplay));
            }

        }
        else
        {
            hudDisplay.SetState(new EmptyDisplay(hudDisplay));
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, Mathf.Infinity, interactableLayerMask))
        {
            ApplyOutline(hit.transform.gameObject);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StationaryInteractableObject"))
            {
                hudDisplay.SetState(new StationaryInteract(hudDisplay));
                if (Input.GetMouseButtonDown(1))
                {
                    hit.transform.gameObject.GetComponent<StationaryInteractableObject>().Use();
                }

            }


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("DynamicInteractableObject"))
            {
                if (DIO != null && DIO is StickyNote) return;
                hudDisplay.SetState(new DynamicInteract(hudDisplay));
                if (Input.GetMouseButtonDown(0))
                {
                    pickedUp = hit.transform.gameObject;
                    // BroadcastRemoteMethod(nameof(TryPickUp));
                    TryPickUp();

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
            StickyNote.AmendShaderLayeringInInteract(currentOutlinedObject.gameObject);

        }

        if (objectToApply == null) return;

        currentOutlinedObject = objectToApply.transform;

        ChangeChildrenLayers("OutlineLayer", tempChildList);
        StickyNote.AmendShaderLayeringInInteract(objectToApply);
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
            tempChildList.Add(child.gameObject);
            GetChildRecursive(child.gameObject, tempChildList);
        }
    }



    public bool GetHeldObjectDroppedOrThrown()
    {
        return heldObject == null;
    }

    private void Place()
    {
        SetLayerRecursively(heldObject, 11);
        LayerMask everythingButHeldObject = ~(1 << 11 | 10);

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, placeReach, everythingButHeldObject, QueryTriggerInteraction.Ignore))
        {
            heldObject.GetComponent<DynamicInteractableObject>().isPickedUp = false;
            SetLayerRecursively(heldObject, 7);

            //placing anim
            PrepareForDropping();

            //specific to placing
            Vector3 bounds = GetRenderersSize(heldObject);
            Vector3 alignsBestWith = GetClosestAxis(hit.normal);
            Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));


            float divider = 2;
            if (heldObject.gameObject.name.Contains("StickyNote") || heldObject.name.Contains("Poster")) divider = 20;
            heldObject.transform.position = hit.point + Vector3.Scale(hit.normal.normalized, temp) / divider;
            rbToTrack.SetPosition(heldObject.transform.position);

            heldObject.transform.forward = -hit.normal;
            rbToTrack.SetRotation(heldObject.transform.rotation);


            if (heldObject.name.Contains("StickyNote") || heldObject.name.Contains("Poster"))
            {
                heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PlacedStickyNote, this);
            }
            Transform hitRoot = hit.collider.transform.root;
            if (hitRoot.name.Contains("CoffeeMachine"))
            {
                hitRoot.GetComponent<CoffeeMachine>().SpecialInteraction(InteractionEnum.PlaceCupInCoffeeMachine, this);
            }

            FinishDropping();
        }
        else
        {
            SetLayerRecursively(heldObject, 7);
        }
    }
    private void Throw()
    {
        //specifics to thtowing
        if (!avatar.IsMe) return;
        PrepareForDropping();
        heldObject.GetComponent<DynamicInteractableObject>().isPickedUp = false;
        PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetThrowAudio);

        //specifics t thowing
        // animatorSync.Animator.SetTrigger("Throwing");
        rbToTrack.AddForce(playerCamera.transform.forward * currentThrowStrength, ForceMode.Impulse);
        currentThrowStrength = 0;
        if (heldObject.name.Contains("StickyNote") || heldObject.name.Contains("Poster")) heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.ThrownStickyNote, this);
        if (heldObject.GetComponent<CoffeeCup>())
        {
            heldObject.GetComponent<CoffeeCup>().SpecialInteraction(InteractionEnum.CoffeeStain, this);
        }
        FinishDropping();
    }

    private Vector3 GetRenderersSize(GameObject obj)
    {
        Renderer[] temp = obj.GetComponentsInChildren<Renderer>();
        List<Renderer> renderers = new List<Renderer>();

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != null)
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
    private void PrepareForDropping()
    {
        HandObjects.ToggleActive(heldObject.name.Replace("(Clone)", ""), false);

        DIO = heldObject.GetComponent<DynamicInteractableObject>();
        DIO.BroadcastRemoteMethod("DynamicAwake");


        heldObject.transform.SetParent(null);
        ResetMomentum();

        rb.useGravity = true;
        rb.freezeRotation = false;
        //DIO.BroadcastRemoteMethod(nameof(DIO.ToggleRigidbodyGravity), true);
       // DIO.BroadcastRemoteMethod(nameof(DIO.ToggleCollider), true);
    }
    private void FinishDropping()
    {
        // Is the despawning item symptom on and is the dropper a machine?
        //if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[0] &&
        //  gameObject.GetComponent<PlayerRole>().GetRole() == Roles.Machine)
        //{
        //    DespawningItems.DespawnItem(heldObject);
        //    StartCoroutine(DespawningItems.DestroyItem(heldObject));
        //}

        DIO = heldObject.GetComponent<DynamicInteractableObject>();
        DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", -1);

        rbToTrack.enabled = true;
        heldObject = null;
        rbToTrack = null;
        rb = null;
        DIO = null;
    }

    private void TryPickUp()
    {
        if (!avatar.IsMe) return;
        if (heldObject != null) { return; }
        finishedPickUp = false;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2)), out hit, grabReach, interactableLayerMask) || pickedUp == spawnedGun)
        {
            PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetPickUp);
            DIO = pickedUp.GetComponent<DynamicInteractableObject>();
            if (DIO != null && DIO.GetCurrentlyOwnedByAvatar() == null)
            {
                heldObject = pickedUp;
                rb = heldObject.GetComponent<Rigidbody>();
                rbToTrack = heldObject.GetComponent<RigidbodySynchronizable>();
                DIO.isPickedUp = true;

                if (heldObject.name.Contains("StickyNote") || heldObject.name.Contains("Poster"))
                {
                    heldObject.GetComponent<StickyNote>().SpecialInteraction(InteractionEnum.PickedUpStickyNote, this);
                }
                else
                {
                //    DIO.BroadcastRemoteMethod(nameof(DIO.ToggleCollider), false);
                }

                //reset physics
                rb.useGravity = false;
                rb.freezeRotation = true;
                //DIO.BroadcastRemoteMethod(nameof(DIO.ToggleRigidbodyGravity), false);
                ResetMomentum();

                heldObject.transform.SetParent(clientHand.transform, true);

                //actually move
                UpdateHeldObjectPhysics();

                DIO.BroadcastRemoteMethod("SetCurrentlyOwnedByAvatar", avatar.Owner.Index);
                DIO.BroadcastRemoteMethod("DynamicAwake");
            }
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
            if (heldObject.name.Contains("StickyNote") || heldObject.name.Contains("Poster"))
            {
                if (heldObject.GetComponent<StickyNote>().isInteractedWith)
                {
                    return;
                }

            }
            Vector3 targetPosition = clientHand.transform.position;
            Quaternion targetRotation = playerCamera.transform.rotation;

            heldObject.transform.position = targetPosition;
            heldObject.transform.rotation = targetRotation;

            rbToTrack.SetPosition(targetPosition);
            rbToTrack.SetRotation(targetRotation);
        }
    }

    private void Drop()
    {
        PrepareForDropping();
        FinishDropping();
    }

    GameObject spawnedGun;
    public void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if (interaction == InteractionEnum.ShotWithGun)
        {
            Gun gun = (Gun)caller;
            Health health = gameObject.GetComponent<Health>();
            health.DamagePlayer(gun.Damage());
        }
        if (interaction == InteractionEnum.RemoveGun)
        {
            if (spawnedGun != null && avatar.IsMe)
            {
                if (heldObject == spawnedGun) Drop();
            }
        }
        if (interaction == InteractionEnum.GivenTaskManagerRole)
        {
            if (heldObject != null) Drop();
            spawnedGun = spawner.Spawn(0, transform.position, Quaternion.identity);
            pickedUp = spawnedGun;

            //BroadcastRemoteMethod(nameof(TryPickUp));
            TryPickUp();
        }
    }
    public GameObject GetHeldObject()
    {
        return heldObject;
    }
    public float GetGrabReach()
    {
        return grabReach;
    }
}



