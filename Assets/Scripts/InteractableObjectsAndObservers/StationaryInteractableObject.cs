using Alteruna;
using UnityEngine;

public abstract class StationaryInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();
}
