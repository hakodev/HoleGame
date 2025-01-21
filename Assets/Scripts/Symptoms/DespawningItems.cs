using DG.Tweening;
using UnityEngine;

public class DespawningItems : MonoBehaviour {
    public static void DespawnItem(GameObject heldObject) {
        heldObject.GetComponent<Collider>().enabled = false;
        heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 1f);
        Debug.Log("Despawn symptom triggered!");
    }
}
