using UnityEngine;

public class DisappearingObjs : MonoBehaviour {
    Alteruna.Avatar avatar;
    private PlayerController playerController;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
    }

    public void CheckIfPlayerHasDisappearingObjectsSymptom(GameObject heldObject) {
        //heldObj = heldObject;
    }

    //private void DisappearDroppedOrThrownIntractable() {
    //    if(avatar.IsMe) {
    //        
    //    }
    //}
}
