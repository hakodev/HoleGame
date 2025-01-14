using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using System.Collections.Generic;

public class CountdownDisplay : AttributesSync {
    private TMP_Text countdown;
    [SynchronizableField] [SerializeField] private int time;
    [SynchronizableField] static Color countdownColor = Color.green;
    [SynchronizableField] bool hasInitiatedTheScreen = false;
    [SerializeField] private int secondsRemainingToTurnRed;
    [SerializeField] private UnityEvent OnTimerEnd;

    private List<PlayerRole> totalPlayers;


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

            if (RoleAssignment.hasGameStarted && !hasInitiatedTheScreen)
            {
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
        OnTimerEnd?.Invoke();

    }
}
