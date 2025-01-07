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

        for(int i = 0; i < maxNumOfInfiltrators; i++) {
            randomNum = Random.Range(0, rolelessPlayers.Count);

            rolelessPlayers[randomNum].Role = Roles.Infiltrator;
            rolelessPlayers.RemoveAt(randomNum);
        }

        foreach(PlayerController player in rolelessPlayers) {
            player.Role = Roles.Machine;
        }

        rolelessPlayers.Clear();

        if(avatar.IsMe) { // Display the local player's role
            if(localPlayer.Role == Roles.Infiltrator) {
                StartCoroutine(DisplayInfiltratorRole());
            }

            if(localPlayer.Role == Roles.Machine) {
                StartCoroutine(DisplayMachineRole());
            }
        }
    }

    private IEnumerator DisplayInfiltratorRole() {
        infiltratorCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(roleDisplayTime);
        infiltratorCanvas.DOFade(0f, 1f);
    }

    private IEnumerator DisplayMachineRole() {
        machineCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(roleDisplayTime);
        machineCanvas.DOFade(0f, 1f);
    }
}

public enum Roles {
    Machine,
    Infiltrator
}
