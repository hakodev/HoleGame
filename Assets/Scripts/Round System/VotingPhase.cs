using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
public class VotingPhase : AttributesSync {

    private List<PlayerRole> totalPlayers;
    private PlayerRole player;

    [SerializeField] private GameObject playerVoteButton;
    [SerializeField] TMP_Text pickedPlayerNameText;
    [SerializeField] private GameObject votingCanvas;
    [SerializeField] private GameObject votedCanvas;
    [SerializeField] private CanvasGroup taskManagerPickedDisplayCanvas;
    [SerializeField] CanvasGroup symptomsNotifCanvas;
    [SerializeField] GameObject votingPhaseObject;
    Alteruna.Avatar avatar;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        player = GetComponent<PlayerRole>();
    }
    private void Start() {
        totalPlayers = RoleAssignment.GetTotalPlayers();

        /*   
        CustomMethods.FindChildRecursivelyQuick(transform, "PickedPlayerNameText");
        pickedPlayerNameText = CustomMethods.foundRecursively.GetComponent<TMP_Text>();

        CustomMethods.FindChildRecursivelyQuick(transform, "SymptomsNotifCanvas");
        symptomsNotifCanvas = CustomMethods.foundRecursively;
        */

       // CustomMethods.foundRecursively = null;
    }

    
    public void InitiateVotingPhase() {
        if (!avatar.IsMe) { return; }
        Debug.Log(totalPlayers.Count);

        votingCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None; // Unlock the mouse for the voting
        Cursor.visible = true;

        float tempYPos = -120;

            if(player.IsTaskManager) { // Player who was task manager in the previous round can't be it again
                player.IsTaskManager = false;
            } else {

                int i = 0;
                foreach(PlayerRole otherPlayer in totalPlayers)
                {
                    i++;
                    if (otherPlayer == player) { continue; }

                    GameObject newPlayerVoteOption = Instantiate(playerVoteButton, votingCanvas.transform);
                    newPlayerVoteOption.GetComponentInChildren<TMP_Text>().text = otherPlayer.gameObject.name;

                    RectTransform rect = newPlayerVoteOption.GetComponent<RectTransform>();
                    rect.anchoredPosition += rect.anchoredPosition * i;

                    newPlayerVoteOption.GetComponent<Button>().onClick.AddListener(() => {
                            otherPlayer.VotedCount++;
                            votingCanvas.SetActive(false);
                            votedCanvas.SetActive(true);
                    });
                }
            }

            player.gameObject.GetComponent<PlayerController>().MovementEnabled = false; // Disable movement until end of voting phase
        }
        
    


    //i can pick no one, or wait
    //if highest ppl have equal votes, then sb is picked at random
    //give gun to CEO through the specialInteractionSystem
    //
    [SynchronizableMethod]
    public void EndVotingPhase()
    {
        votingCanvas.SetActive(false);
        votedCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.gameObject.GetComponent<PlayerController>().MovementEnabled = true; // Enable movement again


        PlayerRole pickedPlayer = totalPlayers[0];
        for (int i = 1; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == player) { continue; }
            if (totalPlayers[i].VotedCount > pickedPlayer.VotedCount) pickedPlayer = totalPlayers[i];
        }

        pickedPlayerNameText.text = pickedPlayer.gameObject.name;
        pickedPlayer.IsTaskManager = true;
        StartCoroutine(DisplayTaskManager());
        StartCoroutine(DisplaySymptomNotif());
    }

    private IEnumerator DisplayTaskManager() {
        taskManagerPickedDisplayCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        taskManagerPickedDisplayCanvas.DOFade(0f, 1f);
    }

    private IEnumerator DisplaySymptomNotif() {
        //symptomsNotifCanvas.SetActive(true);
        symptomsNotifCanvas.DOFade(1f, 0.1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        symptomsNotifCanvas.DOFade(0f, 2f);
       // symptomsNotifCanvas.SetActive(false);
    }
    
}
