using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewSymptom", menuName = "Symptoms/New Symptom")]
public class SymptomsSO : ScriptableObject {
    public string Name;
    [TextArea(1, 3)]
    public string Description;
}
