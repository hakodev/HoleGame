using UnityEngine;

public class CarpetData : MonoBehaviour {
    [SerializeField] private CarpetColor carpetColor;

    public CarpetColor GetColor() {
        return carpetColor;
    }
}
