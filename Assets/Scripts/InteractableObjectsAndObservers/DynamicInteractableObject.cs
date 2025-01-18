using Alteruna;
using UnityEngine;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    [SynchronizableField] Alteruna.Avatar currentlyOwnedByAvatar;

    public bool isPickedUp;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();


    public Alteruna.Avatar GetCurrentlyOwnedByAvatar()
    {
        return currentlyOwnedByAvatar;
    }
    [SynchronizableMethod] public void SetCurrentlyOwnedByAvatar(int newIndex)
    {
        currentlyOwnedByAvatar = GetAvatarByOwnerIndex(newIndex);
        Debug.Log("SetCUrrentlyOwnedAvatar " + currentlyOwnedByAvatar); //important debug log
    }

    public Alteruna.Avatar GetAvatarByOwnerIndex(int ownerIndex)
    {
        Alteruna.Avatar[] avatars = FindObjectsOfType<Alteruna.Avatar>();
        foreach (Alteruna.Avatar avatar in avatars)
        {
            if (avatar.Owner.Index == ownerIndex)
            {
                return avatar;
            }
        }
        return null;
    }
}
