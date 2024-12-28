using Alteruna;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private PlayerController localPlayer;
    private List<PlayerController> players = new();
    Alteruna.Avatar avatar;

    private void Awake() {
        avatar = localPlayer.gameObject.GetComponent<Alteruna.Avatar>();
    }

    private void Start() {
        AssignRoles();
    }

    private void AssignRoles() {
        PlayerController[] totalPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        players.AddRange(totalPlayers);
        //Debug.Log($"Total players: {players.Count}");

        if(avatar.IsMe) {

        }
    }
}
