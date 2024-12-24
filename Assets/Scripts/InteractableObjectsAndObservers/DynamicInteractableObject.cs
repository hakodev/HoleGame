using Alteruna;
using UnityEngine;
using Alteruna;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    public Alteruna.Avatar CurrentlyOwnedByAvatar { get; set; }
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();
}
