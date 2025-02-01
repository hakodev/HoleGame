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
    [SynchronizableField]
    private int userID;

    List<Collider> allStickyColliders;
    Collider parentCollider;
    Transform parentedTo;
    public bool IsPoster { get; private set; } = false;


    public static bool currentlyDrawing=false;

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
            PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetSticky);
        }
        if (interaction == InteractionEnum.ThrownStickyNote)
        {
            isThrown = true;

        }
        if (interaction == InteractionEnum.PickedUpStickyNote)
        {
            isPlaced = false;
            originalPos = transform.position;
            BroadcastRemoteMethod(nameof(GnoreCollisions));
        }
        if(interaction == InteractionEnum.MarkerOnPosterOrStickyNote)
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

        if(!isPlaced)
        {
            if (collision.gameObject.layer == selfLayer) { return; }
            Debug.Log("krankenwagen" + collision.gameObject.name);// + " " + collision.transform.parent.name + " " + collision.transform.root.name);
            if (isThrown || isGameStart)
            {
                AlignWithSurface(collision);
                Stick();
                if (RoleAssignment.hasGameStarted) PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetSticky);
            }
        }  
    }

    public override void Use()
    {
        if (IsPoster) { return; }
        mousePainter = transform.root.GetComponentInChildren<MousePainter>();
        tempCamRef = transform.root.GetComponentInChildren<Camera>();

        if (!isInteractedWith)
        {
            currentlyDrawing = true;
            currentlyOwnedByAvatar.gameObject.GetComponent<PlayerController>().enabled = false;
            currentlyOwnedByAvatar.gameObject.GetComponentInChildren<CameraMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            originalPos = transform.position;
            BroadcastRemoteMethod(nameof(DrawPosition), transform.parent.parent.GetChild(1).position + transform.parent.parent.GetChild(1).forward * 0.4f, Multiplayer.GetUser().Index);
        }
        else
        {
            currentlyDrawing = false;
            currentlyOwnedByAvatar.gameObject.GetComponent<PlayerController>().enabled = true;
            currentlyOwnedByAvatar.gameObject.GetComponentInChildren<CameraMovement>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            BroadcastRemoteMethod(nameof(DrawPosition), originalPos, Multiplayer.GetUser().Index);
        }

    }

    [SynchronizableMethod]
    public void DrawPosition(Vector3 finalPos, int userId)
    {
        transform.position = finalPos;
        isInteractedWith = !isInteractedWith;
        userID = userId;
    }

    protected override void Update()
    {
        base.Update();
        if (isPlaced && transform.parent != null && !transform.parent.gameObject.name.Contains("Hand"))
        {
            StasisInPlace();
        }
        if(userID != Multiplayer.GetUser().Index) { return; }
        if (isInteractedWith && Input.GetMouseButton(0))
        {
            //Debug.Log("wa");
            mousePainter.Paint(tempCamRef);
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

        ResetMomentum();


        placedLocalRot = transform.localEulerAngles;
        placedLocalPos = transform.localPosition;


        isPlaced = true;
        isThrown = false;
        isGameStart = false;

        BroadcastRemoteMethod(nameof(ToggleRigidbody), false);

    }

    private void AlignWithSurface(Collision collision)
    {
        ResetMomentum();

        Vector3 point = Vector3.zero;
        Collider col = collision.collider;

        if (col != null && col.enabled)
        {
            point = col.ClosestPoint(transform.position);

            
            if (col is MeshCollider)
            {
                MeshCollider meshCol = (MeshCollider)col;
                if(!meshCol.convex)
                {
                    RaycastHit slaviOtTheClashers;
                    Physics.Raycast(transform.position, (col.transform.position - transform.position).normalized, out slaviOtTheClashers, 500);
                    Debug.DrawRay(transform.position, (col.transform.position - transform.position).normalized * 500);
                    Debug.Break();
                    point = slaviOtTheClashers.point;
                }   
            }
        }
        Debug.Log("kranken " + point + collision.gameObject.name);
        //its this, cant find point on player's controller collider

        Vector3 hitNormal = transform.position - point;
        Vector3 alignsBestWith = GetClosestAxis(hitNormal);
        Vector3 bounds = GetRenderersSize(gameObject);
        Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));

        //assign correct position and rotation
        gameObject.transform.forward = -hitNormal;
        rbToTrack.SetRotation(transform.rotation);

        if (isGameStart)
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 2.5f;
        }
        else
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 20f;
        }
        rbToTrack.SetPosition(transform.position);
        CheckRaycastForRigidbody();
    }
    private void CheckRaycastForRigidbody()
    {
        //Debug.DrawRay(transform.position, transform.forward * 500, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 500))
        {
            RigidbodySynchronizable potentialParentRBSync = hit.collider.gameObject.GetComponent<RigidbodySynchronizable>();
            if (potentialParentRBSync!=null)
            {
                Guid aaa = potentialParentRBSync.GetUID();
                BroadcastRemoteMethod(nameof(SyncSetParent), aaa);
            }
            else
            {
                TransformSynchronizable transSync = hit.collider.gameObject.GetComponent<TransformSynchronizable>();
                if (transSync != null)
                {
                    Guid aaa = transSync.GetUID();
                    Debug.Log("krankenwagen passed transform");
                    BroadcastRemoteMethod(nameof(SyncSetParent), aaa);
                }
                else
                {
                    //all of the walls, yes this code sucks ty
                    BroadcastRemoteMethod(nameof(SyncSetParentWalls));

                }
            }
        }

    }

    [SynchronizableMethod]
    private void SyncSetParent(Guid hitObjID)
    {
        parentedTo = Multiplayer.GetGameObjectById(hitObjID).transform;
        if (parentedTo != null) 
        {
            Rigidbody parentRB = parentedTo.GetComponent<Rigidbody>();

            transform.SetParent(parentedTo, true);
            parentCollider = parentedTo.GetComponent<Collider>();

            if (parentRB != null)
            {
                parentRB.linearVelocity = Vector3.zero;
                parentRB.angularVelocity = Vector3.zero;
            }
            else
            {
                if(parentedTo.gameObject.layer == 9 || parentedTo.gameObject.layer == 10)
                {
                    parentCollider = parentedTo.GetComponent<PlayerController>().HumanCollider;
                }
            }

            for (int j = 0; j < allStickyColliders.Count; j++)
            {
                Physics.IgnoreCollision(parentCollider, allStickyColliders[j]);
            }
        }
    }

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

    [SynchronizableMethod]
    private void GnoreCollisions()
    {
        if (parentCollider == null) { return; }
            foreach (Collider col in allStickyColliders)
            {
                Physics.IgnoreCollision(parentCollider, col, false);
            }

        parentCollider = null;
        //allStickyColliders.Clear();
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
      //  ResetMomentum();
        transform.localPosition = placedLocalPos;
        transform.localRotation = Quaternion.Euler(placedLocalRot);
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
