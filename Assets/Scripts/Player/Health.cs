using UnityEngine;
using Alteruna;
using System.Collections;


public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] GameObject deadScreen;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private CharacterController characterController;
    //bool dead = false;
    private MishSyncAnimations mishSync;
    private EndGameResolution endGameResolution;
    private TransformSynchronizable transformSynchronizable;

    //[SerializeField] private List<Object> objectsToDestroyUponDeath;
    [SynchronizableField] bool dead = false;
    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();
        transformSynchronizable = GetComponent<TransformSynchronizable>();

        deadScreen.SetActive(false);
    }

    private void Start() {
        endGameResolution = GetComponentInChildren<EndGameResolution>();

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
            PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetDeathStatic);
            dead = true;
            BroadcastRemoteMethod(nameof(KillPlayer));
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");
        //playerController.MovementEnabled = false;
        //characterController.enabled = false;
        //deadScreen.SetActive(true);
        StartCoroutine(DeadScreenDisplay());
        mishSync.SetStance(StanceEnum.Dead);
        VotingPhase.totalALivePlayers.Remove(GetComponent<PlayerRole>());
        transformSynchronizable.RefreshRate = 0f;

        endGameResolution.CheckForEndGame();
    }

    private IEnumerator DeadScreenDisplay() {
        deadScreen.SetActive(true);
        yield return new WaitForSeconds(4f);
        deadScreen.SetActive(false);
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
