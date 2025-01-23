using DG.Tweening;
using TMPro;
using UnityEngine;

public class LobbySystem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI symptomNotifText;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            controlsText.enabled = !controlsText.enabled;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            SymptomsManager.Instance.SetSymptom(0); // Despawning items
            DisplayNotificationText();
        }

        if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            SymptomsManager.Instance.SetSymptom(999); // no symptom
            DisplayNoSymptomText();
        }
    }

    private void OnDestroy() {
        SymptomsManager.Instance.SetSymptom(999); // no symptom
    }

    private void DisplayNotificationText() {
        symptomNotifText.DOKill();
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        symptomNotifText.alpha = 1f;
        symptomNotifText.text = "Simulating new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";
        symptomNotifText.DOFade(0f, 10f).OnComplete(() => symptomNotifText.transform.parent.parent.gameObject.SetActive(false));
    }

    private void DisplayNoSymptomText() {
        symptomNotifText.DOKill();
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        symptomNotifText.alpha = 1f;
        symptomNotifText.text = "All symptoms removed!";
        symptomNotifText.DOFade(0f, 5f).OnComplete(() => symptomNotifText.transform.parent.parent.gameObject.SetActive(false));
    }
}
