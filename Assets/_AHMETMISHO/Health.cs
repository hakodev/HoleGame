using TMPro;
using UnityEngine;
using Alteruna;
using Unity.Netcode;

public class Health : AttributesSync {

   [SynchronizableField] [SerializeField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] private Material debugDeathMaterial; // remove when done testing
    private MeshRenderer debugRenderer; // remove when done testing
    private PlayerController playerController;
    private Alteruna.Avatar avatar;
    

    private void Awake() {
        debugRenderer = GetComponent<MeshRenderer>();
        playerController = GetComponent<PlayerController>();
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Start() {
        if (!avatar.IsMe) { return; }
        currentHealth = maxHealth;
    }
    public void DamagePlayer(float damageAmount) {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            BroadcastRemoteMethod("KillPlayer");
        }
    }

    [SynchronizableMethod]
    private void KillPlayer() {
        Debug.Log("Player died!");
        debugRenderer.material = debugDeathMaterial;
        playerController.MovementEnabled = false;
        //Destroy(this.gameObject);
    }
    
    private void Update()
    {
        if (!avatar.IsMe) { return; }

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
