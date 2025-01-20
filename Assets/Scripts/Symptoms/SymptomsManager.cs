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
}
