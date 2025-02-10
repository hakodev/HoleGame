using Unity.VisualScripting;
using UnityEngine;

public class CrumpablePaper : DynamicInteractableObject
{
    public Collider sphereCollider;
    public Collider planeCollider;

    public GameObject crumple;
    public GameObject straight;

    private bool isCrumpled = false;

    AudioSource source;

    private new void Start()
    {
        base.Start();
        crumple.SetActive(false);
        straight.SetActive(true);
        sphereCollider.enabled = false;
        planeCollider.enabled = true;
    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
    }

    public override void Use()
    {
        BroadcastRemoteMethod(nameof(Crumple));
    }

    [SynchronizableMethod]
    private void Crumple()
    {
        if (!isCrumpled)
        {
            isCrumpled=true;
            PlayerAudioManager.Instance.PlaySound(gameObject, source, PlayerAudioManager.Instance.GetCrumple);
            sphereCollider.enabled = true;
            planeCollider.enabled = false;
            crumple.SetActive(true);
            straight.SetActive(false);
        }
    }
}
