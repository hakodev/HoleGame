using TMPro;
using UnityEngine;
using Alteruna;
using Unity.Netcode;
using System.Collections.Generic;

public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] private Material debugDeathMaterial; // remove when done testing


    private PlayerController playerController;
    private Alteruna.Avatar avatar;
   // private Animator animator;
    private AnimationSynchronizable animatorSync;
    private CharacterController characterController;
    bool dead = false;
    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        //animator = transform.Find("Animation").GetComponent<Animator>();
        animatorSync = transform.Find("Animation").GetComponent<AnimationSynchronizable>();
    }

    private void Start() {
        if (!avatar.IsMe) { return; }
        currentHealth = maxHealth;
    }
    public void DamagePlayer(float damageAmount) {
        currentHealth -= damageAmount;
        Debug.Log(damageAmount);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Reduced HP");
            BroadcastRemoteMethod("KillPlayer");
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");

       // animator.SetBool("Dead", true);
        animatorSync.Animator.SetBool("Dead", true);

        playerController.MovementEnabled = false;
        dead = true;
        //Destroy(this.gameObject);
    }

    bool happenedOnce = false;
    private void Update()
    {
       FixAnimatorOffset();

        if (!avatar.IsMe) { return; }

        if (!happenedOnce && animatorSync.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
   animatorSync.Animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            animatorSync.Animator.speed = 0f;
            //animator.speed = 0f;
            ChangeColliderAfterDeath();
            happenedOnce = true;
        }

    }

    private new void LateUpdate()
    {
      FixAnimatorOffset();
    }
    private void FixAnimatorOffset()
    {
        if (dead) { return; }
        animatorSync.Animator.transform.localPosition = Vector3.zero;
        animatorSync.Animator.transform.rotation = transform.rotation;

        Vector3 temp = animatorSync.Animator.transform.Find("mixamorig:Hips").localPosition;
        animatorSync.Animator.transform.Find("mixamorig:Hips").localPosition = new Vector3(0, temp.y, 0);
        animatorSync.Animator.transform.Find("Human 2.001").localPosition = Vector3.zero;
        //animationTie.transform.localPosition = new Vector3(-0.0130000003f, -0.97299999f, 0);
    }
    private void ChangeColliderAfterDeath()
    {
        characterController.enabled = false;
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
