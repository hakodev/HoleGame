using TMPro;
using UnityEngine;

public class UIDisplayCountDown : MonoBehaviour
{
    TextMeshProUGUI countdown;
    private int UIDisplayedTime;

    private void Awake()
    {
        countdown = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        UpdateDisplayedTime();
        UpdateUI();
    }

    private void UpdateDisplayedTime()
    {
        UIDisplayedTime = CountdownDisplay.sendTimeToUI;
    }
    private void UpdateUI()
    {
        countdown.color = CountdownDisplay.countdownColor;
        countdown.text = UIDisplayedTime.ToString();
    }
}
