using Alteruna;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : AttributesSync {
    private TextMeshProUGUI playerNameText;
    private Alteruna.Avatar avatar;
    private PlayerRole thisPlayerRole;
    private Camera getLocalCamera;

    private void Awake () {
        playerNameText = GetComponent<TextMeshProUGUI>();
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        thisPlayerRole = transform.root.GetComponent<PlayerRole>();

    }

    private void Start() {
        getLocalCamera = Multiplayer.GetAvatar(Multiplayer.GetUser().Index).gameObject.GetComponentInChildren<Camera>();

        playerNameText.text = UIInput.PlayerName;
        transform.root.gameObject.name = playerNameText.text;

        if(avatar.IsMe) {
            playerNameText.enabled = false; // Disable my text for my view
        }
    }

    bool once = true;
    private new void LateUpdate() {
        base.LateUpdate();
        if(avatar.IsMe) return;

        playerNameText.transform.LookAt(getLocalCamera.transform);

        if(RoleAssignment.hasGameStarted && once)
        {
            once = false;
            if (thisPlayerRole.GetRole() == Roles.Infiltrator)
            {
                PlayerRole player = Multiplayer.GetAvatar(Multiplayer.GetUser().Index).gameObject.GetComponent<PlayerRole>();
                if (player.GetRole() == Roles.Infiltrator)
                {
                    playerNameText.color = Color.red; 
                }
            }
        }
    }
}
