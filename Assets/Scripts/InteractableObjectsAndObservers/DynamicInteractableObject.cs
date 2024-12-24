using Alteruna;
using UnityEngine;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();
}
