using Alteruna;
using UnityEngine;

public class CarpetManager : AttributesSync {
    private static bool redCorrupted = false;
    private static bool greenCorrupted = false;
    private static bool blueCorrupted = false;

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
