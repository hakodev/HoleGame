using Alteruna;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownDisplayManager : AttributesSync {

    [SerializeField] GameObject StartDowntime;
    [SerializeField] GameObject PickTaskManager;
    [SerializeField] GameObject CountDown;

    public static bool hasInitiatedTheTimer = false;

    [field: SerializeField][field: SynchronizableField] public int RoundsLeft { get; set; } = 2;
    public string lastChanceText = "End Round";

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

        DeactivateUnusedTimer(objectCalled);
    }
    public void DeactivateUnusedTimer(string objectCalled)
    {
        if (objectCalled == StartDowntime.name) StartDowntime.SetActive(false);
        if (objectCalled == PickTaskManager.name) PickTaskManager.SetActive(false);
        if (objectCalled == CountDown.name) CountDown.SetActive(false);
    }
}

