using UnityEngine;

public class Ball : DynamicInteractableObject
{

    PlayerController player;
    Rigidbody rb;
    [SerializeField] float extraBounce;
    [SerializeField] float kickStrength;

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
        PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetBouncyBall);
        TrampolinePlayer(collision);
    }
    private void TrampolinePlayer(Collision collision)
    {
        player = collision.collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
           if(player.transform.position.y-1f > transform.position.y) player.AddVerticalVelocity(extraBounce);

            Vector3 dir = (player.transform.position - transform.position).normalized;
            rb.AddForce(kickStrength * dir);
        }
    }
}
