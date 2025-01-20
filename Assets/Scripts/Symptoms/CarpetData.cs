using UnityEngine;

public class CarpetData : MonoBehaviour {
    [SerializeField] private CarpetColor carpetColor;

    [field: SerializeField]
    public bool IsCorrupted { get; set; }

    public CarpetColor GetColor() {
        return carpetColor;
    }
}
