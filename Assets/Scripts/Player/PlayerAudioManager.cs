using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClipSettings pickUpAudio;
    [SerializeField] private AudioClipSettings throwAudio;
    [SerializeField] private AudioClipSettings laserBeep;
    [SerializeField] private AudioClipSettings deathStatic;

    [SerializeField] private AudioClipSettings lightHit;
    [SerializeField] private AudioClipSettings heavyHit;

    #region Properties
    public AudioClipSettings GetHeavyHit() => heavyHit;
    public AudioClipSettings GetLightHit() => lightHit;
    public AudioClipSettings GetDeathStatic() => deathStatic;
    public AudioClipSettings GetThrowAudio() => throwAudio;
    public AudioClipSettings GetLaserBeep() => laserBeep;
    public AudioClipSettings GetPickUp() => pickUpAudio;

    private static PlayerAudioManager instance;

    #endregion

    public static PlayerAudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PlayerAudioManager>();
            }

            return instance;
        }
    }

    public void PlaySound(GameObject sourceObject, AudioClipSettings sound)
    {
        AudioSource source;
        if (sourceObject.GetComponent<AudioSource>() == null)
        {
            source = sourceObject.AddComponent<AudioSource>();
        }
        else
        {
            source = sourceObject.GetComponent<AudioSource>();
        }

        SetAudioParams(sound, source);
        source.Play();
    }

    private void SetAudioParams(AudioClipSettings sound, AudioSource source)
    {
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.spatialBlend = sound.spatialBlend;
    }



}

