using Unity.VisualScripting;
using UnityEngine;

public class CrumpablePaper : DynamicInteractableObject
{
    public Collider sphereCollider;
    public Collider planeCollider;

    public GameObject crumple;
    public GameObject straight;

    private bool isCrumpled = false;

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
        planeCollider.enabled = false;
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
            PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetCrumple);
            crumple.SetActive(true);
            straight.SetActive(false);
            sphereCollider.enabled = true;
            planeCollider.enabled = false;
        }
    }
}
