using UnityEngine;

[CreateAssetMenu(fileName = "NewSymptom", menuName = "Symptoms/New Symptom")]
public class SymptomsSO : ScriptableObject {
    public string Name = "Symptom name here";
    [TextArea(1, 3)]
    public string Description = "Symptom description here, keep it short";
}
