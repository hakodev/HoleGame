using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
using System.Linq;
public class VotingPhase : AttributesSync {

    private List<PlayerRole> totalPlayers;
    private PlayerRole player;

    [SerializeField] private GameObject playerVoteButton;
    [SerializeField] TMP_Text pickedPlayerNameText;
    [SerializeField] private GameObject votingCanvas;
    [SerializeField] private GameObject votedCanvas;
    [SerializeField] private CanvasGroup taskManagerPickedDisplayCanvas;
    [SerializeField] private CanvasGroup randomlyVotedPlayerCanvas;
    [SerializeField] CanvasGroup symptomsNotifCanvas;
    [SerializeField] GameObject votingPhaseObject;
    Alteruna.Avatar avatar;

    [SynchronizableField] int randomlyPickedPlayer;
    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        player = GetComponent<PlayerRole>();
    }
    private void Start() {
        totalPlayers = RoleAssignment.GetTotalPlayers();
    }


    private bool hasVoted = false;
    public void InitiateVotingPhase() {
        if (!avatar.IsMe) { return; }

        votingCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.VotedCount = 0;
        hasVoted = false;

            if (player.IsTaskManager) { // Player who was task manager in the previous round can't be it again
                player.IsTaskManager = false;
            player.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.RemoveGun, this);
            } else {

                int i = 0;
                foreach(PlayerRole otherPlayer in totalPlayers)
                {
                    if (otherPlayer == player) { continue; }
                i++;

                GameObject newPlayerVoteOption = Instantiate(playerVoteButton, votingCanvas.transform);
                    newPlayerVoteOption.GetComponentInChildren<TMP_Text>().text = otherPlayer.gameObject.name;

                    RectTransform rect = newPlayerVoteOption.GetComponent<RectTransform>();
                    rect.anchoredPosition += rect.anchoredPosition * i;

                    newPlayerVoteOption.GetComponent<Button>().onClick.AddListener(() => {
                            otherPlayer.VotedCount++;
                            votingCanvas.SetActive(false);
                            votedCanvas.SetActive(true);
                            hasVoted = true;
                    });
                }
            }

            player.gameObject.GetComponent<PlayerController>().MovementEnabled = false; // Disable movement until end of voting phase
        }



    //if highest ppl have equal votes, then sb is picked at random
    //give gun to CEO through the specialInteractionSystem

    //randomly votred

    [SynchronizableField] string taskManagerNameInHost = "";
    public void EndVotingPhase()
    {
        if (!avatar.IsMe) { return; }

        if (!hasVoted)
        {

        }


        votingCanvas.SetActive(false);
        votedCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.gameObject.GetComponent<PlayerController>().MovementEnabled = true; // Enable movement again

        if (Multiplayer.GetUser().IsHost) EndVotingPhaseHost();

        pickedPlayerNameText.text = totalPlayers[0].gameObject.GetComponent<VotingPhase>().taskManagerNameInHost;

        StartCoroutine(DisplayTaskManager());
        StartCoroutine(DisplaySymptomNotif());

        //heldObject = smth
    }
    //local script only accessed by the host, touchin
    private void EndVotingPhaseHost()
    {
        PlayerRole pickedPlayer = totalPlayers[0];
        List<PlayerRole> equallyVotedPlayers = new List<PlayerRole>();

        for (int i = 1; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == player) { continue; } //same player

            if (totalPlayers[i].VotedCount > pickedPlayer.VotedCount) //more votes
            {
                pickedPlayer = totalPlayers[i];
                equallyVotedPlayers.Clear();
                equallyVotedPlayers.Add(pickedPlayer);
            }

            if (totalPlayers[i].VotedCount == pickedPlayer.VotedCount) //equivotes
            {
                equallyVotedPlayers.Add(totalPlayers[i]);
            }
        }


        if (equallyVotedPlayers.Count > 1)
        {
            randomlyPickedPlayer = Random.Range(0, equallyVotedPlayers.Count);
            pickedPlayer = equallyVotedPlayers[randomlyPickedPlayer];
            Debug.Log(Multiplayer.GetUser().Index);
        }

            Debug.Log("yes " + pickedPlayer.gameObject.name);
            pickedPlayer.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.GivenTaskManagerRole, this);
        


        pickedPlayer.IsTaskManager = true;
        StartCoroutine(DisplayRandomlyVotedCanvas());
        taskManagerNameInHost = pickedPlayer.gameObject.name;
    }

    private void VoteRandomly()
    {
        List<PlayerRole> votableCandidates = new List<PlayerRole>();
        for (int i = 1; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == player) { continue; }
            votableCandidates.Add(totalPlayers[i]);
        }

        int randomlyPickedPlayerIndex = Random.Range(0, votableCandidates.Count);
        PlayerRole randomlyVotedPlayer = votableCandidates[randomlyPickedPlayerIndex];
        //trigger canvas
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
    private IEnumerator DisplayRandomlyVotedCanvas()
    {
        randomlyVotedPlayerCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        randomlyVotedPlayerCanvas.DOFade(0f, 1f);
    }

}
