using UnityEngine;
using DG.Tweening;

public class DisappearingObjs : MonoBehaviour {
    Alteruna.Avatar avatar;
    private PlayerController player;
    [SerializeField] private SymptomsSO disappearingObjsSymptom;

    private void Awake() {
        player = GetComponent<PlayerController>();
    }

    public void CheckIfPlayerHasDisappearingObjectsSymptom(GameObject heldObject) {
        if(player.Symptom == disappearingObjsSymptom && avatar.IsMe) {
            heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f);
        }
    }
}
