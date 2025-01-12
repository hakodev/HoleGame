
using UnityEngine;
using Alteruna;


public class Health : AttributesSync {

    [SynchronizableField] float currentHealth = 100f;
    const float maxHealth = 100f;

    MishSyncAnimations mishSync;
    PlayerController playerController;
    Alteruna.Avatar avatar;

    private CharacterController characterController;
    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();
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
            mishSync.SetStance(StanceEnum.Dead);

            Debug.Log("Reduced HP");
            BroadcastRemoteMethod("KillPlayer");
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");
        playerController.MovementEnabled = false;
        characterController.enabled = false;

        //Destroy(this.gameObject);
    }

    bool happenedOnce = false;
    
    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
