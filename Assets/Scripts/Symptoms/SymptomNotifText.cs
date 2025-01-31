using Alteruna;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SymptomNotifText : AttributesSync
{
    TextMeshProUGUI notificationText;
    [SerializeField] LobbySystem lobbySystem;
    Alteruna.Avatar avatar;

    private void Awake()
    {
        //fsr awakes just dont work
        notificationText = GetComponent<TextMeshProUGUI>();

    }
    
    
    private new void OnEnable()
    {
        base.OnEnable();
       // if (lobbySystem != null) {return;}
        SymptomsManager.Instance.PickRandNumberHostAndSetSymptomForAll();
    }

    List<CarpetData> allCarpets;
    public void ApplyEffectsOfSymptom()
    {
        if (avatar == null) avatar = Multiplayer.GetAvatar();
        if(!avatar.IsMe) { return; }

        if(allCarpets==null) allCarpets = FindObjectsByType<CarpetData>(FindObjectsSortMode.None).ToList();

        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[1] && transform.root.GetComponent<PlayerRole>().GetRole() == Roles.Machine)
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
        if (avatar == null) avatar = Multiplayer.GetAvatar();
        if (!avatar.IsMe) { return; }

        if(notificationText==null) notificationText = GetComponent<TextMeshProUGUI>();


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

        notificationText.ForceMeshUpdate();
        Debug.Log("hehe " + notificationText.text);



        // yield return new WaitForSeconds(30f);
        // this.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }
}
