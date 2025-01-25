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
        if(this.isCorrupted && otherCollider.collider.CompareTag("Player")) {
            if(otherCollider.collider.GetComponent<PlayerRole>().GetRole() == Roles.Machine) {
                otherCollider.collider.gameObject.GetComponent<PlayerController>().Jump();
            }
        }
    }
}
