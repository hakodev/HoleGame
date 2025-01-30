using Alteruna;
using UnityEngine;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    protected Alteruna.Avatar currentlyOwnedByAvatar;
    protected CharacterController ownedCharacterController;

    [SynchronizableField] public bool isPickedUp;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();

    RigidbodySynchronizable rbSyncDynamic;
    Rigidbody rbDynamic;
    Collider colliderDynamic;

    [Header("removed serialize fields for speed. Nsync Objects is Here. x - when object is inactive(high number), y - when object is active(low number), also in the script both are 30 2, for ease of access")]
     Vector2 syncEveryNUpdates = new Vector2(30, 2);
     Vector2 fullSyncEveryNSyncs = new Vector2(30, 2);

    [SynchronizableField]float timeSinceLastSignificantMovement = 0;

    protected virtual void Awake()
    {
        rbSyncDynamic = GetComponent<RigidbodySynchronizable>();
        rbDynamic = GetComponent<Rigidbody>();
        colliderDynamic = GetComponent<Collider>();
    }
    protected virtual void Start()
    {
        BroadcastRemoteMethod(nameof(DynamicSleep));
    }
    protected virtual void Update()
    {
        //SelfSleepIfUnmoving();
        //CheckForMovement();
    }
    [SynchronizableMethod]
    public void ToggleRigidbody(bool newstate)
    {
        rbDynamic.useGravity = newstate;
        rbDynamic.freezeRotation = !newstate;
    }
    [SynchronizableMethod]
    public void ToggleIgnoreCollisionsWithOwner(bool newState)
    {
        StickyNote isSticky = GetComponent<StickyNote>();

        if (newState)
        {
            if (currentlyOwnedByAvatar != null)
            {
                ownedCharacterController =currentlyOwnedByAvatar.gameObject.GetComponent<CharacterController>();
                if (ownedCharacterController == null || isSticky!=null) { return; }
                Physics.IgnoreCollision(colliderDynamic, ownedCharacterController, true);
            }
        }
        else
        {
            if (ownedCharacterController == null || isSticky!=null) { return; }
            Physics.IgnoreCollision(colliderDynamic, ownedCharacterController, false);
            ownedCharacterController = null;
        }      
    }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (isPickedUp) return;

            if(rbDynamic.mass > 1)
            {
               // PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetHeavyHit);
            }
            else
            {
             //   PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetLightHit);
            }
        }

    private void SelfSleepIfUnmoving()
    {
        if (RoleAssignment.playerID - 1 != Multiplayer.GetUser().Index) { return; }
        //Debug.Log("yikes " + currentlyOwnedByAvatar==null);
        if (currentlyOwnedByAvatar==null)
        {
            if (rbDynamic.linearVelocity.magnitude < 0.1f)
            {
                timeSinceLastSignificantMovement += Time.deltaTime;
                if (timeSinceLastSignificantMovement > 5f)
                {
                //    Debug.Log("sleep");
                    timeSinceLastSignificantMovement = 0;
                    BroadcastRemoteMethod(nameof(DynamicSleep));
                }
            }
        }
        else
        {
            timeSinceLastSignificantMovement = 0;
        }
    }
    private void CheckForMovement()
    {
        if (currentlyOwnedByAvatar == null || !currentlyOwnedByAvatar.IsMe) { return; }


        if (rbDynamic.linearVelocity.magnitude >= 0.1f || currentlyOwnedByAvatar!=null)
        {
        //    Debug.Log("awake");
            BroadcastRemoteMethod(nameof(DynamicAwake));
        }
    }
    [SynchronizableMethod]
    public void DynamicSleep()
    {
        timeSinceLastSignificantMovement = 0;
        rbSyncDynamic.SyncEveryNUpdates = 999999;
        rbSyncDynamic.FullSyncEveryNSync = 999999;
        // Debug.Log("sleep " + transform.root.gameObject.name);
    }
    [SynchronizableMethod]
    public void DynamicAwake()
    {
        timeSinceLastSignificantMovement = 0;
        rbSyncDynamic.SyncEveryNUpdates = 2;
        rbSyncDynamic.FullSyncEveryNSync = 4;
    }
    
    public Alteruna.Avatar GetCurrentlyOwnedByAvatar()
    {
        return currentlyOwnedByAvatar;
    }
    [SynchronizableMethod]
    public void SetCurrentlyOwnedByAvatar(int newIndex)
    {
        if (newIndex != -1)
        {
            currentlyOwnedByAvatar = GetAvatarByOwnerIndex(newIndex);
        }
        else
        {
            currentlyOwnedByAvatar = null;
        }
        //    Debug.Log("owned by " + currentlyOwnedByAvatar.gameObject.name);
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
