using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownDisplayManager : AttributesSync {

    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;


    private List<GameObject> totalPlayers = new List<GameObject>();
    private List<VotingPhase> playerVotingPhase = new List<VotingPhase>();

    [SynchronizableField] static bool hasInitiatedTheScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

                if (gameObject.activeSelf)
                {
                    StartCoroutine(StartDowntime.GetComponent<CountdownDisplay>().TickDown());
                }
                Debug.Log("calling the damn tickdown");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
     
        
    }

    [SynchronizableMethod]
    private void DeactivateUnusedTimers()//(string deactivatedObject)
    {
        gameObject.SetActive(false);
    }


    [SynchronizableMethod]
    public void ActivateTimer(string objectCalled)
    {
        
        CountdownDisplay affectedDisplay = null;
        if (objectCalled == StartDowntime.name)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
        }
        if (objectCalled == PickTaskManager.name)
        {
            CountDown.SetActive(true);
            affectedDisplay = CountDown.GetComponent<CountdownDisplay>();
        }
        if (objectCalled == CountDown.name)
        {
            PickTaskManager.SetActive(true);
            affectedDisplay = PickTaskManager.GetComponent<CountdownDisplay>();
        }
        


        affectedDisplay.time = affectedDisplay.maxTime;

        StartCoroutine(affectedDisplay.TickDown());
        
            
    }
}
