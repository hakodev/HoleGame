using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using System.Collections.Generic;

public class CountdownDisplay : AttributesSync {
    private TMP_Text countdown;

    [SerializeField] private int secondsRemainingToTurnRed;
    [SerializeField] private UnityEvent OnTimerEnd;

    [SynchronizableField][SerializeField] private int time;
    [SynchronizableField] static Color countdownColor = Color.green;
    bool hasInitiatedTheScreen = false;

    private List<GameObject> totalPlayers = new List<GameObject>();
    private List<VotingPhase> playerVotingPhase = new List<VotingPhase>();


    private void Awake() {
        countdown = GetComponent<TMP_Text>();
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

            if (RoleAssignment.hasGameStarted || !hasInitiatedTheScreen)
            {
                Debug.Log(RoleAssignment.hasGameStarted + " " + hasInitiatedTheScreen);
            }



            if (RoleAssignment.hasGameStarted && !hasInitiatedTheScreen)
            {
                Debug.Log("past if");

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
        Debug.Log(time + "should ivoke the player's things");

        foreach(VotingPhase player in playerVotingPhase)
        {
            player.BroadcastRemoteMethod("InitiateVotingPhase");
        }
    }
}
