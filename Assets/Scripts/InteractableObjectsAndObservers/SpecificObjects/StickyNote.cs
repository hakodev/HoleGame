using Alteruna;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StickyNote : DynamicInteractableObject
{
    Rigidbody rb;
    RigidbodySynchronizable rbToTrack;
    Transform positionForStickies;

    [SynchronizableField] public bool isPlaced = false;
    [SynchronizableField] bool isThrown = false;
    [SynchronizableField] bool isGameStart = true;
    [SynchronizableField] Vector3 placedLocalPos;
    [SynchronizableField] Vector3 placedLocalRot;
    int selfLayer;

    Vector3 finalPosition;
    Vector3 originalPos;
    public bool isInteractedWith = false;
    //disable object colliding with it's child
    //enalbe ticking to self player if it is thrown
    //make paper physics fall as if gliding
    //if object has a sticky note parent it behaves weirdly when thrown after being picked up

    private MousePainter mousePainter;
    private Camera tempCamRef;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rbToTrack = GetComponent<RigidbodySynchronizable>();
    }
    protected override void Start()
    {
        base.Start();
        selfLayer = LayerMask.NameToLayer("SelfPlayerLayer");
    }

    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if (interaction == InteractionEnum.PlacedStickyNote)
        {
            Stick();
        }
        if (interaction == InteractionEnum.ThrownStickyNote)
        {
            isThrown = true;

        }
        if (interaction == InteractionEnum.PickedUpStickyNote)
        {
            isPlaced = false;
            originalPos = transform.position;

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == selfLayer) { return; }
        if (isThrown || isGameStart)
        {
            AlignWithSurface(collision);
            Stick();
        }
    }

    public override void Use()
    {
        mousePainter = transform.root.GetComponentInChildren<MousePainter>();
        tempCamRef = transform.root.GetComponentInChildren<Camera>();

        if (!isInteractedWith)
        {
            transform.root.GetComponent<PlayerController>().enabled = false;
            transform.root.GetComponentInChildren<CameraMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            BroadcastRemoteMethod(nameof(DrawPosition), transform.parent.parent.GetChild(1).position + transform.parent.parent.GetChild(1).forward * 0.4f);


        }
        else
        {
            transform.root.GetComponent<PlayerController>().enabled = true;
            transform.root.GetComponentInChildren<CameraMovement>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            BroadcastRemoteMethod(nameof(DrawPosition), originalPos);
        }

    }

    [SynchronizableMethod]
    public void DrawPosition(Vector3 finalPos)
    {
        transform.position = finalPos;
        isInteractedWith = !isInteractedWith;
    }

    protected override void Update()
    {
        base.Update();
        if (isPlaced && transform.parent != null && !transform.parent.gameObject.name.Contains("Hand"))
        {
            StasisInPlace();
        }

        if (isInteractedWith && Input.GetMouseButton(0))
        {
            mousePainter.Paint();
        }
    }

    private void Stick()
    {
        //physics
        rb.useGravity = false;
        ResetMomentum();


        //essential for sticking
        placedLocalRot = transform.localEulerAngles;
        placedLocalPos = transform.localPosition;

        if (transform.parent != null)
        {
            if (transform.parent.gameObject.name.Contains("StickyNote"))
            {
                if (isGameStart)
                {
                    placedLocalPos = new Vector3(0, 0, 1);
                    placedLocalRot = new Vector3(0, 0, 0);
                    transform.localScale = Vector3.one;
                }
            }
        }
        //make sure sticky notes can stack when spawned without parenting causing issues



        //states of sticky note management
        isPlaced = true;
        isThrown = false;
        isGameStart = false;
    }
    private void AlignWithSurface(Collision collision)
    {
        ResetMomentum();


        //getting point variable from either a regular or a meshcollider because it caused issues
        //make sure mesh collider is always CONVEX  (otherwise it wouldnt detect a hit)
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

        //calc correct position
        Vector3 hitNormal = transform.position - point;
        Vector3 alignsBestWith = GetClosestAxis(hitNormal);
        Vector3 bounds = GetRenderersSize(gameObject);
        Vector3 temp = new Vector3(Mathf.Abs(bounds.x * alignsBestWith.normalized.x), Mathf.Abs(bounds.y * alignsBestWith.normalized.y), Mathf.Abs(bounds.z * alignsBestWith.normalized.z));


        //assign correct position and rotation
        gameObject.transform.forward = -hitNormal;
        rbToTrack.SetRotation(transform.rotation);

        if (isGameStart)
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 12f;
        }
        else
        {
            transform.position = point + Vector3.Scale(hitNormal.normalized, temp) / 12;
        }
        rbToTrack.SetPosition(transform.position);


        //i hate this line
        transform.SetParent(collision.transform, true);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
    }
    private void StasisInPlace()
    {
        ResetMomentum();

        transform.localPosition = placedLocalPos;
        rbToTrack.SetPosition(transform.position);

        transform.localRotation = Quaternion.Euler(placedLocalRot);
        rbToTrack.SetRotation(transform.rotation);
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
