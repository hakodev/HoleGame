using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbySystem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI symptomNotifText;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            controlsText.enabled = !controlsText.enabled;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            SymptomsManager.Instance.SetSymptom(0); // Inverted controls
            DisplayNotificationText();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            SymptomsManager.Instance.SetSymptom(1); // Jumpy carpets
            DisplayNotificationText();
            SetCarpetParams();
        }

        if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
            SymptomsManager.Instance.SetSymptom(2); // Render distance
            DisplayNotificationText();
        }

        if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            SymptomsManager.Instance.SetSymptom(999); // no symptom
            DisplayNoSymptomText();
        }
    }

    private void OnDestroy() {
        SymptomsManager.Instance.SetSymptom(999); // no symptom
    }

    void SetCarpetParams()
    {
        List<CarpetData> allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

        switch (CarpetManager.Instance.GetCarpetColorRandomNum())
        {
            case 0:
                // red carpets
                foreach (CarpetData carpet in allCarpets)
                {
                    if (carpet.GetColor() == CarpetColor.Red)
                    {
                        carpet.IsCorrupted = true;
                        carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
                        carpet.IsCorrupted = false;
                        carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
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
                        carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
                        carpet.IsCorrupted = false;
                        carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
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
                        carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
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

    private void DisplayNotificationText() {
        symptomNotifText.DOKill();
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        symptomNotifText.alpha = 1f;
        symptomNotifText.text = "Simulating new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";

        // If it is the Jumpy Carpets symptom
        if(SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1]) {
            symptomNotifText.text += $"\n\nThe corrupted carpets are:\n";
            symptomNotifText.text += CarpetManager.Instance.GetCarpetColorRandomNum() switch {
                0 => "Red!",
                1 => "Green!",
                2 => "Blue!",
                _ => "If you somehow see this, you broke the game",
            };
        }

        symptomNotifText.DOFade(0f, 10f).OnComplete(() => symptomNotifText.transform.parent.parent.gameObject.SetActive(false));
    }

    private void DisplayNoSymptomText() {
        symptomNotifText.DOKill();
        symptomNotifText.transform.parent.parent.gameObject.SetActive(true);
        symptomNotifText.alpha = 1f;
        symptomNotifText.text = "All symptoms removed!";
        symptomNotifText.DOFade(0f, 5f).OnComplete(() => symptomNotifText.transform.parent.parent.gameObject.SetActive(false));
    }
}
