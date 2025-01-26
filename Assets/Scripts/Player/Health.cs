using UnityEngine;
using Alteruna;


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

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();
        endGameResolution = GetComponentInChildren<EndGameResolution>();

        deadScreen.SetActive(false);
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
            PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetDeathStatic);
            BroadcastRemoteMethod("KillPlayer");
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");
        playerController.MovementEnabled = false;
        characterController.enabled = false;
        deadScreen.SetActive(true);
        mishSync.SetStance(StanceEnum.Dead);
        VotingPhase.totalALivePlayers.Remove(GetComponent<PlayerRole>());
        endGameResolution.CheckForEndGame();
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
