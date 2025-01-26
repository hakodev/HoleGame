using Alteruna;
using UnityEngine;

public class CarpetData : AttributesSync {
    [SerializeField] private CarpetColor carpetColor;

    [SerializeField, SynchronizableField]
    private bool isCorrupted;

    [field: SerializeField] public Material NormalMat { get; private set; }
    [field: SerializeField] public Material CorruptedMat { get; private set; }

    public bool IsCorrupted {
        get { return isCorrupted; }
        set { isCorrupted = value; }
    }

    public CarpetColor GetColor() {
        return carpetColor;
    }

    private void OnCollisionEnter(Collision otherCollider) {
        Debug.Log("Carpet1");
        if(this.isCorrupted && otherCollider.gameObject.CompareTag("Player")) {
            //if(otherCollider.collider.GetComponent<PlayerRole>().GetRole() == Roles.Machine) {
            Debug.Log("Carpet 2");
                otherCollider.collider.gameObject.GetComponent<PlayerController>().Jump();
            //}
        }
    }
}
