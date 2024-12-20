using UnityEngine;

public abstract class StationaryInteractableObject : MonoBehaviour, IObserver, IInteractableObject
{
    public abstract void SpecialInteraction(InteractionEnum interaction);
    public abstract void Use();
}
