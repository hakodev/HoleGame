using UnityEngine;

public class BasicPhysicsObject : DynamicInteractableObject
{
    Rigidbody rb;
    AudioSource source;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
    }

    public override void Use()
    {
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (isPickedUp) { return; }
        if(!RoleAssignment.hasGameStarted) { return; }

        if (rb.linearVelocity.magnitude > 1f)
        {
            if (rb.mass > 1)
            {
                PlayerAudioManager.Instance.PlaySound(this.gameObject, source, PlayerAudioManager.Instance.GetHeavyHit);
            }
            else
            {
                PlayerAudioManager.Instance.PlaySound(this.gameObject, source, PlayerAudioManager.Instance.GetLightHit);
            }
            //Debug.Log("bratle " + gameObject.name + " " + rb.linearVelocity.magnitude);
        }
    }

}
