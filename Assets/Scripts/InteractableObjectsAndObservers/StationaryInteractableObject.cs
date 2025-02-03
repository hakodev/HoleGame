using Alteruna;
using UnityEngine;

public abstract class StationaryInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    protected AudioSource source;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();
}
