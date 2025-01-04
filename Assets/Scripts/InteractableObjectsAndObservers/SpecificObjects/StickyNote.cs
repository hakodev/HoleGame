using Alteruna;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StickyNote : DynamicInteractableObject
{
    Rigidbody rb;
    RigidbodySynchronizable rbToTrack;
    bool isPlaced=false;
    bool isThrown = false;
    bool isGameStart = true;
    Vector3 placedLocalPos;
    Vector3 placedLocalRot;
    int selfLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rbToTrack = GetComponent<RigidbodySynchronizable>();
    }
    private void Start()
    {
        selfLayer = LayerMask.NameToLayer("SelfPlayerLayer");
    }
    //if thrown and hit a surface
    //if placed specifically
    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if(interaction == InteractionEnum.PlacedStickyNote)
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
        //    isThrown = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == selfLayer) { return; }
        if (isThrown || isGameStart)
        {
            AlignWithSurface(collision);
            Stick();
        }
    }

    public override void Use()
    {

    }
    private void Update()
    {
        if(isPlaced)
        {
            StasisInPlace();
        }
    }
    private void Stick()
    {
        isPlaced = true;
        isThrown = false;
        isGameStart = false;

        rb.useGravity = false;
        placedLocalPos = transform.localPosition;
        placedLocalRot = transform.localEulerAngles;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    private void AlignWithSurface(Collision collision)
    {
        //collider only works for rigidbodies for some reason, doesnt detect floor -> make sure collider is always CONVEX
        Vector3 point = collision.gameObject.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 hitNormal = point - transform.position;
        Vector3 bounds = GetRenderersSize(gameObject);


        Vector3 alignsBestWith = GetClosestAxis(hitNormal);

        Vector3 temp = Vector3.zero;
        temp.x = Mathf.Abs(bounds.x * alignsBestWith.normalized.x);
        temp.y = Mathf.Abs(bounds.y * alignsBestWith.normalized.y);
        temp.z = Mathf.Abs(bounds.z * alignsBestWith.normalized.z);

        gameObject.transform.position = point - Vector3.Scale(hitNormal, temp)/4;
        rbToTrack.MovePosition(point - Vector3.Scale(hitNormal, temp) / 4);

       // gameObject.transform.position = point;
      //  rbToTrack.MovePosition(point);

        gameObject.transform.forward = -hitNormal;
        rbToTrack.SetRotation(transform.rotation);

        transform.parent = collision.transform;
    }
    private void StasisInPlace()
    {
        //   rb.linearVelocity = Vector3.zero;
        //   rb.angularVelocity = Vector3.zero;
        transform.localPosition = placedLocalPos;
        transform.localRotation = Quaternion.Euler(placedLocalRot);
        rbToTrack.MovePosition(placedLocalPos + transform.parent.position);
        rbToTrack.SetRotation(Quaternion.Euler(placedLocalRot) * transform.parent.rotation);
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
}
