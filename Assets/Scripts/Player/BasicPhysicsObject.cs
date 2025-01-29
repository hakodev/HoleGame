using UnityEngine;

public class BasicPhysicsObject : DynamicInteractableObject
{

    protected override void Start()
    {

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
    }

}
