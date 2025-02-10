using Alteruna;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StickyNote : DynamicInteractableObject
{
    Rigidbody rb;
    RigidbodySynchronizable rbToTrack;
    [SynchronizableField] public bool isPlaced = false;
    [SynchronizableField] bool isThrown = false;
    [SynchronizableField] bool isGameStart = true;
    [SynchronizableField] Vector3 placedLocalPos;
    [SynchronizableField] Vector3 placedLocalRot;
    int selfLayer;

    Vector3 finalPosition;
    Vector3 originalPos;
    [SynchronizableField] public bool isInteractedWith = false;
    //disable object colliding with it's child
    //enalbe ticking to self player if it is thrown
    //make paper physics fall as if gliding
    //if object has a sticky note parent it behaves weirdly when thrown after being picked up

    private MousePainter mousePainter;
    private Camera tempCamRef;
    [SynchronizableField] private int userID;

    List<Collider> allStickyColliders;
    Collider parentCollider;
    CharacterController playerParentCollider;
    Transform parentedTo;
    AudioSource source;

    StickyNote stackPseudoChild;
    [SynchronizableField] bool isStasis = false;

    public bool IsPoster { get; private set; } = false;


    public static bool currentlyDrawing = false;

    protected override void Awake()
    {
        currentlyDrawing = false;
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rbToTrack = GetComponent<RigidbodySynchronizable>();
        allStickyColliders = transform.GetComponentsInChildren<Collider>().ToList();
        allStickyColliders.Add(GetComponent<Collider>());

    }
    protected override void Start()
    {
        base.Start();
        selfLayer = LayerMask.NameToLayer("SelfPlayerLayer");
        if (gameObject.name.Contains("Poster")) IsPoster = true;
    }

    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if (interaction == InteractionEnum.PlacedStickyNote)
        {
            // BroadcastRemoteMethod(nameof(SyncSetParent));
            CheckRaycastForRigidbody();
            Stick();
            PlayerAudioManager.Instance.PlaySound(gameObject, source, PlayerAudioManager.Instance.GetSticky);
        }
        if (interaction == InteractionEnum.ThrownStickyNote)
        {
            isThrown = true;

        }
        if (interaction == InteractionEnum.PickedUpStickyNote)
        {
            isPlaced = false;
            originalPos = transform.position;
            parentRB = null;
            ChangeLayerIfStuckToPlayer(7);
            BroadcastRemoteMethod(nameof(GnoreCollisions));
            if(stackPseudoChild!=null) BroadcastRemoteMethod(nameof(F));
        }
        if (interaction == InteractionEnum.MarkerOnPosterOrStickyNote)
        {
            Collider boxy = GetComponent<Collider>();
            boxy.enabled = false;
        }
        if (interaction == InteractionEnum.StoppedMarkerOnPosterOrStickyNote)
        {
            Collider boxy = GetComponent<Collider>();
            boxy.enabled = true;
        }
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.layer == selfLayer) { return; }

        //if (!isPlaced) {
            if (isThrown || isGameStart)
            {
                AlignWithSurface(collision);
                Stick();
                PlayerAudioManager.Instance.PlaySound(gameObject, source, PlayerAudioManager.Instance.GetSticky);
            }
       // }

    }

    CursorToggle cursorToggle;
    public override void Use()
    {
        if (IsPoster) { return; }
        mousePainter = transform.root.GetComponentInChildren<MousePainter>();
        tempCamRef = transform.root.GetComponentInChildren<Camera>();
        cursorToggle = transform.root.GetComponent<CursorToggle>();

        if (!isInteractedWith)
        {
            currentlyDrawing = true;
            currentlyOwnedByAvatar.gameObject.GetComponent<PlayerController>().enabled = false;
            currentlyOwnedByAvatar.gameObject.GetComponentInChildren<CameraMovement>().enabled = false;
            cursorToggle.UICursorAndCam(true);
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;

            DrawPosition(transform.parent.parent.GetChild(1).position + transform.parent.parent.GetChild(1).forward * 0.4f, Multiplayer.GetUser().Index);
        }
        else
        {
            currentlyDrawing = false;
            currentlyOwnedByAvatar.gameObject.GetComponent<PlayerController>().enabled = true;
            currentlyOwnedByAvatar.gameObject.GetComponentInChildren<CameraMovement>().enabled = true;
            cursorToggle.UICursorAndCam(false);
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
            DrawPosition(originalPos, Multiplayer.GetUser().Index);
        }

    }

    [SynchronizableMethod] //minimizing data these take full name is StackFallDomino
    private void F()
    {
        if(stackPseudoChild!=null)
        {
            if(!IsPoster)
            {
                stackPseudoChild.ToggleRigidbody(true);
                stackPseudoChild.isStasis = false;

                if (stackPseudoChild.stackPseudoChild != null) stackPseudoChild.BroadcastRemoteMethod(nameof(stackPseudoChild.F));

                stackPseudoChild = null;
            }
        }
    }
    public void DrawPosition(Vector3 finalPos, int userId)
    {
        transform.position = finalPos;
        rbToTrack.MovePosition(finalPos);
        isInteractedWith = !isInteractedWith;
        userID = userId;
    }

    int updateCount;
    protected override void Update()
    {
        base.Update();

        if (userID != Multiplayer.GetUser().Index) { return; }
        if (isInteractedWith && Input.GetMouseButton(0))
        {
            mousePainter.Paint(tempCamRef);
        }
        else
        {
            originalPos = transform.position;
        }
    }
    private void FixedUpdate()
    {
        if (isPlaced && transform.parent != null && !transform.parent.gameObject.name.Contains("Hand"))
        {
            if(isStasis) StasisInPlace();
        }
    }

    /*private new void LateUpdate()
    {
        if (isPickedUp)
        {
            if (display == null || display.gameObject.transform.root != transform.root)
            {
                display = transform.root.GetComponentInChildren<HUDDisplay>();
            }
            display.SetState(new StickyNoteDisplay(display));
        }
    }*/



    private void Stick()
    {
        isStasis = true;

        ChangeLayerIfStuckToPlayer(10);
        ResetMomentum();

        placedLocalRot = transform.localEulerAngles;
        placedLocalPos = transform.localPosition;


        isPlaced = true;
        isThrown = false;
        isGameStart = false;

        //if(!isHitParentSticky)
        BroadcastRemoteMethod(nameof(ToggleRigidbody), false);
        Debug.Log("ball ");

    }
    private void ChangeLayerIfStuckToPlayer(int newLayer) //changes layer only for player it is stuck to
    {
        if (playerParentCollider == null) { return; }

        UserId parentAvatarUserIndex = (UserId)playerParentCollider.GetComponent<Alteruna.Avatar>().Owner.Index;
        //Debug.Log("grill " + parentAvatarUserIndex);
        InvokeRemoteMethod(nameof(ChangeChildrenLayers), parentAvatarUserIndex, newLayer); //removes sticky note on parent's side, enables it in trytopickup
    }

    [SynchronizableMethod]
    private void ChangeChildrenLayers(int newLayer)
    {
        List<GameObject> tempList = new List<GameObject>();
        tempList.Add(gameObject);
        GetChildRecursive(gameObject, tempList);
        foreach (GameObject child in tempList)
        {
            child.gameObject.layer = newLayer;
        }
        tempList.Clear();
    }

    private void GetChildRecursive(GameObject obj, List<GameObject> tempChildList)
    {
        if (null == obj) { return; }

        foreach (Transform child in obj.transform)
        {
            if (null == child) { continue; }
            tempChildList.Add(child.gameObject);
            GetChildRecursive(child.gameObject, tempChildList);
        }
    }


    private void AlignWithSurface(Collision collision)
    {
        ResetMomentum();

        Vector3 point = Vector3.zero;
        Collider col = collision.gameObject.GetComponent<Collider>();
        if (col != null && col.enabled)
        {
            point = col.ClosestPoint(transform.position);
        }


        Vector3 hitNormal = transform.position - point;
        Vector3 alignsBestWith = GetClosestAxis(hitNormal);
        Vector3 bounds = GetRenderersSize(gameObject);
        Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));

        //assign correct position and rotation
        gameObject.transform.forward = -hitNormal;
        rbToTrack.SetRotation(transform.rotation);

        float divider = 2f;
        if (isGameStart)
        {
            if(gameObject.name.Contains("Poster")) divider = 15f;
            if (gameObject.name.Contains("StickyNote")) divider = 2f;
        }
        else
        {
            if (gameObject.name.Contains("Poster")) divider = 15f;
            if (gameObject.name.Contains("StickyNote")) divider = 10f;
        }
        transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / divider;


        rbToTrack.SetPosition(transform.position);
        CheckRaycastForRigidbody();
    }
    private void CheckRaycastForRigidbody()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 500))
        {
            RigidbodySynchronizable potentialParentRBSync = hit.collider.gameObject.GetComponent<RigidbodySynchronizable>();

            StickyNote stickyNoteParent = null;
            if (hit.collider.transform.parent != null) stickyNoteParent = hit.collider.transform.parent.GetComponent<StickyNote>();
            if (stickyNoteParent == null) stickyNoteParent = hit.collider.GetComponent<StickyNote>();
            if (stickyNoteParent != null)
            {
                //Debug.Log("stack parent " + stickyNoteParent.gameObject);
                stickyNoteParent.BroadcastRemoteMethod(nameof(stickyNoteParent.SetStackPseudoChild), GetUID());
            }


            if (potentialParentRBSync != null)
            {
                if (stickyNoteParent==null)
                {
                    Guid aaa = potentialParentRBSync.GetUID();
                    BroadcastRemoteMethod(nameof(SyncSetParent), aaa);
                }
            }
            else
            {
                TransformSynchronizable transSync = hit.collider.gameObject.GetComponent<TransformSynchronizable>();
                if (transSync != null)
                {
                    Guid aaa = transSync.GetUID();
                    BroadcastRemoteMethod(nameof(SyncSetParent), aaa);
                }
                else
                {
                    //all of the walls, yes this code sucks ty
                //    BroadcastRemoteMethod(nameof(SyncSetParentWalls));

                }
            }
        }

    }

    [SynchronizableMethod]
    private void SetStackPseudoChild(Guid stickyGuid)
    {
        stackPseudoChild = (StickyNote) Multiplayer.GetComponentById(stickyGuid);
        //Debug.Log("stack " + stackPseudoChild);
    }

    Rigidbody parentRB;
    [SynchronizableMethod]
    private void SyncSetParent(Guid hitObjID)
    {
        parentedTo = Multiplayer.GetGameObjectById(hitObjID).transform;
        if (parentedTo != null)
        {
            parentRB = parentedTo.GetComponent<Rigidbody>();
            parentCollider = parentedTo.GetComponent<Collider>();

            transform.SetParent(parentedTo, true);

              //makes the game boring
              /*
            if (parentRB != null)
            {
                parentRB.linearVelocity = Vector3.zero;
                parentRB.angularVelocity = Vector3.zero;
            }
            */

            
            if(parentRB==null)
            {
                if (parentedTo.gameObject.layer == 9 || parentedTo.gameObject.layer == 10)
                {
                    playerParentCollider = parentedTo.GetComponent<CharacterController>();
                }
            }

            for (int j = 0; j < allStickyColliders.Count; j++)
            {
                if (playerParentCollider == null) Physics.IgnoreCollision(parentCollider, allStickyColliders[j]);
                if (playerParentCollider != null) Physics.IgnoreCollision(playerParentCollider, allStickyColliders[j]);
            }
            
        }
    }

    /*
    [SynchronizableMethod]
    private void SyncSetParentWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 500))
        {
            parentedTo = hit.collider.transform;
            transform.SetParent(parentedTo, true);
        }
    }
    */

    [SynchronizableMethod]
    private void GnoreCollisions()
    {
        if (parentCollider == null) { return; }
        for (int j = 0; j < allStickyColliders.Count; j++)
        {
            if (playerParentCollider == null) Physics.IgnoreCollision(parentCollider, allStickyColliders[j], false);
            if (playerParentCollider != null) Physics.IgnoreCollision(playerParentCollider, allStickyColliders[j], false);
        }
    

        parentCollider = null;
        playerParentCollider= null;
    }

    public static void AmendShaderLayeringInInteract(GameObject objectToApply)
    {
        StickyNote[] stickyNoteFound = objectToApply.GetComponentsInChildren<StickyNote>();
        if (stickyNoteFound == null) { return; }

        foreach (StickyNote s in stickyNoteFound)
        {
            Queue<GameObject> tempSticky = new Queue<GameObject>();
            tempSticky.Enqueue(s.gameObject);
            CustomMethods.SetLayerRecursively("DynamicInteractableObject", tempSticky);
        }
    }
    private void StasisInPlace()
    {
            transform.localPosition = placedLocalPos;
            transform.localRotation = Quaternion.Euler(placedLocalRot);
       
        //  ResetMomentum();
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

    private void ResetMomentum()
    {
        //helps avoid bs parenting physics glitches
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rbToTrack.velocity = Vector3.zero;
        rbToTrack.angularVelocity = Vector3.zero;
    }
}
