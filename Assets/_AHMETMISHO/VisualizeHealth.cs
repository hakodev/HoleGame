using UnityEngine;
using Alteruna;
using TMPro;
public class VisualizeHealth : MonoBehaviour
{
     Health playerHealth;
    Alteruna.Avatar avatar;
    [SerializeField] TextMeshProUGUI visualizeSelfHealth;
    [SerializeField] TextMeshProUGUI visualizePublicHealth;


    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        playerHealth = GetComponent<Health>();
    }
    private void Start()
    {
        if(!avatar.IsMe) {
            visualizeSelfHealth.gameObject.SetActive(false);
            return; 
        }

        
    }

    private void Update()
    {
        AttemptVisualizingHealth();
    }
    // works for both the self and other players canvas

    private void AttemptVisualizingHealth()
    {
        visualizePublicHealth.text = playerHealth.GetHealth() + "/" + playerHealth.GetMaxHealth();
       if(visualizeSelfHealth.gameObject.activeSelf) visualizeSelfHealth.text = "Health - " + playerHealth.GetHealth() + "/" + playerHealth.GetMaxHealth();
    }
}
