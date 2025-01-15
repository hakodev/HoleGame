using Alteruna;
using DG.Tweening;
using UnityEngine;

public class SymptomsManager : AttributesSync {
    Alteruna.Avatar avatar;
    public static SymptomsManager Instance { get; private set; }
    private PlayerController player;
    [SerializeField] private SymptomsSO disappearingObjsSymptom;
    private SymptomsSO currentSymptom;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
        player = GetComponent<PlayerController>();
    }

    public SymptomsSO GetSymptom() {
        return currentSymptom;
    }

    [SynchronizableMethod]
    public void SetSymptom(SymptomsSO newSymptom) {
        currentSymptom = newSymptom;
    }

    public void TriggerSymptom(GameObject heldObject = null) {
        if(!avatar.IsMe) return;

        if(currentSymptom == disappearingObjsSymptom) {
            heldObject.GetComponent<Collider>().enabled = false;
            heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f);
        } else {
            Debug.Log("Not current");
        }
    }
}
