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
    [SerializeField] private GameObject randomlyVotedPlayer;
    [SerializeField] CanvasGroup symptomsNotifCanvas;
    [SerializeField] GameObject votingPhaseObject;
    Alteruna.Avatar avatar;


    int randomlyPickedPlayer;
    List<VotingPhase> votingPlayers;
    [SynchronizableField] int pickedPlayerIndex;
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
        totalPlayers = RoleAssignment.GetTotalPlayers();

        if (!avatar.IsMe) { return; }
        //if (totalPlayers.Count <= 1) { return; }


        votingPlayers = FindObjectsByType<VotingPhase>(FindObjectsSortMode.None).ToList<VotingPhase>();

        votingCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.VotedCount = 0;
        hasVoted = false;

       // player.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.RemoveGun, this);

        if (player.IsTaskManager) { // Player who was task manager in the previous round can't be it again
                player.IsTaskManager = false;
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

    [SynchronizableField, HideInInspector] public string taskManagerNameInHost = "";
    public void EndVotingPhase()
    {
        if (!avatar.IsMe) { return; }
       // if (totalPlayers.Count <= 1) { return; }

        if (!hasVoted)
        {
            hasVoted = true;
            VoteRandomly();
        }


        votingCanvas.SetActive(false);
        votedCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.gameObject.GetComponent<PlayerController>().MovementEnabled = true; // Enable movement again

        if (avatar.IsMe && Multiplayer.GetUser().IsHost) EndVotingPhaseHost();
    }


    private void EndVotingPhaseHost()
    {
        if (!Multiplayer.GetUser().IsHost || RoleAssignment.playerID-1!=0) { return; }
        if (!avatar.IsMe) { return; }

            PlayerRole pickedPlayer = totalPlayers[0];
        List<PlayerRole> equallyVotedPlayers = new List<PlayerRole>();


        for (int i = 1; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == player) { continue; } //same player

            if (totalPlayers[i].VotedCount > pickedPlayer.VotedCount) //more votes
            {
                pickedPlayer = totalPlayers[i];
                equallyVotedPlayers.Clear();
                equallyVotedPlayers.Add(totalPlayers[i]);
            }

            if (totalPlayers[i].VotedCount == pickedPlayer.VotedCount) //equivotes
            {
                equallyVotedPlayers.Add(totalPlayers[i]);
            }
        }

        randomlyPickedPlayer = 0;
        randomlyPickedPlayer = Random.Range(0, equallyVotedPlayers.Count);
        pickedPlayer = equallyVotedPlayers[randomlyPickedPlayer];
        pickedPlayer.IsTaskManager = true;

        for (int i = 0; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == pickedPlayer)  pickedPlayerIndex = i;
        }
        Debug.Log("DADADA " + gameObject.name + Multiplayer.GetUser().Name);

        foreach (VotingPhase voter in votingPlayers)
        {
            voter.taskManagerNameInHost = pickedPlayer.gameObject.name;
            voter.pickedPlayerIndex = pickedPlayerIndex;
            voter.BroadcastRemoteMethod(nameof(voter.EndVotingPhaseFinale));
        }
    }

    [SynchronizableMethod]
    public void EndVotingPhaseFinale() //needs to be here bc of sequencing errors
    {
        if(!avatar.IsMe) { return; }
        pickedPlayerNameText.text = taskManagerNameInHost;
        if (player.IsTaskManager)
        {
            Debug.Log("DUDUDUUD");
            GetComponent<Interact>().SpecialInteraction(InteractionEnum.GivenTaskManagerRole, this);
        }
        StartCoroutine(DisplayTaskManager());
        StartCoroutine(DisplaySymptomNotif());
    }

    private void VoteRandomly()
    {
        List<PlayerRole> votableCandidates = new List<PlayerRole>();
        for (int i = 0; i < totalPlayers.Count; i++)
        {
            if (totalPlayers[i] == player) { continue; }
            votableCandidates.Add(totalPlayers[i]);
        }


         randomlyPickedPlayer = Random.Range(0, votableCandidates.Count);
        PlayerRole randomlyVotedPlayer = votableCandidates[randomlyPickedPlayer];
        randomlyVotedPlayer.VotedCount++;

        Debug.Log("randomly Chosen " + randomlyVotedPlayer.gameObject.name);

        StartCoroutine(DisplayRandomlyVotedCanvas());
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
        randomlyVotedPlayer.SetActive(true);
        yield return new WaitForSeconds(6f); // How many seconds to display it on screen
        randomlyVotedPlayer.SetActive(false);
    }

}
