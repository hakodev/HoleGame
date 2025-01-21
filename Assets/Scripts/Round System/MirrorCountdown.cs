using TMPro;
using UnityEngine;

public class MirrorCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdown;
    TextMeshProUGUI flavorText;

    private void Start()
    {
        flavorText = transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        countdown.text = CountdownDisplay.sendTimeToUI.ToString();
              flavorText.text = CountdownDisplay.sendFlavorTextToUI;
              countdown.color = CountdownDisplay.countdownColor;
    }
}