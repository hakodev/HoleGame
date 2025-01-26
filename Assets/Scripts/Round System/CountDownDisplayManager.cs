using Alteruna;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownDisplayManager : AttributesSync {

    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;

    public static bool hasInitiatedTheTimer = false;

    [SynchronizableField] public int TimeToEndTheGame = 60 * 6 + 20 * 5;


    private void Start()
    {
        hasInitiatedTheTimer = false;
        StartCoroutine(CheckIfGameStarted());
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



        affectedDisplay.time = affectedDisplay.maxTime;  
    }
}

