using UnityEngine;

public class Ball : DynamicInteractableObject
{
    protected override void Start()
    {
        base.Start();

    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
        if(interaction == InteractionEnum.kickBall)
        {

        }
        if(interaction == InteractionEnum.trampolineBall)
        {
         //  caller. verticalVelocity.y = Mathf.Sqrt(currentjumpHeight * -2f * gravity);
        }
    }

    public override void Use()
    {
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetBouncyBall);
    }
}
