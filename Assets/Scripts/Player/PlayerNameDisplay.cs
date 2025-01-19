using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour {
    private TMP_Text playerNameText;
    private Alteruna.Avatar avatar;
    private PlayerRole thisPlayerRole;
    [SerializeField] private Camera playerCamera;

    private void Awake () {
        playerNameText = GetComponent<TMP_Text>();
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        thisPlayerRole = transform.root.GetComponent<PlayerRole>();
    }

    private void Start() {
        playerNameText.text = "Placeholder"; // Set this to be the player name the user chooses

        if(thisPlayerRole.GetRole() == Roles.Infiltrator) {
            foreach(PlayerRole player in RoleAssignment.GetTotalPlayers()) {
                if(player.GetRole() == Roles.Infiltrator) {
                    player.gameObject.GetComponentInChildren<PlayerNameDisplay>().
                        playerNameText.color = Color.red; // Setting name colors to red, only viewable by infiltrators
                }
            }
        }

        if(avatar.IsMe) {
            playerNameText.enabled = false; // Disable my text for my view
        }
    }

    private void LateUpdate() {
        if(avatar.IsMe) return;

        playerNameText.transform.LookAt(playerCamera.transform);
    }
}
