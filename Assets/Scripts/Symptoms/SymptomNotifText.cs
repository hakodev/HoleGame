using Alteruna;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SymptomNotifText : AttributesSync
{
    private TextMeshProUGUI notificationText;
    LobbySystem lobbySystem;
    static List<SymptomNotifText> symptomTexts = new List<SymptomNotifText>();
    Alteruna.Avatar avatar;

    private void Awake()
    {
        notificationText = GetComponent<TextMeshProUGUI>();
        lobbySystem = transform.root.GetComponentInChildren<LobbySystem>();
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        SymptomsManager.Instance.SymptomNotifTextWasSpawned(this);
        Debug.Log("health " + avatar.name);
    }
    
    public static List<SymptomNotifText> GetSymptomNotifTexts() { return symptomTexts; }

    private void Start()
    {
        symptomTexts.Add(this);
    }

    bool hasBeenCalled = false;
    
    private new void OnEnable()
    {
        base.OnEnable();
        // if (lobbySystem != null) {return;}
        if (!avatar.IsMe) { return; }

        SymptomsManager.Instance.PickRandNumberHostAndSetSymptomForAll();
    }

    List<CarpetData> allCarpets;
    public void ApplyEffectsOfSymptom()
    {
        Debug.Log("healthcare maybe get past this return" + avatar.name);

        if (avatar.IsMe!) { return; }
        //List<SymptomsSO> allSymptoms = SymptomsManager.Instance.GetSymptomsList();
        //int randNum = Random.Range(0, allSymptomsCount);
        //SymptomsManager.Instance.SetSymptom(allSymptoms[randNum]);
        //Debug.Log("rocks " + randNum);

        Debug.Log($"New symptom applied: {SymptomsManager.Instance.GetSymptom().Name}");

        if(allCarpets==null) allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

        // If it is the Jumpy Carpets symptom
        Debug.Log("healthcare " + SymptomsManager.Instance.GetSymptom() + " " + SymptomsManager.Instance.GetSymptomsList()[1]);
        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1] && GetComponent<PlayerRole>().GetRole() == Roles.Machine)
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
        }
        else
        {
            foreach (CarpetData carpet in allCarpets)
            {
                carpet.IsCorrupted = false;
                carpet.gameObject.GetComponent<MeshRenderer>().material = carpet.NormalMat;
            }
        }

        allCarpets.Clear();
    }

    public void ChangeNotifText()
    {
        notificationText.text = "The machines have caught a new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";

        // If it is the Jumpy Carpets symptom
        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1])
        {
            notificationText.text += $"\n\nThe corrupted carpets for this round are:\n";
            notificationText.text += CarpetManager.Instance.carpetColorRandNum switch
            {
                0 => "Red!",
                1 => "Green!",
                2 => "Blue!",
                _ => "If you somehow see this, you broke the game",
            };
        }

       // yield return new WaitForSeconds(30f);
       // this.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }
}
