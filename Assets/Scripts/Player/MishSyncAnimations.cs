using Alteruna;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class MishSyncAnimations : AttributesSync
{
    [SynchronizableField] public bool Jumping;
    [SynchronizableField] public bool Walking;
    [SynchronizableField] public bool Running;

    private Alteruna.Avatar avatar;
    private Animator animator;
    private static List<MishSyncAnimations> mishSyncs = new List<MishSyncAnimations>();

    private Transform mixamo;
    private Transform human;
    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }
    private void Start()
    {
        animator = transform.Find("Animation").GetComponent<Animator>(); // Automatically assign Animator if not set
        mishSyncs.Add(this);

        mixamo = animator.transform.Find("mixamorig:Hips");
        human = animator.transform.Find("Human 2.001");
    }
    public void SetJumping(bool newState)
    {
        Jumping = newState;
        Commit();

    }
    public void SetRunning(bool newState)
    {
        Running = newState;
        Commit();

    }
    public void SetWalking(bool newState)
    {
        Walking = newState;
        Commit();
    }

    private void Update()
    {
        FixAnimatorOffset();
        animator.SetBool("Walking", Walking);
        animator.SetBool("Running", Running);
        animator.SetBool("Jumping", Jumping);
    }

    private new void LateUpdate()
    {
        foreach (MishSyncAnimations player in mishSyncs) 
        {
            FixAnimatorOffset();
        }
    }
    public void FixAnimatorOffset()
    {
        //if (!avatar.IsMe) { return; }

        //if (dead) { return; }
        //  animatorSync.Animator.transform.localPosition = Vector3.zero;
        //  animatorSync.Animator.transform.rotation = transform.rotation;

        //    Vector3 temp = animatorSync.Animator.transform.Find("mixamorig:Hips").localPosition;
        ////   animatorSync.Animator.transform.Find("mixamorig:Hips").localPosition = new Vector3(0, temp.y, 0);
        // animatorSync.Animator.transform.Find("Human 2.001").localPosition = Vector3.zero;



        animator.transform.localPosition = Vector3.zero;
        animator.transform.rotation = transform.rotation;

        Vector3 temp = mixamo.localPosition;
        mixamo.localPosition = new Vector3(0, temp.y, 0);
        human.localPosition = Vector3.zero;
    }
}
