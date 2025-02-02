using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbySystem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI symptomNotifText;
    List<CarpetData> allCarpets;

    private int tempRandom = 0;

    Alteruna.Avatar avatar;
    private void Awake()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
    }
    private void Start()
    {
        allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();
    }

    private void Update() {

        if(!avatar.IsMe) { return; }
        if(Input.GetKeyDown(KeyCode.Tab)) {
            controlsText.enabled = !controlsText.enabled;
        }


        // we are calling the symptom twice with these, once in setsymptom and once in the chain of calls bc we are displaying the notif canvas

        /*
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            SymptomsManager.Instance.JustSetSymptom(0); // Inverted controls
            ResetCarpetParams();
            DisplayNotificationText();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            SymptomsManager.Instance.JustSetSymptom(1);// Jumpy carpets
            SetCarpetParams();
            DisplayNotificationText();



        }

        if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
            SymptomsManager.Instance.JustSetSymptom(2); // Render distance
            ResetCarpetParams();
            DisplayNotificationText();
        }

        if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            SymptomsManager.Instance.JustSetSymptom(999); // no symptom
            ResetCarpetParams();
            DisplayNoSymptomText();
        }
        */
    }

    private void OnDestroy() {
       // SymptomsManager.Instance.JustSetSymptom(999); // no symptom
    }

    void SetCarpetParams()
    {
        switch (CarpetManager.Instance.GetCarpetColorRandomNum())
        {

            case 0:
                // red carpets
                foreach (CarpetData carpet in allCarpets)
                {
                    if (carpet.GetColor() == CarpetColor.Red)
                    {
                        carpet.IsCorruptedLocal = true;
                        carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
                        carpet.IsCorruptedLocal = false;
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
                        carpet.IsCorruptedLocal = true;
                        carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
                        carpet.IsCorruptedLocal = false;
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
                        carpet.IsCorruptedLocal = true;
                        carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.CorruptedMat;
                    }
                    else
                    {
                        carpet.IsCorruptedLocal = false;
                        carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
                    }
                }
                break;
            default:
                        

                break; // never gonna happen
        }


    }

    void ResetCarpetParams()
    {
        foreach (CarpetData carpet in allCarpets)
        {
            carpet.IsCorruptedLocal = false;
            carpet.gameObject.GetComponentInChildren<MeshRenderer>().material = carpet.NormalMat;
        }
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
            symptomNotifText.text += CarpetManager.Instance.carpetColorRandNum switch {
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
