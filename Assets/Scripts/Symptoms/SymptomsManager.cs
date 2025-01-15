using Alteruna;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SymptomsManager : AttributesSync {
    Alteruna.Avatar avatar;
    public static SymptomsManager Instance { get; private set; }
    [Header("MAKE SURE THE SYMPTOMS ARE LISTED IN THE FOLLOWING ORDER:\n" +
            "1. DespawningItems\n" +
            "(more will be listed here as they get added)")]
    [SerializeField] private List<SymptomsSO> symptoms;
    private SymptomsSO currentSymptom;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
        currentSymptom = symptoms[0]; // test, remove later
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

    public void TriggerSymptom(GameObject heldObject = null) { // Setting it to null here so we can use it without an argument in other cases
        if(!avatar.IsMe) return;

        if(currentSymptom == symptoms[0]) { // DespawningItems
            heldObject.GetComponent<Collider>().enabled = false;
            heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f);
            Debug.Log("Despawn symptom triggered!");
        } else {
            Debug.Log("No symptom");
        }
    }
}
