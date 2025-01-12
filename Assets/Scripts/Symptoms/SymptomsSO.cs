using UnityEngine;

[CreateAssetMenu(fileName = "NewSymptom", menuName = "Symptoms/New Symptom")]
public class SymptomsSO : ScriptableObject {
    public string Name;
    [TextArea(1, 3)]
    public string Description;
    // Will add more variables as we decide on them

    public void TriggerSymptom() {
        Debug.Log($"You have caught {Name}!");
    }
}
