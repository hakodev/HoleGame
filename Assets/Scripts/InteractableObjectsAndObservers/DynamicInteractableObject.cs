using UnityEngine;

public abstract class DynamicInteractableObject : MonoBehaviour, IObserver, IInteractableObject
{
    public abstract void SpecialInteraction(InteractionEnum interaction);
    public abstract void Use();
}
