using System.Collections.Generic;
using UnityEngine;

public class SymptomsManager : MonoBehaviour {
    [SerializeField] private GameObject symptomUICanvas;
    [SerializeField] private List<SymptomsSO> listOfSymptoms;
    private List<SymptomsSO> caughtSymptoms = new();

    private void Update() {
        ToggleCanvas();
        GiveSymptom();
    }

    private void ToggleCanvas() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            symptomUICanvas.SetActive(true);
        }

        if(Input.GetKeyUp(KeyCode.Tab)) {
            symptomUICanvas.SetActive(false);
        }
    }

    private void GiveSymptom() {
        if(symptomUICanvas.activeSelf && listOfSymptoms.Count > 0) {
            if(Input.GetKeyDown(KeyCode.Alpha1) && !caughtSymptoms.Contains(listOfSymptoms[0])) {
                caughtSymptoms.Add(listOfSymptoms[0]);
                Debug.Log($"You got symptom: {listOfSymptoms[0].Name}");
            }

            if(Input.GetKeyDown(KeyCode.Alpha2) && !caughtSymptoms.Contains(listOfSymptoms[1])) {
                caughtSymptoms.Add(listOfSymptoms[1]);
                Debug.Log($"You got symptom: {listOfSymptoms[1].Name}");
            }

            if(Input.GetKeyDown(KeyCode.Alpha0) && caughtSymptoms.Count > 0) {
                caughtSymptoms.Clear();
                Debug.Log("Cleared all symptoms!");
            }
        }
    }
}
