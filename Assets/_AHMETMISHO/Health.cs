using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] private float currentHealth = 100f;
    private const float maxHealth = 100f;
    [SerializeField] private Material debugDeathMaterial; // remove when done testing
    private MeshRenderer debugRenderer;

    private void Awake() {
        debugRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        currentHealth = maxHealth;
    }

    public void HealPlayer(float healAmount) {
        currentHealth += healAmount;

        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public void DamagePlayer(float damageAmount) {
        currentHealth -= damageAmount;

        if(currentHealth < 0) {
            currentHealth = 0;
            KillPlayer();
        }
    }

    private void KillPlayer() {
        Debug.Log("Player died!");
        debugRenderer.material = debugDeathMaterial;
        //Destroy(this.gameObject);
    }
}
