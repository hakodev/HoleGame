using Alteruna;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SymptomNotifText : AttributesSync
{
    TextMeshProUGUI notificationText;
    [SerializeField] LobbySystem lobbySystem;
    Alteruna.Avatar avatar;

    public Material ditheringMat;

    private float ditheringStartFloat;


    private void Awake()
    {
        //fsr awakes just dont work
        notificationText = GetComponent<TextMeshProUGUI>();
        ditheringStartFloat = ditheringMat.GetFloat("_Pixelate");
        ditheringMat.SetFloat("_Pixelate", 350);
    }
    
    
    private new void OnEnable()
    {
        base.OnEnable();
        if(RoleAssignment.hasGameStarted) {
            //SymptomsManager.Instance.PickRandNumberHostAndSetSymptomForAll();
            //lobbySystem.gameObject.SetActive(false);
            //DestroyImmediate(lobbySystem.gameObject);
        }
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

    public IEnumerator LerpDithering()
    {
        //PlayerAudioManager.Instance.PlaySound(gameObject, PlayerAudioManager.Instance.GetGlitch);
        bool isBlinkingUp = true;
        float timer = 0f;
        if (isBlinkingUp)
        {
            while (timer < 1)
            {
                timer += Time.deltaTime * 2.5f;
                ditheringMat.SetFloat("_Pixelate", Mathf.Lerp(350, 20, timer));

                if (timer >= 1)
                {
                    isBlinkingUp = false;
                }
                yield return new WaitForEndOfFrame();
            }

        }
        if (!isBlinkingUp)
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime * 2.5f;
                ditheringMat.SetFloat("_Pixelate", Mathf.Lerp(350, 20, timer));

                if (timer <= 0)
                {
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
