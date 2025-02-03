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
    [SynchronizableField] private bool wasMoved = false;

    [field: SerializeField] public float forceWhenThrownMultiplier { get; private set; } = 1;
    public abstract void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
    public abstract void Use();

    RigidbodySynchronizable rbSyncDynamic;
    Rigidbody rbDynamic;
    List<Collider> collidersDynamic;
    [SerializeField] protected float minVelocityToProduceSound = 0.1f;

    bool awake = false;

    [Header("removed serialize fields for speed. Nsync Objects is Here. x - when object is inactive(high number), y - when object is active(low number), also in the script both are 30 2, for ease of access")]
    Vector2 syncEveryNUpdates = new Vector2(30, 2);
    Vector2 fullSyncEveryNSyncs = new Vector2(30, 2);

    [SynchronizableField] float timeSinceLastSignificantMovement = 0;

    protected virtual void Awake()
    {
        rbSyncDynamic = GetComponent<RigidbodySynchronizable>();
        rbDynamic = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        //BroadcastRemoteMethod(nameof(DynamicSleep));
        collidersDynamic = GetComponentsInChildren<Collider>()
            .Where(c => !c.isTrigger)
            .ToList();
    }


    protected virtual void Update()
    {
        SelfSleepIfUnmoving();
        CheckForMovement();
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
        bool Special = (isSticky || isChair || isBall);
       

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
        for (int i = 0; i < collidersDynamic.Count; i++)
        {
            collidersDynamic[i].isTrigger = newState;

        }
    }
    private void DisableCols(bool newState)
    {
        for (int i = 0; i<collidersDynamic.Count; i++)
        {
            collidersDynamic[i].enabled = !newState;

        }
    }
    private void IgnoreCols(bool newState)
    {
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
            if (rbDynamic.linearVelocity.magnitude < 0.05f)
            {
                timeSinceLastSignificantMovement += Time.deltaTime;
                if (timeSinceLastSignificantMovement > 3f)
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
        if (userAvatar == null || !userAvatar.IsMe) { return; }


        if (rbDynamic.linearVelocity.magnitude >= 0.1f || currentlyOwnedByAvatar != null)
        {
            timeSinceLastSignificantMovement = 0;
            if (!awake)
            {
                wasMoved = true;
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
        Debug.Log("awake " + gameObject.name + " " + rbDynamic.linearVelocity.magnitude); //keep this here so we know what causes problems with latency in the future
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
