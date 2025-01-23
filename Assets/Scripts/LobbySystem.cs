using System.Collections;
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
        StopCoroutine(TimedDisplay());
        StopCoroutine(NoSymptomDisplay());
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        StartCoroutine(TimedDisplay());
    }

    private void DisplayNoSymptomText() {
        StopCoroutine(TimedDisplay());
        StopCoroutine(NoSymptomDisplay());
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        StartCoroutine(NoSymptomDisplay());
    }

    private IEnumerator TimedDisplay() {
        symptomNotifText.text = "Simulating new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";
        yield return new WaitForSeconds(10f);
        symptomNotifText.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }

    private IEnumerator NoSymptomDisplay() {
        symptomNotifText.text = "Removed all symptoms!";
        yield return new WaitForSeconds(5f);
        symptomNotifText.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }
}
