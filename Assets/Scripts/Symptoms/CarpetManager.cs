using Alteruna;
using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class CarpetManager : AttributesSync {
    public static CarpetManager Instance { get; private set; }

    [SynchronizableField] public int carpetColorRandNum = 0;
    private const int numOfTotalCarpetColors = 3;

    private List<CarpetData> allCarpets;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();
    }
    public int GetCarpetColorRandomNum() {
        return carpetColorRandNum;
    }
    [SynchronizableMethod]
    public void RandomizeCarpetColor()
    {
        carpetColorRandNum = UnityEngine.Random.Range(0, numOfTotalCarpetColors);
        Debug.Log("carcol " + carpetColorRandNum);
        BroadcastRemoteMethod(nameof(SyncNumb), carpetColorRandNum);
    }
    public void ResetCarpetParams()
    {
        foreach (CarpetData carpet in allCarpets)
        {
            carpet.IsCorruptedLocal = false;
            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
        }
    }
    [SynchronizableMethod]
    public void SyncNumb(int a)
    {
        carpetColorRandNum = a;
        Debug.Log("carcol2 " + carpetColorRandNum);
    }
}


public enum CarpetColor {
    Red = 0,
    Green = 1,
    Blue = 2
}
