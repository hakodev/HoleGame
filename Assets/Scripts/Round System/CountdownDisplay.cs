using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using System.Collections.Generic;
using System.Threading;

public class CountdownDisplay : AttributesSync {
    private TMP_Text countdown;

    [SerializeField] private int secondsRemainingToTurnRed;

    [SynchronizableField][SerializeField] private int time;
    private int maxTime;
    [SynchronizableField] static Color countdownColor = Color.green;
    bool hasInitiatedTheScreen = false;

    private List<GameObject> totalPlayers = new List<GameObject>();
    private List<VotingPhase> playerVotingPhase = new List<VotingPhase>();

    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;

    Alteruna.Avatar avatarOwner;

    private void Awake() {
        countdown = GetComponent<TMP_Text>();
        maxTime = time;
     //   avatarOwner.OnPossessed.AddListener();

    }
    private void Start()
    {
        StartCoroutine(CheckIfGameStarted());
    }
    private IEnumerator CheckIfGameStarted()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

         //   if (RoleAssignment.hasGameStarted || !hasInitiatedTheScreen)
           // {
           //     Debug.Log(RoleAssignment.hasGameStarted + " " + hasInitiatedTheScreen);
           // }



            if (RoleAssignment.hasGameStarted && !hasInitiatedTheScreen)
            {
                List<PlayerRole> temp = RoleAssignment.GetTotalPlayers();
                foreach (PlayerRole role in temp) 
                {
                    totalPlayers.Add(role.gameObject);
                    playerVotingPhase.Add(totalPlayers[totalPlayers.Count - 1].GetComponent<VotingPhase>());
                }
                hasInitiatedTheScreen = true;
                countdown.text = time.ToString();
                countdown.color = countdownColor;
                StartCoroutine(TickDown());
            }
        }
    }

    
    public void StartNewTimer(string heheCorrectObject)
    {
        if(avatarOwner) { return; }
        if (heheCorrectObject != gameObject.name) { return; }

        Debug.Log(heheCorrectObject + " " + gameObject.name);

        StartCoroutine(TickDown());

        if (gameObject == PickTaskManager)
        {
            StartDowntime.SetActive(false);
            CountDown.SetActive(false);
        }
        if (gameObject == CountDown)
        {
            StartDowntime.SetActive(false);
            PickTaskManager.SetActive(false);
        }
    }


    private IEnumerator TickDown() {
        while(time > 0) {
            yield return new WaitForSeconds(1);
            time--;
            countdown.text = time.ToString();

            if(time <= secondsRemainingToTurnRed) {
                countdownColor = Color.red;
            }
            else
            {
                countdownColor = Color.green;
            }
            countdown.color = countdownColor;
        }

        foreach(VotingPhase player in playerVotingPhase)
        {
            player.BroadcastRemoteMethod("InitiateVotingPhase");
        }



        if (gameObject== StartDowntime)
        {
            PickTaskManager.SetActive(true);
            PickTaskManager.GetComponent<CountdownDisplay>().StartNewTimer("CountdownPickTaskMan_20");
        }

        if(gameObject== PickTaskManager)
        {
            CountDown.SetActive(true);
            //CountDown.GetComponent<CountdownDisplay>().BroadcastRemoteMethod("StartNewTimer", "CountdowntDowntime_60");
            CountDown.GetComponent<CountdownDisplay>().StartNewTimer("CountdowntDowntime_60");

        }

        if (gameObject == CountDown)
        {
            PickTaskManager.SetActive(true);
            //PickTaskManager.GetComponent<CountdownDisplay>().BroadcastRemoteMethod("StartNewTimer", "CountdownPickTaskMan_20");
            PickTaskManager.GetComponent<CountdownDisplay>().StartNewTimer("CountdownPickTaskMan_20");
        }

        time = maxTime;
        gameObject.SetActive(false); //just in case

    }
}
