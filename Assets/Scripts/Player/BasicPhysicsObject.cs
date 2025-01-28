using UnityEngine;

public class BasicPhysicsObject : DynamicInteractableObject
{
    private bool isBouncingBall;

    private void Start()
    {
        base.Start();
        if(gameObject.name.Contains("BeachBall"))
        {
            isBouncingBall = true;
        }
    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
    }

    public override void Use()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBouncingBall)
        {
            PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetBouncyBall);
        }
        else
        {
            base.OnCollisionEnter(collision);
        }
    }

}
