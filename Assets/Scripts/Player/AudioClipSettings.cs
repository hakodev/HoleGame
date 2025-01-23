using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipSettings", menuName = "Scriptable Objects/AudioClipSettings")]
public class AudioClipSettings : ScriptableObject
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
    public float spatialBlend;
}
