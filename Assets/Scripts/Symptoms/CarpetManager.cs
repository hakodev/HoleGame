using Alteruna;
using UnityEngine;

public class CarpetManager : AttributesSync {
    public static CarpetManager Instance { get; private set; }

    [SynchronizableField] public int carpetColorRandNum;
    private const int numOfTotalCarpetColors = 3;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public int GetCarpetColorRandomNum() {
        return carpetColorRandNum;
    }
    public void RandomizeCarpetColor()
    {
        carpetColorRandNum = Random.Range(0, numOfTotalCarpetColors);
    }
}

public enum CarpetColor {
    Red = 0,
    Green = 1,
    Blue = 2
}
