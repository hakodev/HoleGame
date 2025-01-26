using Alteruna;
using UnityEngine;

public class CarpetData : AttributesSync {
    [SerializeField] private CarpetColor carpetColor;

    [SerializeField, SynchronizableField]
    private bool isCorrupted
        ;
    private bool isCorruptedLocal;


    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Carpet");
    }

    [field: SerializeField] public Material NormalMat { get; private set; }
    [field: SerializeField] public Material CorruptedMat { get; private set; }

    public bool IsCorrupted {
        get { return isCorrupted; }
        set { isCorrupted = value; }
    }

    public bool IsCorruptedLocal
    {
        get { return isCorruptedLocal; }
        set { isCorruptedLocal = value; }
    }

    public CarpetColor GetColor() {
        return carpetColor;
    }

}
