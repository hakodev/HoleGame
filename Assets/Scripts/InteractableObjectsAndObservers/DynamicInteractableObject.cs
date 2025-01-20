using Alteruna;
using UnityEngine;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    protected Alteruna.Avatar currentlyOwnedByAvatar;

    public bool isPickedUp;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();

    RigidbodySynchronizable rbSyncDynamic;
    Rigidbody rbDynamic;

    [Header("removed serialize fields for speed. Nsync Objects is Here. x - when object is inactive(high number), y - when object is active(low number), also in the script both are 30 2, for ease of access")]
     Vector2 syncEveryNUpdates = new Vector2(30, 2);
     Vector2 fullSyncEveryNSyncs = new Vector2(30, 2);

    float timeSinceLastSignificantMovement = 0;
    [SynchronizableField] bool asleep = false;

    protected virtual void Awake()
    {
        rbSyncDynamic = GetComponent<RigidbodySynchronizable>();
        rbDynamic = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        BroadcastRemoteMethod(nameof(DynamicSleep));
    }
    protected virtual void Update()
    {
        SelfSleepIfUnmoving();
        CheckForMovement();
    }


    private void SelfSleepIfUnmoving()
    {
        if (asleep) { return; }
        if (RoleAssignment.playerID - 1 != Multiplayer.GetUser().Index) { return; }

        if (currentlyOwnedByAvatar==null)
        {
            if (rbDynamic.linearVelocity.magnitude < 0.1f)
            {
                timeSinceLastSignificantMovement += Time.deltaTime;
                if (timeSinceLastSignificantMovement > 1f)
                {
                    timeSinceLastSignificantMovement = 0;
                    BroadcastRemoteMethod(nameof(DynamicSleep));
                }
            }
        }
    }
    private void CheckForMovement()
    {
        if (!asleep) { return; }
        if (RoleAssignment.playerID - 1 != Multiplayer.GetUser().Index) { return; }


        if (rbDynamic.linearVelocity.magnitude >= 0.2f || currentlyOwnedByAvatar!=null)
        {
            timeSinceLastSignificantMovement = 0;
            BroadcastRemoteMethod(nameof(DynamicAwake));
        }
    }
    [SynchronizableMethod]
    public void DynamicSleep()
    {
        timeSinceLastSignificantMovement = 0;
        asleep = true;
       // rbSyncDynamic.SyncEveryNUpdates = 999;
      //  rbSyncDynamic.FullSyncEveryNSync = 999;
        // Debug.Log("sleep " + transform.root.gameObject.name);
    }
    [SynchronizableMethod]
    public void DynamicAwake()
    {
        asleep = false;
        //  Debug.Log(rbDynamic + " " + rbSyncDynamic);
     //   rbSyncDynamic.SyncEveryNUpdates = 1;
     //   rbSyncDynamic.FullSyncEveryNSync = 1;
    }
    
    public Alteruna.Avatar GetCurrentlyOwnedByAvatar()
    {
        return currentlyOwnedByAvatar;
    }
    [SynchronizableMethod]
    public void SetCurrentlyOwnedByAvatar(int newIndex)
    {
        if(newIndex!=-1)currentlyOwnedByAvatar = GetAvatarByOwnerIndex(newIndex);
        if (newIndex == -1) currentlyOwnedByAvatar = null;
    }

    public Alteruna.Avatar GetAvatarByOwnerIndex(int ownerIndex)
    {
        Alteruna.Avatar[] avatars = FindObjectsByType<Alteruna.Avatar>(FindObjectsSortMode.None);
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
