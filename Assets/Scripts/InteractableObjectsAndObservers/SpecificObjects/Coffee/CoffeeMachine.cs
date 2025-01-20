using DG.Tweening.Core.Enums;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeMachine : StationaryInteractableObject
{
    private CoffeeCupCollider cupCollider;
    private CoffeeDripObject coffeeDripObject;

    private AudioSource[] sources;

    private void Start()
    {
        cupCollider = GetComponentInChildren<CoffeeCupCollider>();
        cupCollider.enabled = false;
        coffeeDripObject = GetComponentInChildren<CoffeeDripObject>();
        
        sources = GetComponents<AudioSource>();
    }
    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {

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
