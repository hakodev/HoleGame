using System.Collections;
using TMPro;
using UnityEngine;
using Alteruna;
using System.Collections.Generic;


public class CountdownDisplay : AttributesSync {

    [SerializeField] private int secondsRemainingToTurnRed;

    [SynchronizableField] public int time;
    public int maxTime;
    [SynchronizableField] static Color countdownColor = Color.green;
    [SynchronizableField] static bool hasInitiatedTheScreen = false;

    private List<GameObject> totalPlayers = new List<GameObject>();
    private List<VotingPhase> playerVotingPhase = new List<VotingPhase>();

    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;


    private void Awake() {
        maxTime = time;
    }
    private void Start()
    {
        StartCoroutine(CheckIfGameStarted());
    }
    private IEnumerator CheckIfGameStarted()
    {
        while (!hasInitiatedTheScreen)
        {
            yield return new WaitForSeconds(1);


            if (RoleAssignment.hasGameStarted && !hasInitiatedTheScreen)
            {
                List<PlayerRole> temp = RoleAssignment.GetTotalPlayers();
                foreach (PlayerRole role in temp)
                {
                    totalPlayers.Add(role.gameObject);
                    playerVotingPhase.Add(totalPlayers[totalPlayers.Count - 1].GetComponent<VotingPhase>());
                }


                hasInitiatedTheScreen = true;

               if(gameObject.activeSelf) StartCoroutine(TickDown());
                Debug.Log("calling the damn tickdown");
            }
        }
    }


    //these are meant to be called from the same object to itself so just use BoradcastRemoteMethod("nameofthing")
    [SynchronizableMethod]
    private void DeactivateUnusedTimers()//(string deactivatedObject)
    {
        gameObject.SetActive(false);
    }


    [SynchronizableMethod]
    private void ActivateTimer()
    {
        CountdownDisplay affectedDisplay = null;
        if (gameObject == StartDowntime)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
        }
        if (gameObject == PickTaskManager)
        {
            CountDown.SetActive(true);
            affectedDisplay = CountDown.GetComponent<CountdownDisplay>();
        }
        if (gameObject == CountDown)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
        }


        time = maxTime;
        affectedDisplay.time = affectedDisplay.maxTime;
        if (affectedDisplay.gameObject.activeSelf) StartCoroutine(affectedDisplay.TickDown());
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

    //check voting phase
    public IEnumerator TickDown() {
        Debug.Log("start of tick " + gameObject.name + " " + time + " " + maxTime);
        Debug.Log(time);
        if(time > 0) Debug.Log("wha");
        while(time > 0) {   
            Debug.Log("timer tickin");
            yield return new WaitForSeconds(1);
            Debug.Log("timer tickin");

            time--;
            UpdateUI();
        }

        BroadcastRemoteMethod(nameof(ActivateTimer));
        BroadcastRemoteMethod(nameof(DeactivateUnusedTimers));
        /*
        foreach(VotingPhase player in playerVotingPhase)
        {
            player.BroadcastRemoteMethod("InitiateVotingPhase");
        }
        */
        yield break;
    }
}
