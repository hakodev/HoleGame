using UnityEngine;

public class Gun : DynamicInteractableObject
{
    public override void SpecialInteraction(InteractionEnum interaction)
    {

    }
    public override void Use()
    {
        Debug.Log("SHOOOOOOT");
    }
}
