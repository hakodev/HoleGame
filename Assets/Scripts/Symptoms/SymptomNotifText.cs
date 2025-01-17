using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymptomNotifText : MonoBehaviour {
    private TMP_Text notificationText;
    private VotingPhase phase;
    private SymptomsSO currentSymptom;
    [SerializeField] private List<SymptomsSO> allSymptoms;

    private void Awake() {
        notificationText = GetComponent<TMP_Text>();
        phase = FindFirstObjectByType<VotingPhase>();
    }

    private void OnEnable() {
        ApplyNewSymptom();
        DisplayNotificationText();
    }

    private void ApplyNewSymptom() {
        currentSymptom = GetCurrentSymptom();
    }

    private void DisplayNotificationText() {
        notificationText.text = "New Symptom Caught!\n\n" +
                               $"{currentSymptom.Name}\n" +
                               $"{currentSymptom.Description}\n\n";
    }

    private SymptomsSO GetCurrentSymptom() {
        int randNum = Random.Range(0, allSymptoms.Count);
        return allSymptoms[randNum];
    }
}
