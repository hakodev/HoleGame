using Alteruna;
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
    [SynchronizableField] private int randNum;
    Alteruna.Avatar avatar;
    float renderDistanceTimer = 2;
    RenderDistanceSymptom renderDistanceSymptom = null;

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

    public void SetSymptom(int index) {

        if(GetSymptom() == GetSymptomsList()[2] && avatar.gameObject.GetComponent<PlayerRole>().GetRole() == Roles.Machine)
            renderDistanceSymptom.SetRenderDistanceBack();

        currentSymptom = index == 999 ? null : symptoms[index];

        if (GetSymptom() == GetSymptomsList()[2] && avatar.gameObject.GetComponent<PlayerRole>().GetRole() == Roles.Machine)
            renderDistanceSymptom.SetRenderDistance();
    }

    public int GetRandomNum() {
        randNum = Random.Range(0, symptoms.Count);
        return randNum;
    }

    public void SetRenderDistanceSymptom(RenderDistanceSymptom symptom)
    {
        avatar = Multiplayer.GetAvatar();
        if (!avatar.IsMe) return;
        renderDistanceSymptom = symptom;
    }

}
