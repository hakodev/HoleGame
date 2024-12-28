using TMPro;
using UnityEngine;
using Alteruna;
using Unity.Netcode;

public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] private Material debugDeathMaterial; // remove when done testing


    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private Animator animator;
    private AnimationSynchronizable animatorSync;
    private CharacterController characterController;
    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        animator = transform.Find("Animation").GetComponent<Animator>();
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

        animator.SetBool("Dead", true);
        animatorSync.SetBool("Dead", true);

        playerController.MovementEnabled = false;
        //Destroy(this.gameObject);
    }
    
    private void Update()
    {


        if (!avatar.IsMe) { return; }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
    animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            Debug.Log("die alreday successfully");
            animator.speed = 0; // Freeze the animation
            Vector2 oldValues = new Vector2(characterController.radius, characterController.height);
            characterController.height = oldValues.x;
            characterController.radius = oldValues.y;
        }

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
