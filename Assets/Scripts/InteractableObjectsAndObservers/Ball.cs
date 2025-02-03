using UnityEngine;

public class Ball : DynamicInteractableObject
{

    PlayerController player;
    Rigidbody rb;
    //[SerializeField] float extraBounce;
    // [SerializeField] float kickStrength;

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
        if(!collision.gameObject.CompareTag("Player") && rb.linearVelocity.magnitude > minVelocityToProduceSound) PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetBouncyBall);

        // && rb.
        //for the ball collision with player for kicking go to BeachBallBehavior
    }
}
