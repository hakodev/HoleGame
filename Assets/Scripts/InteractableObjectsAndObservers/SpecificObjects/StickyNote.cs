using Alteruna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
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
            UltimatePain();
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
        if (collision.gameObject.layer == selfLayer) { return; }
        if (isThrown || isGameStart)
        {
            AlignWithSurface(collision);
            Stick();
            PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetSticky);
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
            Debug.Log("wa");
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
        MeshCollider stupidCol = collision.gameObject.GetComponent<MeshCollider>();
        if (col != null && col.enabled)
        {
            point = col.ClosestPoint(transform.position);
        }
        else
        {
            if (stupidCol != null && stupidCol.enabled)
            {
                point = (stupidCol).ClosestPoint(transform.position);
            }
            else
            {
                Debug.Log("Some other collider somehow " + stupidCol + stupidCol.enabled);
            }
        }

        Vector3 hitNormal = transform.position - point;
        Vector3 alignsBestWith = GetClosestAxis(hitNormal);
        Vector3 bounds = GetRenderersSize(gameObject);
        Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));


        //assign correct position and rotation
        gameObject.transform.forward = -hitNormal;
        rbToTrack.SetRotation(transform.rotation);

        if (isGameStart)
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 2f;
        }
        else
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 20f;
        }
        rbToTrack.SetPosition(transform.position);

        //  if(isGameStart) 
        UltimatePain();
        //BroadcastRemoteMethod(nameof(SyncSetParent), );
        //  SyncSetParent();
    }
    private void UltimatePain()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 500))
        {
            if(hit.collider.gameObject.GetComponent<Rigidbody>())
            {
                Debug.Log("ultimate pain " + hit.collider.gameObject.name);
                Guid aaa = hit.collider.GetComponent<RigidbodySynchronizable>().GetUID();
                BroadcastRemoteMethod(nameof(SyncSetParent), aaa);
            }
        }

    }

    [SynchronizableMethod]
    private void SyncSetParent(Guid hitObjID)
    {
        // RaycastHit hit;
        // if (Physics.Raycast(transform.position, transform.forward, out hit, 500)) // this damn raycast doesnt work for the other person 
             parentedTo = Multiplayer.GetGameObjectById(hitObjID).transform;
        Debug.Log("great 0 " + parentedTo.name);
        if (parentedTo != null) 
        {
            Rigidbody parentRB = parentedTo.GetComponent<Rigidbody>();
            Debug.Log("great1 " + parentedTo.name);
            Debug.Log("great2 " + parentedTo.name);
            Debug.Log("great3 " + parentRB);
            //if(parentRB==null) parentRB = parentedTo.GetComponentInChildren<Rigidbody>();
            if(parentRB!=null)
            {
                transform.SetParent(parentedTo, true);
                //parentedTo = hit.transform;

                Debug.Log("great4 " + parentRB);

                parentRB.linearVelocity = Vector3.zero;
                parentRB.angularVelocity = Vector3.zero;

                parentCollider = parentedTo.GetComponent<Collider>();
                Debug.Log("great5 " + parentCollider.gameObject.name);

                    for(int j=0; j< allStickyColliders.Count; j++)
                    {
                        Physics.IgnoreCollision(parentCollider, allStickyColliders[j]);
                        Debug.Log("great6 should be ignoring");
                    }
            }
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


    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
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
