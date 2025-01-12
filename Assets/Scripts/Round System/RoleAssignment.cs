using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAssignment : MonoBehaviour {
    [SerializeField] private PlayerController localPlayer;
    [SerializeField] private CanvasGroup infiltratorCanvas;
    [SerializeField] private CanvasGroup machineCanvas;
    [SerializeField] private float roleDisplayTime;
    private List<PlayerController> rolelessPlayers = new();
    private const int maxNumOfInfiltrators = 2;
    Alteruna.Avatar avatar;

    private void Awake() {
        avatar = localPlayer.gameObject.GetComponent<Alteruna.Avatar>();
    }

    private void OnEnable() {
        AssignRoles();
    }

    private void AssignRoles() {
        PlayerController[] totalPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        rolelessPlayers.AddRange(totalPlayers);
        //Debug.Log($"Total players: {players.Count}");

        int randomNum;

        for(int i = 0; i < maxNumOfInfiltrators; i++) { // Give maxNumOfInfiltrators amount of random players the infiltrator role

            if(rolelessPlayers.Count == 0) break; // Just in case

            randomNum = Random.Range(0, rolelessPlayers.Count);

            rolelessPlayers[randomNum].Role = Roles.Infiltrator;
            rolelessPlayers.RemoveAt(randomNum); // Remove the player from the roleless list after giving them a role
        }

        foreach(PlayerController player in rolelessPlayers) { // Give the rest the machine role
            player.Role = Roles.Machine;
        }

        rolelessPlayers.Clear();

        if(avatar.IsMe) { // Display the local player's role
            if(localPlayer.Role == Roles.Infiltrator) {
                StartCoroutine(DisplayRole(infiltratorCanvas));
            }

            if(localPlayer.Role == Roles.Machine) {
                StartCoroutine(DisplayRole(machineCanvas));
            }
        }
    }

    private IEnumerator DisplayRole(CanvasGroup roleCanvas) {
        roleCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(roleDisplayTime); // How many seconds to display it on screen
        roleCanvas.DOFade(0f, 1f);
    }
}

public enum Roles {
    Machine,
    Infiltrator
}
