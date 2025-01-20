using Alteruna;
using System.Collections;
using TMPro;
using UnityEngine;

public class SymptomNotifText : AttributesSync {
    private TMP_Text notificationText;

    private void Awake() {
        notificationText = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        ApplyNewSymptom();
        DisplayNotificationText();
    }

    private void ApplyNewSymptom() {
        //List<SymptomsSO> allSymptoms = SymptomsManager.Instance.GetSymptomsList();
        //int randNum = Random.Range(0, allSymptomsCount);
        //SymptomsManager.Instance.SetSymptom(allSymptoms[randNum]);

        int allSymptomsCount = SymptomsManager.Instance.GetSymptomsList().Count;
        int randNum = SymptomsManager.Instance.GetRandomNum();
        SymptomsManager.Instance.BroadcastRemoteMethod(nameof(SymptomsManager.Instance.SetSymptom), randNum);
        Debug.Log($"New symptom applied: {SymptomsManager.Instance.GetSymptom().Name}");
    }

    private void DisplayNotificationText() {
        StartCoroutine(TimedDisplay());
    }

    private IEnumerator TimedDisplay() {
        notificationText.text = "The machines have caught a new symptom!\n\n" +
                               $"\"{SymptomsManager.Instance.GetSymptom().Name}\"\n" +
                               $"{SymptomsManager.Instance.GetSymptom().Description}";
        yield return new WaitForSeconds(10f);
        this.transform.parent.parent.gameObject.SetActive(false); // disable canvas
    }
}
