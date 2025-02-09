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

        allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1] && playerRole.GetRole() == Roles.Machine)
        {
            switch (CarpetManager.Instance.GetCarpetColorRandomNum())
            {
                case 0:
                    // red carpets
                    Debug.Log("carcol RED");
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Red)
                        {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                            Debug.Log("carcol RED2");

                        }
                        else
                        {
                            Debug.Log("carcol RED3");
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 1:
                    // green carpets
                    Debug.Log("carcol GREEN");
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Green)
                        {
                            Debug.Log("carcol GREEN2");

                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                        }
                        else
                        {
                            Debug.Log("carcol GREEN3");
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 2:
                    // blue carpets
                    Debug.Log("carcol BLUE");
                    foreach (CarpetData carpet in allCarpets)
                    {
                        if (carpet.GetColor() == CarpetColor.Blue)
                        {
                            Debug.Log("carcol BLUE2");

                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                        }
                        else
                        {
                            Debug.Log("carcol BLUE3");
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
