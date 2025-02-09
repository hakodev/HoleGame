using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarpetSymptom : MonoBehaviour
{
    Alteruna.Avatar avatar;

    private PlayerRole playerRole;

    List<CarpetData> allCarpets;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        playerRole = GetComponent<PlayerRole>();
    }

    public void ApplyEffectsOfSymptom()
    {
        if (!avatar.IsMe) { return; }

        if (playerRole.GetRole() == Roles.Machine)
        {
            //StartCoroutine(LerpDithering());
        }

        if (allCarpets == null) allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1] && playerRole.GetRole() == Roles.Machine)
        {
            switch (CarpetManager.Instance.GetCarpetColorRandomNum())
            {
                case 0:
                    // red carpets
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Red)
                        {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                        }
                        else
                        {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 1:
                    // green carpets
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Green)
                        {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                        }
                        else
                        {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 2:
                    // blue carpets
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Blue)
                        {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                        }
                        else
                        {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                default:
                    break; // never gonna happen
            }
        }
        else
        {
            foreach (CarpetData carpet in allCarpets)
            {
                carpet.IsCorrupted = false;
                carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
            }
        }

        allCarpets.Clear();
    }
}
