using Alteruna;
using System;
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

        JustSetSymptom(index);
        if (avatar == null)
        {
            avatar = Multiplayer.GetAvatar();
            thisAvatarSymptomNotifText = avatar.transform.GetComponentInChildren<SymptomNotifText>(true);
        }

        if (index != 1) CarpetManager.Instance.ResetCarpetParams();
        //if (index == 1) CarpetManager.Instance.BroadcastRemoteMethod(nameof(CarpetManager.Instance.RandomizeCarpetColor));
        avatar.GetComponent<CarpetSymptom>().ApplyEffectsOfSymptom();
        thisAvatarSymptomNotifText.ChangeNotifText();
    }   

    

    public void JustSetSymptom(int index)
    {
        if(index>symptoms.Count-1)
        {
            currentSymptom = null;
        }
        else
        {
            currentSymptom = symptoms[index];
        }
    }

    public void PickRandNumberHostAndSetSymptomForAll() {
        if (!Multiplayer.GetUser().IsHost) { return; }

        //randNum = UnityEngine.Random.Range(0, symptoms.Count);
        randNum = 1;
        BroadcastRemoteMethod(nameof(SyncRandNum), randNum);
        BroadcastRemoteMethod(nameof(SetSymptom), randNum);

        if (randNum == 1)
        {
            int randCarpetNum = UnityEngine.Random.Range(0, 3);
            CarpetManager.Instance.BroadcastRemoteMethod(nameof(CarpetManager.Instance.SyncNumb), randCarpetNum);

           // CarpetManager.Instance.BroadcastRemoteMethod(nameof(CarpetManager.Instance.RandomizeCarpetColor));
        }
    }

    public int GetRandNumber() { return randNum; }

    [SynchronizableMethod]
    private void SyncRandNum(int a)
    {
        randNum = a;
    }
}
