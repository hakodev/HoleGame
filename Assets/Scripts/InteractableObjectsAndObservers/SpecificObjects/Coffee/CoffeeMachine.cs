using Alteruna;
using UnityEngine;

public class CoffeeMachine : StationaryInteractableObject
{
    private CoffeeCupCollider cupCollider;
    private CoffeeDripObject coffeeDripObject;
    private Transform idealCoffeeCupSpot;

    private AudioSource[] sources;


    private void Start()
    {
        cupCollider = GetComponentInChildren<CoffeeCupCollider>();
        cupCollider.enabled = false;
        coffeeDripObject = GetComponentInChildren<CoffeeDripObject>();
        idealCoffeeCupSpot = transform.Find("IdealCoffeeCupSpot");

        sources = GetComponents<AudioSource>();
    }
    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {
        if(interaction == InteractionEnum.PlaceCupInCoffeeMachine)
        {
            Interact inter = (Interact)caller;
            Transform heldCup = inter.GetHeldObject().transform;

            heldCup.position = idealCoffeeCupSpot.position;
            heldCup.rotation = idealCoffeeCupSpot.rotation;
            RigidbodySynchronizable rbSync = heldCup.GetComponent<RigidbodySynchronizable>();
            rbSync.SetPosition(heldCup.position);
            rbSync.SetRotation(heldCup.rotation);

            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA + coffeeing");
        }
    }
    public override void Use()
    {
        BroadcastRemoteMethod(nameof(Pour));
    }

    [SynchronizableMethod]
    public void Pour()
    {
        if (coffeeDripObject.isDripping) return;

        foreach (var source in sources)
        {
            source.Play();
        }
        coffeeDripObject.EnableDrip();
        cupCollider.enabled = true;
    }

    
}
