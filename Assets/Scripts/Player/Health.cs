using UnityEngine;
using Alteruna;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class Health : AttributesSync {

   [SynchronizableField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] GameObject deadScreen;

    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    private CharacterController characterController;
    private MishSyncAnimations mishSync;



    private EndGameResolution endGameResolution;
    private List<EndGameResolution> endGameResolutions = new List<EndGameResolution>(); 

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        avatar = GetComponent<Alteruna.Avatar>();
        mishSync = GetComponent<MishSyncAnimations>();

        deadScreen.SetActive(false);
    }

    private void Start() {
        endGameResolution = GetComponentInChildren<EndGameResolution>();

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
            PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetDeathStatic);
            BroadcastRemoteMethod("KillPlayer");
            //mishSync.SetStance(StanceEnum.Dead);
        }
    }


    [SynchronizableMethod]
    private void KillPlayer() {
        playerController.MovementEnabled = false;
        characterController.enabled = false;
        deadScreen.SetActive(true);
        mishSync.SetStance(StanceEnum.Dead);
        Debug.Log("Player died!" + mishSync.GetCurrentStance() + endGameResolution.transform.root.name);
        VotingPhase.totalALivePlayers.Remove(GetComponent<PlayerRole>());
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

}
