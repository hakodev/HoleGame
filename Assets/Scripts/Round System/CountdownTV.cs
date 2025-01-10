using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTV : MonoBehaviour {
    private TMP_Text countdown;
    private int time = 60;

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

            if(time <= 20) {
                countdown.color = Color.red;
            }
        }
    }
}
