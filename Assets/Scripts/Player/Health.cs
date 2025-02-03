using UnityEngine;
using Alteruna;
using System.Collections.Generic;


public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] GameObject deadScreen;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private CharacterController characterController;
    private MishSyncAnimations mishSync;

    AudioSource source;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();

        deadScreen.SetActive(false);
    }

    private void Start() {
        if (!avatar.IsMe) { return; }
        currentHealth = maxHealth;
    }

    bool firstTimeAdding = true;
    public void DamagePlayer(float damageAmount) {
        currentHealth -= damageAmount;
        Debug.Log(damageAmount);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Reduced HP");
            PlayerAudioManager.Instance.PlaySound(this.gameObject, source, PlayerAudioManager.Instance.GetDeathStatic);
            BroadcastRemoteMethod(nameof(KillPlayer));
            //mishSync.SetStance(StanceEnum.Dead);
        }
    }


    [SynchronizableMethod]
    private void KillPlayer() {
        playerController.MovementEnabled = false;
        characterController.enabled = false;
        deadScreen.SetActive(true);
        mishSync.SetStance(StanceEnum.Dead);
        Debug.Log("Player died!" + mishSync.GetCurrentStance());
        //VotingPhase.totalALivePlayers.Remove(GetComponent<PlayerRole>());
        Multiplayer.GetAvatar().GetComponentInChildren<EndGameResolution>().CheckForEndGame();
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private void OnApplicationQuit()
    {
        BroadcastRemoteMethod(nameof(RemovePlayer));
    }

    [SynchronizableMethod] 
    void RemovePlayer()
    {
        VotingPhase.totalALivePlayers.Remove(GetComponent<PlayerRole>());
        VotingPhase.votingPlayers.Remove(GetComponent<VotingPhase>());
    }

}
