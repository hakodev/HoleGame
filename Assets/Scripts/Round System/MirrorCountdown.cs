using TMPro;
using UnityEngine;

public class MirrorCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField]TextMeshProUGUI flavorText;
    [SerializeField]TextMeshProUGUI roundNumberText;


    private void LateUpdate()
    {
        countdown.text = CountdownDisplay.sendTimeToUI.ToString();
        flavorText.text = CountdownDisplay.sendFlavorTextToUI;
        countdown.color = CountdownDisplay.countdownColor;
        roundNumberText.text = CountdownDisplay.sendRoundsLeft.ToString();
    }
}