using DG.Tweening.Core.Enums;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeMachine : StationaryInteractableObject
{
    private CoffeeCupCollider cupCollider;
    private CoffeeDripObject coffeeDripObject;

    private void Start()
    {
        cupCollider = GetComponentInChildren<CoffeeCupCollider>();
        cupCollider.enabled = false;
        coffeeDripObject = GetComponentInChildren<CoffeeDripObject>();
    }
    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {

    }
    public override void Use()
    {
        //play sound
        coffeeDripObject.EnableDrip();
        cupCollider.enabled = true;
    }

    
}
