using Alteruna;
using System.Collections;
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



    [SynchronizableMethod]
    public void SetSymptom(int index) {

        currentSymptom = symptoms[index];
        if (avatar == null)
        {
            avatar = Multiplayer.GetAvatar();
            thisAvatarSymptomNotifText = avatar.transform.GetComponentInChildren<SymptomNotifText>(true);
        }
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        thisAvatarSymptomNotifText.ApplyEffectsOfSymptom();
        thisAvatarSymptomNotifText.ChangeNotifText();
    }

    public void PickRandNumberHostAndSetSymptomForAll() {
        if (!Multiplayer.GetUser().IsHost) { return; }

        randNum = Random.Range(0, symptoms.Count);
        BroadcastRemoteMethod(nameof(SetSymptom), randNum);
    }

    public int GetRandNumber() { return randNum; }

}
