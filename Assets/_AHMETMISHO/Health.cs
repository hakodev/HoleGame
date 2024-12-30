using TMPro;
using UnityEngine;
using Alteruna;
using Unity.Netcode;
using System.Collections.Generic;

public class Health : AttributesSync {

    [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] private Animator animator; // from Animation child gameobject
    [SerializeField] private AnimationSynchronizable animatorSync; // from Animation child gameobject

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private CharacterController characterController;
    private bool dead = false;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Start() {
        if (!avatar.IsMe) { return; }
        currentHealth = maxHealth;
    }
    public void DamagePlayer(float damageAmount) {
        currentHealth -= damageAmount;
        Debug.Log(damageAmount);
        Debug.Log("Reduced HP");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            BroadcastRemoteMethod(nameof(KillPlayer));
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");

        animator.SetBool("Dead", true);
        animatorSync.SetBool("Dead", true);

        playerController.MovementEnabled = false;
        dead = true;
        //Destroy(this.gameObject);
    }

    bool happenedOnce = false;
    private void Update()
    {
       FixAnimatorOffset();

        if (!avatar.IsMe) { return; }

        if (!happenedOnce && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
    animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            animator.speed = 0f;
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
        animator.transform.localPosition = Vector3.zero;
        animator.transform.rotation = transform.rotation;

        Vector3 temp = animator.transform.Find("mixamorig:Hips").localPosition;
        animator.transform.Find("mixamorig:Hips").localPosition = new Vector3(0, temp.y, 0);
        animator.transform.Find("Human 2.001").localPosition = Vector3.zero;
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
