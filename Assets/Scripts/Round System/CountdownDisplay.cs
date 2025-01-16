using System.Collections;
using TMPro;
using UnityEngine;
using Alteruna;
using System.Collections.Generic;
using Unity.VisualScripting;


public class CountdownDisplay : AttributesSync {

    [SerializeField] private int secondsRemainingToTurnRed;

    [SynchronizableField] public int time;
    public int maxTime;
    [SynchronizableField] static Color countdownColor = Color.green;


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

        while(time > 0)
        {
            time--;
            UpdateUI();
            yield return new WaitForSeconds(1);
        }

        manager.BroadcastRemoteMethod("ActivateTimer", parameters: gameObject.name);

        time = maxTime;
        BroadcastRemoteMethod(nameof(DeactivateUnusedTimers));
        /*
        foreach(VotingPhase player in playerVotingPhase)
        {
            player.BroadcastRemoteMethod("InitiateVotingPhase");
        }
        */

    }
}
