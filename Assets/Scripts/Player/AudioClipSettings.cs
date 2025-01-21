using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipSettings", menuName = "Scriptable Objects/AudioClipSettings")]
public class AudioClipSettings : ScriptableObject
{
    [SerializeField]
    private AudioClip clip;
    [Range(0f, 1f)]
    [SerializeField]
    private float volume;
    [Range(0f, 1f)]
    [SerializeField]
    private float spatialBlend;
}
