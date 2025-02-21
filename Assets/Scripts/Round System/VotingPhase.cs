using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
public class VotingPhase : AttributesSync {

    public static List<PlayerRole> totalALivePlayers = new List<PlayerRole>();
    private PlayerRole player;

    [SerializeField] private GameObject playerVoteButton;
    [SerializeField] TextMeshProUGUI pickedPlayerNameText;
    [SerializeField] private GameObject votingCanvas;
    PopUp votingPopUp;

    [SerializeField] private GameObject votedCanvas;
    [SerializeField] private CanvasGroup taskManagerPickedDisplayCanvas;
    [SerializeField] private GameObject randomlyVotedPlayer;
    [SerializeField] GameObject symptomsNotifCanvas;
    [SerializeField] GameObject votingPhaseObject;
    EndGameResolution endGameResolution;
    Alteruna.Avatar avatar;
    CursorToggle cursorToggle;


    int randomlyPickedPlayer;
    public static List<VotingPhase> votingPlayers = new List<VotingPhase>();
    [SynchronizableField] int pickedPlayerIndex;

    private bool hasVoted = false;
    [SynchronizableField, HideInInspector] public string taskManagerNameInHost = "";

    Spawner spawner;

    public static void StaticReset()
    {
        totalALivePlayers.Clear();
    }
    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        player = GetComponent<PlayerRole>();
        cursorToggle = GetComponent<CursorToggle>();
    }
    private void Start() {

        spawner = FindAnyObjectByType<Alteruna.Spawner>();
        endGameResolution = GetComponentInChildren<EndGameResolution>();
        votingPopUp = votingCanvas.GetComponentInChildren<PopUp>();

        totalALivePlayers.Add(player);
        votingPlayers.Add(this);

    }

    public bool once = false;
    bool wildWestPopUpOnce = false;
    public void InitiateVotingPhase() {
        if (!avatar.IsMe) { return; }
        endGameResolution.CheckForEndGame();
        if(!endGameResolution.inWildWest) DespawnAllGuns();

        if (!wildWestPopUpOnce) { //ensures that the popup for voting doesnt appear if this is hte second time they're gonna get it
            if(endGameResolution.inWildWest) wildWestPopUpOnce = true;

            //if (totalPlayers.Count <= 1) { return; }
            votingCanvas.SetActive(true);
            votingPopUp.PopIn();

            cursorToggle.UICursorAndCam(true);
            player.VotedCount = 0;
            hasVoted = false;

            if (!endGameResolution.inWildWest) player.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.RemoveGun, this);

            if (player.cannotVoteVorWasTaskManager) player.cannotVoteVorWasTaskManager = false;
            if (player.IsTaskManager)
            { // Player who was task manager in the previous round can't be it again
                player.IsTaskManager = false;
                player.cannotVoteVorWasTaskManager = true;
            }

            if (!endGameResolution.inWildWest) SpawnVotingButtons();
        }
    }


    private void SpawnVotingButtons()
    {
        //StartCoroutine(WaitForButtons());
        //

        Health playerHealth = avatar.GetComponent<Health>();
        if (playerHealth.GetHealth() <= 0) { return; }

        int i = 0;
        foreach (PlayerRole otherPlayer in totalALivePlayers)
        {
            if (otherPlayer == player) { continue; }
            if (totalALivePlayers.Count > 3 && otherPlayer.cannotVoteVorWasTaskManager) { continue; }

            Debug.Log("pls4 " + totalALivePlayers.Count + " " + otherPlayer.gameObject.name);
            i++;

            GameObject newPlayerVoteOption = Instantiate(playerVoteButton, votingPopUp.transform);
            newPlayerVoteOption.GetComponentInChildren<TextMeshProUGUI>().text = otherPlayer.gameObject.name;
            allButtons.Add(newPlayerVoteOption);

            RectTransform rect = newPlayerVoteOption.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -80 * i + 120);

            if (i % 2 == 0)
            {
                //  rect.anchorMin = new Vector2(0f, -0.5f);
                //   rect.anchorMax = new Vector2(0f, -0.5f);
                rect.anchoredPosition = new Vector2(734, rect.anchoredPosition.y + 80);
            }


            newPlayerVoteOption.GetComponent<Button>().onClick.AddListener(() => {
                otherPlayer.VotedCount++;
                votingCanvas.SetActive(false);
                votedCanvas.SetActive(true);
                hasVoted = true;
                //Debug.Log("BITTE_Button " + otherPlayer.name);
            });
        }
    }
    public static void RemoveTotalAlivePlayers(PlayerRole role)
    {
        Debug.Log("pls " + totalALivePlayers.Count + role.gameObject.name);
        totalALivePlayers.Remove(role);
        votingPlayers.Remove(role.GetComponent<VotingPhase>());
        Debug.Log("pls2 " + totalALivePlayers.Count + role.gameObject.name);
    }
    List<GameObject> allButtons = new List<GameObject>();
    private IEnumerator WaitForButtons()
    {
        yield return new WaitForSeconds(0.01f);

        if (endGameResolution.inWildWest) {

        }
        else
        {
           
        }       
    }

    public void EndVotingPhase()
    {
        if (!avatar.IsMe) { return; }

       // Debug.Log("BITTE_EndVotingPhase " + avatar.name);

        // if (totalPlayers.Count <= 1) { return; }

        if (!hasVoted)
        {
            hasVoted = true;
            VoteRandomly();
        }


        votingCanvas.SetActive(false);
        votedCanvas.SetActive(false);
        cursorToggle.UICursorAndCam(false);
      //  player.gameObject.GetComponent<PlayerController>().MovementEnabled = true; // Enable movement again

        EndVotingPhaseHost();
    }


    private void EndVotingPhaseHost()
    {
        if (!Multiplayer.GetUser().IsHost) { return; }

       // Debug.Log("BITTE_Host " + avatar.name);


        PlayerRole pickedPlayer = totalALivePlayers[0];
        List<PlayerRole> equallyVotedPlayers = new List<PlayerRole>();
        equallyVotedPlayers.Add(pickedPlayer);


        for (int i = 1; i < totalALivePlayers.Count; i++)
        {
            if (totalALivePlayers[i] == player) { continue; } //same player

            if (totalALivePlayers[i].VotedCount > pickedPlayer.VotedCount) //more votes
            {
                pickedPlayer = totalALivePlayers[i];
                equallyVotedPlayers.Clear();
                equallyVotedPlayers.Add(totalALivePlayers[i]);
            }

            if (totalALivePlayers[i].VotedCount == pickedPlayer.VotedCount) //equivotes
            {
                equallyVotedPlayers.Add(totalALivePlayers[i]);
            }
        }

        if (RoleAssignment.playerID - 1 == 0)
        {
            randomlyPickedPlayer = 0;
            randomlyPickedPlayer = Random.Range(0, equallyVotedPlayers.Count);
            pickedPlayer = equallyVotedPlayers[randomlyPickedPlayer];
            pickedPlayer.IsTaskManager = true;
            pickedPlayer.Commit();



            //Debug.Log("BITTE_PlayerName " + pickedPlayer.name + " " + pickedPlayer.IsTaskManager);
            taskManagerNameInHost = pickedPlayer.gameObject.name;
            //Debug.Log("pompous1 " + taskManagerNameInHost);

            for (int i = 0; i < equallyVotedPlayers.Count; i++)
            {
                if (equallyVotedPlayers[i] == pickedPlayer) pickedPlayerIndex = i;
            }
        }
        BroadcastRemoteMethod("EndVotingPhaseFinaleSync");
    }
    [SynchronizableMethod]
    private void EndVotingPhaseFinaleSync()
    {
        VotingPhase player = Multiplayer.GetAvatar().gameObject.GetComponent<VotingPhase>();
        //Debug.Log("BITTE_FinaleSync " + player.name);
        //Debug.Log("pompous2 " + taskManagerNameInHost);
        player.EndVotingPhaseFinale();
    }

    public void EndVotingPhaseFinale() //needs to be here bc of sequencing errors
    {
        if(!avatar.IsMe) { return; }
        if (endGameResolution.inWildWest)
        {
            endGameResolution.HandOutGuns();
            StartCoroutine(DisplaySymptomNotif());
        }
        else
        {
            //Debug.Log("BITTE_Finale " + avatar.name + " " + player.IsTaskManager);
            VotingPhase hostVoter = Multiplayer.GetAvatars()[0].GetComponent<VotingPhase>();
            taskManagerNameInHost = hostVoter.taskManagerNameInHost;

            pickedPlayerIndex = hostVoter.pickedPlayerIndex;
            pickedPlayerNameText.text = taskManagerNameInHost;

            //Debug.Log("BITTE_Finale2 " + taskManagerNameInHost + " " + pickedPlayerIndex    );

            if (player.IsTaskManager)
            {
                //Debug.Log(player.IsTaskManager + player.gameObject.name);
                GetComponent<Interact>().SpecialInteraction(InteractionEnum.GivenTaskManagerRole, this);
            }

            StartCoroutine(DisplayTaskManager());
            StartCoroutine(DisplaySymptomNotif());
        }
        SymptomsManager.Instance.PickRandNumberHostAndSetSymptomForAll();
        for(int i=0; i< allButtons.Count; i++)
        {
            Destroy(allButtons[i]); 
        }
    }

    private void VoteRandomly()
    {
        /*
        List<PlayerRole> votableCandidates = new List<PlayerRole>();
        for (int i = 0; i < totalALivePlayers.Count; i++)
        {
            if (totalALivePlayers[i] == player) { continue; }
            votableCandidates.Add(totalALivePlayers[i]);
        }


         randomlyPickedPlayer = Random.Range(0, votableCandidates.Count);
        PlayerRole randomlyVotedPlayer = votableCandidates[randomlyPickedPlayer];
        randomlyVotedPlayer.VotedCount++;

        //Debug.Log("randomly Chosen " + randomlyVotedPlayer.gameObject.name);
        */

        StartCoroutine(DisplayRandomlyVotedCanvas());
    }

    private IEnumerator DisplayTaskManager() {
        taskManagerPickedDisplayCanvas.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f); // How many seconds to display it on screen
        taskManagerPickedDisplayCanvas.DOFade(0f, 1f);
    }

    private IEnumerator DisplaySymptomNotif() {
        symptomsNotifCanvas.SetActive(true);
       // symptomsNotifCanvas.DOFade(1f, 0.1f);
        yield return new WaitForSeconds(15f); // How many seconds to display it on screen
      //  symptomsNotifCanvas.DOFade(0f, 2f);
        symptomsNotifCanvas.SetActive(false);
    }
    private IEnumerator DisplayRandomlyVotedCanvas()
    {
        Debug.Log("hoho2 " + Multiplayer.GetAvatar().name + " " + Multiplayer.GetUser().Name);

        randomlyVotedPlayer.SetActive(true);
        yield return new WaitForSeconds(6f); // How many seconds to display it on screen
        randomlyVotedPlayer.SetActive(false);
    }
    [SynchronizableMethod]
    private void DisplaySymptomNotifSync()
    {
        //Debug.Log("come on symptoms");
        if (!avatar.IsMe) { return; }
        StartCoroutine(DisplaySymptomNotif());
    }
    public void AllVotersSymptomNotifStartOfGame()
    {
        Debug.Log("hoho " + Multiplayer.GetAvatar().name + " " + Multiplayer.GetUser().Name);
        for (int i = 0; i < votingPlayers.Count; i++)
        {
            votingPlayers[i].BroadcastRemoteMethod(nameof(DisplaySymptomNotifSync));
        }
        BroadcastRemoteMethod(nameof(DisplaySymptomNotifSync));
    }

    public void DespawnAllGuns()
    {
        Gun[] allGuns = FindObjectsByType<Gun>(FindObjectsSortMode.None);
        //Debug.Log("Despawning guns " + allGuns.Length);

        for (int i=0; i<allGuns.Length; i++)
        {
            //Debug.Log("despawned gun " + allGuns[i].gameObject);
            spawner.Despawn(allGuns[i].gameObject);
        }
    }
}
