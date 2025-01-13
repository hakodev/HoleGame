using Alteruna;
using UnityEngine;
using System.Collections.Generic;
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
        animator = transform.Find("Animation").GetComponent<Animator>(); // Automatically assign Animator if not set
        mishSyncs.Add(this);

        mixamo = animator.transform.Find("mixamorig:Hips");
        human = animator.transform.Find("Human 2.001");

        Debug.Log(gameObject.name);
    }
    public void SetJumping(bool newState)
    {
        Jumping = newState;
    }
    private void Start()
    {
      //  Time.timeScale = 0.05f;
    }
    private void Update()
    {
        if (!avatar.IsMe) { return; }
        foreach (MishSyncAnimations player in mishSyncs) 
        {
            player.FixAnimatorOffset();
            player.animator.SetBool("Walking", player.Walking);
            player.animator.SetBool("Running", player.Running);
            player.animator.SetBool("Jumping", player.Jumping);
            Debug.Log(player.gameObject.name);
        }
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
        if (!avatar.IsMe) { return; }

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
