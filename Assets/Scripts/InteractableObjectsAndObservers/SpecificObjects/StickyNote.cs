using Alteruna;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StickyNote : DynamicInteractableObject
{
    Rigidbody rb;
    [SynchronizableField] public bool isPlaced = false;
    [SynchronizableField] bool isThrown = false;
    [SynchronizableField] bool isGameStart = true;
    Vector3 placedLocalPos;
    Vector3 placedLocalRot;
    int selfLayer;

    Vector3 originalPos;
    [SynchronizableField] public bool isInteractedWith = false;

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
        base.Awake();
        rb = GetComponent<Rigidbody>();
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
            BroadcastRemoteMethod(nameof(SyncSetParent));
            BroadcastRemoteMethod(nameof(Stick));
            //Stick();
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
        if (collision.gameObject.layer == selfLayer) { return; }

        if (isThrown || isGameStart)
        {
            AlignWithSurface(collision);
            //Stick();
            BroadcastRemoteMethod(nameof(Stick));
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
            transform.root.GetComponent<PlayerController>().enabled = false;
            transform.root.GetComponentInChildren<CameraMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            BroadcastRemoteMethod(nameof(DrawPosition), transform.parent.parent.GetChild(1).position + transform.parent.parent.GetChild(1).forward * 0.4f, Multiplayer.GetUser().Index);
        }
        else
        {
            currentlyDrawing = false;
            transform.root.GetComponent<PlayerController>().enabled = true;
            transform.root.GetComponentInChildren<CameraMovement>().enabled = true;
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
            Debug.Log("wa");
            mousePainter.Paint(tempCamRef);
        }
    }

    [SynchronizableMethod]
    private void Stick()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        ResetMomentum();


        placedLocalRot = transform.localEulerAngles;
        placedLocalPos = transform.localPosition;


        isPlaced = true;
        isThrown = false;
        isGameStart = false;
    }

    private void AlignWithSurface(Collision collision)
    {
        ResetMomentum();

        Vector3 point = Vector3.zero;
        Collider col = collision.gameObject.GetComponent<Collider>();
        if (col != null && col.enabled) point = col.ClosestPoint(transform.position);
        parentCollider = col;


        Vector3 hitNormal = transform.position - point;
        Vector3 alignsBestWith = GetClosestAxis(hitNormal);
        Vector3 bounds = GetRenderersSize(gameObject);
        Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));
        gameObject.transform.forward = -hitNormal;

        if (isGameStart)
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 2f;
        }
        else
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 20f;
        }
        BroadcastRemoteMethod(nameof(SyncSetParent));
    }


    [SynchronizableMethod]
    private void SyncSetParent()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1))
        {
            transform.SetParent(hit.transform, true);
            parentedTo = hit.transform;
            parentCollider = hit.collider;

            Rigidbody parentRB = hit.collider.transform.GetComponent<Rigidbody>();
            if(parentRB==null) parentRB = hit.collider.transform.GetComponentInChildren<Rigidbody>();
            if(parentRB!=null)
            {
                parentRB.linearVelocity = Vector3.zero;
                parentRB.angularVelocity = Vector3.zero;


                for (int j = 0; j < allStickyColliders.Count; j++)
                {
                    Physics.IgnoreCollision(parentCollider, allStickyColliders[j], true);
                }
            }
        }
    }

    [SynchronizableMethod]
    private void GnoreCollisions()
    {
        foreach (Collider col in allStickyColliders)
        {
            Physics.IgnoreCollision(parentCollider, col, false);
        }
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
    }
}
