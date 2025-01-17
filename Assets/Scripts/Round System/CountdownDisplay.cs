using System.Collections;
using TMPro;
using UnityEngine;
using Alteruna;


public class CountdownDisplay : AttributesSync {

    [SerializeField] private int secondsRemainingToTurnRed;

    [SynchronizableField] public int time;
    public static int sendTimeToUI;

    private float deltaTime=0;
    public int maxTime;
    [SynchronizableField] public static Color countdownColor = Color.green;


    [SerializeField] TextMeshProUGUI countdown;

    [SerializeField] private CountDownDisplayManager manager;




    private void Awake() {
        maxTime = time;
    }
   


    //these are meant to be called from the same object to itself so just use BoradcastRemoteMethod("nameofthing")
    [SynchronizableMethod]
    private void DeactivateUnusedTimers()//(string deactivatedObject)
    {
        gameObject.SetActive(false);
    }

    [SynchronizableMethod]
    private void InitiateVotingPhaseForAllPlayers()
    {
        VotingPhase[] allVotingPhases = FindObjectsByType<VotingPhase>(FindObjectsSortMode.None);
        foreach (VotingPhase player in allVotingPhases)
        {
            Debug.Log(player.gameObject.name);
            player.InitiateVotingPhase();
        }
    }
    [SynchronizableMethod]
    private void EndVotingPhaseForAllPlayers()
    {
        VotingPhase[] allVotingPhases = FindObjectsByType<VotingPhase>(FindObjectsSortMode.None);
        foreach (VotingPhase player in allVotingPhases)
        {
            Debug.Log(player.gameObject.name);
            player.EndVotingPhase();
        }
    }

    private void Update() {
        if (CountDownDisplayManager.hasInitiatedTheTimer)
        {
            UpdateTickDown();
            UpdateUI();
            sendTimeToUI = time;
        }
    }

    
    private void UpdateUI()
    {
        countdown.text = time.ToString();

        if (time <= secondsRemainingToTurnRed)
        {
            countdownColor = Color.red;
        }
        else
        {
            countdownColor = Color.green;
        }
        countdown.color = countdownColor;
    }

    private void UpdateTickDown()
    {
        if (time > 0) 
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= 1)
            {
                deltaTime = 0;
                if (Multiplayer.GetUser().IsHost) time--;
                //Debug.Log(gameObject.name);
            }
        }
        else
        {
            manager.BroadcastRemoteMethod("ActivateTimer", parameters: gameObject.name);
            BroadcastRemoteMethod(nameof(DeactivateUnusedTimers));
            
            if(gameObject.tag == "DowntimeDisplay") BroadcastRemoteMethod(nameof(InitiateVotingPhaseForAllPlayers));
            if (gameObject.tag == "VotingDisplay") BroadcastRemoteMethod(nameof(EndVotingPhaseForAllPlayers));
        }
    }
}
