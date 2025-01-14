using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownDisplay : MonoBehaviour {
    private TMP_Text countdown;
    [SerializeField] private int time;
    [SerializeField] private int secondsRemainingToTurnRed;
    [SerializeField] private UnityEvent OnTimerEnd;

    private void Awake() {
        countdown = GetComponent<TMP_Text>();
    }

    private void Start() {
        countdown.text = time.ToString();
        countdown.color = Color.green;
        StartCoroutine(TickDown());
    }

    private IEnumerator TickDown() {
        while(time > 0) {
            yield return new WaitForSeconds(1);
            time--;
            countdown.text = time.ToString();

            if(time <= secondsRemainingToTurnRed) {
                countdown.color = Color.red;
            }
        }

        OnTimerEnd?.Invoke();
    }
}
