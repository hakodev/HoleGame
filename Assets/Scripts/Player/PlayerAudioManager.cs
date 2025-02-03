using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClipSettings pickUpAudio;
    [SerializeField] private AudioClipSettings throwAudio;
    [SerializeField] private AudioClipSettings laserBeep;
    [SerializeField] private AudioClipSettings deathStatic;

    [SerializeField] private AudioClipSettings lightHit;
    [SerializeField] private AudioClipSettings heavyHit;
    [SerializeField] private AudioClipSettings stickyHit;
    [SerializeField] private AudioClipSettings mugHit;
    [SerializeField] private AudioClipSettings bouncyHit;
    [SerializeField] private AudioClipSettings paperCrumple;
    [SerializeField] private AudioClipSettings basketballBeep;
    [SerializeField] private AudioClipSettings glitchSound;

    #region Properties
    public AudioClipSettings GetHeavyHit { get => heavyHit; }
    public AudioClipSettings GetLightHit { get => lightHit; }
    public AudioClipSettings GetDeathStatic { get => deathStatic; }
    public AudioClipSettings GetThrowAudio { get => throwAudio; }
    public AudioClipSettings GetLaserBeep { get => laserBeep; }
    public AudioClipSettings GetPickUp { get => pickUpAudio; }
    public AudioClipSettings GetSticky { get => stickyHit; }
    public AudioClipSettings GetMug { get => mugHit; }
    public AudioClipSettings GetBouncyBall { get => bouncyHit; }
    public AudioClipSettings GetCrumple { get => paperCrumple; }
    public AudioClipSettings GetBasketballBeep { get => basketballBeep; }
    public AudioClipSettings GetGlitch { get => glitchSound; }

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

    public void PlaySound(GameObject sourceObject, AudioSource source, AudioClipSettings sound)
    {
        if (source == null)
        {
            source = sourceObject.AddComponent<AudioSource>();
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

