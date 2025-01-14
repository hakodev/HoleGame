using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
public class VotingPhase : AttributesSync {

    private List<PlayerRole> totalPlayers;

    [SerializeField] private GameObject playerVoteButton;
    [SerializeField] private float firstPlayerOptionYPos;
    [SerializeField] TMP_Text pickedPlayerNameText;
    [SerializeField] private GameObject votingCanvas;
    [SerializeField] private GameObject votedCanvas;
    [SerializeField] private CanvasGroup taskManagerPickedDisplayCanvas;
    [SerializeField] GameObject symptomsNotifCanvas;

    Alteruna.Avatar avatar;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }
    private void Start() {
        totalPlayers = RoleAssignment.GetTotalPlayers();

        /*   
        CustomMethods.FindChildRecursivelyQuick(transform, "PickedPlayerNameText");
        pickedPlayerNameText = CustomMethods.foundRecursively.GetComponent<TMP_Text>();

        CustomMethods.FindChildRecursivelyQuick(transform, "SymptomsNotifCanvas");
        symptomsNotifCanvas = CustomMethods.foundRecursively;
        */

        CustomMethods.foundRecursively = null;
    }

    [SynchronizableMethod]
    public void InitiateVotingPhase() {
        if(!avatar.IsMe) { return; }
        
        votingCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None; // Unlock the mouse for the voting
        Cursor.visible = true;

        float tempYPos = firstPlayerOptionYPos;

        foreach(PlayerRole player in totalPlayers) {
            if(player.IsTaskManager) { // Player who was task manager in the previous round can't be it again
                player.IsTaskManager = false;
            } else {
                GameObject newPlayerVoteOption = Instantiate(playerVoteButton, this.transform);
                newPlayerVoteOption.GetComponentInChildren<TMP_Text>().text = player.gameObject.name;
                newPlayerVoteOption.transform.position = new Vector3(newPlayerVoteOption.transform.position.x,
                                                                     tempYPos,
                                                                     newPlayerVoteOption.transform.position.z);

                newPlayerVoteOption.GetComponent<Button>().onClick.AddListener(() => {
                    player.VotedCount++;
                    votingCanvas.SetActive(false);
                    votedCanvas.SetActive(true);
                });

                tempYPos -= 100f;
            }

            player.gameObject.GetComponent<PlayerController>().MovementEnabled = false; // Disable movement until end of voting phase
        }
        
    }

    [SynchronizableMethod]
    public void EndVotingPhase() {
        if (!avatar.IsMe) { return; }

        votingCanvas.SetActive(false);
           votedCanvas.SetActive(false);
           Cursor.lockState = CursorLockMode.Locked;
           Cursor.visible = false;

           PlayerRole pickedPlayer = null;

           for(int i = 0; i < totalPlayers.Count; i++) {
               PlayerRole currentPlayer = totalPlayers[i].GetComponent<PlayerRole>();

               totalPlayers[i].gameObject.GetComponent<PlayerController>().MovementEnabled = true; // Enable movement again

               if(currentPlayer.GetRole() == Roles.Infiltrator)
                   StartCoroutine(DisplaySymptomNotif());

               if(totalPlayers[i] == totalPlayers[0])
                   continue;

               if(totalPlayers[i].VotedCount > totalPlayers[i - 1].VotedCount)
                   pickedPlayer = totalPlayers[i];
           }

           pickedPlayerNameText.text = pickedPlayer.gameObject.name;
           pickedPlayer.IsTaskManager = true;
           StartCoroutine(DisplayTaskManager());
        
    }

    private IEnumerator DisplayTaskManager() {
        taskManagerPickedDisplayCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        taskManagerPickedDisplayCanvas.DOFade(0f, 1f);
    }

    private IEnumerator DisplaySymptomNotif() {
        symptomsNotifCanvas.SetActive(true);
        yield return new WaitForSeconds(10f); // How many seconds to display it on screen
        symptomsNotifCanvas.SetActive(false);
    }
}
