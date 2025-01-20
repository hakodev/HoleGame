using Alteruna;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SymptomsManager : AttributesSync {
    public static SymptomsManager Instance { get; private set; }
    [Header("MAKE SURE THE SYMPTOMS ARE LISTED IN THE FOLLOWING ORDER!\n" +
            "Sym0\n" +
            "Sym1\n" +
            "Sym2")]
    [SerializeField] private List<SymptomsSO> symptoms;
    private SymptomsSO currentSymptom = null;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public SymptomsSO GetSymptom() {
        return currentSymptom;
    }

    public List<SymptomsSO> GetSymptomsList() {
        return symptoms;
    }

    [SynchronizableMethod]
    public void SetSymptom(SymptomsSO newSymptom) {
        currentSymptom = newSymptom;
    }

    public void TriggerSymptom(GameObject heldObject = null) {
        // Setting heldObject to null here so we can use this method
        // without an argument in cases where the symptom isn't Sym0

        if(currentSymptom == symptoms[0]) { // DespawningItems
            heldObject.GetComponent<Collider>().enabled = false;
            heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f);
            Debug.Log("Despawn symptom triggered!");
        } else if(currentSymptom == symptoms[1]) {
            // Code here
        } else if(currentSymptom == symptoms[2]) {
            // Code here
        } else {
            Debug.Log("No symptom to trigger");
        }
    }
}
