using Alteruna;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class DynamicInteractableObject : AttributesSync, IObserver, IInteractableObject
{
    protected Alteruna.Avatar currentlyOwnedByAvatar;
    protected CharacterController currentController;
    Alteruna.Avatar userAvatar;

    [SynchronizableField] public bool isPickedUp;

    [field: SerializeField] public float forceWhenThrownMultiplier { get; private set; } = 1;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();

    RigidbodySynchronizable rbSyncDynamic;
    Rigidbody rbDynamic;
    List<Collider> collidersDynamic;
    [SerializeField] protected float minVelocityToProduceSound = 0.1f;

    [SynchronizableField] bool awake = false;

    [Header("removed serialize fields for speed. Nsync Objects is Here. x - when object is inactive(high number), y - when object is active(low number), also in the script both are 30 2, for ease of access")]
    Vector2 syncEveryNUpdates = new Vector2(30, 2);
    Vector2 fullSyncEveryNSyncs = new Vector2(30, 2);

    [SynchronizableField] float timeSinceLastSignificantMovement = 0;
    float movementThreshhold = 0.43f;

    protected virtual void Awake()
    {
        rbSyncDynamic = GetComponent<RigidbodySynchronizable>();
        rbDynamic = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        //BroadcastRemoteMethod(nameof(DynamicSleep));
        AssignCollidersDynamic();

        /* // gives error when gun is spawned which is very funny
            if (rbSyncDynamic.SyncEveryNUpdates < 999999 || rbSyncDynamic.FullSyncEveryNSync < 999999 && rbSyncDynamic.gameObject.activeInHierarchy)
            {
                Debug.LogError("OBJECT NOT SYNCED: " + rbSyncDynamic.SyncEveryNUpdates + " " + rbSyncDynamic.FullSyncEveryNSync + " " + rbSyncDynamic.gameObject.name);
            }
        */
    }

    private void AssignCollidersDynamic()
    {
        if(collidersDynamic == null)
        {
            collidersDynamic = GetComponentsInChildren<Collider>()
    .Where(c => !c.isTrigger)
    .ToList();
        }
    }

    protected virtual void Update()
    {
        SelfSleepIfUnmoving();
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
        bool isSticky = this is StickyNote; //dont delete theese bools
        bool isChair = (gameObject.CompareTag("Chair"));
        bool isBall = this is Ball;
        bool isCrumpablePaper = this is CrumpablePaper;
        bool Special = (isSticky || isChair || isBall || isCrumpablePaper);
       

        if (newState)
        {
            if (currentlyOwnedByAvatar != null)
            {
                if (Special)
                {
                    if (isSticky) ToTriggerCols(true);
                    if (!isSticky)IgnoreCols(true);
                }
                else
                {
                    DisableCols(true);
                }
            }
        }
        else
        {
            if (Special) {
                if (isSticky) ToTriggerCols(false);
                if (!isSticky) IgnoreCols(false);
            }
            else
            {
                DisableCols(false);
            }
        }
    }

    private void ToTriggerCols(bool newState) //because the sticky notes are just so god damn special
    {
        AssignCollidersDynamic();

        for (int i = 0; i < collidersDynamic.Count; i++)
        {
            collidersDynamic[i].isTrigger = newState;

        }
    }
    private void DisableCols(bool newState)
    {
        AssignCollidersDynamic();

        for (int i = 0; i<collidersDynamic.Count; i++)
        {
            collidersDynamic[i].enabled = !newState;

        }
    }
    private void IgnoreCols(bool newState)
    {
        AssignCollidersDynamic();

        for (int i = 0; i < collidersDynamic.Count; i++)
        {
            Physics.IgnoreCollision(collidersDynamic[i], currentController, newState);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {

    }
    bool initiatedPlayer = false;
    private void CheckForPlayer()
    {
        if (!initiatedPlayer)
        {
            if (RoleAssignment.userAvatar != null)
            {
                userAvatar = RoleAssignment.userAvatar;
                initiatedPlayer = true;
            }
        }
    }
    private void SelfSleepIfUnmoving()
    {
        CheckForPlayer();
        if (userAvatar == null || !userAvatar.IsMe) { return; }

        //Debug.Log("yikes " + currentlyOwnedByAvatar==null);
        if (currentlyOwnedByAvatar == null)
        {
            if (rbDynamic.linearVelocity.magnitude < movementThreshhold)
            {
                timeSinceLastSignificantMovement += Time.deltaTime;
                if (timeSinceLastSignificantMovement > 1f)
                {
                    //    Debug.Log("sleep");
                    timeSinceLastSignificantMovement = 0;
                    if (awake) BroadcastRemoteMethod(nameof(DynamicSleep));
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
        if ((userAvatar == null || !userAvatar.IsMe)) { return; }


        if (rbDynamic.linearVelocity.magnitude >= movementThreshhold || currentlyOwnedByAvatar != null)
        {
            timeSinceLastSignificantMovement = 0;
            if (!awake)
            {
                Debug.Log("awake wtf " + rbDynamic.linearVelocity.magnitude + " " + movementThreshhold + " " + currentlyOwnedByAvatar);
                BroadcastRemoteMethod(nameof(DynamicAwake));
            }
        }
    }
    [SynchronizableMethod]
    public void DynamicSleep()
    {
        awake = false;
        timeSinceLastSignificantMovement = 0;
        rbSyncDynamic.SyncEveryNUpdates = 999999;
        rbSyncDynamic.FullSyncEveryNSync = 999999;
        Debug.Log("sleep " + gameObject.name);
    }
    [SynchronizableMethod]
    public void DynamicAwake()
    {
        awake = true;
        rbSyncDynamic.SyncEveryNUpdates = 4;
        rbSyncDynamic.FullSyncEveryNSync = 8;
        Debug.Log("awake " + gameObject.name + " " + rbDynamic.linearVelocity.magnitude + " " + currentlyOwnedByAvatar); //keep this here so we know what causes problems with latency in the future
    }

    public Alteruna.Avatar GetCurrentlyOwnedByAvatar()
    {
        return currentlyOwnedByAvatar;
    }
    [SynchronizableMethod]
    public void SetCurrentlyOwnedByAvatar(int roleAssignmentIndex)
    {
        if (roleAssignmentIndex != -1)
        {
            currentlyOwnedByAvatar = GetAvatarByOwnerIndex(roleAssignmentIndex);
            currentController = currentlyOwnedByAvatar.GetComponent<CharacterController>();
            //Debug.Log("owned by " + currentlyOwnedByAvatar);
        }
        else
        {
            currentlyOwnedByAvatar = null;
            currentController = null;
            //Debug.Log("not owned " + null);
        }
        Commit();
    }

    public Alteruna.Avatar GetAvatarByOwnerIndex(int roleAssignmentIndex)
    {
        if(roleAssignmentIndex != -1)
        {
            List<Alteruna.Avatar> avatars = Multiplayer.GetAvatars(); 
            return avatars[roleAssignmentIndex];
        }
        else
        {
            return null;
        }
    }


}
