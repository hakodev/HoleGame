using TMPro;
using UnityEngine;

public class MirrorCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdown;

    private void Update()
    {
        countdown.text = CountdownDisplay.sendTimeToUI.ToString();
    }
}
