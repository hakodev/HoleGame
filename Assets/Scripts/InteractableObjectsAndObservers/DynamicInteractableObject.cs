using Alteruna;
using UnityEngine;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    [SynchronizableField] Alteruna.Avatar currentlyOwnedByAvatar;

    public bool isPickedUp;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();

    RigidbodySynchronizable rbSync;
    Rigidbody rb;

    [Header("removed serialize fields for speed. Nsync Objects is Here. x - when object is inactive(high number), y - when object is active(low number), also in the script both are 30 2, for ease of access")]
     Vector2 syncEveryNUpdates = new Vector2(30, 2);
     Vector2 fullSyncEveryNSyncs = new Vector2(30, 2);


    private void Awake()
    {
        rbSync = GetComponent<RigidbodySynchronizable>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rbSync.SyncEveryNUpdates = (int)syncEveryNUpdates.x;
        rbSync.FullSyncEveryNSync = (int)fullSyncEveryNSyncs.x;
    }
      float timeSinceLastSignificantMovement = 0;

     private void Update()
      {
        SelfSleepIfUnmoving();
        CheckForMovement();
       }

    
    private void SelfSleepIfUnmoving()
    {
        if (transform.root.tag != "SelfPlayerLayer" && transform.root.tag != "Player")
        {
            if (rb.linearVelocity.magnitude < 0.02f)
            {
                timeSinceLastSignificantMovement += Time.deltaTime;
                if (timeSinceLastSignificantMovement > 5)
                {
                    rbSync.SyncEveryNUpdates = (int)syncEveryNUpdates.x;
                    rbSync.FullSyncEveryNSync = (int)fullSyncEveryNSyncs.x;
                }
            }
        }
    }
    private void CheckForMovement()
    {
        if (rb.linearVelocity.magnitude >= 0.02f)
        {
            timeSinceLastSignificantMovement = 0;
            rbSync.SyncEveryNUpdates = (int)syncEveryNUpdates.y;
            rbSync.FullSyncEveryNSync = (int)fullSyncEveryNSyncs.y;
        }
    }
    
    public Alteruna.Avatar GetCurrentlyOwnedByAvatar()
    {
        return currentlyOwnedByAvatar;
    }
    [SynchronizableMethod]
    public void SetCurrentlyOwnedByAvatar(int newIndex)
    {
        currentlyOwnedByAvatar = GetAvatarByOwnerIndex(newIndex);
        Debug.Log("SetCUrrentlyOwnedAvatar " + currentlyOwnedByAvatar); //important debug log
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
