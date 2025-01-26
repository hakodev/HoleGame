using Alteruna;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SymptomNotifText : AttributesSync {
    private TextMeshProUGUI notificationText;

    private void Awake() {
        notificationText = GetComponent<TextMeshProUGUI>();
    }

    private new void OnEnable() {
        base.OnEnable();

        if (FindAnyObjectByType<LobbySystem>()) return;
        ApplyNewSymptom();
        DisplayNotificationText();
    }

    private void ApplyNewSymptom() {
        //List<SymptomsSO> allSymptoms = SymptomsManager.Instance.GetSymptomsList();
        //int randNum = Random.Range(0, allSymptomsCount);
        //SymptomsManager.Instance.SetSymptom(allSymptoms[randNum]);
        int randNum = SymptomsManager.Instance.GetRandomNum();
        SymptomsManager.Instance.SetSymptom(randNum);
        Debug.Log($"New symptom applied: {SymptomsManager.Instance.GetSymptom().Name}");

        // If it is the Jumpy Carpets symptom
        if(SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1]) {
            List<CarpetData> allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

            switch(CarpetManager.Instance.GetCarpetColorRandomNum()) {
                case 0:
                    // red carpets
                    foreach(CarpetData carpet in allCarpets) {
                        if(carpet.GetColor() == CarpetColor.Red) {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                        } else {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 1:
                    // green carpets
                    foreach(CarpetData carpet in allCarpets) {
                        if(carpet.GetColor() == CarpetColor.Green) {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                        } else {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                case 2:
                    // blue carpets
                    foreach(CarpetData carpet in allCarpets) {
                        if(carpet.GetColor() == CarpetColor.Blue) {
                            carpet.IsCorrupted = true;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                        } else {
                            carpet.IsCorrupted = false;
                            carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
                        }
                    }
                    break;
                default:
                    break; // never gonna happen
            }

            allCarpets.Clear();
        }
    }

    private void DisplayNotificationText() {
        StartCoroutine(TimedDisplay());
    }

    private IEnumerator TimedDisplay() {
        notificationText.text = "The machines have caught a new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";

        // If it is the Jumpy Carpets symptom
        if(SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1]) {
            notificationText.text += $"\n\nThe corrupted carpets for this round are:\n";
            notificationText.text += CarpetManager.Instance.GetCarpetColorRandomNum() switch {
                0 => "Red!",
                1 => "Green!",
                2 => "Blue!",
                _ => "If you somehow see this, you broke the game",
            };
        }

        yield return new WaitForSeconds(10f);
        this.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }
}
