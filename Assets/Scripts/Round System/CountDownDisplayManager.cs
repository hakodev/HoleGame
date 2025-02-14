using Alteruna;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountDownDisplayManager : AttributesSync {

    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;

    CountdownDisplay currentDisplay;

    public static bool hasInitiatedTheTimer = false;

    [field: SerializeField][field: SynchronizableField] public int RoundsLeft { get; set; } = 2;
    public string lastChanceText = "End Round";

    public static CountDownDisplayManager Instance { get; private set; }

    private int playersMaxCount;
    private void Start()
    {
        Instance = this;
        hasInitiatedTheTimer = false;
        StartCoroutine(CheckIfGameStarted());
        currentDisplay = StartDowntime.GetComponentInChildren<CountdownDisplay>();
    }
    private IEnumerator CheckIfGameStarted()
    {
        while (!hasInitiatedTheTimer)
        {
            yield return new WaitForSeconds(1);

            if (RoleAssignment.playerID != 1) { continue; }

            if (RoleAssignment.hasGameStarted && !hasInitiatedTheTimer)
            {

                BroadcastRemoteMethod(nameof(StartGameForAll));
            }
        }
    }

    [SynchronizableMethod]
    private void StartGameForAll()
    {
        hasInitiatedTheTimer = true;
        playersMaxCount = VotingPhase.totalALivePlayers.Count;
        RoundsLeft = playersMaxCount+1;
        Debug.Log("aaa " + playersMaxCount);
        Commit();
    }

    [SynchronizableMethod]
    public void ActivateTimer(string objectCalled)
    {
        
        CountdownDisplay affectedDisplay = null;
        if (objectCalled == StartDowntime.name)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
            CountdownDisplay.sendFlavorTextToUI = affectedDisplay.transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>().text;
        }
        if (objectCalled == PickTaskManager.name)
        {
            CountDown.SetActive(true);
            affectedDisplay = CountDown.GetComponent<CountdownDisplay>();
            CountdownDisplay.sendFlavorTextToUI = affectedDisplay.transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>().text;

        }
        if (objectCalled == CountDown.name)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
            CountdownDisplay.sendFlavorTextToUI = affectedDisplay.transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>().text;
        }


        currentDisplay = affectedDisplay;
        affectedDisplay.time = affectedDisplay.maxTime;

        /*
        if (objectCalled == CountDown.name)
        {
           affectedDisplay.time = affectedDisplay.time * (VotingPhase.totalALivePlayers.Count / playersMaxCount);
        }
        */


        DeathCheck(affectedDisplay);
        DeactivateUnusedTimer(objectCalled);
    }
    public void DeathCheck(CountdownDisplay affectedDisplay)
    {
        
       // if (VotingPhase.totalALivePlayers.Count == 2)
      //  {
            Multiplayer.GetAvatar().GetComponentInChildren<EndGameResolution>().CheckForEndGame();
            //affectedDisplay.time = 5;
      //  }
        
    }
    public void DeathCheck()
    {
        /*
        if (VotingPhase.totalALivePlayers.Count == 2)
        {
            //Multiplayer.GetAvatar().GetComponentInChildren<EndGameResolution>().CheckForEndGame();
            currentDisplay.time = 5;
        }
        */
    }
    public void DeactivateUnusedTimer(string objectCalled)
    {
        if (objectCalled == StartDowntime.name) StartDowntime.SetActive(false);
        if (objectCalled == PickTaskManager.name) PickTaskManager.SetActive(false);
        if (objectCalled == CountDown.name) CountDown.SetActive(false);
    }
}

