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
    [SynchronizableField] int randNum; //should only be modified by host

    Alteruna.Avatar avatar;
    SymptomNotifText thisAvatarSymptomNotifText;
    float renderDistanceTimer = 2;



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

    public void SymptomNotifTextWasSpawned(SymptomNotifText caller)
    {
        avatar = caller.transform.root.GetComponent<Alteruna.Avatar>();
        Debug.Log("health " + avatar.name);
        thisAvatarSymptomNotifText = caller;
    }

    [SynchronizableMethod]
    public void SetSymptom(int index) {

        currentSymptom = symptoms[index];
        Debug.Log("healthcare2 " + Multiplayer.GetUser().Name + " " + Multiplayer.GetAvatar().name);

        Debug.Log("healthcare avatartext " + thisAvatarSymptomNotifText + avatar.name);
        Debug.Log("healthcare sequencing " + Multiplayer.GetUser().Name + " " + Multiplayer.GetAvatar().name);
        thisAvatarSymptomNotifText.ApplyEffectsOfSymptom();
        thisAvatarSymptomNotifText.ChangeNotifText();
        Debug.Log("healthcare sequencing " + Multiplayer.GetUser().Name + " " + Multiplayer.GetAvatar().name);

    }

    public void PickRandNumberHostAndSetSymptomForAll() {
        if (!Multiplayer.GetUser().IsHost) { return; }

        Debug.Log("healthcare " + Multiplayer.GetUser().Name + " " + Multiplayer.GetAvatar().name);
        randNum = Random.Range(0, symptoms.Count);
        BroadcastRemoteMethod(nameof(SetSymptom), randNum);
    }

    public int GetRandNumber() { return randNum; }

}
