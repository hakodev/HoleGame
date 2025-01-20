using Alteruna;
using UnityEngine;

public class CarpetManager : AttributesSync {
    public static CarpetManager Instance { get; private set; }

    private static bool redCorrupted = false;
    private static bool greenCorrupted = false;
    private static bool blueCorrupted = false;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public static bool GetCorrupted(CarpetColor carpetColor) {
        return carpetColor switch {
            CarpetColor.Red => redCorrupted,
            CarpetColor.Green => greenCorrupted,
            CarpetColor.Blue => blueCorrupted,
            _ => false // This default will never run, just need it to compile
        };
    }

    [SynchronizableMethod]
    public void SetCorrupted(CarpetColor carpetColor, bool corrupted) {
        switch(carpetColor) {
            case CarpetColor.Red:
                redCorrupted = corrupted;
                break;
            case CarpetColor.Green:
                greenCorrupted = corrupted;
                break;
            case CarpetColor.Blue:
                blueCorrupted = corrupted;
                break;
            default:
                break;
        }
    }
}

public enum CarpetColor {
    Red,
    Green,
    Blue
}
