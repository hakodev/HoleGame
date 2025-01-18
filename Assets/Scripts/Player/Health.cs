using UnityEngine;
using Alteruna;
using DG.Tweening;
using System.Collections;


public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] CanvasGroup deadScreen;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private CharacterController characterController;
    //bool dead = false;
    private MishSyncAnimations mishSync;

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
            Debug.Log("Reduced HP");
            BroadcastRemoteMethod("KillPlayer");
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");
        playerController.MovementEnabled = false;
        characterController.enabled = false;
        mishSync.SetStance(StanceEnum.Dead);
      //  dead = true;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private IEnumerator DisplayTaskManager()
    {
        deadScreen.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        deadScreen.DOFade(0f, 1f);
    }
}
