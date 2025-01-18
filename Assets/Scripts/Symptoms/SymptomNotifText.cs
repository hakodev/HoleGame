using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymptomNotifText : MonoBehaviour {
    private TMP_Text notificationText;

    private void Awake() {
        notificationText = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        ApplyNewSymptom();
        DisplayNotificationText();
    }

    private void ApplyNewSymptom() {
        List<SymptomsSO> allSymptoms = SymptomsManager.Instance.GetSymptomsList();
        int randNum = Random.Range(0, allSymptoms.Count);
        SymptomsManager.Instance.SetSymptom(allSymptoms[randNum]);
        Debug.Log($"New symptom applied: {SymptomsManager.Instance.GetSymptom().Name}");
    }

    private void DisplayNotificationText() {
        notificationText.text = "The machines have caught a new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";
    }
}
